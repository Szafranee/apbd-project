using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project_01.Data;
using Project_01.Domain.Employees;
using Project_01.Domain.RefreshTokens;
using Project_01.Interfaces;
using Project_01.RequestModels.Auth;
using Project_01.ResponseModels.Auth;

namespace Project_01.Services;

public class AuthService(DatabaseContext context, IConfiguration config) : IAuthService
{
    public async Task<LoginResponseModel> LoginAsync(LoginRequestModel model)
    {
        var employee = await context.Employees.FirstOrDefaultAsync(e => e.Login == model.Login);
        if (employee != null && VerifyPassword(model.Password, employee.PasswordHash, employee.Salt))
        {
            var token = GenerateJwtToken(employee);
            var refreshToken = GenerateRefreshToken();

            context.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiryDate = DateTime.Now.AddDays(7),
                EmployeeId = employee.Id
            });
            await context.SaveChangesAsync();

            return new LoginResponseModel
            {
                Token = token,
                RefreshToken = refreshToken
            };
        }

        return null;
    }

    public async Task<LoginResponseModel> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await context.RefreshTokens
            .Include(rt => rt.Employee)
            .SingleOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null || storedToken.ExpiryDate <= DateTime.Now) return null;

        var employee = storedToken.Employee;
        var newJwtToken = GenerateJwtToken(employee);
        var newRefreshToken = GenerateRefreshToken();

        storedToken.Token = newRefreshToken;
        storedToken.ExpiryDate = DateTime.Now.AddDays(7);
        context.RefreshTokens.Update(storedToken);
        await context.SaveChangesAsync();

        return new LoginResponseModel
        {
            Token = newJwtToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task<bool> RegisterAsync(RegisterRequestModel model)
    {
        if (await context.Employees.AnyAsync(e => e.Login == model.Login))
        {
            return false;
        }

        var (hashedPassword, salt) = HashPassword(model.Password);

        var employee = new Employee
        {
            Login = model.Login,
            PasswordHash = hashedPassword,
            Salt = salt,
            Role = "Standard" // Default role
        };

        context.Employees.Add(employee);
        await context.SaveChangesAsync();
        return true;
    }

    private string GenerateJwtToken(Employee employee)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
            new Claim(ClaimTypes.Name, employee.Login),
            new Claim(ClaimTypes.Role, employee.Role)
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        var salt = Convert.FromBase64String(storedSalt);
        var hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8);

        return Convert.ToBase64String(hash) == storedHash;
    }

    private static (string hash, string salt) HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(128 / 8);
        var hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8);

        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }
}