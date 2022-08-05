
using Godot;

/// <summary>
/// This is a block with no inputs.
/// </summary>
public class UnitBlock : Block
{
    public override void _Ready() {
        AddChild(new SelectableArea2D());
        
        outputConnector = new ConnectorArea2D(ConnectorArea2D.ConnectorType.OUTPUT, new Vector2(0, 40));
        AddChild(outputConnector);
    }

    public void Run() {
        // No-op.
        // If output is connected, transfer balls to it.
    }
}
