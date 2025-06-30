using BandRecruiting.Core.Entities;

namespace BandRecruiting.Application.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student?> GetAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(Student student, CancellationToken ct = default);
    }
}
