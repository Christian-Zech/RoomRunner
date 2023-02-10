using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private Texture2D[] animation;
        private int frame;
        public Texture2D Texture => animation[frame];
        private int framesUntilChange;
        private readonly int framesBetween;

        public Player(Texture2D playerSheet, GraphicsDevice gd, int framesInbetween = 3, params Rectangle[] rects)
        {
            IsAlive = true;
            animation = RectToTxt(gd, playerSheet, rects);
            framesUntilChange = framesInbetween;
            framesBetween = framesInbetween;
            frame = 0;
        }

        public void Update()
        {
            if (framesUntilChange-- <= 0)
            {
                framesUntilChange = framesBetween;
                frame++;
                if (frame >= animation.Length) 
                    frame = 0;
            }
        }

        public static Texture2D[] RectToTxt(GraphicsDevice gd, Texture2D sheet, params Rectangle[] rects)
        {
            Texture2D[] txts = new Texture2D[rects.Length];
            Color[] pixels = new Color[sheet.Width * sheet.Height];
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
