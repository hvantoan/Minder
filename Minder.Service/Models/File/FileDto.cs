using Minder.Database.Enums;
using Newtonsoft.Json;

namespace Minder.Service.Models.File {

    public partial class FileDto {

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string ItemId { get; set; } = string.Empty;
        public EFile Type { get; set; }
        public EItemType ItemType { get; set; }
        public string? Path { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public byte[]? Data { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? ImportUrl { get; set; }
    }

    public partial class FileDto {

        public static FileDto FromEntity(Database.Models.File entity, string? currentUrl) {
            return new FileDto {
                Id = entity.Id,
                Name = entity.Name,
                ItemId = entity.ItemId,
                Type = entity.Type,
                ItemType = entity.ItemType,
                Path = $"{currentUrl}/{entity.Path}",
            };
        }
    }
}