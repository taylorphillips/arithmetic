using System.Collections.Generic;
using Godot;
using static Block.ConnectorArea2D;
using Object = Godot.Object;


/// <summary>
/// Blocks are base puzzle pieces and can have 0+ inputs but only one output. Blocks
/// can then be assembled together into programs, and the programs can be executed
/// in the form of a measuring or counting.
/// </summary>
public class Block : Node2D
{
    public class SelectableArea2D : Area2D
    {
        public SelectableArea2D() { }

        public override void _Ready() {
            Name = "SelectableArea2D";
            Polygon2D poly = new Polygon2D();
            poly.Color = Colors.Gray;
            poly.Polygon = new Vector2[] {
                new Vector2(-40, -40),
                new Vector2(40, -40),
                new Vector2(40, 40),
                new Vector2(-40, 40),
            };

            Polygon2D poly2 = new Polygon2D();
            poly2.Color = Colors.Black;
            poly2.Polygon = new Vector2[] {
                new Vector2(-42, -42),
                new Vector2(42, -42),
                new Vector2(42, 42),
                new Vector2(-42, 42),
            };
            AddChild(poly2);
            AddChild(poly);

            CollisionPolygon2D collisionPolygon2D = new CollisionPolygon2D();
            collisionPolygon2D.Polygon = poly.Polygon;
            collisionPolygon2D.BuildMode = CollisionPolygon2D.BuildModeEnum.Solids;
            AddChild(collisionPolygon2D);

            // Add static body to contain the units. Could be elsewhere?
            StaticBody2D staticBody2D = new StaticBody2D();
            collisionPolygon2D = (CollisionPolygon2D) collisionPolygon2D.Duplicate();
            collisionPolygon2D.BuildMode = CollisionPolygon2D.BuildModeEnum.Segments;
            staticBody2D.AddChild(collisionPolygon2D.Duplicate());
            AddChild(staticBody2D);
        }

        public override void _InputEvent(Object viewport, InputEvent @event, int shapeIdx) {
            if (@event is InputEventMouseButton mouseEvent) {
                if (mouseEvent.Pressed && mouseEvent.ButtonIndex == (int) ButtonList.Left) {
                    GetParent<Block>().select();
                } else if (!mouseEvent.Pressed && mouseEvent.ButtonIndex == (int) ButtonList.Left) {
                    GetParent<Block>().deselect();
                }
            }
        }
    }

    public class ConnectorArea2D : Area2D
    {
        public enum ConnectorType
        {
            INPUT = 1,
            OUTPUT = 2,
        }

        public readonly Vector2 initPosition;
        public readonly ConnectorType connectorType;
        public readonly float Radius = 20;

        public ConnectorArea2D(ConnectorType connectorType, Vector2 initPosition) {
            this.connectorType = connectorType;
            this.initPosition = initPosition;
        }

        public override void _Ready() {
            ZIndex = -1;
            Name = "ConnectorArea2D-" + connectorType;
            CircleShape2D circle = new CircleShape2D();
            circle.Radius = Radius;
            CollisionShape2D collisionShape2D = new CollisionShape2D();
            collisionShape2D.Shape = circle;
            Position = initPosition;
            AddChild(collisionShape2D);
            Connect("area_entered", this, "_OnAreaEnter");
        }

        public bool CanConnectTo(ConnectorArea2D connector) {
            return connector.connectorType != connectorType;
        }

        public void _OnAreaEnter(Area2D area2D) {
            if (area2D is ConnectorArea2D connectorArea2D && CanConnectTo(connectorArea2D)) {
                GetParent<Block>().connectorCollision(this, connectorArea2D);
            }
        }

        public override void _Draw() {
            DrawCircle(Vector2.Zero, Radius, Colors.PaleGoldenrod);
        }
    }

    public Node2D contentNode;
    public List<ConnectorArea2D> inputConnectors = new List<ConnectorArea2D>();
    public ConnectorArea2D outputConnector;

    public bool IsSelectable = true; 
    protected bool selected = false;
    
    protected ConnectorArea2D snapFrom;
    protected ConnectorArea2D snapTo;

    private RandomNumberGenerator rng;

    public Block() {
        rng = new RandomNumberGenerator();
    }

    public override void _Ready() {
        contentNode = new Node2D();
        contentNode.ZIndex = 10;
        AddChild(contentNode);

        outputConnector = new ConnectorArea2D(ConnectorType.OUTPUT, new Vector2(0, 40));
        AddChild(outputConnector);
        AddChild(new SelectableArea2D());
    }

    public override void _PhysicsProcess(float delta) {
        if (selected) {
            MultiBlock parent = GetParent<MultiBlock>();
            if (getSnapPosition().HasValue) {
                if (parent.GlobalPosition.DistanceTo(GetGlobalMousePosition()) > 40f) {
                    snapFrom = null;
                    snapTo = null;
                } else {
                    parent.GlobalPosition = getSnapPosition().Value;
                }
            } else {
                parent.GlobalPosition = GetGlobalMousePosition();
            }
        }
    }

    public void PushButton() {
        Unit unit = new Unit();
        unit.GlobalPosition = new Vector2(rng.RandfRange(-20, 20), rng.RandfRange(-20, 20));
        contentNode.AddChild(unit);
    }


    public void AddContent(MultiBlock block) {
        contentNode.AddChild(block);
    }

    public void ClearContent() {
        foreach (Node child in contentNode.GetChildren()) {
            contentNode.RemoveChild(child);
        }
    }

    private Vector2 getSnapPosition(ConnectorArea2D from, ConnectorArea2D to) {
        if (from.connectorType == ConnectorType.INPUT) {
            return to.GlobalPosition + new Vector2(0, from.Radius * 2);
        } else {
            return to.GlobalPosition + new Vector2(0, -40);
        }
    }

    private Vector2? getSnapPosition() {
        if (snapFrom != null && snapTo != null) {
            return getSnapPosition(snapFrom, snapTo);
        } else {
            return null;
        }
    }

    private void connectorCollision(ConnectorArea2D snapFrom, ConnectorArea2D snapTo) {
        // TODO: This should handle snapping multiple connectors simultaneously.
        if (selected) {
            this.snapFrom = snapFrom;
            this.snapTo = snapTo;
        }
    }

    private void select() {
        if (IsSelectable) {
            selected = true;
        } else {
            GD.Print("NB: IsSelectable = false");
        }
    }

    private void deselect() {
        selected = false;

        // Confirm the snap
        if (snapFrom != null && snapFrom != null) {
            // TODO: Add to parent 2D as group
            // TODO: Store the graph somewhere.
            snapFrom = null;
            snapTo = null;
        }
    }

    public virtual void Run() {
        if (AreInputsSatisfied()) {
            // How to handle inputs vs internals?
            return;
        }
    }

    public bool AreInputsSatisfied() {
        foreach (ConnectorArea2D connectorArea2D in inputConnectors) {
            if (GetParent<MultiBlock>().GetConnector(connectorArea2D) == null) {
                return false;
            }
        }

        return true;
    }
}
