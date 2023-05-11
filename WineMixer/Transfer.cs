using System.Diagnostics;

namespace WineMixer;

/// <summary>
/// Represents the transfer from a set of input tanks to a set of output tanks.
/// The requirements are:
/// 1) Output tanks are empty
/// 2) Input tanks add up to output tanks.
/// 3) The input and output tanks are not the same. 
/// </summary>
public class Transfer
{
    public TankList Inputs { get; }
    public TankList Outputs { get; }
    
    public Transfer(TankList inputs, TankList outputs)
    {
        Debug.Assert(inputs.Volume == outputs.Volume);
        Inputs = inputs;
        Outputs = outputs;
    }

    public override string ToString()
        => $"({string.Join(",", Inputs.Tanks)}) -> ({string.Join(",", Outputs.Tanks)})";
}
