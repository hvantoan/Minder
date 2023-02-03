using Microsoft.EntityFrameworkCore;
using Minder.Database.Enums;
using Minder.Database.Models;
using Minder.Exceptions;
using Minder.Extensions;
using Minder.Service.Interfaces;
using Minder.Service.Models.File;
using Minder.Services.Common;
using Minder.Services.Models;
using Minder.Services.Resources;
using System;
using System.Threading.Tasks;
using TuanVu.Services.Helpers;

namespace Minder.Service.Implements {

    public class FileService : BaseService, IFileService {

        public FileService(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public async Task<FileDto?> Get(string id, EItemType itemType, EFile type = EFile.Image) {
            var file = await this.db.Files.AsNoTracking()
                .FirstOrDefaultAsync(o => o.Type == type && o.ItemType == itemType && o.ItemId == id);

            return FileDto.FromEntity(file, this.current.Url);
        }

        public async Task CreateOrUpdate(FileDto model) {
            if (model.Data == null || model.Data.Length == 0)
                return;

            if (string.IsNullOrWhiteSpace(model.Id)) {
                model.Id = Guid.NewGuid().ToStringN();
                await Create(model.Type, model.ItemType, model.ItemId, model);
                return;
            }
            await this.Update(model, null);
        }

        private async Task Create(EFile type, EItemType itemType, string itemId, FileDto model) {
            this.logger.Information($"{nameof(FileService)} - {nameof(Create)} - Start", model);

            File entity = new() {
                Id = model.Id ?? Guid.NewGuid().ToStringN(),
                Type = type,
                ItemId = itemId,
                ItemType = itemType,
                Name = model.Name,
                UploadDate = DateTimeOffset.Now,
            };
            entity.Path = await this.UploadFile(entity, model.Data);
            await this.db.AddAsync(entity);

            this.logger.Information($"{nameof(FileService)} - {nameof(Create)} - End", model);
            await this.db.SaveChangesAsync();
        }

        private async Task Update(FileDto model, File? entity) {
            this.logger.Information($"{nameof(FileService)} - {nameof(Update)} - Start", model);
            entity ??= await this.db.Files.FirstOrDefaultAsync(o => o.Id == model.Id);
            ManagedException.ThrowIf(entity == null, Messages.File.File_Error);

            entity.Name = model.Name;
            entity.UploadDate = DateTimeOffset.Now;

            await FtpHelper.DeleteFile(entity.Path, this.configuration);
            entity.Path = await this.UploadFile(entity, model.Data);

            this.db.Files.Update(entity);
            this.logger.Information($"{nameof(FileService)} - {nameof(Update)} - End", model);
            await this.db.SaveChangesAsync();
        }

        public async Task Delete(string id, File? entity) {
            entity ??= await this.db.Files.FirstOrDefaultAsync(o => o.Id == id);
            ManagedException.ThrowIf(entity == null, Messages.File.File_NotFound);

            await FtpHelper.DeleteFile(entity.Path, this.configuration);

            this.db.Files.Remove(entity);
            await this.db.SaveChangesAsync();
        }

        private static (string[], string) GetFilePath(File file) {
            string fileType = file.Type switch {
                EFile.Image => "images",
                EFile.Folder => "folder",
                _ => "temporary",
            };

            string itemType = file.ItemType switch {
                EItemType.UserAvatar => "user-avatar",
                EItemType.UserCover => "user-cover",
                EItemType.TeamAvata => "team-avatar",
                EItemType.TeamCover => "team-cover",
                _ => "other",
            };
            string extentions = System.IO.Path.GetExtension(file.Name);

            string[] directories = new string[] {
                $"{fileType}",
                $"{fileType}/{itemType}"
            };

            return (directories, $"{fileType}/{itemType}/{file.Id}{extentions}");
        }

        private async Task<string> UploadFile(File item, byte[]? data) {
            var (directories, filename) = GetFilePath(item);
            await FtpHelper.UploadFile(directories, filename, data, this.configuration);
            return filename;
        }

        public async Task<FileResult> DownLoad(string id) {
            var file = await this.db.Files.FirstOrDefaultAsync(o => o.Id == id);
            ManagedException.ThrowIf(file == null, Messages.File.File_NotFound);
            var data = await FtpHelper.DownloadBytes(file.Path, this.configuration);
            return new() { ByteArray = data, FileName = file.Name };
        }
    }
}