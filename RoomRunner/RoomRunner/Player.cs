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
    public class Player : Animation
    {
        public const float Gravity = -2.0f; //px per frame
        public const float JumpMovement = 40.0f; //px per frame
        private const int InputDelay = 60;

        private static readonly string[] statesstates = new string[] { "Idle", "Jumping", "Running" }; //NEVER USE THIS VARIABLE!!!!
        public static string[] States => statesstates;

        public bool IsAlive;
        public Vector2 Velocity, Position, Acceleration;
        public Rectangle PlayerRectangle;
        private bool wasStateSet, onGround;
        private KeyboardState oldkb;
        private MouseState oldms;
        public int Coins;
        private int delayLeft;
        public static int floorHeight; //in px
        
        public Player(Vector2 pos, ContentManager cm, GraphicsDevice graphics) : this(cm, graphics)
        {
            Position = pos;
        }
        public Player(ContentManager cm, GraphicsDevice graphics) : base(States)
        {
            PlayerRectangle = new Rectangle((int)Position.X, (int)Position.Y, 150, 100);
            IsAlive = true;
            oldkb = Keyboard.GetState();
            oldms = Mouse.GetState();
            Acceleration.Y = Gravity;
            wasStateSet = false;
            Idle = false;
            floorHeight = 0;
            delayLeft = InputDelay;
            Coins = 0;
            MakePlayerAnimations(cm, graphics);
        }

        private void MakePlayerAnimations(ContentManager cm, GraphicsDevice graphics)
        {
            Rectangle[] jebList = LoadSheet(4, 3, 32, 32);
            Texture2D jebSheet = cm.Load<Texture2D>("jeb (2)");

            Rectangle[] idle = new Rectangle[] { jebList[10], jebList[11] };
            AddAnimation(States[0], jebSheet, graphics, 30, idle);
            Rectangle[] jumping = new Rectangle[] { jebList[4], jebList[5], jebList[6], jebList[7], jebList[8], jebList[9], jebList[10] };
            AddAnimation(States[1], jebSheet, graphics, 2, jumping, false);
            Rectangle[] running = new Rectangle[] { jebList[0], jebList[1], jebList[2], jebList[3] };
            AddAnimation(States[2], jebSheet, graphics, 5, running);
        }
        public new void Update()
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState ms = Mouse.GetState();
            bool stateSet = false;
            if (delayLeft > 0)
            {
                delayLeft--;
                goto Gravity;
            }

            if (IsPressed(kb, Keys.W, Keys.Up, Keys.Space) || ms.LeftButton == ButtonState.Pressed && oldms.LeftButton != ButtonState.Pressed)
            {
                if (onGround) Velocity.Y = JumpMovement;
                else Velocity.Y = JumpMovement / 2;
                SetState("Jumping");
                stateSet = true;
            }
            if (IsPressed(kb, Keys.S, Keys.Down))
                Velocity.Y = -JumpMovement;

            Gravity:
            Velocity.Y += Acceleration.Y;
            Position.Y -= Velocity.Y;

            onGround = Position.Y > 900;

            if (Position.Y > 900)
            {
                Position.Y = 900;
                Velocity.Y = 0;
                if (!wasStateSet) SetState("Running");
                stateSet = true;
            }
            if (Position.Y < floorHeight)
            {
                Position.Y = floorHeight;
                Velocity.Y = 0;
            }

            wasStateSet = stateSet;

            PlayerRectangle.Y = (int)Position.Y;
            PlayerRectangle.X = (int)Position.X;

            base.Update();

            oldkb = kb;
            oldms = ms;
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
            if (!Idle) sb.Draw(CurrentTexture, PlayerRectangle, Color.White);
        }

        public static Rectangle[] LoadSheet(int width, int height, int Swidth, int Sheight, int limit = -1)
        {
            if (limit <= 0) 
                limit = width * height;
            Rectangle[] outp = new Rectangle[limit];
            for (int y = 0, i = 0, c = 0; i < height; i++, y += Sheight)
                for (int x = 0, ii = 0; ii < width; ii++, x += Swidth, c++)
                {
                    outp[c] = new Rectangle(x, y, Swidth, Sheight);
                    if (c + 1 >= limit) 
                        return outp;
                }
            return outp;
        }
    }
}
