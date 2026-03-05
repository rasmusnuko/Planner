namespace HomePlanner.Models;

public class SleepNap
{
    public int       Id         { get; set; }
    public int       SleepLogId { get; set; }
    public SleepLog  SleepLog   { get; set; } = null!;
    public TimeOnly  StartTime  { get; set; }
    public TimeOnly? EndTime    { get; set; }
}
