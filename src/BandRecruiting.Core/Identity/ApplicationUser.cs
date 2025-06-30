using BandRecruiting.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace BandRecruiting.Core.Identity;

/// <summary>
/// Extend later with profile data (e.g. FirstName, LastName) while keeping the entity in the Core layer
/// so that the Domain doesn’t depend on EF-specific types.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    // 🔑 Add this so EF can map AspNetUserTokens and LINQ can see it
    public ICollection<IdentityUserToken<Guid>> Tokens { get; set; } = new List<IdentityUserToken<Guid>>();
    public Student? Student { get; set; }
}
