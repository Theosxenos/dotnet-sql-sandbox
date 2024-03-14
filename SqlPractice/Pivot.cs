namespace SqlPractice;

public class Pivot
{
    public static void PivotDict()
    {
        using var context = new PracticeSqlServerContext();
        using var connection = context.Database.GetDbConnection();
        connection.Open();
        using var command = connection.CreateCommand();

        var columns = string.Join(",", context.Sales.Select(s => $"[{s.Product}]").Distinct());
        command.CommandText = $"""
                               SELECT Month, {columns}
                               FROM (
                                   SELECT Month, Product, Total
                                   FROM Sales
                               ) AS SourceTable
                               PIVOT (SUM(Total) FOR Product in ({columns}))
                               AS PivotTable;
                               """;

        var salesData = new Dictionary<string, Dictionary<string, int>>();
        using var reader = command.ExecuteReader();
        while (reader.Read()) // Assuming 'reader' is an SqlDataReader or similar
        {
            var month = reader.GetString(0); 
            salesData[month] = new();
            for (int i = 1; i < reader.FieldCount; i++)
            {
                var product = reader.GetName(i); 
                var sales = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                salesData[month].Add(product, sales);
            }
        }

        // Assuming all rows have the same columns, you can get the list of columns from the first item
        var allMonths = salesData.Values.First().Keys;

        // Print header
        Console.Write("Month\t");
        foreach (var month in allMonths)
        {
            Console.Write($"{month}\t");
        }

        Console.WriteLine();

        // Print each product's sales data
        foreach (var productEntry in salesData)
        {
            Console.Write($"{productEntry.Key}\t");
            foreach (var month in allMonths)
            {
                // Check if the month exists for the current product to handle cases where it might not
                decimal sales = productEntry.Value.TryGetValue(month, out var salesValue) ? salesValue : 0;
                Console.Write($"{sales}\t");
            }

            Console.WriteLine();
        }
    }

    public static void DataTable()
    {
        var table = new DataTable();
        using var context = new PracticeSqlServerContext();
        var connection = context.Database.GetDbConnection();
        connection.Open();
        using var command = connection.CreateCommand();

        var columns = string.Join(",",
            context.Sales.Select(s => s.Product).Distinct().Select(p => $"[{p}]"));
        command.CommandText = $"""
                               SELECT Month, {columns}
                               FROM (
                                   SELECT Month, Product, Total
                                   FROM Sales
                               ) AS SourceTable
                               PIVOT (SUM(Total) FOR Product in ({columns}))
                               AS PivotTable;
                               """;
        using (var reader = command.ExecuteReader())
        {
            table.Load(reader);
        }
        
        // Print column headers
        foreach (DataColumn column in table.Columns)
        {
            Console.Write($"{column.ColumnName}\t");
        }

        Console.WriteLine();

        // Print rows
        foreach (DataRow row in table.Rows)
        {
            foreach (var item in row.ItemArray)
            {
                Console.Write($"{item}\t");
            }

            Console.WriteLine();
        }

    }
}