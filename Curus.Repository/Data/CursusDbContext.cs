using Curus.Repository.Entities;
using Curus.Repository.ViewModels.Enum;
using Microsoft.EntityFrameworkCore;

namespace Curus.Repository
{
    public class CursusDbContext : DbContext
    {
        public CursusDbContext(DbContextOptions<CursusDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<ReportFeedback> ReportFeedbacks { get; set; }
        public DbSet<StudentInCourse> StudentInCourses { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<BackupCourse> BackupCourses { get; set; }
        public DbSet<BackupChapter> BackupChapters { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<InstructorData> InstructorData { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CourseCategory> CourseCategories { get; set; }
        public DbSet<StudentOrder> StudentOrders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<CommentUser> CommentUsers { get; set; }
        public DbSet<CommentCourse> CommentCourses { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<InstructorPayout> InstructorPayouts { get; set; }
        public DbSet<HistoryCourse> HistoryCourses { get; set; }
        public DbSet<BookmarkedCourse> BookmarkedCourses { get; set; }
        public DbSet<Header> Headers { get; set; }
        public DbSet<Footer> Footers { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<HistoryCourseDiscount> HistoryCourseDiscounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // UserStatus enum conversion
            modelBuilder.Entity<User>()
                .Property(u => u.Status)
                .HasConversion<string>();

            // Course and User (Instructor) relationship
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.InstructorCourses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.NoAction);

            // User and InstructorData relationship
            modelBuilder.Entity<InstructorData>()
                .HasOne(i => i.User)
                .WithOne(u => u.InstructorData)
                .HasForeignKey<InstructorData>(i => i.UserId);

            // CardProvider enum conversion
            modelBuilder.Entity<InstructorData>()
                .Property(i => i.CardProvider)
                .HasConversion<string>();

            // Course vs Chapter: one to many
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Chapters)
                .WithOne(ch => ch.Course)
                .HasForeignKey(ch => ch.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Chapter vs BackupChapter: one to one
            modelBuilder.Entity<Chapter>()
                .HasOne(ch => ch.BackupChapter)
                .WithOne(bc => bc.Chapter)
                .HasForeignKey<BackupChapter>(bc => bc.ChapterId)
                .OnDelete(DeleteBehavior.Restrict);

            // BackupChapter vs BackupCourse: one to many
            modelBuilder.Entity<BackupChapter>()
                .HasOne(bc => bc.BackupCourse)
                .WithMany(bc => bc.BackupChapters)
                .HasForeignKey(bc => bc.BackupCourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // BackupCourse vs Course: one to one
            modelBuilder.Entity<Course>()
                .HasOne(c => c.BackupCourse)
                .WithOne(bc => bc.Course)
                .HasForeignKey<BackupCourse>(bc => bc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Feedback relationships
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Course)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.User)
                .WithMany(u => u.Feedbacks)
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.ReportFeedback)
                .WithOne(rf => rf.Feedback)
                .HasForeignKey<ReportFeedback>(rf => rf.FeedbackId)
                .OnDelete(DeleteBehavior.Cascade);

            // ReportFeedback relationships
            modelBuilder.Entity<ReportFeedback>()
                .HasKey(rf => rf.Id);

            modelBuilder.Entity<ReportFeedback>()
                .Property(rf => rf.ReportReason)
                .HasMaxLength(500); // Assuming max length of 500 characters

            modelBuilder.Entity<ReportFeedback>()
                .Property(rf => rf.IsHidden)
                .IsRequired();

            modelBuilder.Entity<ReportFeedback>()
                .HasOne(rf => rf.User)
                .WithMany(u => u.ReportFeedbacks)
                .HasForeignKey(rf => rf.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of user when a report exists

            // Report relationships
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Course)
                .WithMany(c => c.Reports)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Role seed data
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, RoleName = "Admin" },
                new Role { Id = 2, RoleName = "User" },
                new Role { Id = 3, RoleName = "Instructor" }
            );

            // User and Role relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            // CommentUser relationships
            modelBuilder.Entity<CommentUser>()
                .HasOne(cu => cu.User)
                .WithMany(u => u.CommentsReceived)
                .HasForeignKey(cu => cu.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CommentUser>()
                .HasOne(cu => cu.CommentedBy)
                .WithMany(u => u.CommentByAdmin)
                .HasForeignKey(cu => cu.CommentedById)
                .OnDelete(DeleteBehavior.Restrict);

            // CommentCourse relationships
            modelBuilder.Entity<CommentCourse>()
                .HasOne(cc => cc.User)
                .WithMany(u => u.CourseComments)
                .HasForeignKey(cc => cc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CommentCourse>()
                .HasOne(cc => cc.Course)
                .WithMany(c => c.Comments)
                .HasForeignKey(cc => cc.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category relationships
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(pc => pc.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // CourseCategory relationships
            modelBuilder.Entity<CourseCategory>()
                .HasKey(cc => new { cc.CourseId, cc.CategoryId });

            modelBuilder.Entity<CourseCategory>()
                .HasOne(cc => cc.Course)
                .WithMany(c => c.CourseCategories)
                .HasForeignKey(cc => cc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseCategory>()
                .HasOne(cc => cc.Category)
                .WithMany(c => c.CourseCategories)
                .HasForeignKey(cc => cc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // StudentOrder relationships
            modelBuilder.Entity<StudentOrder>()
                .HasOne(so => so.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(so => so.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentOrder>()
                .HasMany(so => so.OrderDetails)
                .WithOne(od => od.StudentOrder)
                .HasForeignKey(od => od.StudentOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentOrder>()
                .HasMany(o => o.Payments)
                .WithOne(p => p.StudentOrder)
                .HasForeignKey(p => p.StudentOrderId);

            // Payment relationships
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.StudentOrder)
                .WithMany(so => so.Payments)
                .HasForeignKey(p => p.StudentOrderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.InstructorPayout)
                .WithMany(ip => ip.Payments)
                .HasForeignKey(p => p.InstructorPayoutId)
                .OnDelete(DeleteBehavior.NoAction);

            // InstructorPayout relationships
            modelBuilder.Entity<InstructorPayout>()
                .Property(p => p.PayoutStatus)
                .HasConversion(
                    v => v.ToString(),
                    v => (PayoutStatus)Enum.Parse(typeof(PayoutStatus), v));

            modelBuilder.Entity<InstructorPayout>()
                .HasOne(i => i.Instructor)
                .WithMany(u => u.InstructorPayouts)
                .HasForeignKey(i => i.InstructorId)
                .OnDelete(DeleteBehavior.Cascade);

            // HistoryCourse relationships
            modelBuilder.Entity<HistoryCourse>()
                .HasOne(hc => hc.Course)
                .WithMany(c => c.HistoryCourses)
                .HasForeignKey(hc => hc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HistoryCourse>()
                .HasOne(hc => hc.User)
                .WithMany(u => u.HistoryCourses)
                .HasForeignKey(hc => hc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Header>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Headers");
            });

            modelBuilder.Entity<Footer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Footers");
            });

            // Discount relationships
            modelBuilder.Entity<Discount>()
                .Property(d => d.DiscountPercentage)
                .HasPrecision(18, 2); // Precision for decimal value

            modelBuilder.Entity<Discount>()
                .HasMany(d => d.HistoryCourseDiscounts)
                .WithOne(hcd => hcd.Discount)
                .HasForeignKey(hcd => hcd.DiscountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HistoryCourseDiscount>()
                .HasOne(hcd => hcd.Instructor)
                .WithMany(i => i.HistoryCourseDiscounts)
                .HasForeignKey(hcd => hcd.InstructorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HistoryCourseDiscount>()
                .HasOne(hcd => hcd.Course)
                .WithMany(c => c.HistoryCourseDiscounts)
                .HasForeignKey(hcd => hcd.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HistoryCourseDiscount>()
                .HasOne(hcd => hcd.Discount)
                .WithMany(d => d.HistoryCourseDiscounts)
                .HasForeignKey(hcd => hcd.DiscountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

