using Minder.Services.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Minder.Services.Models {

    public class BaseResponse {
        public Response Response { get; set; } = new();

        public static BaseResponse Ok() {
            return new();
        }

        public static BaseResponse Fail(string model) {
            return new BaseResponse() {
                Response = Response.Fail(model)
            };
        }
    }

    public class Response {
        public bool Success { get; set; } = true;
        public int StatusCode { get; set; } = 0;
        public string Message { get; set; } = "Thành công.";
        public string Time { get; set; } = DateTimeOffset.Now.ToLocalTime().DateTime.ToString("dd-MM-yyyy HH:mm:ss");

        public static Response SystemFail() {
            return new Response() {
                StatusCode = 99,
                Message = "Lỗi hệ thống."
            };
        }

        public static Response Fail(string model) {
            ResponseMessage? errorMessage = JsonConvert.DeserializeObject<ResponseMessage>(model);

            if (errorMessage == null) return SystemFail();
            return new Response() {
                StatusCode = errorMessage.Code,
                Message = errorMessage.Message,
            };
        }
    }

    public class BaseResponse<T> : BaseResponse {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T? Data { get; set; }

        public static BaseResponse<T> Ok(T data) {
            return new() { Data = data };
        }
    }

    public class BaseListData<T> {
        public List<T?> Items { get; set; } = new();
        public int Count { get; set; }
    }

    public class BaseSaveResponse : BaseResponse<string> { }

    public class FileResult {
        public string FileName { get; set; } = string.Empty;
        public byte[] ByteArray { get; set; } = Array.Empty<byte>();
    }
}