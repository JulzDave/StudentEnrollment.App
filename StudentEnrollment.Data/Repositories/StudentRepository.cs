using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Data.Contracts;

namespace StudentEnrollment.Data.Repositories
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(StudentEnrollmentDbContext db) : base(db)
        {
        }

        async Task<Student> IStudentRepository.GetStudentDetails(int studentId)
        {
            var student = await _db.Students
                .Include(s => s.Enrollments).ThenInclude(q => q.Course)
                .SingleOrDefaultAsync(q => q.Id == studentId);

            return student;
        }
    }
}
