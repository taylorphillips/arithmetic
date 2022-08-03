using Godot;
using System;

public class Block : RigidBody2D
{
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
        CollisionPolygon2D collisionPolygon2D = new CollisionPolygon2D();
        collisionPolygon2D.Polygon = poly.Polygon;
        AddChild(poly);
        AddChild(collisionPolygon2D);
    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseButton mouseEvent) {
            GD.Print("mouse button event at ", mouseEvent.Position);
        }
    }
}
