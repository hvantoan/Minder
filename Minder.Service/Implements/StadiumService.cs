using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Minder.Database.Enums;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Service.Extensions;
using Minder.Service.Interfaces;
using Minder.Service.Models.File;
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
        private readonly AdministrativeUnitResource administrativeUnitResource;
        private readonly IFileService fileService;

        public StadiumService(IServiceProvider serviceProvider) : base(serviceProvider) {
            this.administrativeUnitResource = serviceProvider.GetRequiredService<AdministrativeUnitResource>();
            this.fileService = serviceProvider.GetRequiredService<IFileService>();
        }

        public async Task<ListStadiumRes> List(ListStadiumReq req) {
            var query = this.db.Stadiums.AsNoTracking().Where(o => !o.IsDeleted);

            if (!string.IsNullOrEmpty(req.SearchText)) {
                req.SearchText = req.SearchText.ReplaceSpace(isUnsignedUnicode: true);
                query = query.Where(o => o.Name.Contains(req.SearchText) || o.Name.GetSumary().Contains(req.SearchText) || o.Code.ToLower().Contains(req.SearchText));
            }
            if (req.PageIndex != 0 || req.PageSize != 0) {
                query = query.Skip(req.PageIndex * req.PageSize);
            }
            var stadiums = await query.OrderBy(o => o.Id).ToListAsync();

            var stadiumIds = stadiums.Select(o => o.Id).ToList();
            var files = await this.db.Files.Where(o => stadiumIds.Contains(o.ItemId) && o.Type == EFile.Image && o.ItemType == EItemType.StadiumAvatar)
                .Select(o => FileDto.FromEntity(o, this.current.Url)).ToDictionaryAsync(k => k!.ItemId, v => v!.Path);

            return new ListStadiumRes() {
                Count = await query.CountIf(req.IsCount, o => o.Id),
                Items = stadiums.Select(o => StadiumDto.FromEntity(o, this.administrativeUnitResource, files.GetValueOrDefault(o.Id))).ToList()
            };
        }

        public async Task<StadiumDto?> Get(GetStadiumReq req) {
            var stadium = await this.db.Stadiums.AsNoTracking().FirstOrDefaultAsync(o => o.Id == req.Id && !o.IsDeleted);
            ManagedException.ThrowIf(stadium == null, Messages.Stadium.Stadium_NotFound);

            var avatar = await this.fileService.Get(req.Id, EItemType.StadiumAvatar);
            return StadiumDto.FromEntity(stadium, administrativeUnitResource, avatar?.Path);
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

            var isExitCode = await this.db.Stadiums.AnyAsync(o => o.Code == model.Code && !o.IsDeleted);
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

            var isExitCode = await this.db.Stadiums.AnyAsync(o => o.Code == model.Code && o.Id != model.Id && !o.IsDeleted);
            ManagedException.ThrowIf(!isExitCode, Messages.Stadium.Stadium_CodeExited);

            var stadium = await this.db.Stadiums.FirstOrDefaultAsync(o => o.Id == model.Id && o.UserId == this.current.UserId && !o.IsDeleted);
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

            var stadium = await this.db.Stadiums.FirstOrDefaultAsync(o => o.Id == req.Id && o.UserId == this.current.UserId && !o.IsDeleted);
            ManagedException.ThrowIf(stadium == null, Messages.Stadium.Stadium_NotFound);

            stadium.IsDeleted = true;
            await this.db.SaveChangesAsync();
        }

        public async Task InitialData() {
            string data = "77u/Q29kZSxOYW1lLFBob25lLExhdGl0dWRlLExvbmdpdHVkZSxQaG90byxQcm92aW5jZSBDb2RlLERpc3RyaWN0IENvZGUsQ29tbXVuZSBDb2RlLGFkZHJlc3MNClNCMDEsU8OibiBCw7NuZyDEkMOhIFBoYW4gQ2h1IFRyaW5oLDkzMTQxMTYxNiwxMC44MTUyMjgwMywxMDYuNzAxMzI2NCxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBPNklUV0xMTVAzUjdxWF81RE5UcDRaUDlRaXI2NUEzM2pmTXphQz13NDA4LWgzMDYtay1ubyw3OSw3NjUsMjQ3NzUsODIgUGhhbiBDaHUgVHJpbmgNClNCMDIsU8OibiBiw7NuZyDEkcOhIG1pbmkgY+G7jyBuaMOibiB04bqhbyBUaMOgbmggUGjDoXQsOTM0MTY4ODI4LDEwLjgyMDU1NzAzLDEwNi43MjU5NzY4LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE9iaXY0c2Vzak1ERzEyc0pxWHQ2TWE3aDZoU0NpdVBOdHg1M2ZKPXc0MDgtaDMwNi1rLW5vLDc5LDc2NSwyNjk2MiwxMDE3IELDrG5oIFF14bubaQ0KU0IwMyxTw6JuIGJhbmggUXV54buBbiBCw6xuaCBUaOG6oW5oLDkwNzA3MTc3NSwxMC44MjAxOTc0NSwxMDYuNzAxNDM3NSxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBPNW15dFBiUkZreV96QnJycTFRMHJFZ2RyaEZEVUliMWhRZW1GMT13NDA4LWgzMDYtay1ubyw3OSw3NjUsMjY4NzIsMzQzIE7GoSBUcmFuZyBMb25nDQpTQjA0LFPDom4gYsOzbmcgxJHDoSBNaW5oIE5o4bqtdCBXZVNwb3J0LDkzMzIyNzkyOSwxMC44MzM4NDgwMywxMDYuNzQxMzc4NCxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBQVHhZdzNWLUtvd2VuUDNWaG1nWnY2OFM1V0RfRjFQRFU4M0MtNj13NDA4LWgzMDYtay1ubyw3OSw3NjUsMjY5NjIsMTEgQsOsbmggUXXhu5tpDQpTQjA1LFPDom4gQsOzbmcgxJDDoSBNaW5pIFZpY3RvcnksOTAzMDA2NjMwLDEwLjgyNDc4NDM1LDEwNi43MzU1NTQ1LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE1HOWZpT1BKNGxPZmd5bXNOb3BZR0xyaG4tQVBNcHIwcE1JaVNuPXc0MjYtaDI0MC1rLW5vLDc5LDc2NSwyNjk2Miw0MjYgQsOsbmggUXXhu5tpDQpTQjA2LFPDom4gQsOzbmcgxJDDoSBIQ0EsOTA4NTY4Nzc3LDEwLjgxMjIzNzUyLDEwNi43MDM1ODQxLGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcFB2N3BfZkhTb3hQV3FtRlItR0E3MHc0blBGYmFsSFh3Mk9helFuPXc0MDgtaDI0My1rLW5vLDc5LDc2NSwyNDc3NSwzMjQgQ2h1IFbEg24gQW4NClNCMDcsU8OibiBCw7NuZyBT4buRIDQsOTgzNTI4OTA3LDEwLjgxMzcyNDA0LDEwNi43MDc5Mjg0LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcFB3XzNHR1NHTVhhYUJkR0NFal90dnBDWExXc2NNUmlQTWJkaHVjPXc0MDgtaDcyNS1rLW5vLDc5LDc2NSwyNjkxNCwzNCDEkMaw4budbmcgc+G7kSA2DQpTQjA4LFPDom4gYsOzbmcgxJHDoSBD4bqndSDEkOG7jyw4OTk0NDcwMTYsMTAuODIyMjI2NDgsMTA2LjcwMjI4MTgsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwUGIyLXJjNjRXckdoM285Nmk5XzdKVlRqMTJVbDZNYk1VYXJHN0Q9dzQwOC1oMzA2LWstbm8sNzksNzY1LDI2ODcyLDUxNiDEkC4gUGjhuqFtIFbEg24gxJDhu5NuZw0KU0IwOSxTw6JuIELDs25nIMSQw6EgTWluaSBTb25nIE5hbSw5MDgzNzQyNjgsMTAuODEyMDc4MjIsMTA2LjcxOTAzNTUsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwT2pVRV9oM1NRQjZxcXFLU0ItMkdNMWdHdXBzVUNLdXl5UHNZdE49dzQwOC1oMzA2LWstbm8sNzksNzY1LDI2OTIwLCI0LCBEMywgS2h1IFbEg24gVGjDoW5oIELhuq9jIg0KU0IxMCxTw6JuIELDs25nIMSQw6EgQS5UIFRo4bunIMSQ4bupYyw5MTQzODM3MzUsMTAuODM3NDY3MTUsMTA2Ljc2MTU1MixodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBOZUNnY2RHQ2NqZlpWbUJkTldnVHhHcm1xMkdvWTFISlVvcE5PeD13NDI3LWgyNDAtay1ubyw3OSw3NjIsMjY4MjcsMzUvMTEgxJAuIFPhu5EgNA0KU0IxMSxTw6JuIGPhu48gbmjDom4gdOG6oW8gTmjDoCB0aGnhur91IG5oaSBUaOG7pyDEkOG7qWMsOTgzNjA2OTg5LDEwLjg1MTIyMTQyLDEwNi43NjgwODEzLGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE9PTmFNaEtjVXNXaHhza2EtQU5sT3QwQ08zX2RUaEV0ZmRTRWcyPXc0MDgtaDMwNi1rLW5vLDc5LDc2MiwyNjgxNSwyODEgxJAuIFbDtSBWxINuIE5nw6JuDQpTQjEyLFPDom4gQsOzbmcgxJDDoSBNaW5pIFNha2UsNzc1MDUwMDg2LDEwLjg0ODI3MTY3LDEwNi43NTExODAyLGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcFB4YnFvMmgwa1liMEY1ZW5zOUtXSUhYUnNRVVBnb2gyUmxkT0I3PXc0MDgtaDMwNi1rLW5vLDc5LDc2MiwyNjgyMSwiMTJhIMSQLiBz4buRIDQyLCBLaHUgUGjhu5EgNiINClNCMTMsU8OibiBiw7NuZyBBIFRow6BuaCw5ODgxNTIwMjgsMTAuODQwNzk3MTYsMTA2LjcxODE2MzcsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwT1FJZEtIaklFbEU3Tlh6UDhHTHBBYmtISy1kUlp4bERKdG1PYVA9dzQwOC1oMzA2LWstbm8sNzksNzYyLDI2ODA5LDE4IMSQxrDhu51uZyBT4buRIDQNClNCMTQsU8OibiBiYW5oIExpbmggWHXDom4sMjg3MzA5MDc5OSwxMC44ODA0NDE1NSwxMDYuNzcyNzQ5NixodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBQQ0lNRFUxbnhNdjNTYTJuSVI4QWQ1ODZSdkc2WmxaNVZNc1Awaj13NDA4LWgzMDYtay1ubyw3OSw3NjIsMjY3OTQsNDkvMTAgUXXhu5FjIGzhu5kgMUENClNCMTUsU8OibiBiw7NuZyBOxINtIE5o4buPLDk3Mjk0NDIwMCwxMC44NzAxODI4OCwxMDYuNzk0NjkyLGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE5DUE9rZE9iSlpCY1B0TjNxTVdodEoxc1N4SnBfY19RSWg5QTgyPXc0MDgtaDU0NC1rLW5vLDc5LDc2MiwyNjgwOSwxOCDEkMaw4budbmcgU+G7kSA0DQpTQjE2LFPDom4gQsOzbmcgxJDDoSAyNyw5ODc4NzkwMzIsMTAuODMyMTUzODEsMTA2LjczMjgxMTQsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwUFhRakpGdC1uZkNyeW1lTl9SX0NyMDFJdDdtRExPSHB3MUJncGQ9dzQyNi1oMjQwLWstbm8sNzksNzYyLDI2ODEyLDcwIMSQLiAyNw0KU0IxNyxTw6JuIELDs25nIMSQw6EgTGluaCBUw6J5IFdlU3BvcnQsMjg3MzAwNzkxOSwxMC44NTMwNTA0MiwxMDYuNzUyNzA4MSxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBNNEtGYnlMNEJldzRGOVpHTE5wMERHNmRTMXR2NS1MeG1PWS1BNz13NzIwLWgyNDAtay1ubyw3OSw3NjIsMjY4MTgsMTYgVHLhuqduIFbEg24gTuG7rWENClNCMTgsU8OibiB24bqtbiDEkeG7mW5nIFRhbyDEkMOgbiwyODczMDU2Mjg4LDEwLjc3NTI1MDUxLDEwNi42OTQ2Mjk5LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE5OT1ZveVdLRktYa2cyRG81XzBrbHJLQmxvVnpPY3oxZHY4RHpqPXc0MjYtaDI0MC1rLW5vLDc5LDc2MCwyNjc0MyxIdXnhu4FuIFRyw6JuIEPDtG5nIENow7phDQpTQjE5LFPDom4gQsOzbmcgxJHDoSBNaW5pIMSQSCBZIETGsOG7o2MsLDEwLjc4NTYwMjcsMTA2LjcwMjQ1OTgsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwTzFtS2dOWXJ3Yy1PUV9IbXhPX1ZobmJza2k4UmZhaENZVmNJUXo9dzQwOC1oMzA2LWstbm8sNzksNzYwLDI2NzQwLDQxIMSQaW5oIFRpw6puIEhvw6BuZw0KU0IyMCxUcnVuZyB0w6JtIFRo4buDIGThu6VjIFRo4buDIHRoYW8gSG9hIEzGsCwyODczMDU3MDg4LDEwLjc4NzgwNjQzLDEwNi43MDE1MDkyLGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE01RXpjWksxWTVTaFI0NExNZzFyeExZN1ctOXRma0lGUjQ0d2lWPXc0MDgtaDMwNi1rLW5vLDc5LDc2MCwyNjczNywyIMSQaW5oIFRpw6puIEhvw6BuZw0KU0IyMSxTw6JuIELDs25nIMSQw6EgTWluaSBD4buPIE5ow6JuIFThuqFvIFRyxrDGoW5nIFF1eeG7gW4sOTAzMDc2MDI2LDEwLjc4NTM2NDk5LDEwNi42Nzc2MTU4LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE0yMUtiNWxJekt0SnNZSEt3Q0U3WDlQT2ZkcFdGVHRQWDdEUkFtPXcxNTYtaDExNC1wLWstbm8sNzksNzcwLDI2ODcyLFPhu5EgOTQ2IFRyxrDhu51uZyBTYQ0KU0IyMixTw6JuIELDs25nIMSQw6EgUGjhuqFtIEdpYSwsMTAuNzg2MjU2ODMsMTA2LjY3OTAxODIsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwTTIxS2I1bEl6S3RKc1lIS3dDRTdYOVBPZmRwV0ZUdFBYN0RSQW09dzQwOC1oNTQ0LWstbm8sNzksNzcwLDI2ODcyLDEyIMSQLiBMw6ogVsSDbiBT4bu5DQpTQjIzLFPDom4gQsOzbmcgY+G7jyBuaMOibiB04bqhbywsMTAuNzg3MjYwMSwxMDYuNjY5NjkzNixodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBNMjFLYjVsSXpLdEpzWUhLd0NFN1g5UE9mZHBXRlR0UFg3RFJBbT13NDA4LWg1NDQtay1ubyw3OSw3NzAsMjQ3OTksMjM5LzY3Lzc4IFRy4bqnbiBWxINuIMSQYW5nDQpTQjI0LFPDom4gY+G7jyBuaMOibiB04bqhbyBRNCwyODIyMTE4MDI2LDEwLjc1Nzc0NTc1LDEwNi42OTg2MTQyLGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcFA5bzV2TGd4cWlNT09CWVJNQ3RqeVZYazl0dnNiY29sT2tkX042PXcxNTYtaDExNC1wLWstbm8sNzksNzczLDE5MzQ4LDIyOSDEkC4gU+G7kSA0OA0KU0IyNSxTw6JuIELDs25nIMSQw6EgS2jDoW5oIEjhu5lpLCwxMC43NTY4OTkyNSwxMDYuNjk5NDMxNywsNzksNzczLDE5MzU0LDEgxJAuIFPhu5EgNDgNClNCMjYsQ8OidSBM4bqhYyBC4buZIELDs25nIMSQw6EsMjgzOTI2Mjg5MiwxMC43NTc3MzI0NiwxMDYuNjk5ODI1NyxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBOWjBfMi1zdEwwQkVYYTk4eDYyUm9pcUlkUVFXTVFILUVKUGZXaz13NDA4LWgzMDYtay1ubyw3OSw3NzMsMTkzNDgsIlFNNVgrM1dXLCDEkC4gU+G7kSA0OCINClNCMjcsQ+G7jyBuaMOibiB04bqhbyBzw6JuIGLDs25nIMSRw6EsODg4NDk5ODM5LDEwLjc1NzkzNjA5LDEwNi42OTMzODgyLGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE0yMUtiNWxJekt0SnNZSEt3Q0U3WDlQT2ZkcFdGVHRQWDdEUkFtPXcxNTYtaDExNC1wLWstbm8sNzksNzczLDE5MzQyLDExIMSQLiBC4bq/biBWw6JuIMSQ4buTbg0KU0IyOCxTw6JuIFRo4buDIHRoYW8gxJBhIG7Eg25nIExhbSBTxqFuLDI4NzMwNTkwODgsMTAuNzYyMzI1NTEsMTA2LjY3OTgyNzksaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwTU90aEVFUUtSM2JFMTZzTV9VWGpCY2o1b19fWTZ3MlR0UzNmZ0k9dzQwOC1oMzA2LWstbm8sNzksNzc0LDE5MzQ1LDEgVHLhuqduIELDrG5oIFRy4buNbmcNClNCMjksVGjhu4MgVGhhbyBQaG9uZyBTxqFuLDkwOTIyMjk1OCwxMC43NTg5MzMyNiwxMDYuNjgxMTk0MixodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBNam5OZmF3Yi02RDhLYzJRR1VrdWUtR0NrQkxPMkV6bDJsUUdtbD13NzIwLWgyNDAtay1ubyw3OSw3NzQsMTkzNDUsMzIwLzEgVHLhuqduIELDrG5oIFRy4buNbmcNClNCMzAsU8OibiBiYW5oIEjDuW5nIFbGsMahbmcsOTM4NTg5NTA5LDEwLjc1NzQwMDUzLDEwNi42NjMzMzI2LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE1EV1BwaVMyR2poTEFiNEpTeklxTEthekVQZG01bU9ySGZRZGtyPXc0MDgtaDI0MC1rLW5vLXBpMS41NzYyOTE0LXlhMzkuMjYwMTQtcm8tNC42MDU0MjEtZm8xMDAsNzksNzc0LDI0Nzc1LFFNNDcrWDg4DQpTQjMxLFPDom4gQsOzbmcgxJDDoSBNaW5pIEhvw6BuZyBQaGksMjg2NjczODE2MywxMC43NDQ1NTkyMSwxMDYuNjIzODEyMSxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBQbjA5RlRxemhwcUtMSlUteExmWHBKeFpBRHVuSFNqSVpYNjlRPXcxNTYtaDExNC1wLWstbm8sNzksNzc1LDI0Nzk5LDQ3NyBBbiBELiBWxrDGoW5nDQpTQjMyLFPDom4gY+G7jyBuaMOibiB04bqhbyBI4bqjaSBMb25nLCwxMC43NTYzMzcxMiwxMDYuNjMyMzUxMyxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBOcXk5eWRrN2RzRXdOTmF4TV81X2dieWhEOE5GbGg5MXVRVExjZD13MTU2LWgxMTQtcC1rLW5vLDc5LDc3NSwyNjg3MiwzMzQgVMOibiBIw7JhIMSQw7RuZw0KU0IzMyxTw6JuIEPhu48gTmjDom4gVOG6oW8gVsaw4budbiBMYW4sOTE5MzQ1NDgyLDEwLjczOTA1NzE5LDEwNi42Mjg1MDU5LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE5VYlVSVnpuckVtdmpTeDBqbEhnakExd3ZIelVteXlQLWRHa2NLPXc0MDgtaDMwNi1rLW5vLDc5LDc3NSwyNDc5NiwyMSDEkMaw4budbmcgU+G7kSAzNA0KU0IzNCxUcnVuZyBUw6JtIFRo4buDIEThu6VjIFRo4buDIFRoYW8gNiwyODM3NTUxOTU0LDEwLjc0NDA1MTg0LDEwNi42MzMwOTQ1LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE9rTEt3VnBCM2dJU2E2dUM2LTYzOWxHcWZjaENhR09wT25POGw3PXc0MDgtaDI1OC1rLW5vLDc5LDc3NSwyNDc5OSwixJDGsOG7nW5nIFPhu5EgMTAsIELDrG5oIFBow7osIGtodSBEw6JuIEPGsCINClNCMzUsQ+G7jyBOaMOibiBU4bqhbyBCw7NuZyDEkMOhLDkzODQ4NTExMiwxMC43NTQ0Njg3NywxMDYuNjMyODM1OCxodHRwczovL21hcHMuZ3N0YXRpYy5jb20vdGFjdGlsZS9wYW5lL2RlZmF1bHRfZ2VvY29kZS0xeC5wbmcsNzksNzc1LDI2ODcyLDIyIELDoCBIb20NClNCMzYsU8OibiBCYW5oIFBoxrDGoW5nIFRo4bqjbyBxdWFuIDcsOTA5MTAxMjAxLDEwLjcxMTQ1MTc2LDEwNi43NDI2MjY5LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE5kZ1M5X1FWdnpDVW5GR3ZWVkJzYllld2pYNFpvMTdXanA4NjNhPXc0MDgtaDMwNi1rLW5vLDc5LDc3OCwyNTc2MiwyMDYvNiDEkMOgbyBUcsOtDQpTQjM3LFPDom4gYsOzbmcgxJHDoSBC4bq/IFbEg24gQ+G6pW0sOTM3MzYxMjY4LDEwLjc1MDY1NzM2LDEwNi43MTEyMjY2LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE9nZ01KSW9XbkhqcW9lVHBsOFZxLTgyT3R2LV9xWG15dU13MUVwPXc1MzItaDI0MC1rLW5vLDc5LDc3OCwyNzQ3MiwyNyBC4bq/IFbEg24gQ+G6pW0NClNCMzgsTMOibSBIw7luZyw5ODExMzk5NTUsMTAuNzQ5ODA4MzQsMTA2LjczMjk4MTEsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwT3E0TFotYWFsaUZseVY3SVA4N3djYnV4bVl2TVRKdlc2SXVkVFI9dzE1Ni1oMTE0LXAtay1ubyw3OSw3NzgsMjc0NjYsMTM2YSBCw7lpIFbEg24gQmENClNCMzksVGjhu4MgVGhhbyBTcG9ydHMgUGFyayw5MzE4Njc4ODAsMTAuNzMwODg4MTMsMTA2LjcyNjI1ODYsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwTzdMcUtKYnJVNGhiQlJEYjhleUgyVWhWODN4T3RvSWVuTFQza1Q9dzE1Ni1oMTE0LXAtay1ubyw3OSw3NzgsMjUxOTUsN2INClNCNDAsQ8OidSBM4bqhYyBC4buZIELDs25nIMSQw6EgUGjDuiBN4bu5LDkzMjM4NjQ4NiwxMC43Mzc4MTc5MiwxMDYuNzI4NDI1MixodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBOSjNTdVVaUWEtWjBJYVUxeHRTUFBPY1YtcVc1R29fSlZvbjFmQj13NDI2LWgyNDAtay1ubyw3OSw3NzgsMjAyNTQsMzggTmd1eeG7hW4gVGjhu4sgVGjhuq1wDQpTQjQxLFPDom4gTmd1eeG7hW4gVGjhu4sgxJDhu4tuaCw5MDkzODQ3MDYsMTAuNzI1MjQ2NTMsMTA2LjYyMTI5OTcsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwTnhXbjk5c3pxaGVCRE1TX1c5V0tTcHJiVXVIVF9QWWJaaVVUQjU9dzQwOC1oMzA2LWstbm8sNzksNzc2LDI2ODc4LCJUcsaw4budbmcgVEhQVCBDaHV5w6puIE7Eg25nIEtoaeG6v3UgVERUVCBOZ3V54buFbiBUaOG7iyDEkOG7i25oLCDEkMaw4budbmcgU+G7kSA0MSINClNCNDIsU8OibiBiw7NuZyDEkcOhIG1pbmkgVuG6oW4gTmd1ecOqbiw5MDgxODAxOTAsMTAuNzI3Njc2OTUsMTA2LjYyOTk2NzMsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwUFhhWjUxdEpod1NTNEE2aUg1YkowVk1wNUxqa1V3WmhvZEluM2o9dzQwOC1oMzA2LWstbm8sNzksNzc2LDI2ODc4LDExOC8yOS8xNS82IFBow7ogxJDhu4tuaA0KU0I0MyxTw6JuIGJvzIFuZyBDYW8gTMO0zIMsOTMyNDAwMDM4LDEwLjczMzg1OTE0LDEwNi42ODIwNDc3LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE5YbXpKekllNjM0cnp1V0FLUl8xMXMwRzNvTXBSbG5PRFI3bUwzPXc0MDgtaDMwNi1rLW5vLDc5LDc3NiwyMjAyNCwiMTcgxJDGsOG7nW5nIEM3QywgQsOsbmggSMawbmciDQpTQjQ0LFPDom4gYsOzbmcgVHJ1bmcgU8ahbiw5MDUyNTUwMDgsMTAuNzQwNTk2NzUsMTA2LjY4NTA4ODQsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwUDZZSGd2VktKX3pQZldOdGh1czRYMUtKSzNHVnowVmMydk5uTkQ9dzQ5My1oMjQwLWstbm8sNzksNzc2LDE5MzU0LMSQLiBT4buRIDkNClNCNDUsU8OibiBCw7NuZyDEkcOhIFRp4buDdSBOZ8awLDI4NzMwMjM3OTksMTAuNzc0MTQzODQsMTA2LjY2OTgzODMsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwTndjaS1KWE1LSjlGeE5sN29PaVIzLTZVWXlOT1ozczZqY1pxVVM9dzQyNi1oMjQwLWstbm8sNzksNzcxLDI0Nzc1LDc4MC8xNGUgU8awIFbhuqFuIEjhuqFuaA0KU0I0Nixzw6JuIGLDs25nIEvhu7MgSG/DoCwzNzg5NzkyODcsMTAuNzc1ODQ2MTEsMTA2LjY2ODcyOTYsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwT3A4VTMtanZlak92SnBQRTlJdEw1eDZfaGwyT0QyamFZdWRJRnQ9dzQyNi1oMjQwLWstbm8sNzksNzcxLDI0Nzc1LEjhurttIDgyNCBTxrAgVuG6oW4gSOG6oW5oDQpTQjQ3LFPDom4gQsOzbmcgxJDDoSBtaW5pIE5ndXnhu4VuIFRyaSBQaMawxqFuZyw5MDYyMjA1MzcsMTAuNzgxMzAzODMsMTA2LjY2MTI4NTYsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwT2N1V1JyZ2JkUTZYektYMnQ3RnQ1RjJscFhyYktVSU51aWlqbDI9dzQwOC1oMzA2LWstbm8sNzksNzcxLDI2ODY5LEYyIGJpcyDEkOG7k25nIE5haQ0KU0I0OCxTw6JuIGLDs25nIMSRw6EgTW9iaVNwb3J0cyw5MDM4NTI0MTksMTAuNzgxNzc2NzQsMTA2LjY2MDE3NTMsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwTU9zSUdoNEZHeVZFTmd4WVFPTFpvVDB5VWpoQWFEU2NCdkV5MHE9dzQwOC1oNTQ0LWstbm8sNzksNzcxLDI2ODY5LDI3IELhuq9jIEjhuqNpDQpTQjQ5LFPDom4gQsOzbmcgxJDDoSBNaW5pIFRo4buRbmcgTmjhuqV0LDI4NzMwNTMwODgsMTAuNzYwMDE4NiwxMDYuNjY0MDQwNCxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBPdm9VZlk3cDlwY1pxSzVUNmdmWlN2dTFDYnJjdGpyRXg1RVBJdj13NDI2LWgyNDAtay1ubyw3OSw3NzEsMjIwMzksMTM4IMSQw6BvIER1eSBU4burDQpTQjUwLFPDom4gQsOzbmcgxJDDoSBNaW5pIFRow6BuaCBQaMOhdCw5MDM2NDE2MjIsMTAuNzY2OTIwNTksMTA2LjY1NDgxODUsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwT05maTJrbG9jNUJsV0hkUmpGX2tRVWNvVjZiaUgya3o5WkN6UGE9dzQwOC1oMzA2LWstbm8sNzksNzcyLDI2ODY5LDQgxJAuIEzDqiDEkOG6oWkgSMOgbmgNClNCNTEsU8OibiBmdXRzYWwgUXVhbmcgVHV54bq/biw5NjMzMDM3MzcsMTAuNzY3MTA5OTcsMTA2LjY1ODU5NTYsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwTkozU3VVWlFhLVowSWFVMXh0U1BQT2NWLXFXNUdvX0pWb24xZkI9dzQyNi1oMjQwLWstbm8sNzksNzcxLDI2ODY5LDIxOSBMw70gVGjGsOG7nW5nIEtp4buHdA0KU0I1MixTw6JuIDcgUGjDuiBUaOG7jSw5MDgxMDcxMzIsMTAuNzY4NDc0MiwxMDYuNjU0OTQ5LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE83aWhtMTNPMU51bnJ5TDZiSlRSU19nT0VGWHFMa1Z6MHRZMGc9dzQyNi1oMjQwLWstbm8sNzksNzcyLDI2ODY5LDIgxJAuIEzDqiDEkOG6oWkgSMOgbmgNClNCNTMsU8OibiBiw7NuZyDEkcOhIG1pbmkgTmjDoCBU4bqtcCBMdXnhu4duIFBow7ogVGjhu40sOTc4NDk0NTQwLDEwLjc2NzU2ODA4LDEwNi42NTgzODI4LGh0dHBzOi8vZHVvbmdtaW5oZmMuY29tL2ltYWdlcy9hbmhib25nZGEvbG9wLWJvbmctZGF5LWNoby10cmUtZW0tY3MtY2VsYWRvbi1jaXR5XzgwMC5qcGcsNzksNzcyLDI2ODY5LDIxOSBMw70gVGjGsOG7nW5nIEtp4buHdA0KU0I1NCxTw6JuIELDs25nIEzhuqFjIExvbmcgUXXDom4sOTAyOTk5NTQ1LDEwLjc2MjIwNTU5LDEwNi42NTEwNDI4LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE1MekdNRjBzVmtHQUd0QVZpRW1NWE84czRrRGVheHZIR0NkOUVEPXc1MDYtaDI0MC1rLW5vLDc5LDc3MiwyMjAxOCwyNTggTMOyIFNpw6p1DQpTQjU1LFPDom4gQsOzbmcgU8OgaSBHw7JuIEZDLDk2NTA5MzU3NSwxMC44NTkzMTg5NSwxMDYuNjMxMzIzOSxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBPV1lKaTNXZXpSMlI2TUVFRU1rZF9TMmxVdGEtaHpLank5dUtfUj13NDA4LWg3MjUtay1ubyw3OSw3NjEsMjY3ODcsNTIgRC4gVGjhu4sgTcaw4budaQ0KU0I1NixTw6JuIGLDs25nIMSRw6EgbWluaSBUaOG7kW5nIE5o4bqldCAyLDkyODAzNzc3NywxMC44NzQ5NjQ0NiwxMDYuNjQxOTEyMyxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBNOGxmWWQyWDRXMDFUSkh2V3FUWVVMamYtX01nR282R0NhYngzej13NDA4LWgzMDYtay1ubyw3OSw3NjEsMjU3NDEsxJDGsOG7nW5nIFRy4bqnbiBUaOG7iyBEbw0KU0I1NyxTw6JuIELDs25nIMSQw6EgxJDhuqFpIE5hbSBXZVNwb3J0LDkwOTc5NzQ4MiwxMC44NDUwNzYyMiwxMDYuNjMwNDgyMixodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBPMndiMkFhZlBWX2paU2ZhQUpKZDM1cjByQ2VwRUtiVUhvbmtwXz13NDA4LWg3MjUtay1ubyw3OSw3NjEsMjY3ODgsMzgvNDAgxJDDtG5nIEjGsG5nIFRodeG6rW4gMTENClNCNTgsU8OibiBCw7NuZyBMYW4gQW5oLDM0NDU4MTU4MSwxMC44MzU3MzU5NywxMDYuNjEzMDgzNyxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBOUFFpazVqS0VDb3JaeGJrV0NSODFQR3JMc1pOY2xlQnNxRmZwSz13NDA4LWgyNjMtay1ubyw3OSw3NjEsMjY3OTEsNzAgxJDGsOG7nW5nIFTDom4gVGjhu5tpIE5o4bqldCAwMg0KU0I1OSxTw6JuIELDs25nIMSQw6EgQ8OieSBTdW5nIDM3OSwyODczMDkwNzk5LDEwLjg3NTg3MzQ5LDEwNi42MTcyMzk2LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE9OQ3laNmVfQXVBcFpITFJuZlc2djhsZ2tPOWMteGxqQ3RmUXc9dzQwOC1oNzI1LWstbm8sNzksNzYxLDI2Nzc2LMSQxrDhu51uZyDEkC4gTmd1eeG7hW4g4bqibmggVGjhu6cNClNCNjAsU8OibiBiw7NuZyDEkcOhIG1pbmkgVMO9IMSQw7QsOTA2MzEyMzc5LDEwLjg1OTg5NDksMTA2LjY1MjY4NjEsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwTkZFR2pFdC1seE0yeHgtQVlWQzl2X2h0MUJ0cE9jWm5JUFBsQXA9dzQwOC1oMzA2LWstbm8sNzksNzY0LDI2ODcyLDE0MjYgxJDGsOG7nW5nIHPhu5EgNw0KU0I2MSxTw6JuIELDs25nIMSQw6EgQ+G7jyBOaMOibiBU4bqhbyBRdWFuZyBUdXnhur9uIDcsMjg2NjcxNzc2MiwxMC44NDczNzU5NSwxMDYuNjQ1ODI1LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE5ZVkp0WmJFeU43Z3BMWVJpem9kbUptZXQ3YUh4TnhDRVNiaUgzPXc0MDgtaDI3MS1rLW5vLDc5LDc2NCwyNjg4Miw1OS8xIFBo4bqhbSBWxINuIENoacOqdQ0KU0I2MiwiU8OibiBiYW5oIG1pbmkgUXVhbmcgVHJ1bmcsIEfDsiBW4bqlcCIsOTM4MzA3NTg2LDEwLjgyODc0NzczLDEwNi42NjgwNTc5LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE4tUGJNTFRrS0w0RTlpWXB6dDZsYnUxWm5MOGRSZGx2VTdwQ1prPXc0MjYtaDI0MC1rLW5vLDc5LDc2NCwyNDc5NiwzMy8zIFF1YW5nIFRydW5nDQpTQjYzLFPDom4gYsOzbmcga2hhbmcgYW4sOTMxMTUxNjE2LDEwLjgzMzQ5MDQxLDEwNi42NzI0MzA1LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE81SklraVFBS0h2UTd0Mm5QZTFad05mQlI0NjBIZ1RacS0yM2NlPXc0MDgtaDMwNi1rLW5vLDc5LDc2NCwyNDc5NiwxOEEgxJAuIFBoYW4gVsSDbiBUcuG7iw0KU0I2NCxTw6JuIGLDs25nIMSRw6EgQW4gSOG7mWksMjgyMjEzNjQ2OCwxMC44Mzc5NTEwOCwxMDYuNjM5NTI0MSxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBQanZweFlzV0FWOFZrbHFnYUExQ1B2V2I2c1haX3V4U0pyZFduVD13NDA4LWgzMDYtay1ubyw3OSw3NjQsMjQ3NzUsMjU2IFBoYW4gSHV5IMONY2gNClNCNjUsU8OibiBCw7NuZyDEkMOhIE1pbmkgUXV54buBbiDEkMOgbyBEdXkgQW5oLDkwNzA3MTc3NSwxMC44MDU1NDg5NSwxMDYuNjc1ODMyMSxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBQY09ITlgxSkJfVU1vS2lTb21aNzNCYkhBdWRmM0pYdWZHSmVGQT13NDA4LWgzMDYtay1ubyw3OSw3NjgsMjIwMjQsMjEgxJDDoG8gRHV5IEFuaA0KU0I2NixTw6JuIGLDs25nIG1pbmkgVHLhuqduIEvhur8gWMawxqFuZywzNjk5NzkxOTEsMTAuODAyNzMxMjUsMTA2LjY4NzExMyxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBNY3BqVkIyTU15WGRydGE2S3hyTzlXckk3QTAwejF5QjgwWTZVRT13NDA4LWgzMDYtay1ubyw3OSw3NjgsMjIwMzYsODdBIFRy4bqnbiBL4bq/IFjGsMahbmcNClNCNjcsU8OibiBj4buPIE5ow6JuIHThuqFvIFRyxrDhu51uZyBUSFBUIFBow7ogTmh14bqtbiw5NDY2Mjg5NDYsMTAuODA5MTE5MywxMDYuNjc2MzE4OSxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBONXprVVRiNFpRenhQQkRTV1dhaGhlZW9mSGRZQm9qZGxETWlGSj13NDI2LWgyNDAtay1ubyw3OSw3NjgsMjIwMjQsNSBIb8OgbmcgTWluaCBHacOhbQ0KU0I2OCxTw6JuIGLDs25nIMSRw6EgTWluaSBUaMSDbmcgTG9uZyw5Mjc3Nzc5NzksMTAuODA0MTI5NjcsMTA2LjY2MTM1OCxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBOX09xNXlFc21wbzluS2xjaFhySVYzVko5MTl0dGlseldvYy1KMj13NDA4LWgzMDYtay1ubyw3OSw3NjYsMTkzNDUsMUEgUGhhbiBUaMO6YyBEdXnhu4duDQpTQjY5LFPDom4gQsOzbmcgMjMwLDkyNTQxODg4OCwxMC44MTg0OTMwMiwxMDYuNjM4NTMwMyxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBPMlFRSDFOdlpDQ2E5SUtWMmx5bS1Jb3pGZ1NwZVNFdlAxa0d3eT13NDU3LWgyNDAtay1ubyw3OSw3NjYsMjY4NjksMyBUw6JuIFPGoW4NClNCNzAsU8OibiBiw7NuZyBtaW5pIGNo4bqjbyBs4butYSAyMCBD4buZbmcgSMOyYSw5MDE1MDA3NzcsMTAuODAyMzA1MjUsMTA2LjY1MzA1NzUsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwT3BkMjZNM2h4YXFhVTVXNnEweklUby1QcDJpSGRzaVYwdlcyTXY9dzQwOC1oMzA2LWstbm8sNzksNzY2LDE5MzQ1LDIwIMSQLiBD4buZbmcgSMOyYQ0KU0I3MSxTw6JuIGLDs25nIMSRw6EgbWluaSBLMzM0LDI4NzMwNTIwODgsMTAuODEwMzk4MywxMDYuNjQ0Mzc2NCxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBPclhocUpacmRGN05aVkNBVjg4ZWpKWERIZTlkcFVoaXpqN2lrbT13NDA4LWgyNzEtay1ubyw3OSw3NjYsMjY4NjksMTE5IFRy4bqnbiBWxINuIETGsA0KU0I3MixTw6JuIELDs25nIMSQw6EgTWluaSBQaMO6YyBZw6puLDkyMjE2OTE2OSwxMC44MjY3MTkxNiwxMDYuNjMwNDU1MSxodHRwczovL2xoNS5nb29nbGV1c2VyY29udGVudC5jb20vcC9BRjFRaXBNOEVDUmhGVWppT1ZEbWlySmhkbkM2Q2luM2ZWUk1FM2owcE5XUT13NDA4LWgzMDYtay1ubyw3OSw3NjYsMjY4NjksNDNBIFBoYW4gSHV5IMONY2gNClNCNzMsU8OibiBCw7NuZyDEkMOhIFNwb3J0IFBsdXMgV2VTcG9ydCw5MzM4NTg1NzksMTAuODA1MDU1MzgsMTA2LjYxMTc5MzgsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwT240c3h5TDY5ZVNzSUdTTGktRHExN08wOHpBbV9uZlI0TUhGdXI9dzQ1Ny1oMjQwLWstbm8sNzksNzY3LDI3MDE2LDMxLzIgxJAuIEvDqm5oIDE5LzUNClNCNzQsU8OibiBCw7NuZyDEkMOhIEdpYSBOZ3V54buFbiwyODM5NjE4Njg0LDEwLjc2OTI0OTk2LDEwNi42MjM5NDQ4LGh0dHBzOi8vbGg1Lmdvb2dsZXVzZXJjb250ZW50LmNvbS9wL0FGMVFpcE5UVzJHRklrc3pDRHAwVGc0UjZjb1BpRkxabmstN2l5VFF0bXVDPXc0MjYtaDI0MC1rLW5vLDc5LDc2NywyNTYzMywyNTIgUGhhbiBBbmgNClNCNzUsU8OibiBCw7NuZyDEkMOhIE1pbmkgSGnhu4dwIFBow6F0LDI4NzMwNTk0ODgsMTAuODA2NDUxNTQsMTA2LjYzMDU1MzcsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwTW5VZ2ktM3pKUWVnSmYweDlzdTRyWDFrWnJvNU5ia2NhM3NCY1M9dzQwOC1oMjY1LWstbm8sNzksNzY3LDI2ODY5LDE4IE5ndXnhu4VuIFPDoW5nDQpTQjc2LEhvYSBUaGFuaCBNaW5pIEZvb3RiYWxsIEdyb3VuZCw5MDc2ODYxNjIsMTAuNzc5MzEyNjMsMTA2LjYzODY5OTksaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwTXVVd3ZZRHNWT0lVTlNJbmd1LWZzMjEtcEZNbGN3VVYwaXJFN0s9dzQwOC1oMjcyLWstbm8sNzksNzY3LDI3MDMxLCIxMTgvOCBIw7JhIFRoYW5oLCBIdeG7s25oIFRoaeG7h24gTOG7mWMiDQpTQjc3LFPDom4gYsOzbmcgVGhhbmggVGhp4buHbiw5MDk4MDQ2OTYsMTAuODE0MjE5NzcsMTA2LjYyNjg4NDMsaHR0cHM6Ly9saDUuZ29vZ2xldXNlcmNvbnRlbnQuY29tL3AvQUYxUWlwTmxHdXVQenBRa0JOMWMzRUpZTHl0bE1wM0tPdXhZWlo5dUlYMEw9dzQyNi1oMjQwLWstbm8sNzksNzY3LDI3MDEzLDE2IMSQxrDhu51uZyAxNg0K";

            var admin = await this.db.Users.FirstOrDefaultAsync(o => o.IsAdmin && o.Id == this.current.UserId);
            ManagedException.ThrowIf(admin == null, Messages.System.System_Error);

            var hasData = await this.db.Stadiums.AnyAsync(o => o.UserId == admin!.Id);
            ManagedException.ThrowIf(hasData, Messages.Stadium.Stadium_InitFounded);

            List<ImportStadiumDto> stadiums;
            UTF8Encoding enc = new();
            using (var reader = new StringReader(enc.GetString(System.Convert.FromBase64String(data))))
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

                    await this.db.AddAsync(item);
                    await this.fileService.Create(new Models.File.FileDto() {
                        ItemId = item.Id,
                        ItemType = EItemType.StadiumAvatar,
                        Type = EFile.Image,
                        Data = file,
                        Name = $"{stadium.Name.GetSumary()}_{stadium.Code}.jpg"
                    });
                    Console.WriteLine($"Done: {stadium.Code}\n");
                } catch (Exception) {
                    continue;
                }
            }
        }
    }
}