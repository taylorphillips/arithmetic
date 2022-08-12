using System.Linq;
using Godot;
using static Block.ConnectorArea2D;
using static ControlPanel;

/// <summary>
/// Has two inputs.
/// </summary>
public class MultiplicationBlock : Block
{
    private bool Memoized;
    private int multiplicand;

    public MultiplicationBlock() : base(true) {
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

        AddChild(new SelectableArea2D(Colors.Purple));
        
        Label label = new Label();
        label.Text = "x";
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

        // Consume input1 FOR ANIMATION PURPOSES ONLY
        int unitCount = input1Block.GetUnits().Count;
        if (!Memoized) {
            multiplicand = unitCount;
            Memoized = true;

            // Turn the units Red;
            foreach (Unit unit in input1Block.GetUnits()) {
                unit.Color = Colors.Red;
                unit.Update();
            }

            return ExitCode.PARTIAL;
        }


        // For each Unit input2, push the input1 button
        foreach (Unit unit in input2Block.GetUnits()) {
            input2Block.contentNode.RemoveChild(unit);
            unit.Free();
            for (int i = 0; i < multiplicand; i++) {
                PushButton();
            }

            return ExitCode.PARTIAL;
        }
        
        // Remove Red units in multiplicand.
        foreach (Unit unit in input1Block.GetUnits()) {
            input1Block.contentNode.RemoveChild(unit);
            unit.Free();
        }
        return ExitCode.SUCCESS;
    }
}
