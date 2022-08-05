using Godot;
using Object = Godot.Object;


public class Block : Node2D
{
    public class SelectableArea2D : Area2D
    {
        private Polygon2D poly;

        public override void _Ready() {
            Name = "SelectableArea2D";
            poly = new Polygon2D();
            poly.Color = Colors.Red;
            poly.Polygon = new Vector2[] {
                new Vector2(-40, -40),
                new Vector2(40, -40),
                new Vector2(40, 40),
                new Vector2(-40, 40),
            };
            CollisionPolygon2D collisionPolygon2D = new CollisionPolygon2D();
            collisionPolygon2D.Polygon = poly.Polygon;
            AddChild(poly);
            AddChild(collisionPolygon2D);
            Connect("mouse_entered", this, "_OnMouseEnter");
            Connect("mouse_exited", this, "_OnMouseExit");
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

        public ConnectorArea2D(ConnectorType connectorType, Vector2 initPosition) {
            this.connectorType = connectorType;
            this.initPosition = initPosition;
        }

        public override void _Ready() {
            Name = "ConnectorArea2D-" + connectorType;
            CircleShape2D circle = new CircleShape2D();
            circle.Radius = 20;
            CollisionShape2D collisionShape2D = new CollisionShape2D();
            collisionShape2D.Shape = circle;
            Position = initPosition;
            AddChild(collisionShape2D);
            Connect("area_entered", this, "_OnAreaEnter");
        }

        public bool ConnectsTo(ConnectorArea2D connector) {
            return connector.connectorType != connectorType;
        }

        public void _OnAreaEnter(Area2D area2D) {
            if (area2D is ConnectorArea2D connectorArea2D && ConnectsTo(connectorArea2D)) {
                GetParent<Block>().connectorCollision(this, connectorArea2D);
            }
        }
    }

    private ConnectorArea2D inputConnector;
    private ConnectorArea2D outputConnector;

    private ConnectorArea2D connectedConnector;
    private ConnectorArea2D snappedConnector;

    private ConnectorArea2D snapFrom;
    private ConnectorArea2D snapTo;
    private bool selected = false;

    public override void _Ready() {
        AddChild(new SelectableArea2D());
        inputConnector = new ConnectorArea2D(ConnectorArea2D.ConnectorType.INPUT, new Vector2(0, 40));
        AddChild(inputConnector);
        outputConnector = new ConnectorArea2D(ConnectorArea2D.ConnectorType.OUTPUT, new Vector2(0, -40));
        AddChild(outputConnector);
    }

    public override void _PhysicsProcess(float delta) {
        if (selected) {
            Transform2D transform = GlobalTransform;
            transform.origin = GetGlobalMousePosition();
            if (getSnapPosition().HasValue) {
                if (GlobalPosition.DistanceTo(GetGlobalMousePosition()) > 40f) {
                    snapFrom = null;
                    snapTo = null;
                } else {
                    transform.origin = getSnapPosition().Value;
                }
            }

            Transform = transform;
        }
    }

    private Vector2 getSnapPosition(ConnectorArea2D from, ConnectorArea2D to) {
        if (from.connectorType == ConnectorArea2D.ConnectorType.INPUT) {
            return to.GlobalPosition + new Vector2(0, -40);
        } else {
            return to.GlobalPosition + new Vector2(0, 40);
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
        if (selected) {
            this.snapFrom = snapFrom;
            this.snapTo = snapTo;
        }
    }

    private void select() {
        selected = true;
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
}
