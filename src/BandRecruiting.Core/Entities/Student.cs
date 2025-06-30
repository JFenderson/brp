using BandRecruiting.Core.Identity;

namespace BandRecruiting.Core.Entities;

/// <summary>
/// Each Student is a 1-to-1 extension of an ApplicationUser that represents a
/// musician prospect.  The aggregate root is Student because we’ll eventually
/// hang Videos, Ratings, Interests, etc. beneath it.
/// </summary>
public sealed class Student
{
    // PK – generated in application code so the API can return it immediately
    public Guid StudentId { get; private set; } = Guid.NewGuid();

    // FK – Identity user that owns this profile
    public Guid UserId { get; private set; }

    // Value objects could be richer later; keep simple for now
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Instrument { get; private set; } = default!;
    public string HighSchool { get; private set; } = default!;
    public string? ProfilePictureUrl { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // Navigation (optional for now, but enables Include())
    public ApplicationUser User { get; private set; } = default!;

    // Empty ctor for EF
    private Student() { }

    // Factory-style constructor keeps aggregate consistent
    public Student(Guid userId,
                   string firstName,
                   string lastName,
                   string instrument,
                   string highSchool,
                   string? profilePictureUrl = null)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        Instrument = instrument;
        HighSchool = highSchool;
        ProfilePictureUrl = profilePictureUrl;
    }
}
