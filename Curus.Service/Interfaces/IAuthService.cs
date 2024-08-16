using Curus.Repository.Entities;
using Curus.Repository.ViewModels;
using Curus.Repository.ViewModels.Request;
using Curus.Repository.ViewModels.Response;
using Curus.Service.ResponseDTO;

namespace Curus.Service.Interfaces;

public interface IAuthService
{
    Task<string>  GetIdFromToken();

    Task RegisterUserAsync(UserRegistrationDTO userRegistrationDto);


    //Google
    Task<UserResponse<object>> UserGetInfoSignUpByGoogle(string token);

    Task<UserResponse<object>> InstructorCompleteSignUpByGoogle(InstructorSignUpGoogleInfo instructorRegistrationDto);
    Task<UserResponse<object>> UserCompleteSignUpByGoogle(SignupGoogleRequest userRegistrationDto);

    //REGISTER
    Task<User> GetByVerificationToken(string token);
    Task<bool> VerifyOtpAsync(string email, string otp);

    //------- SPRINT 1 -------
    //REGISTER INSTRUCTOR
    Task RegisterInstructorAsync(InstructorRegistrationDTO instructorRegistrationDto);
    List<string> GetCardProviders();
    Task<bool> VerifyOtpAndCompleteRegistrationAsync(string email, string otp);

    //LOGIN
    Task<Authenticator> LoginAsync(LoginDTO loginDTO);
    Task<Authenticator> AuthenGoogleUser(string token);
    Task<Authenticator> RefreshToken(string token);   
    Task<bool> DeleteRefreshToken(int userId);


    //CHANGE PASSWORD 
    Task ChangePasswordAsync(string email, ChangePasswordDTO changePasswordDto);

    //FORGOT PASSWORD
    Task RequestPasswordResetAsync(ForgotPasswordRequestDTO forgotPasswordRequestDto);
    Task ResetPasswordAsync(ResetPasswordDTO resetPasswordDto);
}