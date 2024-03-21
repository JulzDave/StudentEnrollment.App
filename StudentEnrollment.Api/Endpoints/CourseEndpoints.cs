using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using StudentEnrollment.Data;
using AutoMapper;
using StudentEnrollment.Api.DTOs;
namespace StudentEnrollment.Api;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Course").WithTags(nameof(Course));

        group.MapGet("/", async (StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            var courses = await db.Courses.ToListAsync();
            var data = mapper.Map<List<CourseDto>>(courses);

            return data;
        })
        .WithName("GetAllCourses")
        .WithOpenApi()
        .Produces<List<CourseDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async Task<Results<Ok<CourseDto>, NotFound>> (int id, StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            return await db.Courses.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Course model
                    ? TypedResults.Ok(mapper.Map<CourseDto>(model))
                    : TypedResults.NotFound();
        })
        .WithName("GetCourseById")
        .WithOpenApi()
        .Produces<CourseDto>(StatusCodes.Status200OK)
        .Produces<NotFound>(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", async (int id, CourseDto courseDto, StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            var foundModel = await db.Courses.FindAsync(id);

            if (foundModel is null){
                return Results.NotFound();
            }

            mapper.Map(courseDto, foundModel);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("UpdateCourse")
        .WithOpenApi()
        .Produces<NoContent>(StatusCodes.Status204NoContent)
        .Produces<NotFound>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateCourseDto courseDto, StudentEnrollmentDbContext db, IMapper mapper) =>
        {
            var course = mapper.Map<Course>(courseDto);
            db.Courses.Add(course);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Course/{course.Id}", course);
        })
        .WithName("CreateCourse")
        .WithOpenApi()
        .Produces<Created<Course>>(StatusCodes.Status201Created);

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, StudentEnrollmentDbContext db) =>
        {
            var affected = await db.Courses
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteCourse")
        .WithOpenApi()
        .Produces<Ok>(StatusCodes.Status200OK)
        .Produces<NotFound>(StatusCodes.Status404NotFound);
    }
}
