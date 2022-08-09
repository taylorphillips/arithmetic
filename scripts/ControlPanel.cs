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
        protected RootMultiBlock root;

        public Button(string name, MultiBlock multiBlock) {
            this.name = name;
            this.multiBlock = multiBlock;
        }

        public override void _Ready() {
            root = GetParent<ControlPanel>().root;
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
                SuccessorBlock successorBlock = new SuccessorBlock();
                MultiBlock multiBlock = new MultiBlock(successorBlock);
                // Children get downscaled 10x?
                multiBlock.Scale = new Vector2(1f / 7, 1f / 7);
                multiBlock.GlobalPosition = root.Block.GetLocalMousePosition();
                root.Block.AddContent(multiBlock);
            } else {
                for (int i = 0; i < 1; i++) {
                    root.Block.PushButton();
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
                Block block = new Block();
                MultiBlock multiBlock = new MultiBlock(block);
                // Children get downscaled 10x?
                multiBlock.Scale = new Vector2(1f / 7, 1f / 7);
                multiBlock.GlobalPosition = root.GetLocalMousePosition();
                root.Block.AddContent(multiBlock);
            } else {
                root.Block.ClearContent();
            }

            base.OnButtonUp();
        }
    }

    public class RunButton : Button
    {
        public RunButton(string name, MultiBlock multiBlock) : base(name, multiBlock) { }

        public override void OnButtonDown() { }

        public override void OnButtonUp() {
            root.RunProgram();
        }
    }

    public class AdditionButton : Button
    {
        public AdditionButton(string name, MultiBlock multiBlock) : base(name, multiBlock) { }

        public override void OnButtonUp() {
            if (isDragging) {
                AdditionBlock additionBlock = new AdditionBlock();
                MultiBlock multiBlock = new MultiBlock(additionBlock);
                // Children get downscaled 10x?
                multiBlock.Scale = new Vector2(1f / 7, 1f / 7);
                multiBlock.GlobalPosition = root.Block.GetLocalMousePosition();
                root.Block.AddContent(multiBlock);
            } else {
                // No clicking behavior for Addition?
            }

            base.OnButtonUp();
        }
    }

    // This is the block that represents the main window.
    private RootMultiBlock root = new RootMultiBlock();

    public override void _Ready() {
        root = GetTree().Root.GetChild<Node2D>(0).GetChild<RootMultiBlock>(0);
        AddChild(new EmptyButton("Empty", new MultiBlock()));
        AddChild(new SuccessorButton("Successor", new MultiBlock()));
        AddChild(new RunButton("Run", new MultiBlock()));
        AddChild(new AdditionButton("Addition", new MultiBlock()));
    }
}
