namespace HomePlanner.Models;

public class SleepLog
{
    public int       Id       { get; set; }
    public DateOnly  Date     { get; set; }
    public TimeOnly? WakeTime { get; set; }
    public TimeOnly? BedTime  { get; set; }
    public string?   Notes    { get; set; }
    public List<SleepNap> Naps { get; set; } = [];
}
