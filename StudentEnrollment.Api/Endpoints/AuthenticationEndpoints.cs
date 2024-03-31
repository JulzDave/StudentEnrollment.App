using Microsoft.AspNetCore.Identity;
using StudentEnrollment.Data;

namespace StudentEnrollment.Api.Endpoints
{
    public static class AuthenticationEndpoints
    {
        public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/Login").WithTags("Authentication");

            group.MapPost("/", async (LoginDto loginDto, UserManager<SchoolUser> userManager) =>
            {
                var user = await userManager.FindByEmailAsync(loginDto.Username);
                if (user is null)
                {
                    return Results.Unauthorized();
                }
                bool isValidCredentials = await userManager.CheckPasswordAsync(user, loginDto.Password);
                if (!isValidCredentials)
                {
                    return Results.Unauthorized();
                }

                return Results.Ok();
            })
            .WithName("Login")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}
