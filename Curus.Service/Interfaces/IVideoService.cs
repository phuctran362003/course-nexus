using Microsoft.AspNetCore.Http;

namespace Curus.Service.Interfaces;

public interface IVideoService
{
    Task<TimeSpan> GetVideoDuration(IFormFile videoFile);
}