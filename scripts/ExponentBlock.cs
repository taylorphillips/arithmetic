using System.Linq;
using Godot;
using static Block.ConnectorArea2D;
using static ControlPanel;

/// <summary>
/// Has two inputs.
/// </summary>
public class ExponentBlock : Block
{
    private bool Memoized;
    private int baseNumber;

    public ExponentBlock() : base(true) {
        contentNode = new Node2D();
        contentNode.ZIndex = ZIndex + 10;
        AddChild(contentNode);

        ConnectorArea2D inputConnector = new ConnectorArea2D(ConnectorType.INPUT, new Vector2(-40, -40));
        inputConnectors.Add(inputConnector);
        AddChild(inputConnector);

        inputConnector = new ConnectorArea2D(ConnectorType.INPUT, new Vector2(40, -40));
        inputConnectors.Add(inputConnector);
        AddChild(inputConnector);

        outputConnector = new ConnectorArea2D(ConnectorType.OUTPUT, new Vector2(0, 40));
        AddChild(outputConnector);

        AddChild(new SelectableArea2D(Colors.DarkGreen));

        Label label = new Label();
        label.Text = "^";
        AddChild(label);
    }

    public override ExitCode Run() {
        MultiBlock multiBlock = GetParent<MultiBlock>();
        ConnectorArea2D input1 = multiBlock.GetConnection(inputConnectors[0]);
        ConnectorArea2D input2 = multiBlock.GetConnection(inputConnectors[1]);


        // If either input is missing, retry.
        if (input1 == null || input2 == null) {
            return ExitCode.RETRY;
        }

        // If either inputs are NOT UnitBlocks, retry.
        Block input1Block = input1.GetParent<Block>();
        Block input2Block = input2.GetParent<Block>();
        if (input1Block.inputConnectors.Any() || input2Block.inputConnectors.Any()) {
            return ExitCode.RETRY;
        }

        int unitCount = input1Block.GetUnits().Count;
        if (!Memoized) {
            baseNumber = unitCount;
            Memoized = true;

            // Turn the units Red;
            foreach (Unit unit in input1Block.GetUnits()) {
                unit.Color = Colors.Red;
                unit.Update();
            }

            return ExitCode.PARTIAL;
        }

        // For each Unit input2, multiply by number
        // TODO: This should be building a program of MultiplicationBlocks.
        foreach (Unit unit in input2Block.GetUnits()) {
            input2Block.contentNode.RemoveChild(unit);
            unit.Free();
            int currentNumber = GetUnits().Count;
            if (currentNumber == 0) {
                currentNumber = 1;
            }
            for (int i = 0; i < currentNumber; i++) {
                for (int j = 0; j < baseNumber; j++) {
                    PushButton();
                }
            }
            return ExitCode.PARTIAL;
        }
        
        // Remove Red units in base number.
        foreach (Unit unit in input1Block.GetUnits()) {
            input1Block.contentNode.RemoveChild(unit);
            unit.Free();
        }
        return ExitCode.SUCCESS;
    }
}
