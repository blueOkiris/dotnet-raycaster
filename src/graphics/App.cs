using SFML.Window;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace raycast {
    class Application {
        private static Application instance = null;
        
        private RenderWindow window;
        private (uint, uint) size;
        private string windowTitle;

        private Player player;
        private float deltaTimeSec;
        private float fps;

        public static Application getInstance() {
            if(instance == null) {
                instance = new Application();
            }

            return instance;
        }

        private Application(float fps = 60) {
            windowTitle = "Dotnet Core/SFML Raycast Example";
            size = (1920, 1080);
            window = new RenderWindow(
                new VideoMode(size.Item1, size.Item2), windowTitle,
                Styles.Close | Styles.Fullscreen
            );

            window.Closed += new EventHandler((object s, EventArgs e) => (s as RenderWindow).Close());
            window.KeyPressed += new EventHandler<KeyEventArgs>(keyPressed);
            window.KeyReleased += new EventHandler<KeyEventArgs>(keyReleased);
            window.MouseMoved += new EventHandler<MouseMoveEventArgs>(mouseMove);

            player = new Player((512, 512), 65, 1000, -80, 180, 256,  (int) size.Item1, size.Item1, 90);
            
            deltaTimeSec = 0;
            this.fps = fps;
            Mouse.SetPosition(new Vector2i((int) size.Item1 / 2, (int) size.Item2 / 2));
        }

        private void mouseMove(object sender, MouseMoveEventArgs e) {
            if(e.X != size.Item1 / 2 || e.Y != size.Item2 / 2) {
                player.MouseMoved((e.X - size.Item1 / 2, e.Y - size.Item2 / 2));
                Mouse.SetPosition(new Vector2i((int) size.Item1 / 2, (int) size.Item2 / 2));
            }
        }

        private void keyPressed(object sender, KeyEventArgs e) {
            if(e.Code == Keyboard.Key.Escape) {
                window.Close();
            }

            player.KeyPressed(e.Code);
        }

        private void keyReleased(object sender, KeyEventArgs e) {
            player.KeyReleased(e.Code);
        }

        private void render(Wall[] walls, ref Player player) {
            window.Clear(new Color(0, 0, 0));

            var intersectionPoints = player.CastRays(walls, (1, size.Item2));
            for(int i = 0; i < intersectionPoints.Length; i++) {
                window.Draw(intersectionPoints[i]);
            }

            window.Draw(player);
            foreach(var wall in walls) {
                window.Draw(wall);
            }

            window.Display();
        }

        public void Run() {
            var walls = new List<Wall>();
            for(int i = 0; i < 10; i ++) {
                walls.Add(new Wall(WallType.Basic, (i * 63 + 128, 128), 90));
                walls.Add(new Wall(WallType.Basic, (10 * 63 + 96, i * 63 + 160), 0));
            }
            
            walls.Add(new Wall(WallType.Wood, (256, 256), 90));
            walls.Add(new Wall(WallType.Wood, (256 + 31, 256 + 31), 0));
            walls.Add(new Wall(WallType.Wood, (256, 256 + 62), -90));
            walls.Add(new Wall(WallType.Wood, (256 - 31, 256 + 31), 180));

            Stopwatch watch = new Stopwatch();

            while(window.IsOpen) {
                watch.Reset();
                watch.Start();

                window.DispatchEvents();
                render(walls.ToArray(), ref player);

                player.Update(deltaTimeSec);

                watch.Stop();
                deltaTimeSec = (float) watch.Elapsed.Milliseconds / 1000;
            }
        }
    }
}