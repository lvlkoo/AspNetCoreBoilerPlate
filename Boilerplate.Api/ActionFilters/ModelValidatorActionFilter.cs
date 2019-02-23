using Boilerplate.Api.Utils;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Boilerplate.Api.ActionFilters
{
    public class ModelValidatorActionFilter: IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new ValidationFailedResult(context.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //nothing to do
        }
    }
}
