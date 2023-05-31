using Minder.Services.Resources;
using Newtonsoft.Json;
using System;

namespace Minder.Service.Models.Stadium {

    public class StadiumDto {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public AdministrativeUnit? Province { get; set; }
        public AdministrativeUnit? District { get; set; }
        public AdministrativeUnit? Commune { get; set; }
        public string? Address { get; set; }
        public string? FullAddress => $"{Address}, {Commune?.Name}, {District?.Name}, {Province?.Name}";
        public DateTimeOffset CreateAt { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Avatar { get; set; }

        public static StadiumDto FromEntity(Database.Models.Stadium entity, AdministrativeUnitResource? administrativeUnitResource = null,
           string? avatar = null) {

            var au = administrativeUnitResource?.GetByCode(entity.Province, entity.District, entity.Commune) ?? new();
            return new StadiumDto {
                Id = entity.Id,
                UserId = entity.UserId,
                Code = entity.Code,
                Name = entity.Name,
                Phone = entity.Phone,
                Longitude = entity.Longitude,
                Latitude = entity.Latitude,
                Province = !string.IsNullOrWhiteSpace(entity.Province) && au.TryGetValue(entity.Province, out var province) ? province : default,
                District = !string.IsNullOrWhiteSpace(entity.District) && au.TryGetValue(entity.District, out var district) ? district : default,
                Commune = !string.IsNullOrWhiteSpace(entity.Commune) && au.TryGetValue(entity.Commune, out var commune) ? commune : default,
                Address = entity.Address,
                CreateAt = entity.CreateAt,
                Avatar = avatar,
            };
        }
    }
}