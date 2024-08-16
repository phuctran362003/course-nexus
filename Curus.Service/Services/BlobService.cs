using Google.Cloud.Storage.V1;
using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Curus.Service.Services
{
    public class BlobService : IBlobService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly IConfiguration _configuration; // Class-level field for configuration

        public BlobService(IConfiguration configuration)
        {
            _configuration = configuration; // Initialize the class-level field
            var keyFilePath = _configuration["GoogleCloudStorage:KeyFilePath"];
            _bucketName = _configuration["GoogleCloudStorage:BucketName"];

            if (string.IsNullOrEmpty(keyFilePath))
            {
                throw new ArgumentNullException(nameof(keyFilePath), "KeyFilePath is not configured.");
            }
            if (string.IsNullOrEmpty(_bucketName))
            {
                throw new ArgumentNullException(nameof(_bucketName), "BucketName is not configured.");
            }

            Console.WriteLine($"KeyFilePath: {keyFilePath}");
            Console.WriteLine($"BucketName: {_bucketName}");

            // Check if the key file path exists
            if (!File.Exists(keyFilePath))
            {
                throw new FileNotFoundException("The Google Cloud Storage key file was not found.", keyFilePath);
            }

            _storageClient = StorageClient.Create(Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(keyFilePath));
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var objectName = GetBlobName(file.FileName);
            using (var stream = file.OpenReadStream())
            {
                await _storageClient.UploadObjectAsync(_bucketName, objectName, null, stream);
            }

            // Generate signed URL
            var url = GetSignedUrl(objectName);
            return url;
        }

        private string GetBlobName(string fileName)
        {
            return $"{Guid.NewGuid()}_{fileName}";
        }

        private string GetSignedUrl(string objectName)
        {
            var keyFilePath = _configuration["GoogleCloudStorage:KeyFilePath"]; // Use class-level field
            var urlSigner = UrlSigner.FromServiceAccountPath(keyFilePath);  // Corrected to use keyFilePath
            var url = urlSigner.Sign(
                _bucketName,
                objectName,
                TimeSpan.FromHours(1),
                HttpMethod.Get);
            return url;
        }
    }
}
