using BandRecruiting.Core.Entities;

namespace BandRecruiting.UnitTests
{
    public class CoreTests
    {
        [Fact]
        public void Student_Ctor_SetsProperties()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var student = new Student(userId, "Miles", "Davis", "Trumpet", "Central HS");

            // Assert
            Assert.Equal(userId, student.UserId);
            Assert.Equal("Miles", student.FirstName);
            Assert.Equal("Davis", student.LastName);
            Assert.Equal("Trumpet", student.Instrument);
            Assert.Equal("Central HS", student.HighSchool);
            Assert.Null(student.ProfilePictureUrl); // since you didn't set one
        }
    }
}
