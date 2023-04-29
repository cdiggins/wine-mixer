namespace Champagne;

public class Step 
{ }

public class TankCombine : Step
{
    public int InputA { get; }
    public int InputB { get; }
    public int Output { get; }

    public TankCombine(int inputA, int inputB, int output)
        => (InputA, InputB, Output) = (inputA, inputB, output);

    public override string ToString()
        => $"Combine tank {InputA} and {InputB} into tank {Output}";
}

public class TankSplit : Step
{
    public int Input { get; }
    public int OutputA { get; }
    public int OutputB { get; }

    public TankSplit(int input, int outputA, int outputB)
        => (Input, OutputA, OutputB) = (input, outputA, outputB);

    public override string ToString()
        => $"Split tank {Input} into tanks {OutputA} and {OutputB}";
}

public class AddWine : Step
{
    public int Tank { get; }
    public int Wine { get; }

    public AddWine(int tank, int wine)
        => (Tank, Wine) = (tank, wine);

    public override string ToString()
        => $"Add wine {Wine} to tank {Tank}";
}

public class RemoveWine : Step
{
    public int Tank { get; }

    public RemoveWine(int tank)
        => (Tank) = (tank);

    public override string ToString()
        => $"Remove wine from tank {Tank}";
}