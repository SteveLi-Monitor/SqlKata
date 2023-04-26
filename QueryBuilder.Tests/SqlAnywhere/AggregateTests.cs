using SqlKata.Compilers;
using SqlKata.Tests.Infrastructure;
using Xunit;

namespace SqlKata.Tests.SqlAnywhere
{
    public class AggregateTests : TestSupport
    {
        [Fact]
        public void Count()
        {
            var query = new Query("A").AsCount();

            var c = Compile(query);

            Assert.Equal("SELECT COUNT(*) AS \"count\" FROM \"A\"", c[EngineCodes.SqlAnywhere]);
        }
    }
}
