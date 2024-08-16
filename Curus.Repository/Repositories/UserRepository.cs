
using AutoMapper;
using Curus.Repository.Entities;
using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Linq.Expressions;

namespace Curus.Repository;

public class UserRepository : IUserRepository
{
    private readonly CursusDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(CursusDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    //CRUD

    public async Task<User> FindByEmailOrPhoneAsync(string email, string phoneNumber)
    {
        return await _context.Users
       .FirstOrDefaultAsync(u => u.Email == email || u.PhoneNumber == phoneNumber);
    }
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }





    public async Task<User> GetUserByEmail(string email)    
    {
        return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> GetUserByVerificationToken(string token)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
    }

    public async Task<bool> ExistsAsync(Expression<Func<User, bool>> predicate)
    {
        return await _context.Users.AnyAsync(predicate);
    }



    public async Task<User> GetUserById(int userId)
    {
        _logger.LogInformation($"Fetching user with ID {userId}");

        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            _logger.LogError($"User with ID {userId} not found");
        }
        else
        {
            _logger.LogInformation($"User with ID {userId} found: {user.FullName}");
        }

        return user;
    }

    public async Task<User> GetAllUserById(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(i => i.UserId == id);
    }

    public async Task AddInstructorDataAsync(InstructorData instructorData)
    {
        await _context.InstructorData.AddAsync(instructorData);
        await _context.SaveChangesAsync();
    }



    public async Task<User> GetUserByEmailOrPhoneNumber(string emailOrPhoneNumber)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == emailOrPhoneNumber || u.PhoneNumber == emailOrPhoneNumber);
    }

    public async Task<User> GetUserByResetToken(string resetToken)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == resetToken);
    }

    public async Task CreateCommentUserAsync(CommentUser? commentUser)
    {
        _context.CommentUsers.Add(commentUser);
        await _context.SaveChangesAsync();
    }

    public async Task<List<User>> GetUsersByRole(int role)
    {
        return await _context.Users.Where(u => u.RoleId == role).ToListAsync();
    }
    
    public async Task<User> CheckPhoneNumber(string phone)
    {
        return await _context.Users.FirstOrDefaultAsync(p => p.PhoneNumber == phone);
    }

    public async Task<List<User>> GetInfoStudent(int PAGE_SIZE = 20, int page = 1)
    {
        var infoStudent = _context.Users
            .Where(u => u.RoleId == 2)
            .ToList();
        return infoStudent;
    }
    public async Task<int> CountAmountStudentCourse(int userId)
    {
        var courseCounts = await _context.StudentInCourses
        .Where(sic => sic.UserId == userId)
        .GroupBy(sic => sic.CourseId)
        .Select(group => new
        {
            CourseId = group.Key,
            CourseCount = group.Count()
        })
        .ToListAsync();

        int totalStudents = courseCounts.Count;

        return totalStudents;
    }

    public async Task<User> GetStudentById(int id)
    {
        var student = await _context.Users.FirstOrDefaultAsync(s => s.UserId == id);
        return student;
    }

    public async Task<User> CheckStudentById()
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.RoleId == 2);
    }

    public async Task ActiveStudent(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<DataTable> GetDataStudent()
    {
        DataTable dataTable = new DataTable();
        dataTable.TableName = "Student Data";
        dataTable.Columns.Add("User Id", typeof(int));
        dataTable.Columns.Add("Email", typeof(string));
        dataTable.Columns.Add("Name", typeof(string));
        dataTable.Columns.Add("Phone Number", typeof(string));
        dataTable.Columns.Add("Number of joined courses", typeof(int));
        dataTable.Columns.Add("Number of in progress courses", typeof(int));

        var _listStudent = await _context.Users.
            Where(u => u.RoleId == 2)
            .ToListAsync();
        if (_listStudent is not null)
        {
            _listStudent.ForEach(student =>
            {
                var countCourse = CountAmountCourseOfStudent(student.UserId);
                dataTable.Rows.Add(student.UserId, student.Email, student.FullName, student.PhoneNumber, countCourse, 0);
            });
        }

        return dataTable;


    }


    private int CountAmountCourseOfStudent(int userId)
    {
        var courseCounts = _context.StudentInCourses
        .Where(sic => sic.UserId == userId)
        .GroupBy(sic => sic.CourseId)
        .Select(group => new
        {
            CourseId = group.Key,
            CourseCount = group.Count()
        })
        .ToList();

        int totalStudents = courseCounts.Count;

        return totalStudents;
    }

    public async Task<List<StudentInCourse>> GetListStudentInCourse()
    {
        var query = await _context.StudentInCourses
                          .Include(sic => sic.Courses)
                          .ToListAsync();

        return query;
    }

    public async Task AddBookmarkedCourse(BookmarkedCourse bookmarkedCourse)
    {
        await _context.BookmarkedCourses.AddAsync(bookmarkedCourse);
        await _context.SaveChangesAsync();
    }

    public async Task UnBookmarkedCourse(int courseId, int userId)
    {
        var checkBookmarked = await _context.BookmarkedCourses.FirstOrDefaultAsync(bm => bm.CourseId == courseId && bm.UserId == userId);
        _context.BookmarkedCourses.Remove(checkBookmarked);
        await SaveChangeAsync();
    }

    public async Task<object> GetBookmarkedCourse(int userId, int courseId)
    {
        var existingBookmark = await _context.BookmarkedCourses.FirstOrDefaultAsync(bc => bc.CourseId == courseId && bc.UserId == userId);
        return existingBookmark;
    }

    public async Task UpdateLastActivityDateAsync(string email)
    {
        var user = await FindByEmailOrPhoneAsync(email,"");
        if (user != null)
        {
            user.LastActiveTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<User>> GetInactiveStudentsAsync(DateTime time)
    {
        return await _context.Users
            .Where(u => u.LastActiveTime == time && !_context.StudentInCourses.Any(sic => sic.UserId == u.UserId))
            .ToListAsync();
    }

    public async Task<List<User>> GetInactiveInstructorsAsync(DateTime since)
    {
        return await _context.Users
            .Where(u => u.LastActiveTime <= since && !_context.Courses.Any(c => c.InstructorId == u.UserId))
            .ToListAsync();
    }
    
    public async Task RemoveUsersAsync(IEnumerable<User> users)
    {
        _context.Users.UpdateRange(users);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> SaveChangeAsync()
    {
        var check = await _context.SaveChangesAsync();
        if (check != 0)
        {
            return true;
        }

        return false;
    }
}