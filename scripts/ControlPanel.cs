using System;
using System.Collections.Generic;
using System.IO;
using Godot;

public class ControlPanel : GridContainer
{
    public class Button : Godot.Button
    {
        protected bool isSelected = false;
        protected bool isDragging = false;

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
            button.Connect("button_up", this, "OnButtonUp");
            AddChild(button);
        }

        public virtual void OnButtonDown() {
            isSelected = true;
        }

        public virtual void OnButtonUp() {
            isSelected = false;
            isDragging = false;
        }

        public override void _Process(float delta) {
            if (isSelected && RectGlobalPosition.DistanceTo(GetGlobalMousePosition()) > 100f) {
                isDragging = true;
            } else {
                isDragging = false;
            }
        }
    }

    public class SuccessorButton : Button
    {
        private static RandomNumberGenerator rng;

        public SuccessorButton(string name, MultiBlock multiBlock) : base(name, multiBlock) { }

        public override void OnButtonUp() {
            if (isDragging) {
                SuccessorBlock successorBlock = new SuccessorBlock();
                MultiBlock multiBlock = new MultiBlock(successorBlock);
                multiBlock.GlobalPosition = GetGlobalMousePosition();
                successorBlock.GlobalPosition = GetGlobalMousePosition();
                //multiBlock.AddChild(successorBlock);

                Node2D root = GetTree().Root.GetChild<Node2D>(0);
                Block rootBlock = root.GetChild<Block>(0);
                // TODO: Make the MultiBlock work.
                root.AddChild(successorBlock);
                isDragging = false;
            } else {
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
    }

    public class EmptyButton : Button
    {
        public EmptyButton(string name, MultiBlock multiBlock) : base(name, multiBlock) { }

        public override void OnButtonUp() {
            if (isDragging) {
                // TODO: Make the MultiBlock work.
                Block block = new Block();
                block.GlobalPosition = GetGlobalMousePosition();
                Node2D root = GetTree().Root.GetChild<Node2D>(0);
                root.AddChild(block);
                isDragging = false;
            } else {
                Node2D root = GetTree().Root.GetChild<Node2D>(0);
                Block rootBlock = root.GetChild<Block>(0);

                foreach (Node child in rootBlock.GetChildren()) {
                    if (child is Unit) {
                        rootBlock.RemoveChild(child);
                    }
                }
            }
        }
    }

    // This is the block that represents the main window.
    private MultiBlock rootBlock = new MultiBlock();


    public override void _Ready() {
        AddChild(new EmptyButton("Empty", new MultiBlock()));
        AddChild(new SuccessorButton("Successor", new MultiBlock()));
    }

    public void AddButton(Button button) {
        AddChild(button);
    }
}
