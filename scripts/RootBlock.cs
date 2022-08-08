using Godot;

public class RootBlock : Block
{
    public RootBlock() : base() {
        ZIndex = -1000;
        //Scale = Vector2.One;
    }

    public override void _PhysicsProcess(float delta) {
        // Overriding this eliminates drag and drop behavior.
    }
}
