using Microsoft.AspNetCore.Http.HttpResults;
using StudentEnrollment.Data;
using AutoMapper;
using StudentEnrollment.Api.DTOs;
using StudentEnrollment.Data.Contracts;
using FluentValidation;
namespace StudentEnrollment.Api.Endpoints;

public static class EnrollmentEndpoints
{
    public static void MapEnrollmentEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Enrollment").WithTags(nameof(Enrollment));

        group.MapGet("/", async (IEnrollmentRepository repo, IMapper mapper) =>
        {
            var enrollments = await repo.GetAllAsync();
            return mapper.Map<List<EnrollmentDto>>(enrollments);
        })
        .WithName("GetAllEnrollments")
        .WithOpenApi()
        .Produces<List<EnrollmentDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async Task<Results<Ok<EnrollmentDto>, NotFound>> (int id, IEnrollmentRepository repo, IMapper mapper) =>
        {
            return await repo.GetAsync(id)
                is Enrollment model
                    ? TypedResults.Ok(mapper.Map<EnrollmentDto>(model))
                    : TypedResults.NotFound();
        })
        .WithName("GetEnrollmentById")
        .WithOpenApi()
        .Produces<EnrollmentDto>(StatusCodes.Status200OK)
        .Produces<NotFound>(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", async (int id, EnrollmentDto enrollmentDto, IEnrollmentRepository repo, IMapper mapper, IValidator<EnrollmentDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(enrollmentDto);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            var foundModel = await repo.GetAsync(id);

            if (foundModel is null)
            {
                return Results.NotFound();
            }

            mapper.Map(enrollmentDto, foundModel);
            await repo.UpdateAsync(foundModel);

            return Results.NoContent();
        })
        .WithName("UpdateEnrollment")
        .WithOpenApi()
        .Produces<NoContent>(StatusCodes.Status204NoContent)
        .Produces<NotFound>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateEnrollmentDto enrollmentDto, IEnrollmentRepository repo, IMapper mapper, IValidator<CreateEnrollmentDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(enrollmentDto);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            
            var enrollment = mapper.Map<Enrollment>(enrollmentDto);
            await repo.AddAsync(enrollment);
            return TypedResults.Created($"/api/Enrollment/{enrollment.Id}", enrollment);
        })
        .WithName("CreateEnrollment")
        .WithOpenApi()
        .Produces<Created>(StatusCodes.Status201Created);

        group.MapDelete("/{id}", async (int id, IEnrollmentRepository repo) =>
        {
            return await repo.DeleteAsync(id) ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteEnrollment")
        .RequireAuthorization(x => x.RequireRole("Administrator"))
        .WithOpenApi()
        .Produces<NoContent>(StatusCodes.Status204NoContent)
        .Produces<NotFound>(StatusCodes.Status404NotFound);
    }
}
