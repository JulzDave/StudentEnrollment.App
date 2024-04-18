using FluentValidation;

namespace StudentEnrollment.Api.DTOs
{
    public class StudentDto : CreateStudentDto
    {
        public int Id { get; set; }
    }

    public class StudentDtoValidator : AbstractValidator<StudentDto>
    {
        public StudentDtoValidator()
        {
            Include(new StudentDtoValidator());

            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}