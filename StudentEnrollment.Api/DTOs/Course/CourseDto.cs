using FluentValidation;

namespace StudentEnrollment.Api.DTOs
{
    public class CourseDto: CreateCourseDto
    {
        public int Id { get; set; }
    }
    public class CourseDtoValidator : AbstractValidator<CourseDto>
    {

        public CourseDtoValidator()
        {
            Include(new CreateCourseDtoValidator());
            
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}