#region pivotstuff

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

Pivot.PivotDict();
Pivot.DataTable();

using (var con = Database.GetConnection())
{
    using var cmd = con.CreateCommand();
    cmd.CommandText = $"""

                       DECLARE @columns NVARCHAR(MAX), @sql NVARCHAR(MAX);

                       -- Prepare the column list for the PIVOT IN clause
                       SELECT @columns = COALESCE(@columns + ',','') + QUOTENAME(Product)
                       FROM (SELECT DISTINCT Product FROM Sales) AS Products;

                       -- Construct the full SQL statement
                       SET @sql = N'SELECT Month, ' + @columns + N'
                                    FROM
                                    (
                                        SELECT Product, Month, Total
                                        FROM Sales
                                    ) x
                                    PIVOT
                                    (
                                        SUM(Total)
                                        FOR Product IN (' + @columns + N')
                                    ) p ';

                       -- Execute the dynamic SQL
                       EXEC sp_executesql @sql;
                       """;
    var dataTable = new DataTable();
    using (var reader = cmd.ExecuteReader())
    {
        dataTable.Load(reader);
    }
}




#endregion

DapperThings.DapperPivot();
DapperThings.DapperJoins();

// SqlServerEfTimespan.Add();
Console.WriteLine(SqlServerEfTimespan.Get());