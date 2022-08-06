using System.Collections.Generic;
using Godot;

public class ControlPanel : GridContainer
{
    public class Button : Godot.Button
    {
        private readonly string name;
        private readonly MultiBlock multiBlock;

        public Button(string name, MultiBlock multiBlock) {
            this.name = name;
            this.multiBlock = multiBlock;
        }
        
        public override void _Ready() {
            Godot.Button button = new Godot.Button();
            button.Text = name;
            button.Connect("button_down", this, "OnButtonDown");
            AddChild(button);
        }

        public void OnButtonDown() {
            for (int i = 0; i < 100; i++) {
                Unit unit = new Unit();
                unit.GlobalPosition = new Vector2(600, 300);
                GetTree().Root.AddChild(unit); 
            }
         
        }

        public void OnButtonUp() {
            
        }

        public void Program() {
        }

        public void ToMultiBlock() {
            
        }
    }

    // This is the block that represents the main window.
    private MultiBlock rootBlock = new MultiBlock();
    
    private Button selectedButton;
    private bool isDragging;

    public override void _Ready() {
        AddChild(new Button("Empty", new MultiBlock()));
        AddChild(new Button("Successor", new MultiBlock()));
    }
    
    public void AddButton(Button button) {
        AddChild(button);
    }
}
