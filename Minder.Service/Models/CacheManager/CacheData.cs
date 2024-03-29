﻿using Minder.Database.Enums;

namespace Minder.Service.Models.CacheManager {

    public class CacheData {
        public string Code { get; set; } = string.Empty;
        public EVerifyType Type { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}