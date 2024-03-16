using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentEnrollment.Api.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
    }

    public class CreateCourseDto
    {
        public string Title { get; set; }
        public int Credits { get; set; }
    }
}