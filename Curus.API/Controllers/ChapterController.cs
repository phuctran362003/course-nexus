using Curus.Repository.ViewModels;
using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Curus.API.Controllers;

[ApiController]
[Route("api/chapter")]
[Authorize(Policy = "InstructorPolicy")]
public class ChapterController : ControllerBase
{
    private readonly IChapterService _chapterService;

    public ChapterController(IChapterService chapterService)
    {
        _chapterService = chapterService;
    }
    /// <summary>
    /// Creates a new chapter.
    /// </summary>
    [HttpPost("chapters")]
    public async Task<IActionResult> CreateChapter([FromForm] ChapterDTO chapterDto)
    {
        var chapter = await _chapterService.CreateChapter(chapterDto);
        return Ok(chapter);
    }

    /// <summary>
    /// Updates a specific chapter by ID.
    /// </summary>
    [HttpPut("chapters/{id}")]
    public async Task<IActionResult> UpdateChapter(int id, [FromForm] UpdateChapterDTO updateChapterDto)
    {
        var chapter = await _chapterService.UpdateChapter(id, updateChapterDto);
        return Ok(chapter);
    }

    /// <summary>
    /// Updates the order of a specific chapter by ID.
    /// </summary>
    [HttpPut("chapters/{id}/order")]
    public async Task<IActionResult> UpdateOrderChapter(int id, [FromForm] UpdateOrderChapterDTO updateOrderChapterDto)
    {
        var chapter = await _chapterService.UpdateOrderChapter(id, updateOrderChapterDto);
        return Ok(chapter);
    }

    /// <summary>
    /// Deletes a specific chapter by ID.
    /// </summary>
    [HttpDelete("chapters/{id}")]
    public async Task<IActionResult> DeleteChapterById(int id)
    {
        var chapter = await _chapterService.DeleteChapterById(id);
        return Ok(chapter);
    }


}