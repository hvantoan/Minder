using FluentFTP;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TuanVu.Services.Helpers {

    public class TvFtpConfig {
        public string Host { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public static class FtpHelper {

        public static async Task UploadImage(string[] directories, string filename, byte[]? data, IConfiguration configuration, string key = "Ftp") {
            await UploadFile(directories, filename, data, configuration.GetSection(key).Get<TvFtpConfig>()!);
        }

        public static async Task UploadFile(string[] directories, string filename, byte[]? data, IConfiguration configuration, string key = "Ftp") {
            await UploadFile(directories, filename, data, configuration.GetSection(key).Get<TvFtpConfig>()!);
        }

        public static async Task UploadFile(string[] directories, string filename, byte[]? data, TvFtpConfig config) {
            if (data == null) return;
            using (var client = new AsyncFtpClient(config.Host, config.Username, config.Password, config: new FtpConfig() { ValidateAnyCertificate = true })) {
                await client.AutoConnect();

                foreach (var directory in directories) {
                    var existed = await client.DirectoryExists(directory);
                    if (existed) continue;
                    await client.CreateDirectory(directory);
                }

                using (Stream stream = new MemoryStream(data)) {
                    await client.UploadStream(stream, filename);
                }
            }
        }

        public static async Task<byte[]> DownloadBytes(string filename, IConfiguration configuration, string key = "Ftp") {
            return await DownloadBytes(filename, configuration.GetSection(key).Get<TvFtpConfig>()!);
        }

        public static async Task<byte[]> DownloadBytes(string path, TvFtpConfig config) {
            using (var client = new AsyncFtpClient(config.Host, config.Username, config.Password, config: new FtpConfig() { ValidateAnyCertificate = true })) {
                await client.AutoConnect();
                var existed = await client.FileExists(path);
                if (!existed) return Array.Empty<byte>();
                return await client.DownloadBytes(path, 0);
            }
        }

        public static async Task<FtpStatus> DownloadFile(string localPath, string filename, IConfiguration configuration, string key = "Ftp") {
            return await DownloadFile(localPath, filename, configuration.GetSection(key).Get<TvFtpConfig>()!);
        }

        public static async Task<FtpStatus> DownloadFile(string localPath, string path, TvFtpConfig config) {
            using (var client = new AsyncFtpClient(config.Host, config.Username, config.Password, config: new FtpConfig() { ValidateAnyCertificate = true })) {
                await client.AutoConnect();
                var existed = await client.FileExists(path);
                if (!existed) return FtpStatus.Failed;
                return await client.DownloadFile(localPath, path, FtpLocalExists.Overwrite);
            }
        }

        public static async Task DeleteImage(string filename, IConfiguration configuration, string key = "Ftp") {
            await DeleteFile(filename, configuration.GetSection(key).Get<TvFtpConfig>()!);
        }

        public static async Task DeleteFile(string filename, IConfiguration configuration, string key = "Ftp") {
            await DeleteFile(filename, configuration.GetSection(key).Get<TvFtpConfig>()!);
        }

        public static async Task DeleteFile(string filename, TvFtpConfig config) {
            using (var client = new AsyncFtpClient(config.Host, config.Username, config.Password, config: new FtpConfig() { ValidateAnyCertificate = true })) {
                await client.AutoConnect();
                var existed = await client.FileExists(filename);
                if (!existed) return;

                await client.DeleteFile(filename);
            }
        }
    }
}