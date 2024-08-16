using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Curus.Repository.ViewModels.User.Student;
public class StudentInfoDto
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public int NumberOfJoinedCourses { get; set; }
    public List<CourseViewModel> Courses { get; set; }
    public int NumberOfCourseInProgress { get; set; }
}

public class StudentDto
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Status { get; set; }
    public DateTime BirthDay { get; set; }
    public bool IsVerified { get; set; }
    public int RoleId { get; set; }
}

    public class CourseViewModel
{
    public int? CourseId { get; set; }
    public int InstructorId { get; set; }
    public int CategoryId { get; set; }
    public string CourseName { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }
    public string Version { get; set; }
}

