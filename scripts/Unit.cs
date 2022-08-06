    using Godot;

    public class Unit : RigidBody2D
    {
        private float Radius;

        public override void _Ready() {
            CircleShape2D circleShape2D = new CircleShape2D();
            circleShape2D.Radius = Radius = 20;

            CollisionShape2D collisionShape2D = new CollisionShape2D();
            collisionShape2D.Shape = circleShape2D;
            AddChild(collisionShape2D);
        }

        public override void _Draw() {
            DrawCircle(new Vector2(0,0), Radius, Colors.Blue);
        }
    }
