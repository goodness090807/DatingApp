using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace API.Interfaces
{
    public interface IPhotoService
    {
        /// <summary>
        /// 新增照片功能
        /// </summary>
         Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        /// <summary>
        /// 刪除照片功能
        /// </summary>
         Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}