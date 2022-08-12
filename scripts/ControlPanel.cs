using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using Array = Godot.Collections.Array;

// TODO: Construct the Game scene with code and make the root multiblock at 0,0 but move the position
// of the RootBlock to where it should be. Then position all children as ??
public class ControlPanel : GridContainer
{
    public class Button : Godot.Button
    {
        protected bool isSelected = false;
        protected bool isDragging = false;


        private readonly string name;
        protected RootMultiBlock root;

        public Button(string name) {
            this.name = name;
        }

        public override void _Ready() {
            root = GetParent<ControlPanel>().root;
            Text = name;
            Connect("button_down", this, "OnButtonDown");
            Connect("button_up", this, "OnButtonUp");
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

        public SuccessorButton(string name) : base(name) {
            Icon = LoadImageTexture("res://assets/BlueButton.png");
        }


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
        public EmptyButton(string name) : base(name) {
            Icon = LoadImageTexture("res://assets/EmptyBlock.png");
        }

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
        public RunButton(string name) : base(name) {
            Icon = LoadImageTexture("res://assets/PlayButton.png");
        }

        public override void OnButtonDown() { }

        public override void OnButtonUp() {
            root.StepProgram();
        }
    }

    public class AdditionButton : Button
    {
        public AdditionButton(string name) : base(name) { }

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

    public class MultiplicationButton : Button
    {
        public MultiplicationButton(string name) : base(name) { }

        public override void OnButtonUp() {
            if (isDragging) {
                MultiplicationBlock multiplicationBlock = new MultiplicationBlock();
                MultiBlock multiBlock = new MultiBlock(multiplicationBlock);
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

    public class ExponentButton : Button
    {
        public ExponentButton(string name) : base(name) { }

        public override void OnButtonUp() {
            if (isDragging) {
                ExponentBlock exponentBlock = new ExponentBlock();
                MultiBlock multiBlock = new MultiBlock(exponentBlock);
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

    public class CustomButton : Button
    {
        private readonly ProgramSerde programSerde;

        public CustomButton(string name, ProgramSerde programSerde) : base(name) {
            this.programSerde = programSerde;
        }

        public override void OnButtonUp() {
            if (isDragging) {
                MultiBlock multiBlock = ProgramSerde.FromProgramSerde(programSerde);
                multiBlock.Scale = new Vector2(1f / 7, 1f / 7);
                multiBlock.GlobalPosition = root.Block.GetLocalMousePosition();
                root.Block.AddContent(multiBlock);
            } else {
                // No clicking behavior for Addition?
            }

            base.OnButtonUp();
        }

        // TODO: HACKY AF
        public void RunToCompletionAndOutputUnitsInside(MultiplicationBlock destinationBlock) {
            MultiBlock multiBlock = ProgramSerde.FromProgramSerde(programSerde);

            int count = 0;
            while (true) {
                if (multiBlock.StepProgram() == Block.ExitCode.FAILURE) {
                    throw new InvalidOperationException("program failed");
                }

                if (multiBlock.GetBlocks().Count == 1) {
                    break;
                } else {
                    count++;
                    if (count > 10) {
                        throw new InvalidOperationException();
                    }
                }
            }

            // Transfer all units.
            multiBlock.GetBlocks()[0].GetUnits().ForEach(unit => {
                destinationBlock.PushButton();
            });

            // Delete the executed MultiBlock
            multiBlock.Free();
        }
    }

    public class SaveButton : Button
    {
        public SaveButton(string name) : base(name) {
            Icon = LoadImageTexture("res://assets/RecordButton.png");
        }

        public override void OnButtonDown() { }

        public override void OnButtonUp() {
            if (isDragging) { } else {
                Block rootBlock = root.GetChild<Block>(0);
                if (rootBlock.contentNode.GetChildren().Count > 1) {
                    GD.Print("Too many children");
                    return;
                } else if (rootBlock.contentNode.GetChildren().Count == 0) {
                    GD.Print("Nothing to save");
                    return;
                }

                MultiBlock multiBlock = rootBlock.contentNode.GetChild<MultiBlock>(0);
                ProgramSerde programSerde = ProgramSerde.ToProgramSerde(multiBlock);

                CustomButton button = new CustomButton("TODO", programSerde);
                GetTree().Root.GetChild<Node2D>(0).GetChild<ControlPanel>(1).AddChild(button);
            }

            base.OnButtonUp();
        }
    }

    // This is the block that represents the main window.
    protected RootMultiBlock root = new RootMultiBlock();

    public override void _Ready() {
        root = GetTree().Root.GetChild<Node2D>(0).GetChild<RootMultiBlock>(0);
        AddChild(new EmptyButton(""));
        AddChild(new SuccessorButton(""));
        // AddChild(new AdditionButton("Addition"));
        // AddChild(new MultiplicationButton("Multiplication"));
        // AddChild(new ExponentButton("Exponentiation"));
    }


    public static ImageTexture LoadImageTexture(string path) {
        StreamTexture streamTexture = GD.Load<StreamTexture>(path);
        ImageTexture imageTexture = new ImageTexture();
        imageTexture.CreateFromImage(streamTexture.GetData());
        return imageTexture;
    }
}
