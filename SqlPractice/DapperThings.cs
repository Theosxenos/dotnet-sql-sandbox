using System.Data.Common;

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
    }

    public static void DapperJoins()
    {
        using var context = new PracticeSqlServerContext();
        using var connection = context.Database.GetDbConnection();

        SingleQuery(connection);
        MultiQuery(connection);
    }

    private static void MultiQuery(DbConnection connection)
    {
        // Define the query to get users and their logs
        var sql = """
                  SELECT * FROM Users;
                  SELECT * FROM UserLogs;
                  """;

        using var multi = connection.QueryMultiple(sql);
        var users = multi.Read<User>().ToList();
        var logs = multi.Read<UserLog>().ToList();

        foreach (var user in users)
        {
            user.UserLogs = logs.Where(log => log.UserId == user.Id).ToList();
        }
    }

    //TODO Doesn't properly map the UserLog.Id property
    private static void SingleQuery(DbConnection connection)
    {
        var joinQuery = connection.Query("select * from Users join UserLogs UL on Users.Id = UL.UserId");
        var userDictionary = new Dictionary<int, User>();

        var sql = """
                  SELECT u.*, ul.Id as UserLogId, ul.Action, ul.Date, ul.UserId
                  FROM Users u
                  INNER JOIN UserLogs ul ON u.Id = ul.UserId
                  """;

        var users = connection.Query<User, UserLog, User>(
            sql,
            (user, userLog) => 
            {
                if (!userDictionary.TryGetValue(user.Id, out var userEntry))
                {
                    userEntry = user;
                    userEntry.UserLogs = [];
                    userDictionary.Add(userEntry.Id, userEntry);
                }

                userEntry.UserLogs.Add(userLog);
                return userEntry;
            },
            splitOn: "UserLogId" // Adjust the splitOn parameter to match the first column of the second entity (UserLog in this case)
        ).Distinct().ToList();
    }
}