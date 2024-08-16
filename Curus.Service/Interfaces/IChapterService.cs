using Curus.Repository.Entities;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Response;

namespace Curus.Service.Interfaces;

public interface IChapterService
{
    Task<UserResponse<object>>  CreateChapter(ChapterDTO chapterDto);

    Task<UserResponse<object>> UpdateChapter(int id, UpdateChapterDTO updateChapterDto);
    Task<UserResponse<object>> UpdateOrderChapter(int id, UpdateOrderChapterDTO updateOrderChapterDto);
    Task<UserResponse<object>> DeleteChapterById(int id);
    Task<UserResponse<object>> StartChapterById(int id);

}