using FoyleSoft.AzureCore.Interfaces;

namespace FoyleSoft.AzureCore.Models
{
    public class BaseResponse<T> : IBaseResponse<T>
    {
        public BaseResponse()
        {
        }
        public BaseResponse(T data)
        {
            Data = data;
            IsSuccess = true;
            ErrorMessage = string.Empty;
        }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public T Data { get; set; }
        public IBaseResponse<K> ConvertError<K>()
        {
            return new BaseResponse<K>
            {
                Data = default(K),
                ErrorMessage = this.ErrorMessage,
                IsSuccess = this.IsSuccess
            };
        }
    }
    
}
