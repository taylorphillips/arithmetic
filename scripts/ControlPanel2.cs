using Godot;

public class ControlPanel2 : ControlPanel
{
    
    public override void _Ready() {
        root = GetTree().Root.GetChild<Node2D>(0).GetChild<RootMultiBlock>(0);
        AddChild(new ControlPanel.SaveButton("Save"));
        AddChild(new ControlPanel.RunButton("Run"));
    }
}
