using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Request;
using Curus.Repository.ViewModels.Response;
using Curus.Service.Interfaces;
using Curus.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Curus.Repository.ViewModels.ChangePasswordDTO;
using System.Security.Claims;
using Curus.Repository.ViewModels.Enum;

namespace Curus.API.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly PasswordService _passwordService;
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly IInstructorService _instructorService;

    public AuthenticationController(PasswordService passwordService, IAuthService authService, IInstructorService instructorService, IEmailService emailService, IUserService userService)
    {
        _passwordService = passwordService;
        _authService = authService;
        _instructorService = instructorService;
        _emailService = emailService;
        _userService = userService;
    }

    /// <summary>
    /// Logs in a user and returns a JWT token.
    /// </summary>
    [HttpPost("user/login")]
    public async Task<IActionResult> Login(LoginDTO loginDTO)
    {
        try
        {
            var token = await _authService.LoginAsync(loginDTO);
            Response.Cookies.Append("refreshToken", token.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.Strict,
            });

            return Ok(new UserResponse<object>("Success", new
            {
                TokenType = "Bearer",
                AccessToken = token.AccessToken
            }));
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Logs in a user using Google authentication and returns a JWT token.
    /// </summary>
    [HttpPost("user/google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] string token)
    {
        try
        {
            var checkToken = await _authService.AuthenGoogleUser(token);
            return Ok(checkToken);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Signs up a user using Google authentication.
    /// </summary>
    [HttpPost("user/google-signup")]
    public async Task<IActionResult> StudentSignupByGoogle([FromQuery] string token)
    {
        try
        {
            var checkToken = await _authService.UserGetInfoSignUpByGoogle(token);
            return Ok(checkToken);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Completes user information for Google sign-up.
    /// </summary>
    [HttpPost("user/complete-info")]
    public async Task<IActionResult> UserCompleteInfoSignUpGoogle([FromBody] SignupGoogleRequest signupGoogleRequest)
    {
        try
        {
            var checkToken = await _authService.UserCompleteSignUpByGoogle(signupGoogleRequest);
            return Ok(checkToken);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Completes instructor information for Google sign-up.
    /// </summary>
    [HttpPost("instructor/complete-info")]
    public async Task<IActionResult> InstructorCompleteInfoSignUpGoogle([FromForm] InstructorSignUpGoogleInfo instructorSignUpGoogleInfo)
    {
        try
        {
            var checkToken = await _authService.InstructorCompleteSignUpByGoogle(instructorSignUpGoogleInfo);
            return Ok(checkToken);
        }
        catch (ApplicationException ex)
        {
            return BadRequest(ex.Message);
        }
    }



    /// <summary>
    /// Retrieves user ID from the JWT token.
    /// </summary>
    [HttpGet("user/id-from-token")]
    public async Task<IActionResult> GetIdFromToken()
    {
        try
        {
            var id = await _authService.GetIdFromToken();
            return Ok(id);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    /// <summary>
    /// Refreshes the JWT token using a refresh token.
    /// </summary>
    [HttpPost("token/refresh")]
    public async Task<IActionResult> RefreshToken(string token)
    {
        try
        {
            var checkRefeshToken = await _authService.RefreshToken(token);
            Response.Cookies.Append("refreshToken", checkRefeshToken.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.Strict,
            });

            return Ok(new UserResponse<object>("Success", new
            {
                TokenType = "Bearer",
                AccessToken = checkRefeshToken.AccessToken
            }));
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Logs out a user and deletes the refresh token.
    /// </summary>
    [HttpPost("user/logout")]
    public async Task<IActionResult> Logout([FromBody] int userId)
    {
        Response.Cookies.Delete("refreshToken");
        await _authService.DeleteRefreshToken(userId);
        return Ok(new UserResponse<object>("Logout Successfully", null));
    }


    /// <summary>
    /// Registers a new student user.
    /// </summary>
    [HttpPost("user/register/student")]
    public async Task<IActionResult> RegisterStudent([FromForm] UserRegistrationDTO userRegistrationDto)
    {
        // Check if the user is authenticated
        if (User.Identity.IsAuthenticated)
        {
            return BadRequest(new { message = "You are already logged in and cannot register again." });
        }
        
        try
        {
            await _authService.RegisterUserAsync(userRegistrationDto);
            return Ok(new { Message = "Registration successful. Please check your email for the OTP." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Registers a new instructor user.
    /// </summary>
    [HttpPost("user/register/instructor")]
    public async Task<IActionResult> RegisterInstructor([FromForm] InstructorRegistrationDTO instructorRegistrationDto)
    {
        // Check if the user is authenticated
        if (User.Identity.IsAuthenticated)
        {
            return BadRequest(new { message = "You are already logged in and cannot register again." });
        }
        try
        {
            await _authService.RegisterInstructorAsync(instructorRegistrationDto);
            return Ok(new { Message = "Registration successful. Please check your email for the OTP." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Verifies the OTP for user email verification.
    /// </summary>
    
    [HttpPost("user/otp/verify")]
    public async Task<IActionResult> VerifyOtp(OtpVerificationDTO otpVerificationDto)
    {
        try
        {
            var isValid = await _authService.VerifyOtpAndCompleteRegistrationAsync(otpVerificationDto.Email, otpVerificationDto.Otp);
            if (!isValid)
            {
                return BadRequest(new { Message = "Invalid OTP or OTP has expired." });
            }

            var user = await _userService.GetByEmail(otpVerificationDto.Email);
            if (user != null)
            {
                user.IsVerified = true;

                if (user.RoleId == 3) // Role 3 is for instructors
                {
                    user.Status = UserStatus.Pending;
                    await _emailService.SendPendingEmailAsync(user.Email);
                }
                else
                {
                    user.Status = UserStatus.Active;
                    await _emailService.SendApprovalEmailAsync(user.Email);
                }

                await _userService.UpdateUserAsync(user);
            }

            return Ok(new { Message = "Email verified successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Changes the user's password.
    /// </summary>
    [HttpPost("user/password/change")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordDTO changePasswordDto)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return Unauthorized(new { Message = "Invalid token." });
            }

            await _authService.ChangePasswordAsync(email, changePasswordDto);
            return Ok(new { Message = "Password changed successfully. Please log in again." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Requests a password reset link to be sent to the user's email.
    /// </summary>
    [HttpPost("user/password/forgot")]
    public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordRequestDTO forgotPasswordRequestDto)
    {
        try
        {
            await _authService.RequestPasswordResetAsync(forgotPasswordRequestDto);
            return Ok(new { Message = "Password reset link sent successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Resets the user's password.
    /// </summary>
    [HttpPost("user/password/reset")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDto)
    {
        try
        {
            await _authService.ResetPasswordAsync(resetPasswordDto);
            return Ok(new { Message = "Password reset successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a list of card providers.
    /// </summary>
    [HttpGet("providers/cards")]
    public IActionResult GetCardProviders()
    {
        var cardProviders = Enum.GetValues(typeof(CardProviderEnum))
                                .Cast<CardProviderEnum>()
                                .Select(e => new { Id = (int)e, Name = e.ToString() })
                                .ToList();
        return Ok(cardProviders);
    }

}
