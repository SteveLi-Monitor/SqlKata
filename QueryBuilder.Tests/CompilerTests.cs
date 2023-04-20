using SqlKata.Compilers;
using SqlKata.Tests.Infrastructure;
using Xunit;

namespace SqlKata.Tests
{
    public class CompilerTests : TestSupport
    {
        [Fact]
        public void Compile_SqlStr_WithArgs()
        {
            var sql = "SELECT * FROM Table1 WHERE Column1 = ? AND Column2 = ?";

            var c = Compilers.Compile(sql, 10, 20);

            var sqlServer = c[EngineCodes.SqlServer];
            Assert.Equal("SELECT * FROM Table1 WHERE Column1 = @p0 AND Column2 = @p1", sqlServer.Sql);
            Assert.Equal(10, sqlServer.NamedBindings["@p0"]);
            Assert.Equal(20, sqlServer.NamedBindings["@p1"]);
        }
    }
}
