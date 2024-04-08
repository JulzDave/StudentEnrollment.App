using Microsoft.AspNetCore.Identity;
using StudentEnrollment.Api.Services;
using StudentEnrollment.Data;

namespace StudentEnrollment.Api.Endpoints
{
    public static class AuthenticationEndpoints
    {
        public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/Login").WithTags("Authentication");

            group.MapPost("/", async (LoginDto loginDto, IAuthManager authManager) =>
            {
                var response = await authManager.Login(loginDto);

                if (response is null)
                {
                    return Results.Unauthorized();
                }

                return Results.Ok(response);
            })
            .WithName("Login")
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}
