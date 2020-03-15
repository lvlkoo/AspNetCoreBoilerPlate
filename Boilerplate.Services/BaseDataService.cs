using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Boilerplate.Commons.Exceptions;
using Boilerplate.EF;
using Boilerplate.Entities;
using Boilerplate.Models;
using Boilerplate.Models.Filters;
using Boilerplate.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Boilerplate.Services
{
    public class BaseDataService
    {
        protected ApplicationDbContext DbContext { get; }
        protected IMapper Mapper { get; }

        protected BaseDataService(ApplicationDbContext context, IMapper mapper)
        {
            DbContext = context;
            Mapper = mapper;
        }

        protected List<T> MapList<T>(object source)
        {
            return Mapper.Map<List<T>>(source);
        }

        protected T Map<T>(object source)
        {
            return Mapper.Map<T>(source);
        }
    }

    public class BaseDataService<TEntity> : BaseDataService where TEntity : class
    {
        protected BaseDataService(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        protected virtual IQueryable<TEntity> Scope => DbContext.Set<TEntity>();

        protected IQueryable<TEntity> FilteredScope(BaseFilter baseFilter)
        {
            var query = Scope.AsQueryable();

            if (baseFilter != null)
            {
                if (baseFilter.Take != null)
                    query = query.Take(baseFilter.Take.Value);

                if (baseFilter.Offset != null)
                    query = query.Skip(baseFilter.Offset.Value);

                if (baseFilter.OrderBy != null && baseFilter.OrderType != null)
                    query = query.OrderByProperty(baseFilter.OrderBy, baseFilter.OrderType.Value);

            }

            return query;
        }
    }

    public class BaseDataService<TEntity, TModel, TFilter> : BaseDataService<TEntity>, ICrudService<TModel, TFilter>
        where TEntity : class, IEntity where TModel : IModel where TFilter : BaseFilter
    {
        protected BaseDataService(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public virtual async Task<IEnumerable<TModel>> Get() =>
            await Scope.ProjectTo<TModel>(Mapper.ConfigurationProvider).ToListAsync();

        public virtual async Task<IEnumerable<TModel>> Get(TFilter filter) =>
            await FilteredScope(filter).ProjectTo<TModel>(Mapper.ConfigurationProvider).ToListAsync();

        public virtual async Task<TModel> Get(Guid id)
        {
            var entity = await Scope
                .FirstOrDefaultAsync(_ => _.Id == id);
            if (entity == null)
                throw new EntityNotFoundException();

            return Map<TModel>(entity);
        }

        public virtual async Task<TModel> Create(TModel model)
        {
            var entity = Mapper.Map<TEntity>(model);

            entity.CreatedDate = DateTime.Now;
            entity.UpdatedDate = DateTime.Now;

            await DbContext.AddAsync(entity);
            await DbContext.SaveChangesAsync();
            return await Get(entity.Id);
        }

        public virtual async Task<TModel> Update(Guid id, TModel model)
        {
            var entity = await DbContext.Set<TEntity>().FindAsync(id);
            if (entity == null)
                throw new EntityNotFoundException();

            var updated = Mapper.Map<TEntity>(model);
            updated.Id = entity.Id;
            updated.CreatedDate = entity.CreatedDate;
            updated.UpdatedDate = DateTime.Now;

            DbContext.Entry(entity).CurrentValues.SetValues(updated);
            await DbContext.SaveChangesAsync();
            return await Get(entity.Id);
        }

        public virtual async Task Delete(Guid id)
        {
            var entity = await DbContext.Set<TEntity>().FindAsync(id);
            if (entity == null)
                throw new EntityNotFoundException();

            DbContext.Remove(entity);
            await DbContext.SaveChangesAsync();
        }
    }

    public class BaseDataService<TEntity, TModel> : BaseDataService<TEntity, TModel, BaseFilter>
        where TEntity : class, IEntity where TModel : IModel
    {
        protected BaseDataService(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}
