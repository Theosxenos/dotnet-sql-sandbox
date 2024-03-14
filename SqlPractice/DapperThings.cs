using Dapper;

namespace SqlPractice;

public class DapperThings
{
    public static void DapperPivot()
    {
        using var context = new PracticeSqlServerContext();
        using var connection = context.Database.GetDbConnection();

        var sql = """
                  select * from (
                      select  Month, Product, Total from Sales
                  ) as SourceTable
                  pivot ( Sum(Total) for Month in ([Jan], [Feb]) ) as PivotTable
                  """;

        using var multi = connection.QueryMultiple(sql);
        var multiResult = multi.Read<(string, int, int)>();
        
        using var multi2 = connection.QueryMultiple(sql);
        var multiResultDynamic = multi2.Read<dynamic>();


        var single = connection.Query(sql);

        Console.ReadKey();
    }
}