using Godot;
using Object = Godot.Object;


public class Block : Node2D
{
    public ConnectorArea2D inputConnector;
    public ConnectorArea2D outputConnector;
    public Vector2? snapPosition;

    public class SelectableArea2D : Area2D
    {
        private Polygon2D poly;
        private bool selected = false;

        public override void _Ready() {
            // Create body
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
                    GD.Print("LEFT CLICK");
                    selected = true;
                } else if (!mouseEvent.Pressed && mouseEvent.ButtonIndex == (int) ButtonList.Left) {
                    GD.Print("LEFT UNCLICK");
                    selected = false;
                }
            }
        }

        public void _OnMouseEnter() {
            poly.Color = Colors.Blue;
        }

        public void _OnMouseExit() {
            poly.Color = Colors.Red;
        }

        public override void _PhysicsProcess(float delta) {
            if (selected) {
                Block block = GetParent<Block>();
                if (block.snapPosition.HasValue) {
                    Transform2D transform = GlobalTransform;
                    transform.origin = block.snapPosition.Value;
                    GetParent<Node2D>().Transform = transform;
                } else {
                    Transform2D transform = GlobalTransform;
                    transform.origin = GetGlobalMousePosition();
                    GetParent<Node2D>().Transform = transform;
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

        private readonly ConnectorType connectorType;
        private readonly Vector2 initPosition;
        private ConnectorArea2D connectedConnector;
        private ConnectorArea2D snappedConnector;

        public ConnectorArea2D(ConnectorType connectorType, Vector2 initPosition) {
            this.connectorType = connectorType;
            this.initPosition = initPosition;
        }

        public override void _Ready() {
            CircleShape2D circle = new CircleShape2D();
            circle.Radius = 20;
            CollisionShape2D collisionShape2D = new CollisionShape2D();
            collisionShape2D.Shape = circle;
            Position = initPosition;
            AddChild(collisionShape2D);
            Name = "Connector" + connectorType;
            Connect("area_entered", this, "_OnAreaEnter");
            Connect("area_exited", this, "_OnAreaExit");
        }

        public bool ConnectsTo(ConnectorArea2D connector) {
            return connector.connectorType != connectorType;
        }

        public void _OnAreaEnter(Area2D area2D) {
            GD.Print(Name + " has been entered by " + area2D.Name);
            if (area2D is ConnectorArea2D connectorArea2D && ConnectsTo(connectorArea2D)) {
                snappedConnector = connectorArea2D;
                GetParent<Block>().snapPosition = snapToPosition(snappedConnector);
            }
        }

        // TODO: This never triggers, figure out how to fix.
        public void _OnAreaExit(Area2D area2D) {
            GD.Print(Name + " has been exited by " + area2D.Name);
            if (snappedConnector != null && area2D is ConnectorArea2D connectorArea2D) {
                if (connectorArea2D.GetPath() == snappedConnector.GetPath()) {
                    snappedConnector = null;
                    GetParent<Block>().snapPosition = null;
                }
            }
        }
        
        private Vector2 snapToPosition(ConnectorArea2D connectorArea2D) {
            // TODO: Make this correct.
            return connectorArea2D.GlobalPosition;
        }
    }

    public override void _Ready() {
        AddChild(new SelectableArea2D());
        inputConnector = new ConnectorArea2D(ConnectorArea2D.ConnectorType.INPUT, new Vector2(0, -40));
        AddChild(inputConnector);
        outputConnector = new ConnectorArea2D(ConnectorArea2D.ConnectorType.OUTPUT, new Vector2(0, 40));
        AddChild(outputConnector);
    }
}
