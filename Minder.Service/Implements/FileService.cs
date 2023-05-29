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
using System.Linq;
using System.Threading.Tasks;
using TuanVu.Services.Helpers;

namespace Minder.Service.Implements {

    public class FileService : BaseService, IFileService {

        public FileService(IServiceProvider serviceProvider) : base(serviceProvider) {
        }

        public async Task<FileDto?> Get(string id, EItemType itemType, EFile type = EFile.Image) {
            var file = await this.db.Files.AsNoTracking().OrderByDescending(o => o.UploadDate)
                .FirstOrDefaultAsync(o => o.Type == type && o.ItemType == itemType && o.ItemId == id);

            return FileDto.FromEntity(file, this.current.Url);
        }

        public async Task Create(FileDto model) {
            if (!string.IsNullOrEmpty(model.ImportUrl)) {
                try {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                    byte[] file = new System.Net.WebClient().DownloadData(new Uri(model.ImportUrl));
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                    model.Data = file;
                } catch (Exception) {
                }
            }

            if (model.Data == null || model.Data.Length == 0)
                return;
            this.logger.Information($"{nameof(FileService)} - {nameof(Create)} - Start", model);

            File entity = new() {
                Id = Guid.NewGuid().ToStringN(),
                Type = model.Type,
                ItemId = model.ItemId,
                ItemType = model.ItemType,
                Name = model.Name,
                UploadDate = DateTimeOffset.Now,
            };

            entity.Path = await this.UploadFile(entity, model.Data);
            await this.db.AddAsync(entity);
            await UpdateFileType(entity.ItemType, entity.ItemId);

            await this.db.SaveChangesAsync();
            this.logger.Information($"{nameof(FileService)} - {nameof(Create)} - End", model);
        }

        private async Task UpdateFileType(EItemType type, string itemId) {
            var query = this.db.Files.AsQueryable();
            switch (type) {
                case EItemType.UserAvatar:
                    query = query.Where(o => o.ItemType == EItemType.UserAvatar && o.ItemId == itemId);
                    break;

                case EItemType.UserCover:
                    query = query.Where(o => o.ItemType == EItemType.UserCover && o.ItemId == itemId);
                    break;

                case EItemType.TeamAvatar:
                    query = query.Where(o => o.ItemType == EItemType.TeamAvatar && o.ItemId == itemId);
                    break;

                case EItemType.TeamCover:
                    query = query.Where(o => o.ItemType == EItemType.TeamCover && o.ItemId == itemId);
                    break;

                case EItemType.StadiumAvatar:
                    query = query.Where(o => o.ItemType == EItemType.StadiumAvatar && o.ItemId == itemId);
                    break;

                case EItemType.GroupAvatar:
                    query = query.Where(o => o.ItemType == EItemType.GroupAvatar && o.ItemId == itemId);
                    break;
            }

            var files = await query.ToListAsync();
            switch (type) {
                case EItemType.UserAvatar:
                case EItemType.UserCover:
                    files.ForEach(o => o.ItemType = EItemType.UserImage);
                    break;

                case EItemType.TeamAvatar:
                case EItemType.TeamCover:
                    files.ForEach(o => o.ItemType = EItemType.TeamImage);
                    break;

                case EItemType.StadiumAvatar:
                    files.ForEach(o => o.ItemType = EItemType.StadiumImage);
                    break;

                case EItemType.GroupAvatar:
                    files.ForEach(o => o.ItemType = EItemType.GroupImage);
                    break;
            }
        }

        public async Task Update(FileDto model) {
            this.logger.Information($"{nameof(FileService)} - {nameof(Update)} - Start", model);
            var entity = await this.db.Files.FirstOrDefaultAsync(o => o.Id == model.Id);
            ManagedException.ThrowIf(entity == null, Messages.File.File_Error);

            entity.Name = model.Name;
            entity.UploadDate = DateTimeOffset.Now;

            await FtpHelper.DeleteFile(entity.Path, this.configuration);
            entity.Path = await this.UploadFile(entity, model.Data);

            this.db.Files.Update(entity);
            this.logger.Information($"{nameof(FileService)} - {nameof(Update)} - End", model);
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
                EItemType.TeamAvatar => "team-avatar",
                EItemType.TeamCover => "team-cover",
                EItemType.StadiumAvatar => "stadium-avatar",
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