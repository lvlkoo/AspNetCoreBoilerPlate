using Boilerplate.Api.Utils;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Boilerplate.Api.ActionFilters
{
    /// <summary>
    /// Provide validation error responses
    /// </summary>
    public class ModelValidatorActionFilter: IActionFilter
    {
        /// <inheritdoc />
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new ValidationFailedResult(context.ModelState);
            }
        }

        /// <inheritdoc />
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //nothing to do
        }
    }
}
