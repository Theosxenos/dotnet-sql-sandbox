using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SqlPractice.Data;

using (var db = new PracticeSqliteContext())
{
    db.Database.EnsureCreated();
}

using (var db = new PracticeSqlServerContext())
{
    db.Database.EnsureCreated();

    using var con = db.Database.GetDbConnection();
    con.Open();
    using var cmd = con.CreateCommand();
    cmd.CommandText =
        "SELECT DISTINCT Product FROM Sales";
    List<string> prods = [];

    using (var reader = cmd.ExecuteReader())
    {
        while (reader.Read())
        {
            prods.Add(reader.GetString(0));
        }
    }
    
    var columns = string.Join(',', prods.Select(p => $"[{p}]"));
    var sql = $"""
               SELECT Month, {columns} 
               FROM (
                   SELECT Month, Product, Total
                   FROM Sales
               ) AS SourceTable 
               PIVOT (SUM(Total) FOR Product in ({columns})) 
               AS PivotTable;
               """;

    cmd.CommandText = sql;
    var rdr = cmd.ExecuteReader();
    while (rdr.Read())
    {
        var s = rdr.GetString("Month");
        
    }
}

DataTable table = new DataTable();
using (var context = new PracticeSqlServerContext())
{
    var connection = context.Database.GetDbConnection();
    await connection.OpenAsync();
    using (var command = connection.CreateCommand())
    {
        // var prods = context.Sales.Select(s => s.Product).Distinct().ToList();
        // var prods = context.Sales.Select(s => s.Product).Distinct().Select(p => $"[{p}]").ToList();
        // var columns = string.Join(',', prods.Select(p => $"[{p}]"));
        var columns = string.Join(",", context.Sales.Select(s => s.Product).Distinct().Select(p => $"[{p}]"));
        command.CommandText = $"""
                               SELECT Month, {columns}
                               FROM (
                                   SELECT Month, Product, Total
                                   FROM Sales
                               ) AS SourceTable
                               PIVOT (SUM(Total) FOR Product in ({columns}))
                               AS PivotTable;
                               """;
        using (var reader = await command.ExecuteReaderAsync())
        {
            table.Load(reader);
        }
    }
}

