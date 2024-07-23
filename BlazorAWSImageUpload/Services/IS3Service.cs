using Microsoft.AspNetCore.Components.Forms;

namespace BlazorAWSImageUpload.Services;

public interface IS3Service
{
    Task<string> UploadFileAsync(IBrowserFile file);
    Task<List<string>> GetAllFilesAsync();
    Task<string> GetFileByKeyAsync(string fileKey);
}
