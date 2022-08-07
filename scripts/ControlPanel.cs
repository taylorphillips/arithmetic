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

        public virtual void OnButtonDown() { }
    }

    public class SuccessorButton : Button
    {
        private static RandomNumberGenerator rng;

        public SuccessorButton(string name, MultiBlock multiBlock) : base(name, multiBlock) { }

        public override void OnButtonDown() {
            rng = new RandomNumberGenerator();
            for (int i = 0; i < 1; i++) {
                Unit unit = new Unit();
                unit.GlobalPosition = new Vector2(rng.RandfRange(-20, 20), rng.RandfRange(-20, 20));
                Node2D root = GetTree().Root.GetChild<Node2D>(0);
                Block rootBlock = root.GetChild<Block>(0);
                rootBlock.AddChild(unit);
            }
        }
    }

    public class EmptyButton : Button
    {
        public EmptyButton(string name, MultiBlock multiBlock) : base(name, multiBlock) { }

        public override void OnButtonDown() {
            Node2D root = GetTree().Root.GetChild<Node2D>(0);
            Block rootBlock = root.GetChild<Block>(0);

            foreach (Node child in rootBlock.GetChildren()) {
                if (child is Unit) {
                    rootBlock.RemoveChild(child);
                }
            }
        }
    }

    // This is the block that represents the main window.
    private MultiBlock rootBlock = new MultiBlock();

    private Button selectedButton;
    private bool isDragging;

    public override void _Ready() {
        AddChild(new EmptyButton("Empty", new MultiBlock()));
        AddChild(new SuccessorButton("Successor", new MultiBlock()));
    }

    public void AddButton(Button button) {
        AddChild(button);
    }
}
