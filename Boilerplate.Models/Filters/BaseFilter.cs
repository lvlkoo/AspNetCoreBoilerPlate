using Boilerplate.Models.Enums;

namespace Boilerplate.Models.Filters
{
    public class BaseFilter
    {
        public int? Take { get; set; }
        public int? Offset { get; set; }
        public OrderType? OrderType { get; set; }
        public string OrderBy { get; set; }
    }
}
