namespace Boilerplate.Models
{
    public class BaseResponse
    {
        public int StatusCode { get; set; } = 200;
        public bool IsSuccess { get; set; } = true;
        public ErrorModel Error { get; set; }
    }

    public class BaseResponse<T>: BaseResponse
    {    
        public T Data { get; set; }
    }
}
