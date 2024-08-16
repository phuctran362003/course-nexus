using Curus.API.Controllers;
using Curus.Repository.ViewModels.Response;
using Curus.Repository.ViewModels;
using Curus.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Assert = Xunit.Assert;
using System.Dynamic;
using Newtonsoft.Json;

namespace Curus.Tests.Controllers
{
    public class HomepageControllerTests
    {
        private readonly Mock<ICourseService> _mockCourseService;
        private readonly HomepageController _homepageController;

        public HomepageControllerTests()
        {
            _mockCourseService = new Mock<ICourseService>();
            _homepageController = new HomepageController(_mockCourseService.Object);
        }

        [Fact]
        public async Task GetCourses_ReturnsOkResult_WithExpectedData()
        {
            // Arrange
            var topCourses = new UserResponse<object>("Success", new List<ViewAndSearchDTO>
        {
        new ViewAndSearchDTO { Name = "Course 1" },
        new ViewAndSearchDTO { Name = "Course 2" }
        });

            var topCategories = new UserResponse<object>("Success", new List<CategoryResponse>
        {
        new CategoryResponse { CategoryName = "Category 1" },
        new CategoryResponse { CategoryName = "Category 2" }
        });

            var topFeedbacks = new UserResponse<object>("Success", new List<FeedbackResponse>
        {
        new FeedbackResponse { Content = "Feedback 1" },
        new FeedbackResponse { Content = "Feedback 2" }
        });

            _mockCourseService.Setup(service => service.GetTopCoursesAsync()).ReturnsAsync(topCourses);
            _mockCourseService.Setup(service => service.GetTopCategoriesAsync()).ReturnsAsync(topCategories);
            _mockCourseService.Setup(service => service.GetTopFeedbacksAsync()).ReturnsAsync(topFeedbacks);

            // Act
            var result = await _homepageController.GetCourses();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseJson = JsonConvert.SerializeObject(okResult.Value);
            var responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);

            Assert.True(responseDict.ContainsKey("BestCourses"));
            Assert.True(responseDict.ContainsKey("TrendingCategories"));
            Assert.True(responseDict.ContainsKey("TopFeedbacks"));

            var bestCoursesJson = responseDict["BestCourses"].ToString();
            var trendingCategoriesJson = responseDict["TrendingCategories"].ToString();
            var feedbacksJson = responseDict["TopFeedbacks"].ToString();

            var bestCourses = JsonConvert.DeserializeObject<List<ViewAndSearchDTO>>(bestCoursesJson);
            var trendingCategories = JsonConvert.DeserializeObject<List<CategoryResponse>>(trendingCategoriesJson);
            var feedbacks = JsonConvert.DeserializeObject<List<FeedbackResponse>>(feedbacksJson);

            Assert.NotNull(bestCourses);
            Assert.NotNull(trendingCategories);
            Assert.NotNull(feedbacks);

            Assert.Equal(2, bestCourses.Count);
            Assert.Equal("Course 1", bestCourses[0].Name);
            Assert.Equal("Course 2", bestCourses[1].Name);

            Assert.Equal(2, trendingCategories.Count);
            Assert.Equal("Category 1", trendingCategories[0].CategoryName);
            Assert.Equal("Category 2", trendingCategories[1].CategoryName);

            Assert.Equal(2, feedbacks.Count);
            Assert.Equal("Feedback 1", feedbacks[0].Content);
            Assert.Equal("Feedback 2", feedbacks[1].Content);
        }


        [Fact]
        public async Task GetHeader_ReturnsOkResult_WithHeaderData()
        {
            // Arrange
            var headerDto = new HeaderDTO
            {
                BranchName = "Main Branch",
                SupportHotline = "123-456-7890"
            };

            _mockCourseService.Setup(service => service.GetHeaderAsync())
                .ReturnsAsync(headerDto);

            // Act
            var result = await _homepageController.GetHeader();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<HeaderDTO>(okResult.Value);

            Assert.Equal("Main Branch", response.BranchName);
            Assert.Equal("123-456-7890", response.SupportHotline);
        }

        [Fact]
        public async Task GetFooter_ReturnsOkResult_WithFooterData()
        {
            // Arrange
            var footerDto = new FooterDTO
            {
                PhoneNumber = "123-456-7890",
                Address = "123 Main St",
                WorkingTime = "9 AM - 5 PM",
                Privacy = "Privacy Policy Content",
                Team_of_use = "Terms of Use Content"
            };

            _mockCourseService.Setup(service => service.GetFooterAsync())
                .ReturnsAsync(footerDto);

            // Act
            var result = await _homepageController.GetFooter();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<FooterDTO>(okResult.Value);

            Assert.Equal("123-456-7890", response.PhoneNumber);
            Assert.Equal("123 Main St", response.Address);
            Assert.Equal("9 AM - 5 PM", response.WorkingTime);
            Assert.Equal("Privacy Policy Content", response.Privacy);
            Assert.Equal("Terms of Use Content", response.Team_of_use);
        }

        [Fact]
        public async Task UpdateHeader_ReturnsOkResult_WithUpdatedHeaderData()
        {
            // Arrange
            var headerDto = new HeaderDTO
            {
                BranchName = "New Branch Name",
                SupportHotline = "987-654-3210"
            };

            var updatedHeader = new HeaderDTO
            {
                BranchName = "New Branch Name",
                SupportHotline = "987-654-3210"
            };

            _mockCourseService.Setup(service => service.UpdateHeaderAsync(headerDto))
                .ReturnsAsync(new UserResponse<HeaderDTO>("Success", updatedHeader));

            // Act
            var result = await _homepageController.UpdateHeader(headerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<UserResponse<HeaderDTO>>(okResult.Value);

            Assert.Equal("Success", response.Message);
            Assert.Equal("New Branch Name", response.Data.BranchName);
            Assert.Equal("987-654-3210", response.Data.SupportHotline);
        }

        [Fact]
        public async Task UpdateFooter_ReturnsOkResult_WithUpdatedFooterData()
        {
            // Arrange
            var footerDto = new FooterDTO
            {
                PhoneNumber = "123-456-7890",
                Address = "New Address",
                WorkingTime = "9 AM - 5 PM",
                Privacy = "New Privacy Policy",
                Team_of_use = "Updated Terms of Use"
            };

            var updatedFooter = new FooterDTO
            {
                PhoneNumber = "123-456-7890",
                Address = "New Address",
                WorkingTime = "9 AM - 5 PM",
                Privacy = "New Privacy Policy",
                Team_of_use = "Updated Terms of Use"
            };

            _mockCourseService.Setup(service => service.UpdateFooterAsync(footerDto))
                .ReturnsAsync(new UserResponse<FooterDTO>("Success", updatedFooter));

            // Act
            var result = await _homepageController.UpdateFooter(footerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<UserResponse<FooterDTO>>(okResult.Value);

            Assert.Equal("Success", response.Message);
            Assert.Equal("123-456-7890", response.Data.PhoneNumber);
            Assert.Equal("New Address", response.Data.Address);
            Assert.Equal("9 AM - 5 PM", response.Data.WorkingTime);
            Assert.Equal("New Privacy Policy", response.Data.Privacy);
            Assert.Equal("Updated Terms of Use", response.Data.Team_of_use);
        }

        [Fact]
        public async Task AdminDashboard_ReturnsOkResult_WithCorrectData()
            {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 12, 31);

            var topPurchasedCourses = new List<ViewAndSearchDTO>
            {
            new ViewAndSearchDTO
            {
            Name = "Course 1",
            CreatedDate = DateTime.Now,
            Thumbnail = "Thumbnail1",
            InstructorName = "Instructor 1",
            Point = 10,
            Price = 100,
            StudentsInCourse = 20,
            ShortSummary = "Summary 1",
            CategoryName = new List<ViewCategoryNameDTO>(),
            Description = "Description 1"
            }
        };

            var topBadCourses = new List<ViewAndSearchDTO>
        {
        new ViewAndSearchDTO
            {
            Name = "Course 2",
            CreatedDate = DateTime.Now,
            Thumbnail = "Thumbnail2",
            InstructorName = "Instructor 2",
            Point = 5,
            Price = 50,
            ShortSummary = "Summary 2",
            CategoryName = new List<ViewCategoryNameDTO>(),
            Description = "Description 2"
            }
        };

            var topInstructorPayouts = new List<InstructorPayoutDTO>
            {
        new InstructorPayoutDTO
            {
            InstructorId = 1,
            InstructorName = "Instructor 1",
            PayoutAmount = 500,
            PayoutDate = DateTime.Now
            }
        };

            _mockCourseService.Setup(service => service.GetTopPurchasedCoursesAsync(startDate, endDate))
                .ReturnsAsync(new UserResponse<object>("Success", topPurchasedCourses));

            _mockCourseService.Setup(service => service.GetTopBadCoursesAsync(startDate, endDate))
                .ReturnsAsync(new UserResponse<object>("Success", topBadCourses));

            _mockCourseService.Setup(service => service.GetTopInstructorPayoutsAsync(startDate, endDate))
                .ReturnsAsync(new UserResponse<object>("Success", topInstructorPayouts));

            // Act
            var result = await _homepageController.AdminDashboard(startDate, endDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(okResult.Value);
            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

            var topPurchasedCoursesResult = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ViewAndSearchDTO>>(response["TopPurchasedCourses"].ToString());
            var topBadCoursesResult = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ViewAndSearchDTO>>(response["TopBadCourses"].ToString());
            var topInstructorPayoutsResult = Newtonsoft.Json.JsonConvert.DeserializeObject<List<InstructorPayoutDTO>>(response["TopInstructorPayouts"].ToString());

            Assert.NotNull(topPurchasedCoursesResult);
            Assert.NotNull(topBadCoursesResult);
            Assert.NotNull(topInstructorPayoutsResult);

            Assert.Single(topPurchasedCoursesResult);
            Assert.Single(topBadCoursesResult);
            Assert.Single(topInstructorPayoutsResult);

            Assert.Equal("Course 1", topPurchasedCoursesResult[0].Name);
            Assert.Equal("Course 2", topBadCoursesResult[0].Name);

            var instructorPayout = topInstructorPayoutsResult[0];
            Assert.Equal("Instructor 1", instructorPayout.InstructorName);
            Assert.Equal(500, instructorPayout.PayoutAmount);
        }
    }
}
