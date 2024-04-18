using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StudentEnrollment.Api.DTOs.Authentication;
using StudentEnrollment.Data;

namespace StudentEnrollment.Api.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<SchoolUser> _userManager;
        private readonly IConfiguration _configuration;
        private SchoolUser? _user;

        public AuthManager(UserManager<SchoolUser> userManager, IConfiguration configuration)
        {
            this._configuration = configuration;
            this._userManager = userManager;
        }
        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            var _user = await _userManager.FindByEmailAsync(loginDto.EmailAddress);
            if (_user is null)
            {
                return default;
            }
            bool isValidCredentials = await _userManager.CheckPasswordAsync(_user, loginDto.Password);
            if (!isValidCredentials)
            {
                return default;
            }

            var token = await GenerateTokenAsync(_user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = _user.Id
            };
        }

        public async Task<IEnumerable<IdentityError>> Register(RegisterDto registerDto)
        {
            var _user = new SchoolUser
            {
                Email = registerDto.EmailAddress,
                UserName = registerDto.EmailAddress,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                DateOfBirth = registerDto.DateOfBirth
            };

            var result = await _userManager.CreateAsync(_user, registerDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(_user, "User");
            }

            return result.Errors;
        }

        private async Task<string> GenerateTokenAsync(SchoolUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("userId", user.Id),
            }.Union(userClaims).Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(Convert.ToInt32(_configuration["JwtSettings:DurationInHours"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}