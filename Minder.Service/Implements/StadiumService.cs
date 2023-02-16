using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minder.Database.Enums;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Service.Extensions;
using Minder.Service.Interfaces;
using Minder.Service.Models.Stadium;
using Minder.Services.Common;
using Minder.Services.Extensions;
using Minder.Services.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minder.Service.Implements {

    public class StadiumService : BaseService, IStadiumService {
        private readonly IFileService fileService;
        private readonly AdministrativeUnitResource administrativeUnitResource;

        public StadiumService(IServiceProvider serviceProvider) : base(serviceProvider) {
            this.fileService = serviceProvider.GetRequiredService<IFileService>();
            this.administrativeUnitResource = serviceProvider.GetRequiredService<AdministrativeUnitResource>();
        }

        public async Task<ListStadiumRes> List(ListStadiumReq req) {
            var query = this.db.Stadiums.AsNoTracking().Where(o => !o.IsDelete);

            if (!string.IsNullOrEmpty(req.SearchText)) {
                req.SearchText = req.SearchText.ReplaceSpace(isUnsignedUnicode: true);
                query = query.Where(o => o.Name.Contains(req.SearchText) || o.Name.GetSumary().Contains(req.SearchText) || o.Code.ToLower().Contains(req.SearchText));
            }

            return new ListStadiumRes() {
                Count = await query.CountIf(req.IsCount, o => o.Id),
                Items = await query.OrderBy(o => o.Id).Skip(req.PageIndex * req.PageSize).Take(req.PageSize).Select(o => StadiumDto.FromEntity(o, administrativeUnitResource)).ToListAsync()
            };
        }

        public async Task<StadiumDto?> Get(GetStadiumReq req) {
            var stadium = await this.db.Stadiums.AsNoTracking().FirstOrDefaultAsync(o => o.Id == req.Id && !o.IsDelete);
            ManagedException.ThrowIf(stadium == null, Messages.Stadium.Stadium_NotFound);

            return StadiumDto.FromEntity(stadium, administrativeUnitResource);
        }

        public async Task<string> CreateOrUpdate(StadiumDto model) {
            if (string.IsNullOrEmpty(model.Id)) {
                return await this.Create(model);
            } else {
                return await this.Update(model);
            }
        }

        private async Task<string> Create(StadiumDto model) {
            this.logger.Information($"{nameof(Stadium)} - {nameof(Create)} - Start", model);

            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Code), Messages.Stadium.Stadium_CodeRequired);
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Name), Messages.Stadium.Stadium_NameRequired);

            var isExitCode = await this.db.Stadiums.AnyAsync(o => o.Code == model.Code && !o.IsDelete);
            ManagedException.ThrowIf(isExitCode, Messages.Stadium.Stadium_CodeExited);

            var stadium = new Stadium() {
                Id = Guid.NewGuid().ToStringN(),
                UserId = this.current.UserId,
                Code = model.Code,
                Name = model.Name,
                Phone = model.Phone,
                Longitude = model.Longitude,
                Latitude = model.Latitude,
                Province = model.Province?.Code ?? string.Empty,
                District = model.District?.Code ?? string.Empty,
                Commune = model.Commune?.Code ?? string.Empty,
                Address = model.Address ?? string.Empty,
                CreateAt = DateTimeOffset.Now,
            };

            await this.db.Stadiums.AddAsync(stadium);
            await this.db.SaveChangesAsync();
            this.logger.Information($"{nameof(Team)} - {nameof(Create)} - End", stadium.Id);
            return stadium.Id;
        }

        public async Task<string> Update(StadiumDto model) {
            this.logger.Information($"{nameof(Team)} - {nameof(Update)} - Start", model);

            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Code), Messages.Stadium.Stadium_CodeRequired);
            ManagedException.ThrowIf(string.IsNullOrWhiteSpace(model.Name), Messages.Stadium.Stadium_NameRequired);

            var isExitCode = await this.db.Stadiums.AnyAsync(o => o.Code == model.Code && o.Id != model.Id && !o.IsDelete);
            ManagedException.ThrowIf(!isExitCode, Messages.Stadium.Stadium_CodeExited);

            var stadium = await this.db.Stadiums.FirstOrDefaultAsync(o => o.Id == model.Id && o.UserId == this.current.UserId && !o.IsDelete);
            ManagedException.ThrowIf(stadium == null, Messages.Stadium.Stadium_NotFound);

            stadium.Code = model.Code;
            stadium.Name = model.Name;
            stadium.Phone = model.Phone;
            stadium.Latitude = model.Latitude;
            stadium.Longitude = model.Longitude;
            stadium.Province = model.Province?.Code ?? string.Empty;
            stadium.Commune = model.Commune?.Code ?? string.Empty;
            stadium.District = model.District?.Code ?? string.Empty;
            stadium.Address = model.Address ?? string.Empty;

            await this.db.Stadiums.AddAsync(stadium);
            await this.db.SaveChangesAsync();

            this.logger.Information($"{nameof(Team)} - {nameof(Create)} - End", stadium.Id);

            return stadium.Id;
        }

        public async Task Delete(DeleteStadiumReq req) {
            this.logger.Information($"{nameof(Stadium)} - {nameof(Delete)} - Start", req.Id);

            var stadium = await this.db.Stadiums.FirstOrDefaultAsync(o => o.Id == req.Id && o.UserId == this.current.UserId && !o.IsDelete);
            ManagedException.ThrowIf(stadium == null, Messages.Stadium.Stadium_NotFound);

            stadium.IsDelete = true;
            await this.db.SaveChangesAsync();
        }

        public async Task ImportStadium(ImportStadiumReq req) {
            List<ImportStadiumDto> stadiums;
            UTF8Encoding enc = new();
            using (var reader = new StringReader(enc.GetString(req.Data!)))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture)) {
                csv.Context.RegisterClassMap<ImportStadiumMap>();
                stadiums = csv.GetRecords<ImportStadiumDto>().Where(o => !o.IsEmpty()).ToList();
            }

            foreach (var stadium in stadiums) {
                try {
                    var item = new Stadium() {
                        Id = Guid.NewGuid().ToStringN(),
                        UserId = this.current.UserId,
                        Code = stadium.Code,
                        Phone = "0" + stadium.Phone,
                        Name = stadium.Name.ToUpperFirstChar(),
                        Longitude = stadium.Longitude,
                        Latitude = stadium.Latitude,
                        Province = stadium.Province,
                        District = stadium.District,
                        Commune = stadium.Commune,
                        Address = stadium.Address,
                        CreateAt = DateTimeOffset.Now,
                    };

                    if (string.IsNullOrWhiteSpace(stadium.Image)) continue;
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                    byte[] file = new System.Net.WebClient().DownloadData(new Uri(stadium.Image));
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                    if (file == null || !file.Any()) {
                        Console.WriteLine($"Fail:{item.Code}");
                    }

                    await this.fileService.CreateOrUpdate(new Models.File.FileDto() {
                        ItemId = item.Id,
                        ItemType = EItemType.StadiumAvatar,
                        Type = EFile.Image,
                        Data = file,
                        Name = $"{stadium.Name.GetSumary()}_{stadium.Code}.jpg"
                    }, false);

                    await this.db.AddAsync(item);
                    Console.WriteLine($"Done: {stadium.Code}\n");
                } catch (Exception) {
                    continue;
                }
            }
            await this.db.SaveChangesAsync();
        }
    }
}