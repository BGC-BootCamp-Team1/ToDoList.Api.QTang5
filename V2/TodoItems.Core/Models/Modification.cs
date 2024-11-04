namespace TodoItems.Core;

public class Modification
{
    public DateOnly TimesStamp { get; set; }
    public Modification()
    {
        TimesStamp = DateOnly.FromDateTime(DateTime.Now);
    }
}

