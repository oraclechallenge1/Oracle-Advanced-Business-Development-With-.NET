using MedSave.DTOs.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MedSave.Repositories.UsersSys.Interfaces;
using MedSave.Services.UsersSys;

namespace MedSave.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUsersSysRepository _usersSysRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUsersSysRepository usersSysRepository,
            IConfiguration configuration)
        {
            _usersSysRepository = usersSysRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequest)
        {
            var user = await _usersSysRepository.GetByEmailAsync(loginRequest.Email);

            if (user == null)
                throw new Exception("Usuário não encontrado.");

            var senhaValida = BCrypt.Net.BCrypt.Verify(
                loginRequest.PasswordUser,
                user.PasswordUser
            );

            if (!senhaValida)
                throw new Exception("Senha inválida.");

            var token = GenerateJwtToken(user);

            return new LoginResponseDTO
            {
                Token = token,
                UserId = user.UserId,
                NameUser = user.NameUser,
                Email = user.Email,
                RoleUserId = user.RoleUserId,
                ProfUserId = user.ProfUserId
            };
        }

        private string GenerateJwtToken(Model.UsersSys user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey!)
            );

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim("userId", user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddHours(25),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}