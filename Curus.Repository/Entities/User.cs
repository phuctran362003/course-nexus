using Curus.Repository.ViewModels.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public UserStatus? Status { get; set; }
        public string? Password { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? Birthday { get; set; }
        public string? VerificationToken { get; set; }
        public bool IsVerified { get; set; } = false;
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
        public string? Otp { get; set; }
        public DateTime? OtpExpiryTime { get; set; }
        public int RoleId { get; set; }
        public DateTime LastActiveTime { get; set; }
        public bool IsDelete { get; set; }

        // Navigation Properties
        public virtual Role Role { get; set; }
        public virtual ICollection<StudentOrder> Orders { get; set; } = new HashSet<StudentOrder>();
        public virtual ICollection<Course>? InstructorCourses { get; set; } 
        public virtual InstructorData? InstructorData { get; set; } 
        public virtual ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();
        public virtual ICollection<ReportFeedback> ReportFeedbacks { get; set; } = new HashSet<ReportFeedback>();
        public virtual ICollection<Report>? Reports { get; set; } = new HashSet<Report>();
        public virtual ICollection<InstructorPayout> InstructorPayouts { get; set; } = new HashSet<InstructorPayout>();
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
        public virtual ICollection<CommentUser> CommentByAdmin { get; set; } = new HashSet<CommentUser>();
        public virtual ICollection<CommentUser> CommentsReceived { get; set; } = new HashSet<CommentUser>();
        public virtual ICollection<CommentCourse> CourseComments { get; set; } = new HashSet<CommentCourse>();
        public virtual ICollection<HistoryCourse> HistoryCourses { get; set; } = new HashSet<HistoryCourse>();
        public virtual ICollection<BookmarkedCourse> BookmarkedCourses { get; set; } = new HashSet<BookmarkedCourse>();
        public virtual ICollection<HistoryCourseDiscount> HistoryCourseDiscounts { get; set; } = new HashSet<HistoryCourseDiscount>();

    }




}

