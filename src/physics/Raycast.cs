using SFML.Graphics;
using SFML.System;
using System;

namespace raycast {
    struct Wall : Drawable {
        public readonly WallType Type;
        public readonly (float, float) Position;
        public readonly (float, float) Size;
        public readonly float Direction;
        public readonly ((float, float), (float, float)) CollisionBox;

        private RectangleShape drawBox, lineBox;

        public Wall(WallType type, (float, float) position, float direction) {
            Type = type;
            Position = position;
            Size = (64, 64);
            Direction = direction;

            drawBox = new RectangleShape(new Vector2f(Size.Item1, Size.Item2));
            drawBox.Origin = new Vector2f(Size.Item1 / 2, Size.Item2 / 2);
            drawBox.Position = new Vector2f(position.Item1, position.Item2);

            Texture wallTex = TextureLoader.getInstance().GetTexture(type);
            if(wallTex == null) {
                drawBox.FillColor = Color.Magenta;
            } else {
                drawBox.Texture = wallTex;
            }

            lineBox = new RectangleShape(new Vector2f(4, Size.Item2));
            lineBox.Origin = new Vector2f(2, Size.Item2 / 2);
            lineBox.Position = new Vector2f(position.Item1, position.Item2);
            lineBox.FillColor = Color.Red;

            drawBox.Rotation = Direction;
            lineBox.Rotation = Direction;

            var bounds = lineBox.GetGlobalBounds();
            CollisionBox = (
                (bounds.Left - 1, bounds.Top - 1),
                (bounds.Left + bounds.Width + 1, bounds.Top + bounds.Height + 1)
            );
        }

        public void Draw(RenderTarget target, RenderStates states) {
            target.Draw(drawBox);
            target.Draw(lineBox);
        }
    }

    struct Ray : Drawable {
        public (float, float) Position;
        public float Direction;
        public readonly float Length;

        private RectangleShape line;

        public Ray((float, float) start, float dir) {
            Position = start;
            Direction = dir;
            Length = 64;

            line = new RectangleShape(new Vector2f(Length, 1));
            line.Position = new Vector2f(Position.Item1, Position.Item2);
            line.Rotation = dir;
        }

        public void Rotate(float angle) {
            Direction += angle;
            line.Rotation += angle;
        }

        public void MoveTo((float, float) position) {
            Position = position;
            line.Position = new Vector2f(Position.Item1, Position.Item2);
        }

        public (float, float) Cast(Wall wall) {
            (float, float) directionVector = (
                Length * (float) Math.Cos(Direction * Math.PI / 180), 
                Length * (float) Math.Sin(Direction * Math.PI / 180)
            );
            var tip = (Position.Item1 + directionVector.Item1, Position.Item2 + directionVector.Item2);

            var x1 = wall.CollisionBox.Item1.Item1;
            var y1 = wall.CollisionBox.Item1.Item2;
            var x2 = wall.CollisionBox.Item2.Item1;
            var y2 = wall.CollisionBox.Item2.Item2;
            var x3 = Position.Item1;
            var y3 = Position.Item2;
            var x4 = tip.Item1;
            var y4 = tip.Item2;

            var denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            
            if(denominator == 0) {
                return (-1, -1);
            }

            var tNumerator = (x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4);
            var uNumerator = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3));

            var t = tNumerator / denominator;
            var u = uNumerator / denominator;

            if(t > 0 && t < 1 && u > 0) {
                return (
                    x1 + t * (x2 - x1),
                    y1 + t * (y2 - y1)
                );
            } else {
                return (-1, -1);
            }
        }

        public void Draw(RenderTarget target, RenderStates states) {
            target.Draw(line);
        }
    }
}
