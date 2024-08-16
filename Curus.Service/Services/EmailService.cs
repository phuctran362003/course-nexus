
using System.Globalization;
using Curus.Repository.Entities;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using Curus.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Curus.Repository.ViewModels;
using Microsoft.Extensions.Logging;

namespace Curus.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendEmailAsync(EmailDTO request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailUserName"]));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = request.Body
            };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_configuration["EmailHost"], 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_configuration["EmailUserName"], _configuration["EmailPassword"]);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as per your needs
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }

        public async Task SendVerificationEmailAsync(string email, string token)
        {
            var verificationUrl = $"https://localhost:7203/api/auth/verify?token={token}";
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Email Verification",
                Body =
                    $"Please verify your email by clicking on the following link: <a href='{verificationUrl}'>Verify Email</a>"
            };

            await SendEmailAsync(emailDto);
        }


        public async Task SendOtpEmailAsync(string email, string otp)
        {
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Email Verification OTP",
                Body = $"Your OTP for email verification is: {otp}"
            };

            await SendEmailAsync(emailDto);
        }

        //PENDING MAIL
        public async Task SendPendingEmailAsync(string email)
        {
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Account Pending Approval",
                Body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #333;'>Thank you for registering as an instructor!</h2>
                            <p style='color: #555;'>We have received your registration and it is currently being reviewed by our admin team.</p>
                            <p style='color: #555;'>You will receive a notification once your account is approved. Please be patient during this process.</p>
                            <p style='color: #555;'>Thank you for your understanding.</p>
                            <p style='color: #555;'>Best regards,<br />Cursus Team</p>
                        </div>
                    </body>
                    </html>"
            };

            await SendEmailAsync(emailDto);
        }

        //APPROVE MAIL
        public async Task SendApprovalEmailAsync(string email)
        {
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Account Approval",
                Body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #333;'>Congratulations!</h2>
                            <p style='color: #555;'>Your account has been approved. You can now log in and start using the system.</p>
                            <p style='color: #555;'>Thank you for joining our system.</p>
                            <p style='color: #555;'>Best regards,<br />Cursus Team</p>
                        </div>
                    </body>
                    </html>"
            };

            await SendEmailAsync(emailDto);
        }

        //REJECT MAIL
        public async Task SendRejectionEmailAsync(string email, string reason)
        {
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Instructor Rejection",
                Body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #333;'>Registration Update</h2>
                            <p style='color: #555;'>We regret to inform you that your account registration has been rejected.</p>
                            <p style='color: #555;'>Reason: {reason}</p>
                            <p style='color: #555;'>If you have any questions or need further assistance, please contact our support team.</p>
                            <p style='color: #555;'>Best regards,<br />Cursus Team</p>
                        </div>
                    </body>
                    </html>"
            };

            await SendEmailAsync(emailDto);
        }

        //Active Mail
        public async Task SendActiveEmailAsync(string email)
        {
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Account Reactivation",
                Body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <h2 style='color: #333;'>Welcome Back!</h2>
                    <p style='color: #555;'>We are excited to inform you that your account has been reactivated. You can now log in and continue using our system.</p>
                    <p style='color: #555;'>Thank you for being a valued member of our community.</p>
                    <p style='color: #555;'>Best regards,<br />Cursus Team</p>
                </div>
            </body>
            </html>"
            };

            await SendEmailAsync(emailDto);
        }

        //Deactive Mail
        public async Task SendDeactiveEmailAsync(string email, string reason)
        {
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Account Deactivation Notice",
                Body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <h2 style='color: #333;'>Account Deactivation</h2>
                    <p style='color: #555;'>We regret to inform you that your account has been deactivated.</p>
                    <p style='color: #555;'>Reason: {reason}</p>
                    <p style='color: #555;'>If you have any questions or need further assistance, please contact our support team.</p>
                    <p style='color: #555;'>We appreciate your understanding.</p>
                    <p style='color: #555;'>Best regards,<br />Cursus Team</p>
                </div>
            </body>
            </html>"
            };

            await SendEmailAsync(emailDto);
        }

        public async Task SendSubmitCourseEmailAsync(string email, EmailSubmitDTO emailSubmitDto)
        {
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Course Submission Approval Required",
                Body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #333;'>New Course Submission</h2>
                            <p style='color: #555;'>An instructor has submitted a new course for approval. Please review and approve the course.</p>
                            <p style='color: #555;'>Course ID: {emailSubmitDto.CourseId}</p>
                            <p style='color: #555;'>Course Title: {emailSubmitDto.Name}</p>
                            <p style='color: #555;'>Thank you for your prompt attention to this matter.</p>
                            <p style='color: #555;'>Best regards,<br />Cursus Team</p>
                        </div>
                    </body>
                    </html>"
            };

            await SendEmailAsync(emailDto);
        }

        public async Task SendChangeStatusEmailAsync(string email, string reason)
        {
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Change Status Course",
                Body = reason
            };

            await SendEmailAsync(emailDto);
        }
        
        public async Task SendReportStudentMail(string email, string reason)
        {
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Report Course From Student",
                Body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <h2 style='color: #333;'>Course Report</h2>
                    <p style='color: #555;'>Reason: {reason}</p>
                    <p style='color: #555;'>Best regards,<br />Cursus Team</p>
                </div>
            </body>
            </html>"
            };

            await SendEmailAsync(emailDto);
        }

        public async Task SendApprovalCourseEmailAsync(string email)
        {
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Course Approval",
                Body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #333;'>Congratulations!</h2>
                            <p style='color: #555;'>Your course has been approved. You can now see it.</p>
                            <p style='color: #555;'>Thank you for joining our system.</p>
                            <p style='color: #555;'>Best regards,<br />Cursus Team</p>
                        </div>
                    </body>
                    </html>"
            };

            await SendEmailAsync(emailDto);
        }

        public async Task SendRejectionCourseEmailAsync(string email, string reason)
        {
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Course Reject",
                Body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #333;'>Oh no!</h2>
                            <p style='color: #555;'>Your course has been rejected. This is reason by admin: {reason}.</p>
                            <p style='color: #555;'>Thank you for joining our system.</p>
                            <p style='color: #555;'>Best regards,<br />Cursus Team</p>
                        </div>
                    </body>
                    </html>"
            };

            await SendEmailAsync(emailDto);
        }

        // Payout Approval Email
        public async Task SendPayoutApprovalEmail(string email, decimal amount)
        {
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Payout Approved",
                Body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #333;'>Payout Approved</h2>
                            <p style='color: #555;'>Your payout request of {amount:C} has been approved.</p>
                            <p style='color: #555;'>The amount will be transferred to your account shortly.</p>
                            <p style='color: #555;'>Thank you for your contribution.</p>
                            <p style='color: #555;'>Best regards,<br />Cursus Team</p>
                        </div>
                    </body>
                    </html>"
            };

            await SendEmailAsync(emailDto);
        }

        // Payout Rejection Email
        public async Task SendPayoutRejectionEmail(string email, string reason)
        {
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Payout Rejected",
                Body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #333;'>Payout Rejected</h2>
                            <p style='color: #555;'>We regret to inform you that your payout request has been rejected.</p>
                            <p style='color: #555;'>Reason: {reason}</p>
                            <p style='color: #555;'>If you have any questions or need further assistance, please contact our support team.</p>
                            <p style='color: #555;'>Best regards,<br />Cursus Team</p>
                        </div>
                    </body>
                    </html>"
            };

            await SendEmailAsync(emailDto);
        }

         public async Task SendOrderPaidEmailAsync(StudentOrder order)
        {
            if (order == null)
            {
                _logger.LogError("Order is null in SendOrderPaidEmailAsync");
                throw new ArgumentNullException(nameof(order));
            }

            if (order.User == null)
            {
                _logger.LogError("Order.User is null in SendOrderPaidEmailAsync");
                throw new ArgumentNullException(nameof(order.User));
            }

            if (order.OrderDetails == null || !order.OrderDetails.Any())
            {
                _logger.LogError("Order.OrderDetails is null or empty in SendOrderPaidEmailAsync");
                throw new ArgumentNullException(nameof(order.OrderDetails));
            }

            var emailDto = new EmailDTO
            {
                To = order.User.Email,
                Subject = "Order Confirmation",
                Body = $@"
    <html>
    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
            <h2 style='color: #333;'>Order Confirmation</h2>
            <p style='color: #555;'>Dear {order.User.FullName},</p>
            <p style='color: #555;'>We are pleased to inform you that your payment for order <strong>#{order.Id}{DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss")}</strong> was successful.</p>
              <p style='color: #555;'>You have purchased the following courses:</p>
                <ul style='color: #555;'>
                    {string.Join("", order.OrderDetails.Select(od => $"<li>{od.Course.Name} - {od.Course.Price:C}</li>"))}
                </ul>
            <p style='color: #555;'>Total Price: {order.TotalPrice.ToString("C0", new CultureInfo("vi-VN"))}</p>
            <p style='color: #555;'>Thank you for your purchase. We hope you enjoy the courses.</p>
            <p style='color: #555;'>Best regards,<br />Cursus Team</p>
        </div>
    </body>
    </html>"
            };

            await SendEmailAsync(emailDto);
        }


        public async Task SendDiscountEmail(string email, EmailDiscountDTO emailDiscountDto)
        {
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Exclusive Discount for You!",
                Body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                    <h2 style='color: #333;'>Exclusive Discount</h2>
                    <p style='color: #555;'>We are excited to offer you an exclusive discount on our courses!</p>
                    <p style='color: #555;'>Discount Code: <strong>{emailDiscountDto.DiscountCode}</strong></p>
                    <p style='color: #555;'>Discount Percentage: <strong>{emailDiscountDto.DiscountPercentage}%</strong></p>
                    <p style='color: #555;'>Expiration Date: <strong>{emailDiscountDto.ExpireDateTime:MMMM dd, yyyy HH:mm:ss}</strong></p>
                    <p style='color: #555;'>Use this code to discount for your course.</p>
                    <p style='color: #555;'>If you have any questions or need further assistance, please contact our support team.</p>
                    <p style='color: #555;'>Best regards,<br />Cursus Team</p>
                </div>
            </body>
            </html>"
            };
            await SendEmailAsync(emailDto);
        }

        public async Task SendRemindActiveEmail(string email, string reason)
        {
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = "Payout Rejected",
                Body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #333;'>Reminder Active</h2>
                            <p style='color: #555;'>{reason}</p>
                            <p style='color: #555;'>If you have any questions or need further assistance, please contact our support team.</p>
                            <p style='color: #555;'>Best regards,<br />Cursus Team</p>
                        </div>
                    </body>
                    </html>"
            };

            await SendEmailAsync(emailDto);
        }
        
        public async Task SendFinishCourse(string email, string subject)
        {
            var emailDto = new EmailDTO
            {
                To = email,
                Subject = subject,
                Body = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #333;'>Finish Course</h2>
                            <p style='color: #555;'>Congratulations on completing the course..</p>
                            <p style='color: #555;'>If you have any questions or need further assistance, please contact our support team.</p>
                            <p style='color: #555;'>Best regards,<br />Cursus Team</p>
                        </div>
                    </body>
                    </html>"
            };

            await SendEmailAsync(emailDto);
        }

    }
}
