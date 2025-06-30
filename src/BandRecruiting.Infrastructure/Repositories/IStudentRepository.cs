using BandRecruiting.Application.Interfaces;
using BandRecruiting.Core.Entities;
using BandRecruiting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace BandRecruiting.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _db;

        public StudentRepository(ApplicationDbContext db)
            => _db = db;

        public async Task<Student?> GetAsync(Guid id, CancellationToken ct = default) =>
            await _db.Students
                     .Include(s => s.User)
                     .FirstOrDefaultAsync(s => s.StudentId == id, ct);

        public async Task AddAsync(Student student, CancellationToken ct = default)
        {
            _db.Students.Add(student);
            await _db.SaveChangesAsync(ct);
        }
    }
}
