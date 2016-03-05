using System.Linq;
using System.Reflection;
using Xunit;

namespace dnex2winjs.Test
{
    public class DNEXTest
    {
        [Fact]
        public void GetAllExceptionsTest()
        {
            // Arrange
            var assembly = Assembly.Load("mscorlib");
            var allExceptions = assembly.GetTypes().Where(t => t.IsPublic)
                                                .Where(t => t.Name != "_Exception")
                                                .Where(t => t.Name.EndsWith("Exception"))
                                                .Select(t => t.Name);

            // Act
            var result = DNEX.GetAllExceptions();

            // Assert
            Assert.Equal(allExceptions.Count(), result.Count);
            Assert.Equal(allExceptions.Intersect(result.Keys).Count(), result.Count);
        }
    }
}
