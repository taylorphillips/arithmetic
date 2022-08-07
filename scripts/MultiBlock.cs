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
    private List<Block> blocks = new List<Block>();
    private Dictionary<ConnectorArea2D, ConnectorArea2D> edges;

    public MultiBlock() { }

    public MultiBlock(Block block) {
        blocks.Add(block);
    }

    // TODO: Make more robust and handle multiple connectors at once.
    public void ConnectBlock(Block block, ConnectorArea2D from, ConnectorArea2D to) {
        edges[from] = to;
        edges[to] = from;
        blocks.Add(block);
    }

    public void DisconnectBlock(Block block) {
        // TODO: Detect disjoint graph.

        foreach (ConnectorArea2D connector in block.inputConnectors) {
            edges[edges[connector]] = null;
            edges[connector] = null;
            blocks.Remove(block);
        }
    }

    public ConnectorArea2D GetConnector(ConnectorArea2D connectorArea2D) {
        return edges[connectorArea2D];
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
        IEnumerable<Block> seeds = blocks.Select(block => {
            if (block.inputConnectors.Count == 0) {
                return block;
            }

            return null;
        });

        foreach (Block block in seeds) {
            block.Run();
        }
    }

    public void SaveProgram() { }

    /// <summary>
    /// Set's this MultiBlock 
    /// </summary>
    public void LoadProgram() { }
}
