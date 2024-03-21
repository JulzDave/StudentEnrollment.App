namespace StudentEnrollment.Api.DTOs
{
    public class EnrollmentDto : CreateEnrollmentDto
    {
        public virtual CourseDto Course { get; set; }
        public virtual StudentDto Student { get; set; }
    }
}