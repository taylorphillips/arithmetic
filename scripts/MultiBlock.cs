using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Sockets;
using Godot;
using static Block;

/// <summary>
/// A body of several connected Blocks which can be selected and moved together. The body
/// also influences the execution or measuring of the programs.'
///
/// Each block has a MultiBlock and when blocks connect, they join Multiblocks. If a block
/// wants to know about its connections, it asks its MultiBlock.
///
/// Rename to Program?
/// </summary>
public class MultiBlock : Node2D
{
    protected List<Block> blocks = new List<Block>();
    protected Dictionary<ConnectorArea2D, ConnectorArea2D> edges = new Dictionary<ConnectorArea2D, ConnectorArea2D>();

    public MultiBlock() { }

    public MultiBlock(Block block) {
        blocks.Add(block);
        AddChild(block);
    }

    // 
    /// <summary>
    /// Transferings the fromBlock to the toBlock (this).
    /// Idempotent.
    /// TODO: Make more robust and handle multiple connectors at once.
    /// TODO: Not null checks?
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void Connect(ConnectorArea2D from, ConnectorArea2D to) {
        if (from is null) {
            throw new ArgumentNullException();
        }

        if (to is null) {
            throw new ArgumentNullException();
        }

        Block fromBlock = from.GetParent<Block>();
        if (!blocks.Contains(fromBlock)) {
            blocks.Add(fromBlock);
        }

        edges[from] = to;
        edges[to] = from;

        // If they are separate MultiBlock, combine them delete old from MultiBlock
        MultiBlock fromMultiBlock = fromBlock.GetParent<MultiBlock>();
        if (!Equals(fromMultiBlock)) {
            fromMultiBlock.RemoveChild(fromBlock);
            fromMultiBlock.blocks.Remove(fromBlock);
            if (fromMultiBlock.blocks.Any()) {
                throw new InvalidOperationException("not supported");
            }

            AddChild(fromBlock);
            fromMultiBlock.GetParent().RemoveChild(fromMultiBlock);
            fromMultiBlock.Free();
        }
    }

    public MultiBlock DisconnectBlock(Block block) {
        // TODO: Detect disjoint graph and do good things.

        // Remove connectors
        RemoveConnection(block.outputConnector);
        foreach (ConnectorArea2D connector in block.inputConnectors) {
            RemoveConnection(connector);
        }

        // Remove the blocks
        blocks.Remove(block);
        RemoveChild(block);

        // Wrap disconnected piece in new MultiBlock and return
        return new MultiBlock(block);
    }

    private void RemoveConnection(ConnectorArea2D connectorArea2D) {
        if (connectorArea2D == null) {
            throw new InvalidOperationException();
        }

        if (GetConnection(connectorArea2D) != null) {
            // TODO: Make this more gentle so that order doesn't matter
            if (edges[edges[connectorArea2D]].Equals(connectorArea2D)) {
                edges.Remove(edges[connectorArea2D]);
            } else {
                GD.Print("MISMATCH HAPPENED");
            }

            edges.Remove(connectorArea2D);
        }
    }

    public ConnectorArea2D GetConnection(ConnectorArea2D connectorArea2D) {
        if (edges.ContainsKey(connectorArea2D)) {
            return edges[connectorArea2D];
        } else {
            return null;
        }
    }

    public Block GetOutputBlock(Block block) {
        if (!edges.ContainsKey(block.outputConnector)) {
            return null;
        }

        return edges[block.outputConnector].GetParent<Block>();
    }

    public void RunProgram() {
        // TODO: Be able to choose between breadth first, depth first execution and simultaneous.
        // TODO: Be able to fast forward programs.

        // UnitBlocks are the seeds of program execution.
        IEnumerable<Block> seeds = blocks.Where(block => block.inputConnectors.Count == 0);
        foreach (Block block in seeds.ToList()) {
            block.Run();
        }
    }

    public void SaveProgram() {
        List<Block> blocks = this.blocks.Where(block => GetOutputBlock(block) == null).ToList();
        if (blocks.Count == 0) {
            throw new InvalidOperationException("aint no blocks bruh");
        }

        if (blocks.Count > 1) {
            throw new InvalidOperationException("must have only one output node to save");
        }

        Block terminalBlock = blocks[0];
        Stack<Block> stack = new Stack<Block>();
        HashSet<Block> completedBlocks = new HashSet<Block>();
        List<string> nodes = new List<string>();
        List<string> edges = new List<string>();

        // Depth first traversal starting at the end.
        stack.Push(terminalBlock);
        int nodeNum = 0;
        while (stack.Any()) {
            Block block = stack.Pop();
            if (completedBlocks.Contains(block)) {
                continue;
            }

            // Serialize the current block
            nodes.Add(block.GetSerializedName() + nodeNum);

            // Serialize the output edge (since we are traversing backwards).
            ConnectorArea2D outputConnector = GetConnection(block.outputConnector);
            if (outputConnector != null) {
                Block outputBlock = outputConnector.GetParent<Block>();
                string edge1 = block.GetSerializedName() + nodeNum + "-out0";
                string edge2 = "";
                for (int i = 0; i < outputBlock.inputConnectors.Count; i++) {
                    if (outputBlock.inputConnectors[i].Equals(outputConnector)) {
                        edge2 = outputBlock.GetSerializedName() + nodeNum + "-input" + i;
                        break;
                    }
                }

                if (edge2 == "") {
                    throw new InvalidOperationException("missing edge");
                }

                edges.Add(edge1 + "," + edge2);
            }

            // Loop through input Blocks and enqueue them
            foreach (ConnectorArea2D inputConnector in block.inputConnectors) {
                ConnectorArea2D targetConnector = GetConnection(inputConnector);
                if (targetConnector == null) {
                    throw new InvalidOperationException("no open inputs allowed yet");
                }

                Block inputBlock = targetConnector.GetParent<Block>();
                stack.Push(inputBlock);
            }

            nodeNum++;
        }

        GD.Print("Program Complete");
        foreach (string node in nodes) {
            GD.Print(node);
        }

        foreach (string edge in edges) {
            GD.Print(edge);
        }
    }

    public static MultiBlock LoadProgram() {
        MultiBlock multiBlock = new MultiBlock();
        return multiBlock;
    }
}
