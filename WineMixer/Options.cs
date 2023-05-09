namespace WineMixer;

public class Options
{
    public bool MinimizeWaste { get; set; }
    public bool OutputStepsAsJson { get; set; }
    public long TimeOutInSeconds { get; set; }
    public bool OuputStepsInLongForm { get; set; }
    public bool WineAlreadyAdded { get; set; }
    public int MaxInputOrOutputTanks { get; set; } = 4;
}