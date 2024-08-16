using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Response;

namespace Curus.Service.Interfaces;

public interface IDiscountService
{
    Task<UserResponse<object>> CreateDiscount(CreateDiscountDTO createDiscountDto);

    Task<UserResponse<object>> SendDiscount(SendDiscountDTO sendDiscountDto,int id);
    
    Task<UserResponse<object>> UseDiscountForCourse(int id, DiscountCourseDTO discountCourseDto);
    
}