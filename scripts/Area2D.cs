using Godot;
using System;
using System.Configuration;
using Object = Godot.Object;

public class Area2D : Godot.Area2D
{
    private Polygon2D poly;
    private bool selected = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        poly = new Polygon2D();
        poly.Color = Colors.Red;
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

    public void _OnMouseEnter() {
        poly.Color = Colors.Blue;
    }

    public void _OnMouseExit() {
        poly.Color = Colors.Red;
    }

    public void _OnAreaEntered(Area2D area2D) {
        GD.Print(Name + " has been entered by " + area2D.Name);
    }

    public void _OnAreaExited(Area2D area2D) {
        GD.Print(Name + " has been exited by " + area2D.Name);
    }

    public override void _PhysicsProcess(float delta) {
        if (selected) {
            Transform2D transform = GlobalTransform;
            transform.origin = GetGlobalMousePosition();
            GlobalTransform = transform;
        }
    }
}
