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
        public const float InitialJumpMovement = 40.0f; //px per frame
        public const float JumpMovement = 30.0f; //px per frame
        public const int InputDelay = 20; //In Frames
        private const int FireDelay = 10; //In Frames
        public const int FrameBetweenFlash = 5; //In Frames
        public const int MaxInvinciblity = 120; //In Frames
        public const int frameHeight = 1000; //px

        private static readonly string[] statesstates = new string[] { "Idle", "Jumping", "Running" }; //NEVER USE THIS VARIABLE!!!!
        public static string[] States { get { return statesstates; } }
        public static readonly Dictionary<PlayerHats, Texture2D> Hats;
        public static float JumpMultiplier;
        public static float GravityMultiplier;
        public static int MaxHealth 
        { 
            get 
            { 
                switch (Game1.difficulty) 
                {
                    case Game1.Difficulty.Easy:
                        return 5;
                    case Game1.Difficulty.Hard:
                        return 1;
                    default:
                        return 3;
                }
            } 
        }

        public bool IsAlive, Shown, Invulnerable;
        public Vector2 Velocity, Position, Acceleration;
        public Rectangle PlayerRectangle, HatRectangle;
        private bool wasStateSet, onGround;
        private KeyboardState oldkb;
        public List<Keys> Up, Down, Left, Shoot;
        private MouseState oldms;
        public int Coins, Health, distanceTraveled, distanceHighScore; //save these
        public int delayLeft, fireCooldown, InvinciblityTimer, FlashTimer;
        public static int ceilingHeight, floorHeight; //in px
        public PlayerHats currentHat; //save this
        public List<int> ownedHats; //save this
        public static Texture2D Heart;
        public static int players;
        public readonly int id;
        private int FireTimer;

        static Player()
        {

            Hats = new Dictionary<PlayerHats, Texture2D>();
            ceilingHeight = frameHeight;
            floorHeight = 0;
            JumpMultiplier = GravityMultiplier = 1.0f;
            Heart = Program.Game.Content.Load<Texture2D>("Heart");
            players = 0;
        }
        public Player(Vector2 pos) : this()
        {
            Position = pos;
        }
        public Player() : base(States)
        {
            id = players++;
            ownedHats = new List<int>();
            PlayerRectangle = new Rectangle((int)Position.X, (int)Position.Y, 150, 100);
            HatRectangle = new Rectangle(PlayerRectangle.X, PlayerRectangle.Y, 150, 100); //head is 13 x 12
            IsAlive = true;
            Shown = true;
            Health = MaxHealth;
            oldkb = Keyboard.GetState();
            oldms = Mouse.GetState();
            Acceleration.Y = Gravity * GravityMultiplier;
            wasStateSet = false;
            Idle = false;
            Invulnerable = false;
            Up = new List<Keys> { Keys.Up, Keys.W, Keys.Space };
            Down = new List<Keys> { Keys.Down, Keys.S };
            Left = new List<Keys> { Keys.Left, Keys.A };
            Shoot = new List<Keys> { Keys.Right, Keys.D };
            delayLeft = InputDelay;
            InvinciblityTimer = MaxInvinciblity;
            FlashTimer = 0;
            FireTimer = 10;
            currentHat = PlayerHats.None;
            Coins = 1000;
            distanceTraveled = 0;
            distanceHighScore = 0;
            fireCooldown = 0;
            MakePlayerAnimations();
            MakePlayerHats();
            Load();
        }

        private void MakePlayerHats()
        {
            Game1 game = Program.Game;
            Rectangle[] rects = LoadSheet(5, 5, 32, 32, 1);
            Texture2D sheet = game.cosmeticSheet;
            for (int i = 1, c = 0; i < rects.Length; i += 2, c++)
                Hats[(PlayerHats)c + 1] = RectToTxt(game.graphics.GraphicsDevice, sheet, rects[i])[0];
        }
        private void MakePlayerAnimations()
        {
            Game1 game = Program.Game;
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
            if (!IsAlive) return;
            KeyboardState kb = Keyboard.GetState();
            MouseState ms = Mouse.GetState();
            bool stateSet = false;
            if (delayLeft > 0)
            {
                delayLeft--;
                goto Gravity;
            }

            if (IsPressed(kb, Up.ToArray()))
            {
                if (onGround) Velocity.Y = InitialJumpMovement * JumpMultiplier;
                else Velocity.Y = JumpMovement * JumpMultiplier;
                SetState("Jumping");
                stateSet = true;
            }
            if (IsPressed(kb, Down.ToArray()))
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
            if (IsHeld(kb, Shoot.ToArray()) && fireCooldown == 0)
            {
                Projectile toLaunch = Projectile.Defaults[Projectiles.PlayerShot].Clone();
                toLaunch.Position = new Point((int)Position.X, (int)Position.Y + 50);
                Program.Game.projectileList.Add(toLaunch);
                fireCooldown = FireDelay;
            }

            if (InvinciblityTimer > 0)
            {
                InvinciblityTimer--;
                if (InvinciblityTimer == 0)
                {
                    FlashTimer = 0;
                    Shown = true;
                }
                if (FlashTimer > 0)
                    FlashTimer--;
            }
            if (FlashTimer == 0 && InvinciblityTimer > 0)
            {
                Shown = !Shown;
                FlashTimer = FrameBetweenFlash;
            }
            if (distanceTraveled > distanceHighScore)
                distanceHighScore = distanceTraveled;


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
            if (!Idle && Shown && IsAlive)
            {
                sb.Draw(CurrentTexture, PlayerRectangle, Color.White);
                if (currentHat != PlayerHats.None)
                {
                    if (currentHat - 7 >= 0 && (int)(currentHat - 7) <= 2)
                    {
                        if (FireTimer == 0)
                        {
                            currentHat++;
                            if (currentHat > PlayerHats.Fire3)
                                currentHat = PlayerHats.Fire1;
                            FireTimer = 10;
                        }
                        else FireTimer--;
                    }
                    sb.Draw(Hats[currentHat], HatRectangle, Color.White);
                }
            }
            if (Idle) return;

            const int shift = 75;
            Color col = new Color(255 - shift * id, 255, 255);
            for (int i = 0, x = 180 * id; i < Health; i++, x += 55)
                sb.Draw(Heart, new Rectangle(x, 0, 50, 50), col);
        }
        public void Damage()
        {
            if (InvinciblityTimer > 0 || Invulnerable) return;
            Health--;
            if (Health <= 0)
                IsAlive = false;
            InvinciblityTimer = MaxInvinciblity;
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
        public void Save()
        {
            string hats = "";
            for (int i = 0; i < ownedHats.Count; i++)
                hats += ownedHats[i] + " ";

            string str = Coins + "\n" + distanceHighScore + "\n" + currentHat + "\n" + hats;
            SaveAndLoad.Save(str, "playerData.txt");
        }
        public void Load()
        {
            string data = SaveAndLoad.Load("playerData.txt");
            if (data.Equals(""))
                return;
            string[] lines = data.Split('\n');
            Coins = int.Parse(lines[0]);
            distanceHighScore = int.Parse(lines[1]);
            currentHat = (PlayerHats)Enum.Parse(typeof(PlayerHats), lines[2]);
            string[] hats = lines[3].Split(' ');
            if (!hats[0].Equals(""))
            {
                for (int i = 0; i < hats.Length; i++)
                    if (!hats[i].Equals("\r"))
                        ownedHats.Add(int.Parse(hats[i]));
            }

        }
        public void ClearSave()
        {
            Coins = 1000;
            distanceHighScore = 0;
            currentHat = PlayerHats.None;
            ownedHats.Clear();
            //Save();
        }
    }
    public enum PlayerHats
    {
        None,
        Ski_Mask,
        Construction,
        Hair,
        Headphones,
        Santa_Hat,
        Headband,
        Fire1,
        Fire2,
        Fire3,
        Army_Hat,
        Red_Bandana,
        Blue_Bandana
    }
}

