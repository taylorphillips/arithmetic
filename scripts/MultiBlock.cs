using System;
using System.Collections;
using System.Collections.Generic;
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
    private Dictionary<ConnectorArea2D, List<ConnectorArea2D>> edges;

    public MultiBlock(Block block) {
        blocks.Add(block);
    }

    // TODO: Make more robust and handle multiple connectors at once.
    public void ConnectBlock(Block block, ConnectorArea2D from, ConnectorArea2D to) {
        edges[from].Add(to);
        edges[to].Add(from);
        blocks.Add(block);
    }

    public void DisconnectBlock(Block block) {
        // TODO: Detect disjoint graph.

        foreach (ConnectorArea2D connector in block.inputConnectors) {
            List<ConnectorArea2D> connections = edges[connector];
            edges.Remove(connector);
            foreach (ConnectorArea2D connectorArea2D in connections) {
                edges[connectorArea2D].Remove(connector);
            }

            blocks.Remove(block);
        }
    }

    public void RunProgram() {
        // TODO: Be able to choose between breadth first, depth first execution and simultaneous.
        // TODO: Be able to fast forward programs.

        // Blocks without inputs are seeds of executions
        List<Block> seeds = new List<Block>();
        foreach (Block block in seeds) {
            // TODO: These Types should probably be a subclass somehow.
            switch (block.GetClass()) {
                case "EMPTY":
                    return;
                case "SUCCESSOR":
                // Validate it has input.
                // Insert a ball.
                // Remove the successor block.
                case "ADDITION":
                // Validate it has inputs.
                // Combine the inputs.
                // Remove the addition block.
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
