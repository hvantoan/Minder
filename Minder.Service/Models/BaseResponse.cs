using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Minder.Services.Models {

    public class BaseResponse {
        public bool Success { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Message { get; set; }

        public static BaseResponse Ok() {
            return new() { Success = true };
        }

        public static BaseResponse Fail(string? message = null) {
            return new() { Message = message };
        }
    }

    public class BaseResponse<T> : BaseResponse {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T? Data { get; set; }

        public static BaseResponse<T> Ok(T data) {
            return new() { Success = true, Data = data };
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