using System.Linq;
using Godot;
using static Block.ConnectorArea2D;

/// <summary>
/// Has one input.
/// </summary>
public class SuccessorBlock : Block
{
    public SuccessorBlock() : base(true) {
        // TODO: Style this as a successor.

        contentNode = new Node2D();
        contentNode.ZIndex = ZIndex + 10;
        AddChild(contentNode);

        ConnectorArea2D inputConnector = new ConnectorArea2D(ConnectorType.INPUT, new Vector2(0, -40));
        inputConnectors.Add(inputConnector);
        AddChild(inputConnector);

        outputConnector = new ConnectorArea2D(ConnectorType.OUTPUT, new Vector2(0, 40));
        AddChild(outputConnector);

        AddChild(new SelectableArea2D());
    }

    public override ExitCode Run() {
        MultiBlock multiBlock = GetParent<MultiBlock>();
        ConnectorArea2D input = multiBlock.GetConnection(inputConnectors[0]);

        if (input != null) {
            Block inputBlock = input.GetParent<Block>();

            // Verify input is UnitBlocks.
            if (inputBlock.inputConnectors.Any()) {
                return ExitCode.RETRY;
            }

            // Copy in the input units.
            foreach (Unit unit in inputBlock.GetUnits()) {
                inputBlock.contentNode.RemoveChild(unit);
                unit.Free();
                PushButton();
            }

            // Successor pushes the button.
            PushButton();
            return ExitCode.SUCCESS;
        }

        return ExitCode.FAILURE;
    }
}
