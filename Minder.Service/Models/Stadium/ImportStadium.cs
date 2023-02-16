using CsvHelper.Configuration;

namespace Minder.Service.Models.Stadium {

    public partial class ImportStadiumDto {
        public string Code { get; set; } = string.Empty;

        //Info

        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // Location

        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string Province { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Commune { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        public bool IsEmpty() {
            if (string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(Code)
                           && string.IsNullOrWhiteSpace(Phone) && string.IsNullOrWhiteSpace(Address)
                           && string.IsNullOrWhiteSpace(Province) && string.IsNullOrWhiteSpace(District)
                           && string.IsNullOrWhiteSpace(Commune)) return true;
            return false;
        }
    }

    public class ImportStadiumMap : ClassMap<ImportStadiumDto> {

        public ImportStadiumMap() {
            Map(o => o.Code).Index(0);
            Map(o => o.Name).Index(1);
            Map(o => o.Phone).Index(2);
            Map(o => o.Latitude).Index(3);
            Map(o => o.Longitude).Index(4);
            Map(o => o.Image).Index(5);
            Map(o => o.Province).Index(6);
            Map(o => o.District).Index(7);
            Map(o => o.Commune).Index(8);
            Map(o => o.Address).Index(9);
        }
    }
}