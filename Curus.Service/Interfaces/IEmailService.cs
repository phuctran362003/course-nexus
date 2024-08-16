using Curus.Repository.Entities;
using Curus.Repository.ViewModels;

namespace Curus.Service.Interfaces
{
    public interface IEmailService
    {
        // General email functions
        Task SendEmailAsync(EmailDTO request);

        // Activation/Deactivation email functions
        Task SendActiveEmailAsync(string email);
        Task SendDeactiveEmailAsync(string email, string reason);


        // Verification and OTP email functions
        Task SendVerificationEmailAsync(string email, string token);
        Task SendOtpEmailAsync(string email, string otp);

        // Status update email functions
        Task SendPendingEmailAsync(string email);
        Task SendApprovalEmailAsync(string email);
        Task SendRejectionEmailAsync(string email, string reason);

        //Notice submit course
        Task SendSubmitCourseEmailAsync(string email, EmailSubmitDTO emailSubmitDto);

        Task SendReportStudentMail(string email, string reason);

        Task SendChangeStatusEmailAsync(string email, string reason);
        
        Task SendApprovalCourseEmailAsync(string email);
        Task SendRejectionCourseEmailAsync(string email, string reason);


        Task SendPayoutRejectionEmail(string email, string reason);

        Task SendPayoutApprovalEmail(string email, decimal amount);
        Task SendOrderPaidEmailAsync(StudentOrder order);
        
        Task SendDiscountEmail(string email, EmailDiscountDTO emailDiscountDto);
        

        Task SendRemindActiveEmail(string email, string reason);

        Task SendFinishCourse(string email, string subject);
    }
}