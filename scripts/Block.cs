using Godot;
using System;
using System.Data;

public class Block : RigidBody2D
{
    private bool selected = false;
    private CollisionPolygon2D collisionPolygon2D;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
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

    public override void _Input(InputEvent @event) {
        if (@event is InputEventKey inputEvent) {
            if (@event.IsPressed() && inputEvent.Scancode == (uint) KeyList.Space) {
                GD.Print("SPACEBAR");
            }
        } else if (@event is InputEventMouseButton mouseEvent) {
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == (int) ButtonList.Left) {
                GD.Print("LEFT CLICK");
                selected = true;
                LinearVelocity = Vector2.Zero;
                AngularVelocity = 0f;
                Mode = ModeEnum.Static;
            } else if (!mouseEvent.Pressed && mouseEvent.ButtonIndex == (int) ButtonList.Left) {
                GD.Print("LEFT UNCLICK");
                selected = false;
                Mode = ModeEnum.Rigid;
                // Wakes up the sleeping rigid body
                Sleeping = false;
                //ApplyImpulse(Vector2.Zero, Vector2.Zero);
            }
        }
    }

    public override void _PhysicsProcess(float delta) {
        if (selected) {
            Transform2D transform = GlobalTransform;
            transform.origin = GetGlobalMousePosition();
            GlobalTransform = transform;
        }
    }
}
