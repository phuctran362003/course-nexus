using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public static class EmailTemplates
    {
        public const string EmailVerification = "Please verify your email by clicking on the following link: <a href='{VerificationUrl}'>Verify Email</a>";

        public const string OtpEmail = "Your OTP for email verification is: {OTP}";

        public const string PendingEmail = @"
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
    </html>";

        public const string ApprovalEmail = @"
    <html>
    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
            <h2 style='color: #333;'>Congratulations!</h2>
            <p style='color: #555;'>Your account has been approved. You can now log in and start using the system.</p>
            <p style='color: #555;'>Thank you for joining our system.</p>
            <p style='color: #555;'>Best regards,<br />Cursus Team</p>
        </div>
    </body>
    </html>";

        public const string RejectionEmail = @"
    <html>
    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
            <h2 style='color: #333;'>Registration Update</h2>
            <p style='color: #555;'>We regret to inform you that your account registration has been rejected.</p>
            <p style='color: #555;'>Reason: {Reason}</p>
            <p style='color: #555;'>If you have any questions or need further assistance, please contact our support team.</p>
            <p style='color: #555;'>Best regards,<br />Cursus Team</p>
        </div>
    </body>
    </html>";

        public const string ActiveEmail = @"
    <html>
    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
            <h2 style='color: #333;'>Welcome Back!</h2>
            <p style='color: #555;'>We are excited to inform you that your account has been reactivated. You can now log in and continue using our system.</p>
            <p style='color: #555;'>Thank you for being a valued member of our community.</p>
            <p style='color: #555;'>Best regards,<br />Cursus Team</p>
        </div>
    </body>
    </html>";

        public const string DeactiveEmail = @"
    <html>
    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
            <h2 style='color: #333;'>Account Deactivation</h2>
            <p style='color: #555;'>We regret to inform you that your account has been deactivated.</p>
            <p style='color: #555;'>Reason: {Reason}</p>
            <p style='color: #555;'>If you have any questions or need further assistance, please contact our support team.</p>
            <p style='color: #555;'>We appreciate your understanding.</p>
            <p style='color: #555;'>Best regards,<br />Cursus Team</p>
        </div>
    </body>
    </html>";

        public const string SubmitCourseEmail = @"
    <html>
    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
        <div style='max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
            <h2 style='color: #333;'>New Course Submission</h2>
            <p style='color: #555;'>An instructor has submitted a new course for approval. Please review and approve the course.</p>
            <p style='color: #555;'>Course ID: {CourseId}</p>
            <p style='color: #555;'>Course Title: {Name}</p>
            <p style='color: #555;'>Thank you for your prompt attention to this matter.</p>
            <p style='color: #555;'>Best regards,<br />Cursus Team</p>
        </div>
    </body>
    </html>";
    }

}
