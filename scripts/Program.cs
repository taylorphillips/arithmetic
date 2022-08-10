using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class ProgramSerde
{
    public enum BlockTypeEnum
    {
        Successor,
        Block,
    }

    public class BlockSerde
    {
        public BlockTypeEnum BlockType { get; set; }
        public ulong Identifier { get; set; }
    }

    public class ConnectorSerde
    {
        public BlockSerde Block { get; set; }
        public Block.ConnectorArea2D.ConnectorType ConnectorType { get; set; }
        public int Index { get; set; }
    }

    public class EdgeSerde
    {
        public ConnectorSerde Connector1 { get; set; }
        public ConnectorSerde Connector2 { get; set; }
    }

    public List<BlockSerde> Blocks { get; set; }
    public List<EdgeSerde> Edges { get; set; }

    public ProgramSerde() {
        Blocks = new List<BlockSerde>();
        Edges = new List<EdgeSerde>();
    }

    public static ProgramSerde ToProgramSerde(MultiBlock multiBlock) {
        ProgramSerde programSerde = new ProgramSerde();

        List<Block> blocks = multiBlock.GetBlocks().Where(block => multiBlock.GetOutputBlock(block) == null).ToList();
        if (blocks.Count == 0) {
            throw new InvalidOperationException("aint no blocks bruh");
        }

        if (blocks.Count > 1) {
            throw new InvalidOperationException("must have only one output node to save");
        }

        Block terminalBlock = blocks[0];
        Stack<Block> stack = new Stack<Block>();
        HashSet<Block> completedBlocks = new HashSet<Block>();

        // Depth first traversal starting at the end.
        stack.Push(terminalBlock);
        while (stack.Any()) {
            Block block = stack.Pop();
            if (completedBlocks.Contains(block)) {
                continue;
            }

            // Serialize the current block
            BlockTypeEnum blockTypeEnum;
            Enum.TryParse(block.GetSerializedName(), out blockTypeEnum);
            BlockSerde blockSerde1 = new BlockSerde() {
                BlockType = blockTypeEnum,
                Identifier = block.GetInstanceId(),
            };
            programSerde.Blocks.Add(blockSerde1);

            // Serialize the output edge (since we are traversing backwards).
            Block.ConnectorArea2D outputConnector = multiBlock.GetConnection(block.outputConnector);
            if (outputConnector != null) {
                Block outputBlock = outputConnector.GetParent<Block>();
                Enum.TryParse(block.GetSerializedName(), out blockTypeEnum);
                BlockSerde blockSerde2 = new BlockSerde() {
                    BlockType = blockTypeEnum,
                    Identifier = outputBlock.GetInstanceId(),
                };
                ConnectorSerde connectorSerde1 = new ConnectorSerde() {
                    Block = blockSerde1,
                    ConnectorType = Block.ConnectorArea2D.ConnectorType.OUTPUT,
                    Index = 0,
                };
                ConnectorSerde connectorSerde2 = new ConnectorSerde();


                for (int i = 0; i < outputBlock.inputConnectors.Count; i++) {
                    if (outputBlock.inputConnectors[i].Equals(outputConnector)) {
                        connectorSerde2 = new ConnectorSerde() {
                            Block = blockSerde2,
                            ConnectorType = Block.ConnectorArea2D.ConnectorType.INPUT,
                            Index = i,
                        };
                        break;
                    }
                }

                // TODO: Check for empty connectorSerde2?

                EdgeSerde edgeSerde = new EdgeSerde() {
                    Connector2 = connectorSerde1,
                    Connector1 = connectorSerde2,
                };
                programSerde.Edges.Add(edgeSerde);
            }

            // Loop through input Blocks and enqueue them
            foreach (Block.ConnectorArea2D inputConnector in block.inputConnectors) {
                Block.ConnectorArea2D targetConnector = multiBlock.GetConnection(inputConnector);
                if (targetConnector == null) {
                    throw new InvalidOperationException("no open inputs allowed yet");
                }

                Block inputBlock = targetConnector.GetParent<Block>();
                stack.Push(inputBlock);
            }
        }

        return programSerde;
    }

    public static MultiBlock FromProgramSerde(ProgramSerde programSerde) {
        MultiBlock multiBlock = new MultiBlock();

        // Add the blocks.
        Dictionary<ulong, Block> blockMap = new Dictionary<ulong, Block>();
        foreach (BlockSerde blockSerde in programSerde.Blocks) {
            Block block;
            blockMap.TryGetValue(blockSerde.Identifier, out block);
            if (block == null) {
                block = new Block();
                blockMap[blockSerde.Identifier] = block;
            }
            multiBlock.AddBlock(block);
        }
        
        // Add the connections and positions.
        programSerde.Edges.ForEach(edge => {
            // TODO: This is a convenient hack since all edges were currently serialized in reverse from output to input.
            Block block1 = blockMap[edge.Connector1.Block.Identifier];
            Block block2 = blockMap[edge.Connector2.Block.Identifier];
            multiBlock.Connect(block1.outputConnector, block2.inputConnectors[edge.Connector2.Index]);
            
            // TODO: How to position them?
        });
        
        return multiBlock;
    }
}
