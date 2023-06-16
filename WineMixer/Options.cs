using System.Text.Json;

namespace WineMixer;

public class Options
{
    // public bool MinimizeWaste { get; set; } 
    // public long TimeOutInSeconds { get; set; }
    public int MaxInputOrOutputTanks { get; set; } = 3;
    public bool RunInParallel { get; set; } = true;
    public string? OutputBlendFileName { get; set; } = "result.txt";
    public string? OutputStepsFileName { get; set; } = "steps.txt";
    public string? OutputFolder { get; set; } = "output";
    public int MaxNumSteps { get; } = 10;
    public override string ToString()
        => JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });
}
