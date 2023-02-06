using Minder.Services.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Minder.Services.Models {

    public class BaseRes {
        public Response Response { get; set; } = new();

        public static BaseRes Ok() {
            return new();
        }

        public static BaseRes Fail(string model) {
            return new BaseRes() {
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
                Success = false,
                StatusCode = 99,
                Message = "Lỗi hệ thống."
            };
        }

        public static Response Fail(string model) {
            ResponseMessage? errorMessage = JsonConvert.DeserializeObject<ResponseMessage>(model);

            if (errorMessage == null) return SystemFail();
            return new Response() {
                Success = false,
                StatusCode = errorMessage.Code,
                Message = errorMessage.Message,
            };
        }
    }

    public class BaseRes<T> : BaseRes {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T? Data { get; set; }

        public static BaseRes<T> Ok(T data) {
            return new() { Data = data };
        }
    }

    public class BaseSaveRes<T> : BaseRes {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Item Data { get; set; } = new();

        public class Item {

            [JsonPropertyName(nameof(T) + "Id")]
            public string Id { get; set; } = string.Empty;
        }

        public static BaseSaveRes<T> Ok(string id) {
            return new() {
                Data = new Item() {
                    Id = id
                }
            };
        }
    }

    public class BaseListRes<T> {
        public List<T?> Items { get; set; } = new();
        public int Count { get; set; }
    }

    public class FileResult {
        public string FileName { get; set; } = string.Empty;
        public byte[] ByteArray { get; set; } = Array.Empty<byte>();
    }
}