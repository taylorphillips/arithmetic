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
        // Add input1
        // Add input2
        // Remove addition block, this becomes a numena block.
        // If output is connected, transfer balls to it.
        return ExitCode.FAILURE;
    }
}
