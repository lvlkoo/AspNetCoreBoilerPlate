using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Boilerplate.DAL;
using Boilerplate.DAL.Entities;
using Boilerplate.Models;
using Boilerplate.Models.Exceptions;
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
    }

    public class BaseDataService<TEntity> : BaseDataService where TEntity : class
    {
        public BaseDataService(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        protected async Task<IEnumerable<TEntity>> Get() =>
            await DbContext.Set<TEntity>().ToListAsync();

        protected async Task<TEntity> Get(Guid id)
        {
            var entity = await DbContext.Set<TEntity>().FindAsync(id);
            
            if (entity == null)
                throw new EntityNotFoundException();

            return entity;
        }
    }

    public class BaseDataService<TEntity, TModel> : BaseDataService where TEntity : class, IEntity where TModel : IModel
    {
        protected BaseDataService(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        protected async Task<IEnumerable<TModel>> Get() =>
            await DbContext.Set<TEntity>().ProjectTo<TModel>(Mapper.ConfigurationProvider).ToListAsync();

        protected async Task<TModel> Get(Guid id)
        {
            var entity = await DbContext.Set<TEntity>().ProjectTo<TModel>(Mapper.ConfigurationProvider).FirstOrDefaultAsync(_ => _.Id == id);
            if (entity == null)
                throw new EntityNotFoundException();

            return entity;
        }

        protected async Task<TModel> Create(TModel model)
        {
            var entity = Mapper.Map<TEntity>(model);
            await DbContext.AddAsync(entity);
            await DbContext.SaveChangesAsync();
            return await Get(entity.Id);
        }

        protected async Task<TModel> Update(Guid id, TModel model)
        {
            var entity = await DbContext.Set<TEntity>().FindAsync(id);
            if (entity == null)
                throw new EntityNotFoundException();
            
            var updated = Mapper.Map<TEntity>(model);
            DbContext.Entry(entity).CurrentValues.SetValues(updated);
            await DbContext.SaveChangesAsync();
            return await Get(entity.Id);
        }

        protected async Task Delete(Guid id)
        {
            var entity = await DbContext.Set<TEntity>().FindAsync(id);
            if (entity == null)
                throw new EntityNotFoundException();

            DbContext.Remove(entity);
            await DbContext.SaveChangesAsync();
        }
    }
}
