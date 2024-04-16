using Microsoft.AspNetCore.Http.HttpResults;
using StudentEnrollment.Data;
using AutoMapper;
using StudentEnrollment.Api.DTOs;
using StudentEnrollment.Data.Contracts;
namespace StudentEnrollment.Api.Endpoints;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Course").WithTags(nameof(Course));

        group.MapGet("/", async (ICourseRepository repo, IMapper mapper) =>
        {
            var courses = await repo.GetAllAsync();
            var data = mapper.Map<List<CourseDto>>(courses);

            return data;
        })
        .WithName("GetAllCourses")
        .AllowAnonymous()
        .WithOpenApi()
        .Produces<List<CourseDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", async Task<Results<Ok<CourseDto>, NotFound>> (int id, ICourseRepository repo, IMapper mapper) =>
        {
            return await repo.GetAsync(id)
                is Course model
                    ? TypedResults.Ok(mapper.Map<CourseDto>(model))
                    : TypedResults.NotFound();
        })
        .WithName("GetCourseById")
        .AllowAnonymous()
        .WithOpenApi()
        .Produces<CourseDto>(StatusCodes.Status200OK)
        .Produces<NotFound>(StatusCodes.Status404NotFound);

        group.MapGet("GetStudents/{id}", async Task<Results<Ok<CourseDetailsDto>, NotFound>> (int id, ICourseRepository repo, IMapper mapper) =>
        {
            return await repo.GetStudentList(id)
                is Course model
                    ? TypedResults.Ok(mapper.Map<CourseDetailsDto>(model))
                    : TypedResults.NotFound();
        })
        .WithName("GetCourseDetailsById")
        .WithOpenApi()
        .Produces<CourseDetailsDto>(StatusCodes.Status200OK)
        .Produces<NotFound>(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", async (int id, CourseDto courseDto, ICourseRepository repo, IMapper mapper) =>
        {
            var foundModel = await repo.GetAsync(id);

            if (foundModel is null)
            {
                return Results.NotFound();
            }

            mapper.Map(courseDto, foundModel);
            await repo.UpdateAsync(foundModel);
            return Results.NoContent();
        })
        .WithName("UpdateCourse")
        .WithOpenApi()
        .Produces<NoContent>(StatusCodes.Status204NoContent)
        .Produces<NotFound>(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateCourseDto courseDto, ICourseRepository repo, IMapper mapper) =>
        {
            var course = mapper.Map<Course>(courseDto);
            await repo.AddAsync(course);
            return TypedResults.Created($"/api/Course/{course.Id}", course);
        })
        .WithName("CreateCourse")
        .WithOpenApi()
        .Produces<Created<Course>>(StatusCodes.Status201Created);

        group.MapDelete("/{id}", async (int id, ICourseRepository repo) =>
        {

            return await repo.DeleteAsync(id) ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteCourse")
        .RequireAuthorization(x => x.RequireRole("Administrator"))
        .WithOpenApi()
        .Produces<Ok>(StatusCodes.Status204NoContent)
        .Produces<NotFound>(StatusCodes.Status404NotFound);
    }
}
