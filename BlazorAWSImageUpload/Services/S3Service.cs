using Amazon.S3.Transfer;
using Amazon.S3;
using Microsoft.AspNetCore.Components.Forms;
using Amazon.S3.Model;

namespace BlazorAWSImageUpload.Services;

public class S3Service : IS3Service
{
    private readonly IAmazonS3 _s3Client;
    private const string BucketName = "fs-sinanfen-customer";

    public S3Service(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task<string> UploadFileAsync(IBrowserFile file)
    {
        var fileTransferUtility = new TransferUtility(_s3Client);

        using var memoryStream = new MemoryStream();
        await file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var fileKey = $"{Guid.NewGuid()}{Path.GetExtension(file.Name)}";

        await fileTransferUtility.UploadAsync(new TransferUtilityUploadRequest
        {
            InputStream = memoryStream,
            BucketName = BucketName,
            Key = fileKey
        });

        return $"https://{BucketName}.s3.eu-west-1.amazonaws.com/{fileKey}";
    }

    public async Task<List<string>> GetAllFilesAsync()
    {
        var request = new ListObjectsV2Request
        {
            BucketName = BucketName
        };

        var result = await _s3Client.ListObjectsV2Async(request);
        return result.S3Objects.Select(obj => GetFileUrl(obj.Key)).ToList();
    }

    public async Task<string> GetFileByKeyAsync(string fileKey)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = BucketName,
            Key = fileKey,
            Expires = DateTime.Now.AddMinutes(5) // URL will be valid for 5 minutes
        };

        return _s3Client.GetPreSignedURL(request);
    }

    private string GetFileUrl(string fileKey)
    {
        return $"https://{BucketName}.s3.eu-west-1.amazonaws.com/{fileKey}";
    }
}