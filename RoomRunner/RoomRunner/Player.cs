using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{
    public class Player
    {
        public bool IsAlive;
        public Vector2 Velocity, Position;
        private string currentAnimation;
        private readonly Dictionary<string, Texture2D[]> animations;
        private int frame;
        public Texture2D Texture => animations["idle"][frame];
        private readonly Dictionary<string, int> framesBetween, framesLeft;
        private KeyboardState oldkb;

        public Player(Vector2 pos) : this()
        {
            Position = pos;
        }
        public Player()
        {
            IsAlive = true;
            animations = new Dictionary<string, Texture2D[]>();
            framesBetween = new Dictionary<string, int>();
            framesLeft = new Dictionary<string, int>();
            frame = 0;
            oldkb = Keyboard.GetState();
            currentAnimation = "idle";
        }

        public void AddAnimation(string name, Texture2D sheet, GraphicsDevice gd, int framesInbetween = 3, params Rectangle[] rects)
        {
            animations[name] = RectToTxt(gd, sheet, rects);
            framesBetween[name] = framesInbetween;
            framesLeft[name] = framesInbetween;
        }
        public void Update()
        {
            KeyboardState kb = Keyboard.GetState();
            if (framesLeft[currentAnimation]-- <= 0)
            {
                framesLeft[currentAnimation] = framesBetween[currentAnimation];
                frame++;
                if (frame >= animations[currentAnimation].Length) 
                    frame = 0;
            }

            if (IsPressed(kb, Keys.W)) Velocity.Y = 2.0f;

            oldkb = kb;
        }
        private bool IsPressed(KeyboardState kb, Keys k) => kb.IsKeyDown(k);


        public static Texture2D[] RectToTxt(GraphicsDevice gd, Texture2D sheet, params Rectangle[] rects)
        {
            Texture2D[] txts = new Texture2D[rects.Length];
            Color[] pixels = new Color[sheet.Width * sheet.Height];
            sheet.GetData(pixels);
            int c = 0;
            foreach (Rectangle r in rects)
            {
                Color[] newPixels = new Color[r.Width * r.Height];
                for (int row = r.Y, newRow = 0; row < r.Y + r.Height; row++, newRow++)
                    for (int col = r.X, newCol = 0; col < r.X + r.Width; col++, newCol++)
                        newPixels[newRow * r.Width + newCol] = pixels[row * sheet.Width + col];
                txts[c] = new Texture2D(gd, r.Width, r.Height);
                txts[c].SetData(newPixels);
                c++;
            }
            return txts;
        }
    }
}
