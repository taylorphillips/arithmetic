using Godot;

/// <summary>
/// This is a block with no inputs.
/// If there is no output, it is done.
/// </summary>
public class UnitBlock : Block
{
    public override void _Ready() {
        AddChild(new SelectableArea2D());

        outputConnector = new ConnectorArea2D(ConnectorArea2D.ConnectorType.OUTPUT, new Vector2(0, 40));
        AddChild(outputConnector);
    }

    public void Run() {
        Block outputBlock = GetParent<MultiBlock>().GetOutputBlock(this);

        // If there is no output, this is done.
        if (outputBlock == null) {
            return;
        }

        // Transfer the units from this block to the output block.
        foreach (Unit unit in units) {
            // unit.Remove();
            outputBlock.PushButton();
        }
    }
}
