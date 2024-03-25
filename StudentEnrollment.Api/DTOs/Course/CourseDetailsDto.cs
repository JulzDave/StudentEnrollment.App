namespace StudentEnrollment.Api.DTOs
{
    public class CourseDetailsDto : CourseDto
    {
        public List<StudentDto> Students { get; set; } = new List<StudentDto>();
    }
}