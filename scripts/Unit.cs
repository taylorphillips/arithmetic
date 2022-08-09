using Godot;

public class Unit : RigidBody2D
{
    public static readonly float MAX_DISTANCE = 45;
    
    private readonly float Radius = 10;

    public override void _Ready() {
        CanSleep = false;
        ZIndex = 1;
        CircleShape2D circleShape2D = new CircleShape2D();
        circleShape2D.Radius = Radius;

        CollisionShape2D collisionShape2D = new CollisionShape2D();
        collisionShape2D.Shape = circleShape2D;
        AddChild(collisionShape2D);
    }

    public override void _PhysicsProcess(float delta) {
        Block block = GetParent().GetParent<Block>();
        if (block.GlobalPosition.DistanceTo(GlobalPosition) > MAX_DISTANCE) {
            GlobalPosition = block.GlobalPosition;
        }
    }

    public override void _Draw() {
        DrawCircle(new Vector2(0, 0), Radius, Colors.Blue);
    }
}
