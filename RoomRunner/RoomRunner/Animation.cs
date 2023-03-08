using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{
    public class Animation
    {
        public readonly string[] AnimationNames;
        public readonly Dictionary<string, int> FramesLeft, TimeBetweenChanges;
        public readonly Dictionary<string, Texture2D[]> Animations;
        public bool Idle;
        private readonly Dictionary<string, bool> Repeat;
        public int Frame { get; private set; }
        public string SelectedAnimation { get; private set; }
        public Texture2D CurrentTexture { get { return Animations[SelectedAnimation][Frame]; } }
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
        public Animation(Animation anim)
        {
            SelectedAnimation = anim.SelectedAnimation;
            AnimationNames = (string[])anim.AnimationNames.Clone();
            Frame = anim.Frame;
            TimeBetweenChanges = new Dictionary<string, int>(anim.TimeBetweenChanges);
            FramesLeft = new Dictionary<string, int>(anim.FramesLeft);
            Animations = new Dictionary<string, Texture2D[]>(anim.Animations);
            Repeat = new Dictionary<string, bool>(anim.Repeat);
            Idle = anim.Idle;
        }
        public Animation(string[] names) : this(names, names[0]) { }
        public Animation(string name) : this(new string[] { name }, name) { }

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
        public void SetState(string state) { ChangeCurrentAnimation(state); }
        public void SetFrameDelay(string state, int newDelay) { TimeBetweenChanges[state] = newDelay; }
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

        public Animation Clone() { return new Animation(this); }

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
    public class OnetimeAnimation : Animation
    {
        public new int FramesLeft;
        public static Dictionary<OnetimeAnims, OnetimeAnimation> Anims;
        public bool Delete { get { return FramesLeft <= 0; } }
        public Animation Next;
        public new Texture2D CurrentTexture
        {
            get
            {
                if (Delete && Next != default) 
                    return Next.CurrentTexture;
                Update();
                FramesLeft--;
                return base.CurrentTexture;
            }
        }

        static OnetimeAnimation()
        {
            Anims = new Dictionary<OnetimeAnims, OnetimeAnimation>();
            Game1 game = Program.Game;
            Rectangle[] rects;
            Texture2D sheet;

            rects = Player.LoadSheet(2, 3, 32, 32);
            sheet = game.Content.Load<Texture2D>("Projectile/Fireball");
            Anims[OnetimeAnims.Fireball] = new OnetimeAnimation(20, game.GraphicsDevice, sheet, rects)
            {
                Next = new Animation("thing")
            };
            Anims[OnetimeAnims.Fireball].Next.AddAnimation("thing", sheet, game.GraphicsDevice, 15, rects[4], rects[5]);
        }
        public OnetimeAnimation(int framesPerFrame, params Texture2D[] frames) : base("thing")
        {
            AddAnimation("thing", framesPerFrame, frames);
            FramesLeft = frames.Length * framesPerFrame;
        }
        public OnetimeAnimation(int framesPerFrame, GraphicsDevice gd, Texture2D sheet, params Rectangle[] frames) : base("thing")
        {
            AddAnimation("thing", sheet, gd, framesPerFrame, frames);
            FramesLeft = frames.Length * framesPerFrame;
        }
        private OnetimeAnimation(Animation copy, OnetimeAnimation otherCopy, int framesLeft) : base(copy)
        {
            FramesLeft = framesLeft;
            if (otherCopy.Next != default) Next = otherCopy.Next.Clone();
        }

        public new OnetimeAnimation Clone()
        {
            return new OnetimeAnimation(base.Clone(), this, FramesLeft);
        }
    }
    public enum OnetimeAnims
    {
        Fireball
    }
}
