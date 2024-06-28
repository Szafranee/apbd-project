using Project_01.RequestModels.Auth;
using Project_01.ResponseModels.Auth;

namespace Project_01.Interfaces;

public interface IAuthService
{
    Task<LoginResponseModel> LoginAsync(LoginRequestModel model);
    Task<LoginResponseModel> RefreshTokenAsync(string refreshToken);
    Task<bool> RegisterAsync(RegisterRequestModel model);
}