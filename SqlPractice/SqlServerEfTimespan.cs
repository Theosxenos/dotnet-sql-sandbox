namespace SqlPractice;

public class SqlServerEfTimespan
{
    public static void Add()
    {
        using var context = new PracticeSqlServerContext();
        var start = DateTime.Now;
        var end = start.AddHours(1).AddMinutes(30);
        context.Shifts.Add(new()
        {
            Start = start,
            End = end,
            Duration = end - start
        });
        context.SaveChanges();
    }

    public static TimeSpan Get()
    {
        using (var context = new PracticeSqlServerContext())
        {
            return context.Shifts.First().Duration;
        }
    }
}