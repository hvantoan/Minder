using Minder.Service.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Minder.Services.Resources {

    public class AddressMap {
        public string Name { get; set; } = string.Empty;
        public AdministrativeUnit? AdministrativeUnit { get; set; }
    }

    public class AdministrativeUnit {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    internal class AuStorage {
        public string Lv1 { get; set; } = null!;
        public string? Lv2 { get; set; }
        public string? Lv3 { get; set; }
        public string Name { get; set; } = null!;
    }

    public class AdministrativeUnitResource {
        private readonly List<AuStorage> items = new();

        public AdministrativeUnitResource() {
            string filename = "Resources\\administrative_unit.json";
            if (File.Exists(filename)) {
                try {
                    items = JsonConvert.DeserializeObject<List<AuStorage>>(File.ReadAllText(filename)) ?? new List<AuStorage>();
                } finally {
                    if (items == null) items = new();
                }
            }
        }

        public async Task<List<AdministrativeUnit>> List(string? lv1Code, string? lv2Code) {
            List<AdministrativeUnit> result;
            if (string.IsNullOrWhiteSpace(lv1Code)) {
                result = items.Where(o => !string.IsNullOrWhiteSpace(o.Lv1) && string.IsNullOrWhiteSpace(o.Lv2) && string.IsNullOrWhiteSpace(o.Lv3))
                    .Select(o => new AdministrativeUnit {
                        Code = o.Lv1,
                        Name = o.Name,
                    }).ToList();
                return await Task.FromResult(result);
            }

            if (string.IsNullOrWhiteSpace(lv2Code)) {
                result = items.Where(o => o.Lv1 == lv1Code && !string.IsNullOrWhiteSpace(o.Lv2) && string.IsNullOrWhiteSpace(o.Lv3))
                    .Select(x => new AdministrativeUnit {
                        Code = x.Lv2!,
                        Name = x.Name,
                    }).ToList();
                return await Task.FromResult(result);
            }

            result = items.Where(o => o.Lv1 == lv1Code && o.Lv2 == lv2Code && !string.IsNullOrWhiteSpace(o.Lv3))
                .Select(o => new AdministrativeUnit {
                    Code = o.Lv3!,
                    Name = o.Name,
                }).ToList();
            return await Task.FromResult(result);
        }

        public Dictionary<string, AdministrativeUnit> GetByCode(params string?[] codes) {
            codes = codes.Where(code => !string.IsNullOrWhiteSpace(code)).ToArray();
            if (codes.Length == 0) return new Dictionary<string, AdministrativeUnit>();

            var lv1Items = items.Where(o => codes.Contains(o.Lv1) && string.IsNullOrWhiteSpace(o.Lv2) && string.IsNullOrWhiteSpace(o.Lv3))
                .Select(o => new AdministrativeUnit {
                    Code = o.Lv1,
                    Name = o.Name,
                }).ToList();

            var lv2Items = items.Where(o => !string.IsNullOrWhiteSpace(o.Lv1) && codes.Contains(o.Lv2) && string.IsNullOrWhiteSpace(o.Lv3))
                .Select(o => new AdministrativeUnit {
                    Code = o.Lv2!,
                    Name = o.Name,
                }).ToList();

            var lv3Items = items.Where(o => !string.IsNullOrWhiteSpace(o.Lv1) && !string.IsNullOrWhiteSpace(o.Lv2) && codes.Contains(o.Lv3))
                .Select(o => new AdministrativeUnit {
                    Code = o.Lv3!,
                    Name = o.Name,
                }).ToList();

            return lv1Items.Union(lv2Items).Union(lv3Items).ToDictionary(k => k.Code);
        }

        public AdministrativeUnit? GetByCode(string? code) {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            return items.Where(o => o.Lv1 == code || o.Lv2 == code || o.Lv3 == code).Select(o => new AdministrativeUnit {
                Code = code,
                Name = o.Name,
            }).FirstOrDefault();
        }

        public List<AddressMap> GetByName(params string?[] names) {
            var tempNames = names.Where(name => !string.IsNullOrWhiteSpace(name)).Select(name => name!.Replace("-", " ").ReplaceSpace(true)).ToArray();
            if (tempNames.Length == 0) return new List<AddressMap>();

            var data = items.Where(o => tempNames.Contains(o.Name.Replace("-", " ").ReplaceSpace(true))
                || tempNames.Any(n => o.Name.Replace("-", " ").ReplaceSpace(true).Contains(n)))
                .Select(o => new AdministrativeUnit {
                    Code = o.Lv3 ?? o.Lv2 ?? o.Lv1,
                    Name = o.Name,
                }).ToList();

            return data.Select(o => new AddressMap() {
                Name = o.Name.Replace("-", " ").ReplaceSpace(true),
                AdministrativeUnit = o
            }).ToList();
        }
    }
}