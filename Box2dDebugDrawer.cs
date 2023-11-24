using Box2DSharp.Collision.Shapes;
using Box2DSharp.Common;
using Box2DSharp.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Numerics;

namespace Platformer
{
    internal class Box2dDebugDrawer : IDrawer
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly float DebugThickness;

        public Box2dDebugDrawer(SpriteBatch spriteBatch, float debugThickness = 0.1f)
        {
            _spriteBatch = spriteBatch;
            DebugThickness = debugThickness;
            Flags = DrawFlag.DrawShape & DrawFlag.DrawCenterOfMass;
        }

        public DrawFlag Flags { get; set; }

        public void DrawCircle(in Vector2 center, float radius, in Color color)
        {
            _spriteBatch.DrawCircle(center, radius, 24, color.ToXna(), thickness: DebugThickness);
        }

        public void DrawPoint(in Vector2 p, float size, in Color color)
        {
            _spriteBatch.DrawPoint(p, color.ToXna(), size);
        }

        public void DrawPolygon(Span<Vector2> vertices, int vertexCount, in Color color)
        {
            for (int i = 0; i < vertexCount; i++)
            {
                int j = (i + 1) % vertexCount;

                DrawSegment(vertices[i], vertices[j], color);
            }
        }

        public void DrawSegment(in Vector2 p1, in Vector2 p2, in Color color)
        {
            _spriteBatch.DrawLine(p1, p2, color.ToXna(), thickness: DebugThickness);
        }

        public void DrawSolidCircle(in Vector2 center, float radius, in Vector2 axis, in Color color)
        {
            DrawCircle(center, radius, color);
        }

        public void DrawSolidPolygon(Span<Vector2> vertices, int vertexCount, in Color color)
        {
            DrawPolygon(vertices, vertexCount, color);
        }

        public void DrawTransform(in Transform xf)
        {
            DrawPoint(xf.Position, DebugThickness, new Color());
        }

        internal void Draw(Body body)
        {
            Color color = GetColor(body);
            Transform xf = body.GetTransform();
            foreach (var fixture in body.FixtureList)
            {
                switch (fixture.ShapeType)
                {
                    case ShapeType.Circle:
                        CircleShape circle = (CircleShape)fixture.Shape;
                        Vector2 center = MathUtils.Mul(in xf, in circle.Position);
                        float radius = circle.Radius;
                        ref readonly Rotation rotation = ref xf.Rotation;
                        Vector2 v = new Vector2(1f, 0f);
                        Vector2 axis = MathUtils.Mul(in rotation, in v);
                        DrawSolidCircle(center, radius, axis, color);
                        break;
                    case ShapeType.Polygon:
                        PolygonShape polygon = (PolygonShape)fixture.Shape;
                        int count = polygon.Count;
                        Span<Vector2> vertices = new Vector2[count];
                        for (int i = 0; i < count; i++)
                        {
                            vertices[i] = MathUtils.Mul(in xf, in polygon.Vertices[i]);
                        }
                        DrawPolygon(vertices, count, color);
                        break;
                    case ShapeType.Edge:
                        EdgeShape edge = (EdgeShape)fixture.Shape;
                        Vector2 p3 = MathUtils.Mul(in xf, in edge.Vertex1);
                        Vector2 p4 = MathUtils.Mul(in xf, in edge.Vertex2);
                        DrawSegment(in p3, in p4, in color);
                        if (!edge.OneSided)
                        {
                            DrawPoint(in p3, 4f, in color);
                            DrawPoint(in p4, 4f, in color);
                        }
                        break;
                    case ShapeType.Chain:
                        ChainShape chain = (ChainShape)fixture.Shape;
                        int count2 = chain.Count;
                        Vector2[] vertices2 = chain.Vertices;
                        Vector2 p = MathUtils.Mul(in xf, in vertices2[0]);
                        for (int j = 1; j < count2; j++)
                        {
                            Vector2 p2 = MathUtils.Mul(in xf, in vertices2[j]);
                            DrawSegment(in p, in p2, in color);
                            p = p2;
                        }
                        break;
                }
            }
        }

        private static Color GetColor(Body value)
        {
            if (value.BodyType == BodyType.DynamicBody && value.Mass.Equals(0f))
            {
                return Color.FromArgb(1f, 0f, 0f);
            }
            else if (!value.IsEnabled)
            {
                return Color.FromArgb(128, 128, 77);
            }
            else if (value.BodyType == BodyType.StaticBody)
            {
                return Color.FromArgb(127, 230, 127);
            }
            else if (value.BodyType == BodyType.KinematicBody)
            {
                return Color.FromArgb(127, 127, 230);
            }
            else if (!value.IsAwake)
            {
                return Color.FromArgb(153, 153, 153);
            }
            else
            {
                return Color.FromArgb(230, 179, 179);
            }
        }
    }
}