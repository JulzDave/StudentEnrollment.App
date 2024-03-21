using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using StudentEnrollment.Data;
using AutoMapper;
using StudentEnrollment.Api.DTOs;
namespace StudentEnrollment.Api;

public static class EnrollmentEndpoints
{
    public static void MapEnrollmentEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Enrollment").WithTags(nameof(Enrollment));

        group.MapGet("/", async (StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            var enrollments = await db.Enrollments.ToListAsync();
            return mapper.Map<List<EnrollmentDto>>(enrollments);
        })
        .WithName("GetAllEnrollments")
        .WithOpenApi()
        .Produces<List<EnrollmentDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async Task<Results<Ok<EnrollmentDto>, NotFound>> (int id, StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            return await db.Enrollments.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Enrollment model
                    ? TypedResults.Ok(mapper.Map<EnrollmentDto>(model))
                    : TypedResults.NotFound();
        })
        .WithName("GetEnrollmentById")
        .WithOpenApi()
        .Produces<EnrollmentDto>(StatusCodes.Status200OK)
        .Produces<NotFound>(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", async (int id, Enrollment enrollment, StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            var foundModel = await db.Enrollments.FindAsync(id);

            if (foundModel is null){
                return Results.NotFound();
            }

            mapper.Map(enrollment, foundModel);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("UpdateEnrollment")
        .WithOpenApi()
        .Produces<NoContent>(StatusCodes.Status204NoContent)
        .Produces<NotFound>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateEnrollmentDto enrollmentDto, StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            var enrollment = mapper.Map<Enrollment>(enrollmentDto);
            db.Enrollments.Add(enrollment);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Enrollment/{enrollment.Id}", enrollment);
        })
        .WithName("CreateEnrollment")
        .WithOpenApi()
        .Produces<Created>(StatusCodes.Status201Created);

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, StudentEnrollmentDbContext db) =>
        {
            var affected = await db.Enrollments
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteEnrollment")
        .WithOpenApi()
        .Produces<NoContent>(StatusCodes.Status204NoContent)
        .Produces<NotFound>(StatusCodes.Status404NotFound);
    }
}
