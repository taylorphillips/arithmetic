using Godot;

public class Unit : RigidBody2D
{
    public static readonly float MAX_DISTANCE = 45 * 7f;

    public Color Color { get; set; }
    private readonly float Radius = 10;

    public override void _Ready() {
        CanSleep = false;
        ZIndex = 1;
        Color = Colors.Blue;
        CircleShape2D circleShape2D = new CircleShape2D();
        circleShape2D.Radius = Radius;

        CollisionShape2D collisionShape2D = new CollisionShape2D();
        collisionShape2D.Shape = circleShape2D;
        AddChild(collisionShape2D);
    }

    public override void _PhysicsProcess(float delta) {
        Block block = GetParent().GetParent<Block>();
        MultiBlock multiBlock = block.GetParent<MultiBlock>();
        // TODO: This is a garbage hack to keep the balls in the box.
        if (block.GlobalPosition.DistanceTo(GlobalPosition) > MAX_DISTANCE * multiBlock.Scale[0]) {
            GlobalPosition = block.GlobalPosition;
        }
    }

    public override void _Draw() {
        DrawCircle(new Vector2(0, 0), Radius, Color);
    }
}
