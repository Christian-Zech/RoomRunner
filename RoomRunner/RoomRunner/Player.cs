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

        private static readonly string[] statesstates = new string[] { "Idle", "Jumping", "Running" }; //NEVER USE THIS VARIABLE!!!!
        public static string[] States => statesstates;

        public bool IsAlive;
        public Vector2 Velocity, Position, Acceleration;
        public Rectangle PlayerRectangle;
        private bool wasStateSet, onGround;
        private KeyboardState oldkb;
        private MouseState oldms;
        public int Coins;
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
            Coins = 0;
            MakePlayerAnimations(cm, graphics);
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
            AddAnimation(States[0], jebSheet, graphics, 30, idle);
            AddAnimation(States[1], jebSheet, graphics, 30, idle);
            Rectangle[] running = new Rectangle[] { jebList[0], jebList[1], jebList[2] };
            AddAnimation(States[2], jebSheet, graphics, 5, running);
        }
        public new void Update()
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState ms = Mouse.GetState();
            bool stateSet = false;

            if (IsPressed(kb, Keys.W, Keys.Up, Keys.Space) || ms.LeftButton == ButtonState.Pressed && oldms.LeftButton != ButtonState.Pressed)
            {
                if (onGround) Velocity.Y = JumpMovement;
                else Velocity.Y = JumpMovement / 2;
                SetState("Jumping");
                stateSet = true;
            }
            if (IsPressed(kb, Keys.S, Keys.Down))
                Velocity.Y = -JumpMovement;

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

    }
}
