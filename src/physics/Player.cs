using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;

namespace raycast {
    class Player : Drawable {
        public (float, float) Position;
        public float ViewWidth, ViewDistance;
        public float Direction;
        public float MaxMouseChange;
        public float MaxTurn;
        public float MoveSpeed;

        private Ray[] rays;
        private int numVectors;
        private CircleShape playerDrawing;
        private bool moveForward, moveBack, moveLeft, moveRight;

        public Player(
                (float, float) position, float viewWidth, float viewDistance, float startDirection,
                float rotateSpeed, float moveSpeed, int numVectors,
                float maxMouseChange, float maxTurn) {
            Position = position;
            ViewWidth = viewWidth;
            ViewDistance = viewDistance;
            Direction = startDirection;

            MoveSpeed = moveSpeed;
            moveForward = false;
            moveBack = false;
            moveLeft = false;
            moveRight = false;

            this.numVectors = numVectors;

            MaxMouseChange = maxMouseChange;
            MaxTurn = maxTurn;

            var rayList = new List<Ray>();
            for(float angle = -viewWidth / 2; angle < viewWidth / 2; angle += viewWidth / (float) numVectors) {
                rayList.Add(new Ray(position, Direction + angle));
            }
            rays = rayList.ToArray();

            playerDrawing = new CircleShape(16);
            playerDrawing.Origin = new Vector2f(16, 16);
            playerDrawing.Position = new Vector2f(Position.Item1, Position.Item2);
            playerDrawing.FillColor = Color.Blue;
        }

        public void MouseMoved((float, float) mouseDelta) {
            rotate((mouseDelta.Item1 / MaxMouseChange) * MaxTurn);
        }

        public void KeyPressed(Keyboard.Key key) {
            if(key == Keyboard.Key.A) {
                moveLeft = true;
            }
            
            if(key == Keyboard.Key.D) {
                moveRight = true;
            }

            if(key == Keyboard.Key.W) {
                moveForward = true;
            }

            if(key == Keyboard.Key.S) {
                moveBack = true;
            }
        }

        public void KeyReleased(Keyboard.Key key) {
            if(key == Keyboard.Key.A) {
                moveLeft = false;
            }
            
            if(key == Keyboard.Key.D) {
                moveRight = false;
            }

            if(key == Keyboard.Key.W) {
                moveForward = false;
            }

            if(key == Keyboard.Key.S) {
                moveBack = false;
            }
        }

        public void Update(float deltaTimeSec) {
            if(moveForward) {
                move(MoveSpeed * deltaTimeSec);
            }
            
            if(moveBack) {
                move(-MoveSpeed * deltaTimeSec);
            }

            if(moveLeft) {
                strafe(-MoveSpeed * deltaTimeSec);
            }

            if(moveRight) {
                strafe(MoveSpeed * deltaTimeSec);
            }
        }

        private void strafe(float distance) {
            (float, float) directionVector = (
                distance * (float) Math.Cos((Direction + 90) * Math.PI / 180), 
                distance * (float) Math.Sin((Direction + 90) * Math.PI / 180)
            );

            Position.Item1 += directionVector.Item1;
            Position.Item2 += directionVector.Item2;

            for(int i = 0; i < rays.Length; i++) {
                rays[i].MoveTo(Position);
            }

            playerDrawing.Position = new Vector2f(Position.Item1, Position.Item2);
        }

        private void move(float distance) {
            (float, float) directionVector = (
                distance * (float) Math.Cos(Direction * Math.PI / 180), 
                distance * (float) Math.Sin(Direction * Math.PI / 180)
            );

            Position.Item1 += directionVector.Item1;
            Position.Item2 += directionVector.Item2;

            for(int i = 0; i < rays.Length; i++) {
                rays[i].MoveTo(Position);
            }

            playerDrawing.Position = new Vector2f(Position.Item1, Position.Item2);
        }

        private void rotate(float angle) {
            Direction += angle;
            for(int i = 0; i < rays.Length; i++) {
                rays[i].Rotate(angle);
            }
        }

        public WallSlice[] CastRays(Wall[] walls, (float, float) stripSize) {
            var intersectionPoints = new List<WallSlice>();

            int rayIndex = 0;
            foreach(var ray in rays) {
                float closestDistance = ViewDistance;
                (float, float) closestPoint = (-1, -1);
                Wall closestWall = walls[0];

                foreach(var wall in walls) {
                    var point = ray.Cast(wall);

                    if(point.Item1 != -1) {
                        var distance = Math.Sqrt(
                            Math.Pow(point.Item1 - ray.Position.Item1, 2)
                            + Math.Pow(point.Item2 - ray.Position.Item2, 2)
                        );

                        if(distance < closestDistance) {
                            closestDistance = (float) distance;
                            closestPoint = point;
                            closestWall = wall;
                        }
                    }
                }

                if(closestPoint.Item1 != -1) {
                    var beta = ray.Direction - Direction;
                    var perspectiveDistance = closestDistance * (float) Math.Cos(beta * Math.PI / 180);
                    var slice = new WallSlice(
                        closestWall.CollisionBox.Item1, closestPoint, closestWall.CollisionBox.Item2,
                        closestWall.Type,
                        (stripSize.Item1, 
                            stripSize.Item2 - (perspectiveDistance * stripSize.Item2) / ViewDistance), 
                        rayIndex, stripSize.Item2, 1
                    );
                    intersectionPoints.Add(slice);
                }
                rayIndex++;
            }

            return intersectionPoints.ToArray();
        }

        public void Draw(RenderTarget target, RenderStates states) {
            foreach(var ray in rays) {
                target.Draw(ray);
            }

            target.Draw(playerDrawing);
        }
    }
}