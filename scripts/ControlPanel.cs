using System;
using System.Collections.Generic;
using System.IO;
using Godot;

// TODO: Construct the Game scene with code and make the root multiblock at 0,0 but move the position
// of the RootBlock to where it should be. Then position all children as ??
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

        public override void _Draw() {
            if (isDragging) {
                // TODO: Would be ideal if this referenced the block's own drawing.
                Rect2 rect2 = new Rect2(
                    GetGlobalMousePosition() - new Vector2(20, 40),
                    new Vector2(40, 40)
                );
                DrawRect(rect2, Colors.DarkGray, true);
            }
        }

        public override void _Process(float delta) {
            if (isSelected && RectGlobalPosition.DistanceTo(GetGlobalMousePosition()) > 100f) {
                isDragging = true;
                Update();
            } else {
                isDragging = false;
            }
        }

        public virtual void OnButtonDown() {
            isSelected = true;
        }

        public virtual void OnButtonUp() {
            isSelected = false;
            isDragging = false;
            Update();
        }
    }

    public class SuccessorButton : Button
    {
        private static RandomNumberGenerator rng;

        public SuccessorButton(string name, MultiBlock multiBlock) : base(name, multiBlock) { }


        public override void OnButtonUp() {
            if (isDragging) {
                Node2D root = GetTree().Root.GetChild<Node2D>(0);
                Block rootBlock = root.GetChild<Block>(0);
                SuccessorBlock successorBlock = new SuccessorBlock();
                MultiBlock multiBlock = new MultiBlock(successorBlock);
                // Children get downscaled 10x?
                multiBlock.Scale = new Vector2(1f / 7, 1f / 7);
                multiBlock.GlobalPosition = rootBlock.GetLocalMousePosition();
                multiBlock.AddChild(successorBlock);
                rootBlock.AddContent(multiBlock);
            } else {
                Node2D root = GetTree().Root.GetChild<Node2D>(0);
                Block rootBlock = root.GetChild<Block>(0);
                for (int i = 0; i < 1; i++) {
                    rootBlock.PushButton();
                }
            }

            base.OnButtonUp();
        }
    }

    public class EmptyButton : Button
    {
        public EmptyButton(string name, MultiBlock multiBlock) : base(name, multiBlock) { }

        public override void OnButtonUp() {
            if (isDragging) {
                Node2D root = GetTree().Root.GetChild<Node2D>(0);
                Block rootBlock = root.GetChild<Block>(0);
                Block block = new Block();
                MultiBlock multiBlock = new MultiBlock(block);
                // Children get downscaled 10x?
                multiBlock.Scale = new Vector2(1f / 7, 1f / 7);
                multiBlock.GlobalPosition = rootBlock.GetLocalMousePosition();
                multiBlock.AddChild(block);
                rootBlock.AddContent(multiBlock);
            } else {
                Node2D root = GetTree().Root.GetChild<Node2D>(0);
                Block rootBlock = root.GetChild<Block>(0);
                rootBlock.ClearContent();
            }

            base.OnButtonUp();
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
