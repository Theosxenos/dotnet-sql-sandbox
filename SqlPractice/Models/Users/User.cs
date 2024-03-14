namespace SqlPractice.Models.Users;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<UserLog> UserLogs { get; set; } = [];
}

public class UserLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Action { get; set; }
    public DateTime Date { get; set; }
}