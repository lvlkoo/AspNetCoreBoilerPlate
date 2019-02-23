using System.Collections.Generic;

namespace Boilerplate.Models
{
    public class ValidationResultModel: ErrorModel
    {
        public List<ValidationErrorModel> Errors { get; set; }
    }
}
