﻿using Microsoft.Xna.Framework;
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
        public const float InitialJumpMovement = 40.0f; //px per frame
        public const float JumpMovement = 30.0f; //px per frame
        public const int InputDelay = 20; //Frames
        private const int FireDelay = 10; //Frames
        public const int frameHeight = 1000; //px

        private static readonly string[] statesstates = new string[] { "Idle", "Jumping", "Running" }; //NEVER USE THIS VARIABLE!!!!
        public static string[] States { get { return statesstates; } }
        public static readonly Dictionary<PlayerHats, Texture2D> Hats;
        public static float JumpMultiplier;
        public static float GravityMultiplier;

        public bool IsAlive;
        public Vector2 Velocity, Position, Acceleration;
        public Rectangle PlayerRectangle, HatRectangle;
        private bool wasStateSet, onGround;
        private KeyboardState oldkb;
        private MouseState oldms;
        public int Coins;
        public int delayLeft, fireCooldown;
        public static int ceilingHeight, floorHeight; //in px
        public PlayerHats currentHat;
        public List<int> ownedHats;
        private readonly Game1 game;
        
        static Player()
        {
            
            Hats = new Dictionary<PlayerHats, Texture2D>();
            ceilingHeight = frameHeight;
            floorHeight = 0;
            JumpMultiplier = GravityMultiplier = 1.0f;
        }
        public Player(Vector2 pos, Game1 game) : this(game)
        {
            Position = pos;
        }
        public Player(Game1 game) : base(States)
        {
            this.game = game;
            ownedHats = new List<int>();
            PlayerRectangle = new Rectangle((int)Position.X, (int)Position.Y, 150, 100);
            HatRectangle = new Rectangle(PlayerRectangle.X, PlayerRectangle.Y, 150, 100); //head is 13 x 12
            IsAlive = true;
            oldkb = Keyboard.GetState();
            oldms = Mouse.GetState();
            Acceleration.Y = Gravity * GravityMultiplier;
            wasStateSet = false;
            Idle = false;
            delayLeft = InputDelay;
            currentHat = PlayerHats.Bandana;
            Coins = 0;
            fireCooldown = 0;
            MakePlayerAnimations(game);
            MakePlayerHats(game);
        }

        private void MakePlayerHats(Game1 game)
        {
            Rectangle[] rects = LoadSheet(5, 5, 32, 32, 1);
            Texture2D sheet = game.cosmeticSheet;
            for (int i = 1, c = 0; i < rects.Length - 1; i += 2, c++)
                Hats[(PlayerHats)c + 1] = RectToTxt(game.graphics.GraphicsDevice, sheet, rects[i])[0];
        }
        private void MakePlayerAnimations(Game1 game)
        {
            Rectangle[] jebList = LoadSheet(4, 3, 32, 32);
            Texture2D jebSheet = game.jebSheet;
            GraphicsDevice graphics = game.graphics.GraphicsDevice;

            Rectangle[] idle = new Rectangle[] { jebList[10], jebList[11] };
            AddAnimation(States[0], jebSheet, graphics, 30, idle);
            Rectangle[] jumping = jebList.Skip(4).Take(7).ToArray();
            AddAnimation(States[1], jebSheet, graphics, 2, jumping, false);
            Rectangle[] running = jebList.Take(4).ToArray();
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
                if (onGround) Velocity.Y = InitialJumpMovement * JumpMultiplier;
                else Velocity.Y = JumpMovement * JumpMultiplier;
                SetState("Jumping");
                stateSet = true;
            }
            if (IsPressed(kb, Keys.S, Keys.Down))
                Velocity.Y = -JumpMovement * JumpMultiplier;

            Gravity:
            Velocity.Y += Acceleration.Y;
            Position.Y -= Velocity.Y;

            onGround = Position.Y > frameHeight - floorHeight - 100;

            if (onGround)
            {
                Position.Y = frameHeight - floorHeight - 100;
                Velocity.Y = 0;
                if (!wasStateSet) SetState("Running");
                stateSet = true;
            }
            if (Position.Y < frameHeight - ceilingHeight)
            {
                Position.Y = frameHeight - ceilingHeight;
                Velocity.Y = 0;
            }

            wasStateSet = stateSet;

            PlayerRectangle.Y = (int)Position.Y;
            PlayerRectangle.X = (int)Position.X;
            HatRectangle.X = PlayerRectangle.X;
            HatRectangle.Y = PlayerRectangle.Y;
            if (SelectedAnimation == "Running" && (Frame == 1 || Frame == 3)) 
                HatRectangle.Y -= 2;

            if (fireCooldown > 0) fireCooldown--;
            if (IsHeld(kb, Keys.D, Keys.LeftAlt, Keys.Right) && fireCooldown == 0)
            {
                Projectile toLaunch = Projectile.Defaults[Projectiles.PlayerShot].Clone();
                toLaunch.Position = new Point((int)Position.X, (int)Position.Y+50);
                game.projectileList.Add(toLaunch);
                fireCooldown = FireDelay;
            }
            
            
            
            base.Update();

            oldkb = kb;
            oldms = ms;
        }
        private bool IsPressed(KeyboardState kb, Keys k) { return kb.IsKeyDown(k) && !oldkb.IsKeyDown(k); }
        public bool IsPressed(KeyboardState kb, params Keys[] keys)
        {
            foreach (Keys k in keys) if (IsPressed(kb, k)) return true;
            return false;
        }
        private bool IsHeld(KeyboardState kb, Keys k) { return kb.IsKeyDown(k); }
        public bool IsHeld(KeyboardState kb, params Keys[] keys)
        {
            foreach (Keys k in keys) if (IsHeld(kb, k)) return true;
            return false;
        }
        public void Draw(SpriteBatch sb)
        {
            if (Idle) return;
            sb.Draw(CurrentTexture, PlayerRectangle, Color.White);
            sb.Draw(Hats[currentHat], HatRectangle, Color.White);
        }

        public static Rectangle[] LoadSheet(int width, int height, int Swidth, int Sheight, int extraWhitespace = 0)
        {
            int limit = width * height - extraWhitespace;
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
    public enum PlayerHats
    {
        None,
        Robber,
        Builder,
        Hair,
        Headphones,
        Santa,
        Bandana,
        Fire1,
        Fire2,
        Fire3,
        Military,
        RedHat,
        BlueHat
    }
}
