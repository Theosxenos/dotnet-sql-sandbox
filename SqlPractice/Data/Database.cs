using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace SqlPractice.Data;

public class Database
{
    public static DbConnection GetConnection()
    {
        var context = new PracticeSqlServerContext();
        var connection = context.Database.GetDbConnection();
        connection.Open();
        return connection;
    }
}