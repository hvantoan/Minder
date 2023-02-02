using Minder.Database.Enums;
using Minder.Database.Models;
using Minder.Service.Models.File;
using Minder.Services.Models;
using System.Threading.Tasks;

namespace Minder.Service.Interfaces {

    public interface IFileService {

        Task<FileDto?> Get(string id, EItemType itemType, EFile type = EFile.Image);

        Task CreateOrUpdate(FileDto model);

        Task Delete(string id, File? entity);

        Task<FileResult> DownLoad(string id);
    }
}