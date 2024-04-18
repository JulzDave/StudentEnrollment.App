using FluentValidation;
using StudentEnrollment.Api.DTOs.Authentication;
using StudentEnrollment.Api.DTOs.ErrorResponseDto;
using StudentEnrollment.Api.Services;

namespace StudentEnrollment.Api.Endpoints
{
    public static class AuthenticationEndpoints
    {
        public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api").WithTags("Authentication");

            group.MapPost("/Login", async (LoginDto loginDto, IAuthManager authManager, IValidator<LoginDto> validator) =>
            {
                var validationResult = validator.Validate(loginDto);

                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.ToDictionary());
                }

                var response = await authManager.Login(loginDto);

                if (response is null)
                {
                    return Results.Unauthorized();
                }

                return Results.Ok(response);
            })
            .WithName("Login")
            .AllowAnonymous()
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapPost("/Register", async (RegisterDto registerDto, IAuthManager authManager, IValidator<RegisterDto> validator) =>
            {

                var validationResult = validator.Validate(registerDto);

                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(validationResult.ToDictionary());
                }

                var response = await authManager.Register(registerDto);

                if (!response.Any())
                {
                    return Results.Ok();
                }
                var errors = new List<ErrorResponseDto>();
                foreach (var error in response)
                {
                    errors.Add(new ErrorResponseDto
                    {
                        Code = error.Code,
                        Description = error.Description
                    });
                }

                return Results.BadRequest(errors);
            })
            .WithName("Register")
            .AllowAnonymous()
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
