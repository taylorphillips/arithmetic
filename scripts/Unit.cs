using Godot;

public class Unit : RigidBody2D
{
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

    public override void _Draw() {
        DrawCircle(new Vector2(0, 0), Radius, Colors.Blue);
    }
}
