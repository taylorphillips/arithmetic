using Godot;

public class RootMultiBlock : MultiBlock
{
    public Block Block;
    
    public RootMultiBlock()  {
        ZIndex = -1000;
        Scale = new Vector2(7, 7);
        Block = new Block();
        Block.IsSelectable = false;
        blocks.Add(Block);
        AddChild(Block);
    }
}
