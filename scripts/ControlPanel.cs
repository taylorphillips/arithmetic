using Godot;

public class ControlPanel : GridContainer
{
    public class Button : Node2D
    {
        public override void _Ready() {
            Godot.Button button = new Godot.Button();
            button.Text = "Successor";
            AddChild(button);
        }
    }

    public override void _Ready() {
        AddChild(new Button());
    }
    
    public void AddButton(Button button) {
        AddChild(button);
    }
}
