using Godot;
using System;
using System.Data;
using Object = Godot.Object;

public class Kinematic : KinematicBody2D
{
    private bool selected = false;
    private CollisionPolygon2D collisionPolygon2D;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        InputPickable = true;

        Polygon2D poly = new Polygon2D();
        poly.Color = Colors.White;
        poly.Polygon = new Vector2[] {
            new Vector2(-10, -10),
            new Vector2(10, -10),
            new Vector2(10, 10),
            new Vector2(-10, 10),
        };
        collisionPolygon2D = new CollisionPolygon2D();
        collisionPolygon2D.Polygon = poly.Polygon;
        AddChild(poly);
        AddChild(collisionPolygon2D);
    }

    public override void _Process(float delta) { }


    public override void _InputEvent(Object viewport, InputEvent @event, int shapeIdx) {
        base._InputEvent(viewport, @event, shapeIdx);
        if (@event is InputEventMouseButton mouseEvent) {
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == (int) ButtonList.Left) {
                GD.Print("LEFT CLICK");
                selected = true;
            } else if (!mouseEvent.Pressed && mouseEvent.ButtonIndex == (int) ButtonList.Left) {
                GD.Print("LEFT UNCLICK");
                selected = false;
            }
        }
    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventKey inputEvent) {
            if (@event.IsPressed() && inputEvent.Scancode == (uint) KeyList.Space) {
                GD.Print("SPACEBAR");
            }
        }
    }

    public override void _PhysicsProcess(float delta) {
        if (selected) {
            Transform2D transform = GlobalTransform;
            transform.origin = GetGlobalMousePosition();
            GlobalTransform = transform;
            KinematicCollision2D collision = MoveAndCollide(Vector2.Zero, true, true, true);
            if (collision != null) {
                RigidBody2D rigidBody2D = (RigidBody2D) collision.Collider;
                Node node = (Node) collision.Collider;
                GD.Print( node.Name + "," + rigidBody2D.GlobalPosition);
            }
        }
    }
}
