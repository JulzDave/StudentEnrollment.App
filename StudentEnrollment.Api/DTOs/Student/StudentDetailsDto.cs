namespace StudentEnrollment.Api.DTOs
{
    public class StudentDetailsDto : StudentDto
    {
        public List<CourseDto> Courses { get; set; } = new List<CourseDto>();
    }
}