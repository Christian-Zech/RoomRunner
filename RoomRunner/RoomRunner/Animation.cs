using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{
    public abstract class Animation
    {
        public readonly string[] AnimationNames;
        public readonly Dictionary<string, int> FramesLeft, TimeBetweenChanges;
        public readonly Dictionary<string, Texture2D[]> Animations;
        public bool Idle;
        private readonly Dictionary<string, bool> Repeat;
        public int Frame { get; private set; }
        public string SelectedAnimation { get; private set; }
        public Texture2D CurrentTexture => Animations[SelectedAnimation][Frame];
        public Texture2D LastUsedSheet;

        public Animation(string[] names, string selected)
        {
            SelectedAnimation = selected;
            AnimationNames = names;
            Frame = 0;
            TimeBetweenChanges = new Dictionary<string, int>();
            FramesLeft = new Dictionary<string, int>();
            Animations = new Dictionary<string, Texture2D[]>();
            Repeat = new Dictionary<string, bool>();
            Idle = false;
        }
        public Animation(string[] names) : this(names, names[0]) { }

        public void Update()
        {
            if (Idle) return;
            if (FramesLeft[SelectedAnimation] <= 0)
            {
                if (Frame + 1 >= Animations[SelectedAnimation].Length)
                {
                    if (!Repeat[SelectedAnimation]) return;
                    Frame = 0;
                }
                else Frame++;
                FramesLeft[SelectedAnimation] = TimeBetweenChanges[SelectedAnimation];
            }
            else FramesLeft[SelectedAnimation]--;
        }
        public void ChangeCurrentAnimation(string state)
        {
            Frame = 0;
            FramesLeft[SelectedAnimation] = TimeBetweenChanges[SelectedAnimation];
            SelectedAnimation = state;
        }
        public void SetState(string state) => ChangeCurrentAnimation(state);
        public void SetFrameDelay(string state, int newDelay) => TimeBetweenChanges[state] = newDelay;
        public void AddAnimation(string state, Texture2D sheet, GraphicsDevice gd, int framesInbetween = 5, params Rectangle[] rects)
        {
            LastUsedSheet = sheet;
            Animations[state] = RectToTxt(gd, sheet, rects);
            TimeBetweenChanges[state] = framesInbetween;
            FramesLeft[state] = framesInbetween;
            Repeat[state] = true;
        }
        public void AddAnimation(string state, Texture2D sheet, GraphicsDevice gd, int framesInbetween, Rectangle[] rects, bool repeat)
        {
            Animations[state] = RectToTxt(gd, sheet, rects);
            TimeBetweenChanges[state] = framesInbetween;
            FramesLeft[state] = framesInbetween;
            Repeat[state] = repeat;
        }
        public void AddAnimation(string state, int framesInbetween = 5, params Texture2D[] txts)
        {
            Animations[state] = txts;
            TimeBetweenChanges[state] = framesInbetween;
            FramesLeft[state] = framesInbetween;
            Repeat[state] = true;
        }
        public void AddAnimation(string state, int framesInbetween, Texture2D[] txts, bool repeat)
        {
            Animations[state] = txts;
            TimeBetweenChanges[state] = framesInbetween;
            FramesLeft[state] = framesInbetween;
            Repeat[state] = repeat;
        }

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
