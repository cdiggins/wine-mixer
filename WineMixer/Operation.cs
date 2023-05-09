namespace WineMixer;

/// <summary>
/// Represents a component of a step in the process of blending wines:
/// * Adding wine
/// * Combining two tanks into a third
/// * Splitting the contents of a tank into two input tanks
///
/// NOTE: if tanks can be combined from more than two, and merged into multiple tanks,
/// and this process can happen simultaneously, then we can imagine a single Operation class
/// that has a list of inputs, and a list of outputs.
///
/// An edge in the graph is considered to be a "transition", which combines multiple operations. 
/// </summary>
public class Operation 
{ }

/// <summary>
/// Represents the combining of two tanks into a third.
/// The output tank is expected to be the size of the two input tanks. 
/// </summary>
public class TankCombine : Operation
{
    public int InputA { get; }
    public int InputB { get; }
    public int Output { get; }

    public TankCombine(int inputA, int inputB, int output)
        => (InputA, InputB, Output) = (inputA, inputB, output);

    public override string ToString()
        => $"Combine tank {InputA} and {InputB} into tank {Output}";
}

/// <summary>
/// NOTE: Currently unused.
/// This operation represents the splitting of a tank into two output tanks.
/// This would only be valid if the two output tank size are equal to the input tank. 
/// </summary>
public class TankSplit : Operation
{
    public int Input { get; }
    public int OutputA { get; }
    public int OutputB { get; }

    public TankSplit(int input, int outputA, int outputB)
        => (Input, OutputA, OutputB) = (input, outputA, outputB);

    public override string ToString()
        => $"Split tank {Input} into tanks {OutputA} and {OutputB}";
}

/// <summary>
/// Represents the addition of wine to a single tank.
/// The tank is assumed to contain nothing but that wine afterwards.
/// Depending on the exact nature of the algorithm the tank may be required to be empty. 
/// </summary>
public class AddWine : Operation
{
    public int Tank { get; }
    public int Wine { get; }

    public AddWine(int tank, int wine)
        => (Tank, Wine) = (tank, wine);

    public override string ToString()
        => $"Add wine {Wine} to tank {Tank}";
}

/// <summary>
/// This is a generic operation that moves multiple input tanks and multiple output tanks.
/// The requirements are:
/// 1) Output tanks are empty
/// 2) Input tanks add up to output tanks.
/// 3) The input and output tanks are not the same. 
/// </summary>
public class MultiSplit : Operation
{
    public IReadOnlyList<int> Inputs { get; }
    public IReadOnlyList<int> Outputs { get; }

    public MultiSplit(IReadOnlyList<int> inputs, IReadOnlyList<int> outputs)
    {
        Inputs = inputs;
        Outputs = outputs;
    }

    public override string ToString()
        => $"({string.Join(",", Inputs)}) -> ({string.Join(",", Outputs)})";
}
