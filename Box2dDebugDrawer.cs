using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using nkast.Aether.Physics2D.Collision.Shapes;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Diagnostics;
using nkast.Aether.Physics2D.Dynamics;

namespace Platformer
{
    internal class Box2dDebugDrawer : DebugViewBase
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly float DebugThickness;

        public Box2dDebugDrawer(World world, SpriteBatch spriteBatch, float debugThickness = 0.1f) : base(world)
        {
            _spriteBatch = spriteBatch;
            DebugThickness = debugThickness;
            Flags = DebugViewFlags.Shape & DebugViewFlags.CenterOfMass;
        }

        public override void DrawCircle(Vector2 center, float radius, Color color)
        {
            _spriteBatch.DrawCircle(center, radius, 24, color, thickness: DebugThickness);
        }

        public override void DrawPolygon(Vector2[] vertices, int vertexCount, Color color, bool closed = true)
        {
            for (int i = 0; i < vertexCount; i++)
            {
                int j = (i + 1) % vertexCount;

                DrawSegment(vertices[i], vertices[j], color);
            }
        }

        public override void DrawSegment(Vector2 p1, Vector2 p2, Color color)
        {
            _spriteBatch.DrawLine(p1, p2, color, thickness: DebugThickness);
        }

        public override void DrawSolidCircle(Vector2 center, float radius, Vector2 axis, Color color)
        {
            DrawCircle(center, radius, color);
        }

        public override void DrawSolidPolygon(Vector2[] vertices, int vertexCount, Color color)
        {
            DrawPolygon(vertices, vertexCount, color);
        }

        public override void DrawTransform(ref Transform xf)
        {
            _spriteBatch.DrawPoint(xf.p, Color.Blue, DebugThickness);
        }

        public void Draw()
        {
            foreach(var body in World.BodyList)
            {
                foreach (var fixture in body.FixtureList)
                    Draw(fixture);
            }
        }

        public void Draw(Fixture fixture)
        {
            Color color = GetColor(fixture.Body);
            Transform transform = fixture.Body.GetTransform();

            switch (fixture.Shape.ShapeType)
            {
                case ShapeType.Circle:
                    CircleShape circle = (CircleShape)fixture.Shape;
                    Vector2 center = Transform.Multiply(circle.Position, ref transform);
                    float radius = circle.Radius;
                    Vector2 axis = Vector2.UnitX;
                    axis.Rotate(fixture.Body.Rotation);
                    DrawSolidCircle(center, radius, axis, color);
                    break;
                case ShapeType.Polygon:
                    PolygonShape polygon = (PolygonShape)fixture.Shape;
                    int count = polygon.Vertices.Count;
                    Vector2[] vertices = new Vector2[count];
                    for (int i = 0; i < count; i++)
                    {
                        vertices[i] = Transform.Multiply(polygon.Vertices[i], ref transform);
                    }
                    DrawPolygon(vertices, count, color);
                    break;
                case ShapeType.Edge:
                    EdgeShape edge = (EdgeShape)fixture.Shape;
                    Vector2 p3 = Transform.Multiply(edge.Vertex1, ref transform);
                    Vector2 p4 = Transform.Multiply(edge.Vertex2, ref transform);
                    DrawSegment(p3, p4, color);
                    break;
                case ShapeType.Chain:
                    ChainShape chain = (ChainShape)fixture.Shape;
                    int count2 = chain.Vertices.Count;
                    Vector2[] vertices2 = [.. chain.Vertices];
                    Vector2 p = Transform.Multiply(vertices2[0], ref transform);
                    for (int j = 1; j < count2; j++)
                    {
                        Vector2 p2 = Transform.Multiply(vertices2[j], ref transform);
                        DrawSegment(p, p2, color);
                        p = p2;
                    }
                    break;
            }
        }

        private static Color GetColor(Body value)
        {
            if (value.BodyType == BodyType.Dynamic && value.Mass.Equals(0f))
                return Color.IndianRed;
            if (!value.Enabled)
                return Color.DimGray;
            if (value.BodyType == BodyType.Static)
                return Color.LightGreen;
            if (value.BodyType == BodyType.Kinematic)
                return Color.Goldenrod;
            if (!value.Awake)
                return Color.SlateGray;
            
           return Color.White;
        }
    }
}