using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Boilerplate.Services.Abstractions
{
    public interface IFilterableService<TModel, TFilter>
    {
        Task<IEnumerable<TModel>> Get(TFilter filter);
    }
}
