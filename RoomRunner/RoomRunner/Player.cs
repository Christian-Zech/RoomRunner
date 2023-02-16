using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        public const float Gravity = -2.0f; //px per frame
        public const float JumpMovement = 40.0f; //px per frame

        public bool IsAlive;
        public Vector2 Velocity, Position, Acceleration;
        public Rectangle PlayerRectangle;
        private PlayerState currentAnimation;
        private readonly Dictionary<PlayerState, Texture2D[]> animations;
        private int frame;
        private bool wasStateSet;
        public Texture2D Texture => animations[currentAnimation][frame];
        private readonly Dictionary<PlayerState, int> framesBetween, framesLeft;
        private KeyboardState oldkb;

        public Player(Vector2 pos, ContentManager cm, GraphicsDevice graphics) : this(cm, graphics)
        {
            Position = pos;
        }
        public Player(ContentManager cm, GraphicsDevice graphics)
        {
            PlayerRectangle = new Rectangle((int)Position.X, (int)Position.Y, 150, 100);
            IsAlive = true;
            animations = new Dictionary<PlayerState, Texture2D[]>();
            framesBetween = new Dictionary<PlayerState, int>();
            framesLeft = new Dictionary<PlayerState, int>();
            frame = 0;
            oldkb = Keyboard.GetState();
            currentAnimation = PlayerState.Idle;
            Acceleration.Y = Gravity;
            wasStateSet = false;
            MakePlayerAnimations(cm, graphics);
        }

        public void AddAnimation(PlayerState state, Texture2D sheet, GraphicsDevice gd, int framesInbetween = 3, params Rectangle[] rects)
        {
            animations[state] = RectToTxt(gd, sheet, rects);
            framesBetween[state] = framesInbetween;
            framesLeft[state] = framesInbetween;
        }
        private void MakePlayerAnimations(ContentManager cm, GraphicsDevice graphics)
        {
            List<Rectangle> jebList = new List<Rectangle>();
            Texture2D jebSheet = cm.Load<Texture2D>("jeb");

            jebList.Add(new Rectangle(0, 0, 32, 32));
            jebList.Add(new Rectangle(32, 0, 32, 32));
            jebList.Add(new Rectangle(0, 32, 32, 32));
            jebList.Add(new Rectangle(32, 32, 32, 32));
            jebList.Add(new Rectangle(0, 64, 32, 32));

            Rectangle[] idle = new Rectangle[] { jebList[3], jebList[4] };
            AddAnimation(PlayerState.Idle, jebSheet, graphics, 30, idle);
            AddAnimation(PlayerState.Jumping, jebSheet, graphics, 30, idle);
            Rectangle[] running = new Rectangle[] { jebList[0], jebList[1], jebList[2] };
            AddAnimation(PlayerState.Running, jebSheet, graphics, 5, running);
        }
        public void Update()
        {
            KeyboardState kb = Keyboard.GetState();
            bool stateSet = false;

            if (IsPressed(kb, Keys.W, Keys.Up, Keys.Space))
            {
                Velocity.Y = JumpMovement;
                SetState(PlayerState.Jumping);
                stateSet = true;
            }
            if (IsPressed(kb, Keys.S, Keys.Down))
                Velocity.Y = -JumpMovement;

            Velocity.Y += Acceleration.Y;
            Position.Y -= Velocity.Y;

            if (Position.Y > 900)
            {
                Position.Y = 900;
                Velocity.Y = 0;
                if (!wasStateSet) SetState(PlayerState.Running);
                stateSet = true;
            }
            if (Position.Y < 0)
            {
                Position.Y = 0;
                Velocity.Y = 0;
            }

            wasStateSet = stateSet;

            PlayerRectangle.Y = (int)Position.Y;
            PlayerRectangle.X = (int)Position.X;

            if (framesLeft[currentAnimation]-- <= 0)
            {
                framesLeft[currentAnimation] = framesBetween[currentAnimation];
                frame++;
                if (frame >= animations[currentAnimation].Length) 
                    frame = 0;
            }

            oldkb = kb;
        }
        private void SetState(PlayerState state)
        {
            wasStateSet = true;
            framesLeft[currentAnimation] = framesBetween[currentAnimation];
            frame = 0;
            currentAnimation = state;
        }
        private bool IsPressed(KeyboardState kb, Keys k) => kb.IsKeyDown(k) && !oldkb.IsKeyDown(k);
        public bool IsPressed(KeyboardState kb, params Keys[] keys)
        {
            foreach (Keys k in keys) if (IsPressed(kb, k)) return true;
            return false;
        }
        private bool IsHeld(KeyboardState kb, Keys k) => kb.IsKeyDown(k);
        public bool IsHeld(KeyboardState kb, params Keys[] keys)
        {
            foreach (Keys k in keys) if (IsHeld(kb, k)) return true;
            return false;
        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture, PlayerRectangle, Color.White);
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
    public enum PlayerState
    {
        Idle, Jumping, Running
    }
}
