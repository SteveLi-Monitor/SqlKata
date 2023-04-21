namespace SqlKata.Compilers
{
    public class SqlAnywhereCompiler : Compiler
    {
        public SqlAnywhereCompiler()
        {
            parameterPrefix = ":p";
        }

        public override string EngineCode => EngineCodes.SqlAnywhere;

        protected override SqlResult CompileSelectQuery(Query query)
        {
            if (!query.HasOffset(EngineCode))
            {
                return base.CompileSelectQuery(query);
            }

            query = query.Clone();

            var ctx = new SqlResult
            {
                Query = query,
            };

            var limit = query.GetLimit(EngineCode);
            var offset = query.GetOffset(EngineCode);

            if (!query.HasComponent("select"))
            {
                query.Select("*");
            }

            var order = CompileOrders(ctx) ?? "ORDER BY (SELECT 0)";

            query.SelectRaw($"ROW_NUMBER() OVER ({order}) AS [row_num]", ctx.Bindings.ToArray());

            query.ClearComponent("order");

            var result = base.CompileSelectQuery(query);

            if (limit == 0)
            {
                result.RawSql = $"SELECT * FROM ({result.RawSql}) AS \"results_wrapper\" WHERE \"row_num\" >= {parameterPlaceholder}";
                result.Bindings.Add(offset + 1);
            }
            else
            {
                result.RawSql = $"SELECT * FROM ({result.RawSql}) AS \"results_wrapper\" WHERE \"row_num\" BETWEEN {parameterPlaceholder} AND {parameterPlaceholder}";
                result.Bindings.Add(offset + 1);
                result.Bindings.Add(limit + offset);
            }

            return result;
        }

        protected override string CompileColumns(SqlResult ctx)
        {
            var compiled = base.CompileColumns(ctx);

            var limit = ctx.Query.GetLimit(EngineCode);
            var offset = ctx.Query.GetOffset(EngineCode);

            // If there is a limit on the query, but not an offset, we will add the top clause to the query
            if (limit > 0 && offset == 0)
            {
                // top bindings should be inserted first
                ctx.Bindings.Insert(0, limit);

                ctx.Query.ClearComponent("limit");

                // handle distinct
                if (compiled.IndexOf("SELECT DISTINCT") == 0)
                {
                    return $"SELECT DISTINCT TOP ({parameterPlaceholder}){compiled.Substring(15)}";
                }

                return $"SELECT TOP ({parameterPlaceholder}){compiled.Substring(6)}";
            }

            return compiled;
        }

        public override string CompileLimit(SqlResult ctx)
        {
            return null;
        }

        public override string CompileTrue()
        {
            return "1";
        }

        public override string CompileFalse()
        {
            return "0";
        }
    }
}
