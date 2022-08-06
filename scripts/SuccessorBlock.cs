using Godot;
using static Block.ConnectorArea2D;

/// <summary>
/// Has one input.
/// </summary>
public class SuccessorBlock : Block
{
    public override void _Ready() {
        // TODO: Style this as a successor.
        AddChild(new SelectableArea2D());

        ConnectorArea2D inputConnector = new ConnectorArea2D(ConnectorType.INPUT, new Vector2(0, -40));
        inputConnectors.Add(inputConnector);
        AddChild(inputConnector);

        outputConnector = new ConnectorArea2D(ConnectorType.OUTPUT, new Vector2(0, 40));
        AddChild(outputConnector);
    }

    public override void Run() {
        base.Run();
        
        this.PushButton();
        // Empty above Block into this Block.
        // Push the button and add a ball.
        // Change this to be a UnitBlock
    }
}
