using Godot;
using System;
using System.Data;

public class Block : RigidBody2D
{
    private bool selected = false;
    private Vector2 mousePosition;
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

    public override void _Process(float delta) {
    }

    public override void _IntegrateForces(Physics2DDirectBodyState state) {
        if (selected) {
            Transform2D transform = new Transform2D();
            transform.origin = mousePosition;
            LinearVelocity = Vector2.Zero;
            state.Transform = transform;
        }
    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventKey inputEvent) {
            if (@event.IsPressed() && inputEvent.Scancode == (uint) KeyList.Space) {
                GD.Print("SPACEBAR");
            }
        } else if (@event is InputEventMouseButton mouseEvent) {
            if (mouseEvent.Pressed && mouseEvent.ButtonIndex == (int) ButtonList.Left) {
                GD.Print("LEFT CLICK");
                selected = true;
                mousePosition = mouseEvent.GlobalPosition;
                //this.Mode = ModeEnum.Static;
            } else if (!mouseEvent.Pressed && mouseEvent.ButtonIndex == (int) ButtonList.Left) {
                GD.Print("LEFT UNCLICK");
                selected = false;
                //this.Mode = ModeEnum.Rigid;
            }
        } else if (@event is InputEventMouseMotion mouseMotion) {
            if (selected) {
                // Adjust Transform??
                mousePosition = mouseMotion.GlobalPosition;
                GD.Print("TRACKING");
            }
        }
    }
    
    public override void _PhysicsProcess(float delta) {
        base._PhysicsProcess(delta);
    }
}
