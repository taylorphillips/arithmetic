public class RootBlock : Block
{
    public RootBlock() {
        ZIndex = -2;
    }
    
    public override void _PhysicsProcess(float delta) {
        // Overriding this eliminates drag and drop behavior.
    }
}
