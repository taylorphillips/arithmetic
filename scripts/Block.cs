using Godot;
using Object = Godot.Object;


public class Block : Node2D
{
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
            GD.Print("ENTERED");
        }

        public void _OnMouseExit() {
            poly.Color = Colors.Red;
        }

        public override void _PhysicsProcess(float delta) {
            if (selected) {
                Transform2D transform = GlobalTransform;
                transform.origin = GetGlobalMousePosition();
                GetParent<Node2D>().Transform = transform;
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
        private readonly Vector2 initGlobalPosition;

        public ConnectorArea2D(ConnectorType connectorType, Vector2 globalPosition) {
            this.connectorType = connectorType;
            initGlobalPosition = globalPosition;
        }

        public override void _Ready() {
            CircleShape2D circle = new CircleShape2D();
            circle.Radius = 10;
            CollisionShape2D collisionShape2D = new CollisionShape2D();
            collisionShape2D.Shape = circle;
            Position = initGlobalPosition;
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
                GD.Print("CONNECTABLE");
            }
        }

        public void _OnAreaExit(Area2D area2D) {
            GD.Print(Name + " has been exited by " + area2D.Name);
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        AddChild(new SelectableArea2D());
        AddChild(new ConnectorArea2D(ConnectorArea2D.ConnectorType.INPUT, new Vector2(0, -40)));
        AddChild(new ConnectorArea2D(ConnectorArea2D.ConnectorType.OUTPUT, new Vector2(0, 40)));
    }
}
