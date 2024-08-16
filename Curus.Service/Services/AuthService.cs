using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Curus.Repository.Entities;
using Curus.Repository.Helper;
using Curus.Repository.Interfaces;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Enum;
using Curus.Repository.ViewModels.Request;
using Curus.Repository.ViewModels.Response;
using Curus.Service.Interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Curus.Service.Services;

public class AuthService : IAuthService
{
    private readonly IMapper _mapper;
    private readonly IAuthRepository _authRepository;
    private readonly TokenGenerators _tokenGenerators;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailService _emailService;
    private readonly IBlobService _blobService;
    private readonly IConfiguration _configuration;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IRedisService _redisService;
    private readonly IOtpService _otpService;
    private readonly ITemporaryStoreService _temporaryStoreService;

    private static readonly List<string> _cardProviders = new()
    {
        "Visa",
        "MasterCard",
        "American Express",
        "Discover"
    };

    public AuthService(IMapper mapper, IAuthRepository authRepository, TokenGenerators tokenGenerators,
        IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, IEmailService emailService,
        IBlobService blobService, IConfiguration configuration, IInstructorRepository instructorRepository,
        IRedisService redisService, IOtpService otpService, ITemporaryStoreService temporaryStoreService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
        _tokenGenerators = tokenGenerators ?? throw new ArgumentNullException(nameof(tokenGenerators));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _httpContextAccessor = httpContextAccessor;
        _emailService = emailService;
        _blobService = blobService;
        _configuration = configuration;
        _instructorRepository = instructorRepository;
        _redisService = redisService;
        _otpService = otpService;
        _temporaryStoreService = temporaryStoreService;
    }

    public async Task<UserResponse<object>> CreateUser(UserDTO userDto)
    {
        var mapUser = _mapper.Map<User>(userDto);
        var check = await _authRepository.CreateUser(mapUser);
        var result = new UserResponse<object>(check ? "Create success" : "Create Fail", null);
        return result;
    }


    private string GenerateOtp()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var byteArray = new byte[4];
            rng.GetBytes(byteArray);
            var otp = BitConverter.ToUInt32(byteArray, 0) % 1000000; // Generate a 6-digit OTP
            return otp.ToString("D6");
        }
    }


    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private string GenerateResetToken()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var byteArray = new byte[32];
            rng.GetBytes(byteArray);
            return Convert.ToBase64String(byteArray);
        }
    }

    //REGISTER
    public async Task RegisterUserAsync(UserRegistrationDTO userRegistrationDto)
    {
        try
        {
            if (await _userRepository.ExistsAsync(u =>
                    u.Email == userRegistrationDto.Email || u.PhoneNumber == userRegistrationDto.PhoneNumber))
            {
                throw new Exception("User with this email or phone number already exists.");
            }

            var otp = GenerateOtp();
            var user = new User
            {
                FullName = userRegistrationDto.FullName,
                Birthday = userRegistrationDto.Birthday,
                PhoneNumber = userRegistrationDto.PhoneNumber,
                Email = userRegistrationDto.Email,
                Address = userRegistrationDto.Address,
                Status = UserStatus.Pending, // Pending status until OTP is verified
                Password = HashPassword(userRegistrationDto.Password),
                RoleId = 2, // Role 2 is for student
                Otp = otp,
                OtpExpiryTime = DateTime.UtcNow.AddMinutes(10)
            };

            await _userRepository.AddAsync(user);
            await _emailService.SendOtpEmailAsync(user.Email, otp);
        }
        catch (ArgumentNullException ex)
        {
            // Handle cases where required information is missing
            throw new ApplicationException("Missing required registration information.", ex);
        }
        catch (InvalidOperationException ex)
        {
            // Handle cases where an operation is invalid, such as duplicate user registration
            throw new ApplicationException("Invalid operation during user registration.", ex);
        }
        catch (Exception ex)
        {
            // General exception handling
            throw new ApplicationException("An error occurred while registering the user.", ex);
        }
    }

    public async Task<User> GetByVerificationToken(string token)
    {
        try
        {
            return await _userRepository.GetUserByVerificationToken(token);
        }
        catch (Exception ex)
        {
            // Handle potential exceptions such as token not found
            throw new ApplicationException("An error occurred while retrieving the user by verification token.", ex);
        }
    }

    public async Task<bool> VerifyOtpAsync(string email, string otp)
    {
        try
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            if (user.Otp != otp || user.OtpExpiryTime < DateTime.UtcNow)
            {
                return false;
            }

            user.IsVerified = true;
            user.Otp = null;
            user.OtpExpiryTime = null;
            user.Status = UserStatus.Active; // Update status to Active

            await _userRepository.UpdateAsync(user);
            return true;
        }
        catch (KeyNotFoundException ex)
        {
            // Handle cases where the user is not found
            throw new ApplicationException("User not found for OTP verification.", ex);
        }
        catch (Exception ex)
        {
            // General exception handling
            throw new ApplicationException("An error occurred while verifying the OTP.", ex);
        }
    }

    public async Task RegisterInstructorAsync(InstructorRegistrationDTO instructorRegistrationDto)
    {
        var existingUser = await _userRepository.FindByEmailOrPhoneAsync(instructorRegistrationDto.Email,
            instructorRegistrationDto.PhoneNumber);

        if (existingUser != null)
        {
            Console.WriteLine(
                $"Found user: {existingUser.Email}, Status: {existingUser.Status}, IsVerified: {existingUser.IsVerified}"); // Add detailed log

            if (existingUser.Status == UserStatus.Rejected && existingUser.IsVerified)
            {
                Console.WriteLine($"Re-registering user with ID: {existingUser.UserId}"); // Debugging log
                var otp = GenerateOtp();
                existingUser.Otp = otp;
                existingUser.OtpExpiryTime = DateTime.UtcNow.AddMinutes(10);
                existingUser.Status = UserStatus.Pending; // Update status to pending for re-registration
                await _userRepository.UpdateAsync(existingUser); // Ensure status is updated in DB
                await _emailService.SendOtpEmailAsync(existingUser.Email, otp);
                await _otpService.StoreOtpAsync(existingUser.UserId, otp, TimeSpan.FromMinutes(10));
                return;
            }
            else
            {
                Console.WriteLine(
                    $"User with email {instructorRegistrationDto.Email} or phone {instructorRegistrationDto.PhoneNumber} already exists but not eligible for re-registration."); // Debugging log
                throw new Exception("User with this email or phone number already exists.");
            }
        }

        var newOtp = GenerateOtp();
        var user = new User
        {
            FullName = instructorRegistrationDto.FullName,
            Birthday = instructorRegistrationDto.Birthday,
            PhoneNumber = instructorRegistrationDto.PhoneNumber,
            Email = instructorRegistrationDto.Email,
            Status = UserStatus.Pending,
            Password = HashPassword(instructorRegistrationDto.Password),
            RoleId = 3,
            Otp = newOtp,
            OtpExpiryTime = DateTime.UtcNow.AddMinutes(10)
        };

        await _userRepository.AddAsync(user);
        await _emailService.SendOtpEmailAsync(user.Email, newOtp);
        await _otpService.StoreOtpAsync(user.UserId, newOtp, TimeSpan.FromMinutes(10));
    }


    public async Task<bool> VerifyOtpAndCompleteRegistrationAsync(string email, string otp)
    {
        var user = await _userRepository.GetUserByEmail(email);
        if (user == null || user.Otp != otp || user.OtpExpiryTime < DateTime.UtcNow)
        {
            return false;
        }

        user.IsVerified = true;
        user.Status = user.RoleId == 3 ? UserStatus.Pending : UserStatus.Active; // Set status correctly based on role
        user.Otp = null;
        user.OtpExpiryTime = null;

        await _userRepository.UpdateAsync(user);

        if (user.RoleId == 3)
        {
            var instructorRegistrationDto = await _temporaryStoreService.GetInstructorRegistrationAsync(user.UserId);
            if (instructorRegistrationDto != null)
            {
                var instructorData = new InstructorData
                {
                    UserId = user.UserId,
                    TaxNumber = instructorRegistrationDto.TaxNumber,
                    CardNumber = instructorRegistrationDto.CardNumber,
                    CardName = instructorRegistrationDto.CardName,
                    CardProvider = instructorRegistrationDto.CardProvider,
                    Certification = await _blobService.UploadFileAsync(instructorRegistrationDto.Certification)
                };

                await _instructorRepository.AddAsync(instructorData);
            }
        }

        return true;
    }


    public List<string> GetCardProviders()
    {
        return _cardProviders;
    }

    //GOOGLE
    public async Task<UserResponse<object>> UserGetInfoSignUpByGoogle(string token)
    {
        string clientId = "1019925993732-rhnc7rvabfisdmhprbtcbffmra6fadnu.apps.googleusercontent.com";

        if (string.IsNullOrEmpty(clientId))
        {
            return new UserResponse<object>("ClientId is null!", null);
        }

        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new List<string> { clientId }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

        if (payload == null)
        {
            return new UserResponse<object>("Credential incorrect!", null);
        }

        var userEmail = payload.Email;
        var userFullName = payload.Name;
        var isCheckUser = await _userRepository.GetUserByEmail(userEmail);

        if (isCheckUser != null)
        {
            return new UserResponse<object>("This email already exist!", null);
        }

        // Store user details in Redis
        var userDetails = new { Email = userEmail, Name = userFullName };
        var userDetailsJson = JsonConvert.SerializeObject(userDetails);
        await _redisService.SetStringAsync(userEmail, userDetailsJson, TimeSpan.FromMinutes(10));

        return new UserResponse<object>("Successfully, please complete info to register", userDetails);
    }

    public async Task<UserResponse<object>> InstructorCompleteSignUpByGoogle(
        InstructorSignUpGoogleInfo instructorRegistrationDto)
    {
        // Lấy dữ liệu từ Redis bằng cách sử dụng email từ userRegistrationDto
        var userDetailsJson = await _redisService.GetStringAsync(instructorRegistrationDto.Email);
        if (userDetailsJson == null)
        {
            return new UserResponse<object>("Email not found in Redis", instructorRegistrationDto.Email);
        }

        // Deserialize JSON thành đối tượng UserDetails
        var userDetails = JsonConvert.DeserializeObject<SignUpGoogleInfo>(userDetailsJson);

        // Kiểm tra xem email trong Redis có khớp với email trong userRegistrationDto không
        if (userDetails.Email != instructorRegistrationDto.Email)
        {
            return new UserResponse<object>("Email mismatch", instructorRegistrationDto.Email);
        }

        var isCheckPhone = await _userRepository.CheckPhoneNumber(instructorRegistrationDto.PhoneNumber);

        if (isCheckPhone != null)
        {
            if (isCheckPhone.Status == UserStatus.Rejected && isCheckPhone.IsVerified)
            {
                isCheckPhone.Status = UserStatus.Pending;
                isCheckPhone.Otp = GenerateOtp();
                isCheckPhone.OtpExpiryTime = DateTime.UtcNow.AddMinutes(10);
                isCheckPhone.FullName = instructorRegistrationDto.FullName;
                isCheckPhone.Birthday = instructorRegistrationDto.Birthday;
                await _userRepository.UpdateAsync(isCheckPhone);

                var existingInstructorData = await _instructorRepository.GetInstructorByIdAsync(isCheckPhone.UserId);
                if (existingInstructorData != null)
                {
                    existingInstructorData.TaxNumber = instructorRegistrationDto.TaxNumber;
                    existingInstructorData.CardNumber = instructorRegistrationDto.CardNumber;
                    existingInstructorData.CardName = instructorRegistrationDto.CardName;
                    existingInstructorData.CardProvider = instructorRegistrationDto.CardProvider;
                    existingInstructorData.Certification =
                        await _blobService.UploadFileAsync(instructorRegistrationDto.Certification);
                    await _instructorRepository.UpdateAsync(existingInstructorData);
                }

                await _emailService.SendOtpEmailAsync(isCheckPhone.Email, isCheckPhone.Otp);
            }
            else
            {
                return new UserResponse<object>("Phone number already exists", null);
            }
        }

        var user = new User
        {
            FullName = userDetails.Name,
            Birthday = instructorRegistrationDto.Birthday,
            PhoneNumber = instructorRegistrationDto.PhoneNumber,
            Email = userDetails.Email,
            Status = UserStatus.Pending,
            RoleId = 3,
        };

        await _userRepository.AddAsync(user);

        var certificationUrl = await _blobService.UploadFileAsync(instructorRegistrationDto.Certification);

        var instructorData = new InstructorData
        {
            UserId = user.UserId,
            TaxNumber = instructorRegistrationDto.TaxNumber,
            CardNumber = instructorRegistrationDto.CardNumber,
            CardName = instructorRegistrationDto.CardName,
            CardProvider = instructorRegistrationDto.CardProvider,
            Certification = certificationUrl
        };

        await _instructorRepository.AddAsync(instructorData);
        await _emailService.SendPendingEmailAsync(user.Email);

        await _redisService.DeleteKeyAsync(instructorRegistrationDto.Email);

        return new UserResponse<object>("Register Successfully", null);
    }


    public async Task<UserResponse<object>> UserCompleteSignUpByGoogle(SignupGoogleRequest userRegistrationDto)
    {
        var userDetailsJson = await _redisService.GetStringAsync(userRegistrationDto.Email);
        if (userDetailsJson == null)
        {
            return new UserResponse<object>("Email not found in Redis", userRegistrationDto.Email);
        }

        // Deserialize JSON into UserDetails object
        var userDetails = JsonConvert.DeserializeObject<SignUpGoogleInfo>(userDetailsJson);

        // Check if email in Redis matches email in userRegistrationDto
        if (userDetails.Email != userRegistrationDto.Email)
        {
            return new UserResponse<object>("Email mismatch", userRegistrationDto.Email);
        }

        var isCheckPhone = await _userRepository.CheckPhoneNumber(userRegistrationDto.PhoneNumber);

        if (isCheckPhone != null)
        {
            return new UserResponse<object>("Phone number already exists!", null);
        }

        // Create User object from Redis information and userRegistrationDto
        var user = new User
        {
            FullName = userDetails.Name, // Use name from Redis
            Birthday = userRegistrationDto.Birthday,
            PhoneNumber = userRegistrationDto.PhoneNumber,
            Email = userDetails.Email, // Use email from Redis
            IsVerified = true,
            Status = UserStatus.Active,
            RoleId = 2, // Role 2 is for student
        };

        await _userRepository.AddAsync(user);

        // Optionally, delete data from Redis after using it to avoid unnecessary storage
        await _redisService.DeleteKeyAsync(userRegistrationDto.Email);

        return new UserResponse<object>("Register Successfully", null);
    }


    //login
    public async Task<Authenticator> LoginAsync(LoginDTO loginDTO)
    {
        try
        {
            var user = await _userRepository.GetUserByEmail(loginDTO.Email);

            if (user == null)
            {
                throw new KeyNotFoundException("Invalid email or account does not exist.");
            }

            if (!user.IsVerified)
            {
                throw new InvalidOperationException("Account is not activated. Please verify your email.");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid password.");
            }

            // Generate JWT token
            var token = await GenerateJwtToken(user);
            return token;
        }
        catch (KeyNotFoundException ex)
        {
            // Handle cases where the user is not found
            throw new ApplicationException("Invalid email or account does not exist.", ex);
        }
        catch (InvalidOperationException ex)
        {
            // Handle cases where the account is not verified
            throw new ApplicationException("Account is not activated. Please verify your email.", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            // Handle cases where the password is invalid
            throw new ApplicationException("Invalid password.", ex);
        }
        catch (Exception ex)
        {
            // General exception handling
            throw new ApplicationException("An error occurred during login.", ex);
        }
    }


    public async Task<Authenticator> AuthenGoogleUser(string token)
    {
        string clientId = "1019925993732-rhnc7rvabfisdmhprbtcbffmra6fadnu.apps.googleusercontent.com";

        if (string.IsNullOrEmpty(clientId))
        {
            throw new Exception("ClientId is null!");
        }

        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = new List<string> { clientId }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

        if (payload == null)
        {
            throw new Exception("Credential incorrect!");
        }

        var userEmail = payload.Email;
        var user = await _userRepository.GetUserByEmail(userEmail);

        if (user == null)
        {
            throw new ApplicationException("User not found.");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.RoleName),
        };

        var (accessToken, refreshToken) = _tokenGenerators.GenerateTokens(claims);

        await _authRepository.UpdateRefreshToken(user.UserId, refreshToken);
        return new Authenticator()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }

    private async Task<Authenticator> GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("id", user.UserId.ToString()), // Ensuring UserId claim is added
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.RoleName) // Ensuring Role claim is added
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30), // Token expiration set to 30 minutes
            signingCredentials: creds
        );

        var refreshToken = Guid.NewGuid().ToString();
        await _authRepository.UpdateRefreshToken(user.UserId, refreshToken);

        return new Authenticator
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken
        };
    }


    public async Task<Authenticator> RefreshToken(string token)
    {
        //Check refreshToken have validate
        var checkRefreshToken = _tokenGenerators.ValidateRefreshToken(token);
        if (!checkRefreshToken)
            return null;
        //Check refreshToken in DB
        var user = await _authRepository.GetRefreshToken(token);
        if (user == null) return null;
        List<Claim> claims = new()
        {
            new Claim("id", user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Role.RoleName),
        };

        var (accessToken, refreshToken) = _tokenGenerators.GenerateTokens(claims);

        await _authRepository.UpdateRefreshToken(user.UserId, refreshToken);
        return new Authenticator()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }

    //logout

    public async Task<bool> DeleteRefreshToken(int userId)
    {
        return await _authRepository.DeleteRefreshToken(userId);
    }


    //PASSWORD
    public async Task ChangePasswordAsync(string email, ChangePasswordDTO changePasswordDto)
    {
        try
        {
            var user = await _userRepository.GetUserByEmail(email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(changePasswordDto.OldPassword, user.Password))
            {
                throw new ArgumentException("Invalid old password.");
            }

            if (changePasswordDto.NewPassword == changePasswordDto.OldPassword)
            {
                throw new InvalidOperationException("New password cannot be the same as the old password.");
            }

            if (!ValidatePassword(changePasswordDto.NewPassword))
            {
                throw new ArgumentException(
                    "New password must contain at least one uppercase letter and one special character.");
            }

            user.Password = HashPassword(changePasswordDto.NewPassword);
            await _userRepository.UpdateAsync(user);
        }
        catch (ArgumentException ex)
        {
            // Handle cases where the provided password details are invalid
            throw new ApplicationException("Password change failed due to invalid input.", ex);
        }
        catch (InvalidOperationException ex)
        {
            // Handle cases where the new password is the same as the old password
            throw new ApplicationException("Password change failed due to operational constraints.", ex);
        }
        catch (Exception ex)
        {
            // General exception handling
            throw new ApplicationException("An error occurred while changing the password.", ex);
        }
    }

    private bool ValidatePassword(string password)
    {
        bool hasUpperCase = password.Any(char.IsUpper);
        bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));
        bool isValidLength = password.Length >= 6;

        return hasUpperCase && hasSpecialChar && isValidLength;
    }

    public async Task RequestPasswordResetAsync(ForgotPasswordRequestDTO forgotPasswordRequestDto)
    {
        try
        {
            var user = await _userRepository.GetUserByEmailOrPhoneNumber(forgotPasswordRequestDto.EmailOrPhoneNumber);

            if (user == null || !user.IsVerified)
            {
                throw new KeyNotFoundException("User not found or not activated.");
            }

            var token = GenerateResetToken();
            user.ResetToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            await _userRepository.UpdateAsync(user);

            //var resetLink = $"{_configuration["AppSettings:FrontendUrl"]}/reset-password?token={token}"; -- FRONT-END ONLY

            await _emailService.SendEmailAsync(new EmailDTO
            {
                To = user.Email,
                Subject = "Password Reset Request",
                //Body = $"Please reset your password by clicking on the following link: <a href='{resetLink}'>Reset Password</a>" -- FRONT-END ONLY

                Body = @$"Your token for resetting password is: {token}"
            });
        }
        catch (KeyNotFoundException ex)
        {
            // Handle cases where the user is not found or not activated
            throw new ApplicationException("Password reset request failed due to user not found or not activated.", ex);
        }
        catch (Exception ex)
        {
            // General exception handling
            throw new ApplicationException("An error occurred while requesting the password reset.", ex);
        }
    }

    public async Task ResetPasswordAsync(ResetPasswordDTO resetPasswordDto)
    {
        try
        {
            var user = await _userRepository.GetUserByResetToken(resetPasswordDto.Token);

            if (user == null || user.ResetTokenExpiry < DateTime.UtcNow)
            {
                throw new ArgumentException("Invalid or expired token.");
            }

            if (!ValidatePassword(resetPasswordDto.NewPassword))
            {
                throw new ArgumentException(
                    "New password must contain at least one uppercase letter, one special character, and be at least 6 characters long.");
            }

            user.Password = HashPassword(resetPasswordDto.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            await _userRepository.UpdateAsync(user);
        }
        catch (ArgumentException ex)
        {
            // Handle cases where the token is invalid or the new password does not meet requirements
            throw new ApplicationException("Password reset failed due to invalid input.", ex);
        }
        catch (Exception ex)
        {
            // General exception handling
            throw new ApplicationException("An error occurred while resetting the password.", ex);
        }
    }


    public async Task<string> GetIdFromToken()
    {
        var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ")
            .Last();

        if (token == null)
            throw new Exception("Token not found");

        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

        if (jwtToken == null)
            throw new Exception("Invalid token");

        var userId = jwtToken.Claims.First(claim => claim.Type == "id").Value;

        return userId;
    }
}