namespace SqlKata.Compilers
{
    public class SqlAnywhereCompiler : Compiler
    {
        public SqlAnywhereCompiler()
        {
            parameterPrefix = ":p";
        }

        public override string EngineCode => EngineCodes.SqlAnywhere;
    }
}
