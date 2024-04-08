using Microsoft.AspNetCore.Http.HttpResults;
using StudentEnrollment.Api.Endpoints;

namespace StudentEnrollment.Api.Services
{
    public interface IAuthManager
    {
        Task<AuthResponseDto> Login(LoginDto loginDto);
    }
}