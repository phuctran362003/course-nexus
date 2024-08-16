using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Curus.Repository.Entities;
using Curus.Repository;
using Curus.Service.Library;
using System.Security.Cryptography;
using Curus.Repository.ViewModels.Request;
using Curus.Service.Interfaces;
using Microsoft.Extensions.Logging;
using Curus.Repository.ViewModels;
using Curus.Service.ResponseDTO;
using Microsoft.Data.SqlClient;

public class OrderService : IOrderService
{
    private readonly CursusDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly ILogger _logger;

    public OrderService(CursusDbContext context, IConfiguration configuration, IEmailService emailService, ILogger<OrderService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResponse<StudentOrderDTO>> CreateOrderAsync(int userId, List<int> courseIds)
{
    try
    {
        _logger.LogInformation($"Creating order for user {userId} with course IDs {string.Join(",", courseIds)}");

        // Validate user
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"User not found for userId: {userId}");
            return new ServiceResponse<StudentOrderDTO> { Success = false, Message = "User not found" };
        }

        // Remove duplicate course IDs
        courseIds = courseIds.Distinct().ToList();

        // Validate course IDs
        var courses = await _context.Courses.Where(c => courseIds.Contains(c.Id)).ToListAsync();
        if (courses.Count != courseIds.Count)
        {
            _logger.LogWarning($"Some courses not found for the provided course IDs: {string.Join(", ", courseIds)}");
            return new ServiceResponse<StudentOrderDTO> { Success = false, Message = "Some courses not found" };
        }

        // Check for existing completed orders with the same course IDs
        var existingCourseIds = await _context.StudentOrders
            .Where(o => o.UserId == userId && o.OrderStatus == "Completed")
            .SelectMany(o => o.OrderDetails)
            .Where(od => courseIds.Contains(od.CourseId))
            .Select(od => od.CourseId)
            .ToListAsync();

        if (existingCourseIds.Any())
        {
            var existingCourses = string.Join(", ", existingCourseIds);
            _logger.LogWarning($"User {userId} is attempting to purchase already owned courses: {existingCourses}");
            return new ServiceResponse<StudentOrderDTO> { Success = false, Message = $"User already owns courses: {existingCourses}" };
        }

        // Create order details including the course price
        var orderDetails = courses.Select(c => new OrderDetail
        {
            CourseId = c.Id,
            CoursePrice = c.Price // Set the course price here
        }).ToList();

        // Calculate total price including VAT
        var totalPrice = courses.Sum(c => c.Price) * 1.1m * 23000; // Including 10% VAT

        var order = new StudentOrder
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            OrderStatus = "Pending",
            TotalPrice = totalPrice,
            OrderDetails = orderDetails
        };

        try
        {
            _context.StudentOrders.Add(order);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
        {
            if (sqlEx.Number == 2601) // Duplicate key
            {
                _logger.LogError(ex, "Duplicate order detected. The order with the same details already exists.");
                return new ServiceResponse<StudentOrderDTO> { Success = false, Message = "Duplicate order detected. The order with the same details already exists." };
            }
            else
            {
                _logger.LogError(ex, "A database error occurred while saving the order.");
                return new ServiceResponse<StudentOrderDTO> { Success = false, Message = "A database error occurred while saving the order." };
            }
        }

        var orderDetailDTOs = courses.Select(c => new OrderDetailDTO
        {
            CourseId = c.Id,
            CourseName = c.Name,
            Price = c.Price
        }).ToList();

        var studentOrderDTO = new StudentOrderDTO
        {
            Id = order.Id,
            UserId = order.UserId,
            OrderDate = order.OrderDate,
            OrderStatus = order.OrderStatus,
            TotalPrice = order.TotalPrice,
            OrderDetails = orderDetailDTOs,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        _logger.LogInformation($"Order created successfully for user {userId} with order ID {order.Id}");
        return new ServiceResponse<StudentOrderDTO>
        {
            Success = true,
            Data = studentOrderDTO
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An unexpected error occurred while creating the order.");
        return new ServiceResponse<StudentOrderDTO> { Success = false, Message = "An unexpected error occurred while creating the order." };
    }
}





    public async Task<string> GeneratePaymentUrlAsync(int orderId)
{
    try
    {
        _logger.LogInformation($"Generating payment URL for order ID {orderId}");

        // Retrieve the order along with associated user
        var order = await _context.StudentOrders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null)
        {
            _logger.LogWarning($"Order not found for orderId: {orderId}");
            throw new KeyNotFoundException("Order not found");
        }

        // Retrieve VNPay configuration settings
        var vnp_Returnurl = _configuration["VnpaySettings:ReturnUrl"];
        var vnp_Url = _configuration["VnpaySettings:Url"];
        var vnp_TmnCode = _configuration["VnpaySettings:TmnCode"];
        var vnp_HashSecret = _configuration["VnpaySettings:HashSecret"];

        _logger.LogInformation($"VNPay configuration - ReturnUrl: {vnp_Returnurl}, Url: {vnp_Url}, TmnCode: {vnp_TmnCode}");

        var vnpay = new VnPayLibrary();

        // Add VNPay request data
        vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
        vnpay.AddRequestData("vnp_Amount", ((int)(order.TotalPrice * 100)).ToString()); // Amount in VND without decimal points
        vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
        vnpay.AddRequestData("vnp_Locale", "vn");
        vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.Id);
        vnpay.AddRequestData("vnp_OrderType", "other");
        vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);

        // Use a shorter and unique transaction reference
        string txnRef = $"{orderId}-{DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmssff")}";
        vnpay.AddRequestData("vnp_TxnRef", txnRef);

        // Create payment URL
        string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

        // Create a new payment record
        var payment = new Payment
        {
            UserId = order.UserId,
            StudentOrderId = order.Id,
            PaymentUrl = paymentUrl,
            PaymentStatus = "Pending",
            PaymentDate = DateTime.UtcNow.AddHours(7),
            PaymentMethod = "VNPAY"
        };

        // Add payment to the order and save changes
        order.Payments.Add(payment);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Payment URL generated successfully for order ID {orderId}");
        return paymentUrl;
    }
    catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
    {
        _logger.LogError(ex, "A database error occurred while generating the payment URL.");
        throw new Exception("A database error occurred while generating the payment URL.", ex);
    }
    catch (KeyNotFoundException)
    {
        throw; // Let this bubble up to the controller
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An unexpected error occurred while generating the payment URL.");
        throw new Exception("An unexpected error occurred while generating the payment URL.", ex);
    }
}






    public async Task<bool> HandleVnPayPaymentReturnAsync(VnPayIPNRequest request, IDictionary<string, string> queryParams)
    {
        try
        {
            _logger.LogInformation("Starting HandleVnPayPaymentReturnAsync");

            if (request == null)
            {
                _logger.LogError("VnPayIPNRequest is null");
                throw new ArgumentNullException(nameof(request));
            }

            if (queryParams == null)
            {
                _logger.LogError("Query parameters are null");
                throw new ArgumentNullException(nameof(queryParams));
            }

            var vnpay = new VnPayLibrary();
            foreach (var key in queryParams.Keys)
            {
                if (key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, queryParams[key]);
                }
            }

            var secureHash = request.vnp_SecureHash;
            var hashSecret = _configuration["VnpaySettings:HashSecret"];

            _logger.LogInformation($"SecureHash: {secureHash}, HashSecret: {hashSecret}");

            if (string.IsNullOrEmpty(secureHash))
            {
                _logger.LogError("SecureHash is null or empty");
                throw new ArgumentNullException(nameof(secureHash));
            }

            if (string.IsNullOrEmpty(hashSecret))
            {
                _logger.LogError("HashSecret is null or empty");
                throw new ArgumentNullException(nameof(hashSecret));
            }

            if (!vnpay.ValidateSignature(secureHash, hashSecret))
            {
                _logger.LogError($"Invalid signature. Expected: {vnpay.ValidateSignature(secureHash, hashSecret)}, Received: {secureHash}");
                throw new Exception("Invalid signature");
            }

            long orderId;
            try
            {
                orderId = long.Parse(request.vnp_TxnRef.Split('-')[0]);
                _logger.LogInformation($"Parsed orderId: {orderId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing orderId from vnp_TxnRef");
                throw;
            }

            var order = await _context.StudentOrders
                .Include(o => o.Payments)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Course)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                _logger.LogError($"Order not found for orderId: {orderId}");
                throw new Exception("Order not found");
            }

            if (order.User == null)
            {
                _logger.LogError($"User not found for orderId: {orderId}");
                throw new Exception("User not found");
            }

            if (order.OrderDetails == null || !order.OrderDetails.Any())
            {
                _logger.LogError($"OrderDetails not found for orderId: {orderId}");
                throw new Exception("OrderDetails not found");
            }

            var payment = order.Payments.FirstOrDefault();
            if (payment == null)
            {
                _logger.LogError($"Payment not found for orderId: {orderId}");
                throw new Exception("Payment not found");
            }

            if (request.vnp_ResponseCode == "00")
            {
                payment.PaymentStatus = "Paid";
                payment.TransactionId = request.vnp_TransactionNo;
                order.OrderStatus = "Completed";

                await _emailService.SendOrderPaidEmailAsync(order);  // Send email after payment confirmation
                await ActivateCoursesForUserAsync(order.UserId, order.OrderDetails.Select(od => od.CourseId).ToList());
            }
            else
            {
                payment.PaymentStatus = "Failed";
                order.OrderStatus = "Failed";
            }

            await _context.SaveChangesAsync();
            return payment.PaymentStatus == "Paid";
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
        {
            _logger.LogError(ex, "A database error occurred while handling the VNPay payment return.");
            throw new Exception("A database error occurred while handling the VNPay payment return.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while handling the VNPay payment return.");
            throw new Exception("An unexpected error occurred while handling the VNPay payment return.");
        }
    }




    private async Task ActivateCoursesForUserAsync(int userId, List<int> courseIds)
    {
        _logger.LogInformation("Activating courses for user");

        // Retrieve the user's role (Assuming a method GetUserRoleAsync is available)
        var userRole = await GetUserRoleAsync(userId);
    
        // Retrieve courses with instructor information
        var courses = await _context.Courses
            .Where(c => courseIds.Contains(c.Id))
            .Include(c => c.Instructor) // Ensure Instructor is included
            .ToListAsync();

        var userCourses = courses.Select(course => new StudentInCourse()
        {
            UserId = userId,
            CourseId = course.Id,
            InstructorId = course.InstructorId,
            Rating = 0, // Default rating
            CreatedDate = DateTime.UtcNow,
            CreatedBy = userRole, // Set to user's role name
            ModifiedDate = DateTime.UtcNow,
            ModifiedBy = userRole, // Set to user's role name
            IsDelete = false
        }).ToList();

        await _context.StudentInCourses.AddRangeAsync(userCourses);
        await _context.SaveChangesAsync();
    }

// Example method to retrieve the user's role
    private async Task<string> GetUserRoleAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Role) // Assuming User entity has a Roles navigation property
            .FirstOrDefaultAsync(u => u.UserId == userId);

        return user?.Role.RoleName ?? "Unknown"; // Assuming the user has at least one role
    }



    private static class Utils
    {
        public static string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        public static string GetIpAddress()
        {
            return "127.0.0.1";
        }
    }
}
