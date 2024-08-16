using System.IdentityModel.Tokens.Jwt;
using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels.Request;
using Curus.Repository.ViewModels.Response;
using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Curus.Service.Services;

public class ReportFeedbackService : IReportFeedbackService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IReportFeedbackRepository _reportFeedbackRepository;
    private readonly IFeedBackRepository _feedBackRepository;

    public ReportFeedbackService(IHttpContextAccessor httpContextAccessor, IReportFeedbackRepository reportFeedbackRepository, IFeedBackRepository feedBackRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _reportFeedbackRepository = reportFeedbackRepository;
        _feedBackRepository = feedBackRepository;
    }

    public async Task<UserResponse<object>> reportReviewToAdmin(int id,ReportFeedbackRequest reportFeedbackRequest)
    {
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")
            .Last();

        if (token == null)
            throw new Exception("Token not found");

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            throw new Exception("Invalid token");

        var userId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "id").Value);
        
        var reportFeedback = await _reportFeedbackRepository.GetReportFeedbackByFeedbackId(id);
        if (reportFeedback != null)
        {
            return new UserResponse<object>("this feedback is already reported", null);
        }
        else
        {
            ReportFeedback newReportFeedback = new ReportFeedback()
            {
                ReportReason = reportFeedbackRequest.Reason,
                IsHidden = false,
                FeedbackId = id,
                UserId = userId
            };
            bool check = await _reportFeedbackRepository.CreateReportFeedback(newReportFeedback);
            if (!check)
            {
                return new UserResponse<object>("Something wrong when report feedback", null);
            }

            return new UserResponse<object>("Report this feedback success", null);
        }
    }

    public async Task<UserResponse<object>> acceptReportFeedback(int id)
    {
        var reportFeedback = await _reportFeedbackRepository.GetReportFeedbackById(id);
        if (reportFeedback == null)
        {
            return new UserResponse<object>("This reportFeedback is not available", null);
        }

        reportFeedback.IsHidden = true;
        reportFeedback.IsDelete = true;
        bool check = await _reportFeedbackRepository.EditReportFeedback(reportFeedback);
        if (!check)
        {
            return new UserResponse<object>("Something wrong", null);
        }

        var feedBack = await _feedBackRepository.GetFeedbackById(reportFeedback.FeedbackId);
        feedBack.IsDelete = true;
        bool feedbackCheck = await _feedBackRepository.UpdateFeedback(feedBack);
        if (!feedbackCheck)
        {
            return new UserResponse<object>("Something wrong when hide feedback", null);
        }

        return new UserResponse<object>("Success accept", null);
    }
    
    public async Task<UserResponse<object>> rejectReportFeedback(int id)
    {
        var reportFeedback = await _reportFeedbackRepository.GetReportFeedbackById(id);
        if (reportFeedback == null)
        {
            return new UserResponse<object>("This reportFeedback is not available", null);
        }

        
        reportFeedback.IsDelete = true;
        bool check = await _reportFeedbackRepository.EditReportFeedback(reportFeedback);
        if (!check)
        {
            return new UserResponse<object>("Something wrong", null);
        }
        
        var feedBack = await _feedBackRepository.GetFeedbackById(reportFeedback.FeedbackId);
        feedBack.IsDelete = false;
        bool feedbackCheck = await _feedBackRepository.UpdateFeedback(feedBack);
        if (!feedbackCheck)
        {
            return new UserResponse<object>("Something wrong when unhide feedback", null);
        }
        

        return new UserResponse<object>("Success reject", null);
    }
}