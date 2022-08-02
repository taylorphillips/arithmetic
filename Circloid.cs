using Godot;
using System;

public class Circloid : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    public override void _Draw() {
        DrawCircle(Vector2.Zero, 10, Colors.Aqua);
    }
}
