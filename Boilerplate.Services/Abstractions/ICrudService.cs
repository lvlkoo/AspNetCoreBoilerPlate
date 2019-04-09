using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Boilerplate.Models;
using Boilerplate.Models.Filters;

namespace Boilerplate.Services.Abstractions
{
    public interface ICrudService<TModel, TFilter> : IFilterableService<TModel, TFilter> where TModel : IModel where TFilter : BaseFilter
    {
        Task<IEnumerable<TModel>> Get();
        Task<TModel> Get(Guid id);
        Task<TModel> Create(TModel model);
        Task<TModel> Update(Guid id, TModel model);
        Task Delete(Guid id);
    }

    public interface ICrudService<TModel> : ICrudService<TModel, BaseFilter> where TModel : IModel
    {

    }
}
