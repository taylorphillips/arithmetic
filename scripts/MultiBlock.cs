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

        MultiBlock fromMultiBlock = fromBlock.GetParent<MultiBlock>();
        if (GetHashCode() != fromMultiBlock.GetHashCode()) {
            // TODO: Improve this logic.
            fromMultiBlock.RemoveChild(fromBlock);
            if (fromMultiBlock.blocks.Any()) {
                fromMultiBlock.Free();
            }

            AddChild(fromBlock);
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
        edges.Remove(new ConnectorArea2D(ConnectorArea2D.ConnectorType.INPUT, Vector2.Down));
        if (GetConnection(connectorArea2D) != null) {
            edges.Remove(edges[connectorArea2D]);
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
        if (edges[block.outputConnector] != null) {
            return null;
        }

        return edges[block.outputConnector].GetParent<Block>();
    }

    public void RunProgram() {
        // TODO: Store program so that it can be restored after being destroyed (computed).
        // TODO: Be able to choose between breadth first, depth first execution and simultaneous.
        // TODO: Be able to fast forward programs.

        // Unit blocks are the seeds of program execution.
        IEnumerable<Block> seeds = blocks.Where(block => block.inputConnectors.Count == 0);
        foreach (Block block in seeds.ToList()) {
            block.Run();
        }
    }

    public void SaveProgram() { }

    /// <summary>
    /// Set's this MultiBlock 
    /// </summary>
    public void LoadProgram() { }
}
