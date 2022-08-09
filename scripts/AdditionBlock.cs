using System.Linq;
using Godot;
using static Block.ConnectorArea2D;

/// <summary>
/// Has two inputs.
/// </summary>
public class AdditionBlock : Block
{
    public override void _Ready() {
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
        
        AddChild(new SelectableArea2D());

    }

    public override  ExitCode Run() {
        MultiBlock multiBlock = GetParent<MultiBlock>();
        ConnectorArea2D input1 = multiBlock.GetConnection(inputConnectors[0]);
        ConnectorArea2D input2 = multiBlock.GetConnection(inputConnectors[1]);

        if (input1 != null && input2 != null) {
            Block input1Block = input1.GetParent<Block>();
            Block input2Block = input2.GetParent<Block>();

            // Verify both input are UnitBlocks.
            if (input1Block.inputConnectors.Any() || input2Block.inputConnectors.Any()) {
                return ExitCode.RETRY;
            }
            
            foreach (Unit unit in input1Block.GetUnits()) {
                // PushButton or re-parent existing unit? Depends on animation?
                input1Block.contentNode.RemoveChild(unit);
                PushButton();
            }
            
            foreach (Unit unit in input2Block.GetUnits()) {
                // PushButton or re-parent existing unit? Depends on animation?
                input2Block.contentNode.RemoveChild(unit);
                unit.Free();
                PushButton();
            }
        }

        return ExitCode.SUCCESS;
    }
}
