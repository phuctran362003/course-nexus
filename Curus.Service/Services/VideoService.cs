

using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Xabe.FFmpeg;

namespace Curus.Service.Services;

public class VideoService : IVideoService
{
    private readonly string _ffmpegPath;

    public VideoService()
    {
        // Set the full path to the directory containing ffmpeg.exe
        _ffmpegPath = "C:\\Users\\ADMIN\\Downloads\\ffmpeg-7.0.1-full_build\\bin";

        if (string.IsNullOrEmpty(_ffmpegPath))
        {
            throw new ArgumentException("FFmpeg path is not set.");
        }
        FFmpeg.SetExecutablesPath(_ffmpegPath);
    }

    public async Task<TimeSpan> GetVideoDuration(IFormFile videoFile)
    {
        if (videoFile == null || videoFile.Length == 0)
        {
            throw new ArgumentException("Video file is not provided or is empty.");
        }

        var tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Path.GetExtension(videoFile.FileName));
        try
        {
            // Save the uploaded file to disk
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await videoFile.CopyToAsync(stream);
            }

            // Get video information from the saved file
            var mediaInfo = await FFmpeg.GetMediaInfo(tempFilePath);
            return mediaInfo.Duration;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error processing video: {ex.Message}");
        }
        finally
        {
            // Delete the temporary file after processing
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }
}
