
using Newtonsoft.Json.Linq;

namespace ArabazonPlatform.Generic.Response
{
    public class BaseResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string? ErrorCode { get; set; }
        public BaseResponse()
        {

        }
        public BaseResponse(bool isSuccess = false, string message = null, string errorCode = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            ErrorCode = errorCode;
        }
        public BaseResponse(BaseResponse baseResponse)
        {
            IsSuccess = baseResponse.IsSuccess;
            Message = baseResponse.Message;
            ErrorCode = baseResponse.ErrorCode;
        }
    }
    public class BaseResponse<T> : BaseResponse
    {
        public T DataObject { get; set; }

        public BaseResponse()
        {

        }

        public BaseResponse(BaseResponse baseResponse) : base(baseResponse.IsSuccess, baseResponse.Message, baseResponse.ErrorCode)
        {

        }

        public BaseResponse(bool isSuccess, string message = null, string errorCode = null, T dataObject = default(T)) : base(isSuccess, message, errorCode)
        {
            this.DataObject = dataObject;
        }

        public BaseResponse(bool isSuccess, string message = null, T dataObject = default(T)) : base(isSuccess, message)
        {
            this.DataObject = dataObject;
        }

        public BaseResponse(bool isSuccess, string message = null) : base(isSuccess, message)
        {

        }

        public BaseResponse(JObject jObject) : base((bool)jObject["isSuccess"], jObject["message"].ToString(), jObject["errorCode"].ToString())
        {
            this.DataObject = jObject["dataObject"].ToObject<T>();
        }

        public BaseResponse(BaseResponse<dynamic> brokerBaseResponse) : base(true, null)
        {
            if (brokerBaseResponse.DataObject != null)
            {
                if (brokerBaseResponse.DataObject is Newtonsoft.Json.Linq.JObject)
                {
                    JObject jObject = brokerBaseResponse.DataObject as Newtonsoft.Json.Linq.JObject;
                    this.IsSuccess = (bool)jObject["isSuccess"];
                    this.Message = jObject["message"].ToString();
                    this.ErrorCode = jObject["errorCode"].ToString();

                    if (jObject["dataObject"] != null)
                        this.DataObject = jObject["dataObject"].ToObject<T>();
                }
                else
                {
                    this.IsSuccess = brokerBaseResponse.IsSuccess;
                    this.Message = brokerBaseResponse.Message;
                    this.ErrorCode = brokerBaseResponse.ErrorCode;
                    this.DataObject = (T)brokerBaseResponse.DataObject;
                }
            }
            else
            {
                this.IsSuccess = brokerBaseResponse.IsSuccess;
                this.Message = brokerBaseResponse.Message;
                this.ErrorCode = brokerBaseResponse.ErrorCode;
            }
        }
    }
}
