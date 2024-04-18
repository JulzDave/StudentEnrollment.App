using FluentValidation;
using StudentEnrollment.Data.Contracts;

namespace StudentEnrollment.Api.DTOs
{
    public class EnrollmentDto : CreateEnrollmentDto
    {
        public virtual CourseDto Course { get; set; }
        public virtual StudentDto Student { get; set; }
    }

    public class EnrollmentDtoValidator : AbstractValidator<EnrollmentDto>
    {
        public EnrollmentDtoValidator(ICourseRepository courseRepository, IStudentRepository studentRepository)
        {
            Include(new CreateEnrollmentDtoValidator(courseRepository, studentRepository));
        }
    }
}