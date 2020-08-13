using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace raycast {
    enum WallType {
        Basic,
        Wood
    }

    struct WallSlice : Drawable {
        public readonly (float, float) TopLeft;
        public readonly (float, float) Position;
        public readonly (float, float) BottomRight;        
        public readonly Texture Tex;
        public readonly RectangleShape DrawRect;

        public WallSlice(
                (float, float) topLeft, (float, float) position,
                (float, float) bottomRight, WallType type,
                (float, float) size, float screenIndex,
                float windowHeight, int texSize) {
            TopLeft = topLeft;
            Position = position;
            BottomRight = bottomRight;

            Tex = TextureLoader.getInstance().GetTexture(type);

            float wallLength = (float) Math.Sqrt(
                Math.Pow(topLeft.Item1 - bottomRight.Item1, 2)
                + Math.Pow(topLeft.Item2 - bottomRight.Item2, 2)
            );
            float distance = (float) Math.Sqrt(
                Math.Pow(topLeft.Item1 - position.Item1, 2)
                + Math.Pow(topLeft.Item2 - position.Item2, 2)
            );
            float distScaled = (distance / wallLength) * Tex.Size.X;

            DrawRect = new RectangleShape(new Vector2f(size.Item1 * 3, size.Item2 / 2));
            DrawRect.Origin = new Vector2f(-size.Item1, size.Item2 / 4);
            DrawRect.Position = new Vector2f(screenIndex * size.Item1, windowHeight / 2);
            DrawRect.Texture = Tex;
            DrawRect.TextureRect = new IntRect((int) distScaled, 0, texSize, (int) Tex.Size.Y);
        }

        public void Draw(RenderTarget target, RenderStates states) {
            target.Draw(DrawRect);
        }
    }

    class TextureLoader {
        private static TextureLoader instance = null;

        private Dictionary<WallType, Texture> wallTextures; 

        public static TextureLoader getInstance() {
            if(instance == null) {
                instance = new TextureLoader();
            }

            return instance;
        }
        
        private TextureLoader() {
            wallTextures = new Dictionary<WallType, Texture>();

            Texture basicWallTex = null;
            Texture basicWoodTex = null;

            try {
                basicWallTex = new Texture("img/wall-tex.jpg");
                basicWoodTex = new Texture("img/wood.jpg");
            } catch(SFML.LoadingFailedException lfe) {
                Console.WriteLine(lfe.Message);
                Console.WriteLine("Ignoring and replacing with GREEN");
            }

            wallTextures.Add(WallType.Basic, basicWallTex);
            wallTextures.Add(WallType.Wood, basicWoodTex);
        }

        public Texture GetTexture(WallType type) {
            return wallTextures[type];
        }
    }
}