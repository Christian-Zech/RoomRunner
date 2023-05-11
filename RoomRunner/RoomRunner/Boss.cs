using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{
    public class Boss : Animation
    {
        private const int Insets = 20; //in px
        public const int WarningTime = 120; //in frames
        public const int TimeBetweenPatterns = WarningTime + 50; //in frames

        public static double SpeedMultiplier { get { return 2 * Program.Game.DifficultyMultiplier; } }

        public int TimeBeforeNextPattern, TimeLeftInPattern;
        public bool DoingPattern;
        public BossPattern CurrentPattern;
        public static Dictionary<BossPattern, int> PatternTimes;

        private Rectangle rect, bossBarRect, drawRect;
        private readonly Rectangle originRect;
        public Rectangle Rectangle { get { return rect; } }
        public Point Position { get { return new Point(rect.X, rect.Y); } }
        public int Health
        {
            get
            {
                return health;
            }
            set
            {
                health = value;
                if (health <= 0)
                {
                    IsDead = true;
                    return;
                }
                BossBarPercent = health / (float)maxHealth;
                bossBarRect.Width = (int)((1900.0f - Insets * 2) * BossBarPercent);
            }
        }
        public int MaxHealth
        {
            get
            {
                return maxHealth;
            }
            set
            {
                maxHealth = value;
            }
        }
        private int health;
        private int maxHealth;
        private int timer1, warningTime, whiteSpace;
        private Rectangle warningRect;
        private bool showWarning;
        private static Texture2D warning;
        private float BossBarPercent;
        public bool IsDead, FlipProjX, FlipProjY;
        private bool IsDown;
        public Vector2 Velocity;
        public readonly Bosses Name;
        private GraphicsDevice graphics;
        public List<ProjectileClump> projList, projBuffer;

        static Boss()
        {
            PatternTimes = new Dictionary<BossPattern, int>
            {
                [BossPattern.Attack] = 180,
                [BossPattern.Pound] = 300,
                [BossPattern.BigPound_Bottom] = 300,
                [BossPattern.BigPound_Top] = 300,
                [BossPattern.Move] = 60,
                [BossPattern.MoveForward] = 140
            };
            warning = Program.Game.Content.Load<Texture2D>("warning");
        }
        public Boss(Bosses boss, int health, Texture2D sheet, GraphicsDevice gd) : base(new string[] { "Idle" })
        {
            Name = boss;
            IsDown = true;
            Velocity = Vector2.Zero;
            TimeBeforeNextPattern = (int)(TimeBetweenPatterns / SpeedMultiplier);
            TimeLeftInPattern = 0;
            DoingPattern = false;
            projList = new List<ProjectileClump>();
            projBuffer = new List<ProjectileClump>();
            graphics = gd;
            rect = new Rectangle(1500,500,200,200);
            whiteSpace = (int)Math.Round(rect.Width % 52 * rect.Width / 52.0);
            drawRect = new Rectangle(rect.X - whiteSpace / 2, rect.Y - whiteSpace / 2, rect.Width + whiteSpace, rect.Height + whiteSpace);
            originRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            MakeAnimation(boss, sheet, gd);
            maxHealth = this.health = health;
            BossBarPercent = 1.0f;
            IsDead = false;
            showWarning = false;
            warningTime = 0;
            FlipProjX = FlipProjY = false;
            bossBarRect = new Rectangle(Insets, 900, 1900 - Insets * 2, 50);
        }

        public new void Update()
        {
            base.Update();
            if (TimeLeftInPattern > 0)
            {
                TimeLeftInPattern--;
                if (TimeLeftInPattern == 0)
                {
                    FinishPattern();
                    CurrentPattern = (BossPattern)Program.Game.rand.Next(0, 6);
                    InitWarning();
                }
            }
            else if (TimeBeforeNextPattern-- <= 0)
            {
                TimeBeforeNextPattern = (int)(TimeBetweenPatterns / SpeedMultiplier);
                DoingPattern = true;
                TimeLeftInPattern = (int)(PatternTimes[CurrentPattern] / SpeedMultiplier);
                InitPattern();
            }
            if (DoingPattern)
                UpdatePattern();
            if (warningTime > 0)
                UpdateWarning();

            rect.X += (int)Velocity.X;
            rect.Y -= (int)Velocity.Y;
            drawRect.X = rect.X - whiteSpace / 2;
            drawRect.Y = rect.Y - whiteSpace / 2;
        }
        private void InitWarning()
        {
            warningTime = (int)(WarningTime / SpeedMultiplier);
            showWarning = true;
            switch (CurrentPattern)
            {
                case BossPattern.Attack:
                    warningRect = new Rectangle(Game1.window.Width * 3 / 4, 0, 150, Game1.window.Height);
                    break;
                case BossPattern.BigPound_Bottom:
                    warningRect = new Rectangle(0, Game1.window.Height - 150 - Player.floorHeight, Game1.window.Width, 150);
                    break;
                case BossPattern.BigPound_Top:
                    warningRect = new Rectangle(0, 0, Game1.window.Width, 150);
                    break;
                case BossPattern.Move:
                    warningRect = new Rectangle(0,0,0,0);
                    break;
                case BossPattern.MoveForward:
                    warningRect = new Rectangle(0, rect.Y - 50, Game1.window.Width, rect.Height + 100);
                    break;
                case BossPattern.Pound:
                    warningRect = new Rectangle(0, Game1.window.Height - 50 - Player.floorHeight, Game1.window.Width, 50);
                    break;
            }

        }
        private void UpdateWarning()
        {
            --warningTime;
            if (warningTime % (int)(40 / SpeedMultiplier) == 0) 
                showWarning = !showWarning;
        }
        private void FinishPattern()
        {
            DoingPattern = false;
            switch (CurrentPattern)
            {
                case BossPattern.Attack:
                    break;
                case BossPattern.Pound:
                    break;
                case BossPattern.BigPound_Bottom:
                    break;
                case BossPattern.BigPound_Top:
                    break;
                case BossPattern.Move:
                    Velocity = Vector2.Zero;
                    break;
                case BossPattern.MoveForward:
                    Velocity = Vector2.Zero;
                    rect.X = originRect.X;
                    break;
            }
            projBuffer.Clear();
            projList.Clear();
            SetState("Idle");
            CurrentPattern = default;
        }
        private void InitPattern()
        {
            int numOfProjs, floor, ceiling, frames;
            List<int> availablePoints;
            Rectangle[] rects;
            Texture2D sheet;
            switch (CurrentPattern)
            {
                case BossPattern.Attack:
                    timer1 = 0;
                    FlipProjX = true;
                    FlipProjY = false;
                    SetState("Attack");
                    numOfProjs = 3;
                    availablePoints = new List<int>();
                    for (int i = 10; i < Player.frameHeight - Player.floorHeight - 10; i++)
                        availablePoints.Add(i);

                    for (int i = 0; i < numOfProjs && availablePoints.Count > 0; i++)
                    {
                        int rdmNum = availablePoints.ElementAt(Program.Game.rand.Next(0, availablePoints.Count));
                        for (int ii = rdmNum - 100; ii <= rdmNum + 100; ii++)
                            availablePoints.Remove(ii);
                        projBuffer.Add(new ProjectileClump(FlipProjX, FlipProjY, new Projectile(new Rectangle(1500, rdmNum, 100, 100), 0, new Point((int)(-12 * SpeedMultiplier), 0), OnetimeAnimation.Anims[OnetimeAnims.Boss_Fireball].Clone(), false, true)));
                        Program.Game.rand.Next(DateTime.UtcNow.Millisecond);
                    }
                    break;
                case BossPattern.Pound:
                    timer1 = 0;
                    FlipProjX = false;
                    FlipProjY = false;
                    SetState("Pound");
                    floor = Game1.window.Height - Player.floorHeight;
                    sheet = Program.Game.Content.Load<Texture2D>("Level1/Enemies/Obstacles");
                    rects = Player.LoadSheet(3, 3, 32, 32, 1);
                    frames = (int)(6 / SpeedMultiplier);
                    for (int x = Game1.window.Width; x > 0; x -= 100)
                    {
                        List<Projectile> toClump = new List<Projectile>
                        {
                            new Projectile(true, new Rectangle(x, floor - 30, 100, 30), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(true, new Rectangle(x, floor - 60, 100, 60), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(true, new Rectangle(x, floor - 90, 100, 90), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(true, new Rectangle(x, floor - 60, 100, 60), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(true, new Rectangle(x, floor - 30, 100, 30), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true)
                        };
                        projBuffer.Add(new ProjectileClump(FlipProjX, FlipProjY, toClump.ToArray()));
                    }
                    break;
                case BossPattern.BigPound_Bottom:
                    FlipProjX = false;
                    FlipProjY = false;
                    SetState("Pound");
                    floor = Game1.window.Height - Player.floorHeight;
                    sheet = Program.Game.Content.Load<Texture2D>("Level1/Enemies/Obstacles");
                    rects = Player.LoadSheet(3, 3, 32, 32, 1);
                    frames = (int)(12 / SpeedMultiplier);
                    for (int x = Game1.window.Width; x >= 0; x -= 100)
                    {
                        List<Projectile> toClump = new List<Projectile>
                        {
                            new Projectile(true, new Rectangle(x, floor - 60, 100, 60), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(true, new Rectangle(x, floor - 120, 100, 120), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(true, new Rectangle(x, floor - 180, 100, 180), 0, frames * 10, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(true, new Rectangle(x, floor - 120, 100, 120), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(true, new Rectangle(x, floor - 60, 100, 60), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true)
                        };
                        projList.Add(new ProjectileClump(FlipProjX, FlipProjY, toClump.ToArray()));
                    }
                    break;
                case BossPattern.BigPound_Top:
                    FlipProjX = false;
                    FlipProjY = true;
                    SetState("PoundUp");
                    ceiling = Game1.window.Height - Player.ceilingHeight;
                    sheet = Program.Game.Content.Load<Texture2D>("Level1/Enemies/Obstacles");
                    rects = Player.LoadSheet(3, 3, 32, 32, 1);
                    frames = (int)(12 / SpeedMultiplier);
                    for (int x = Game1.window.Width; x >= 0; x -= 100)
                    {
                        List<Projectile> toClump = new List<Projectile>
                        {
                            new Projectile(true, new Rectangle(x, ceiling, 100, 60), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(true, new Rectangle(x, ceiling, 100, 120), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(true, new Rectangle(x, ceiling, 100, 180), 0, frames * 10, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(true, new Rectangle(x, ceiling, 100, 120), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(true, new Rectangle(x, ceiling, 100, 60), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true)
                        };
                        projList.Add(new ProjectileClump(FlipProjX, FlipProjY, toClump.ToArray()));
                    }
                    break;
                case BossPattern.Move:
                    IsDown = !IsDown;
                    timer1 = 0;
                    break;
                case BossPattern.MoveForward:
                    timer1 = 0;
                    SetState("MoveForward");
                    FlipProjX = false;
                    FlipProjY = false;
                    projList.Add(new ProjectileClump(FlipProjX, FlipProjY, new Projectile(true, () => rect, 0, 140, default, false, true)));
                    break;
            }
        }
        private void UpdatePattern()
        {
            switch (CurrentPattern)
            {
                case BossPattern.Attack:
                    if (timer1 > 0) timer1--;
                    else
                    {
                        timer1 = (int)(30 / SpeedMultiplier);
                        for (int i = 0; i < projBuffer.Count; i++)
                            if (projBuffer[i] != null)
                            {
                                projList.Add(projBuffer[i]);
                                projBuffer[i] = null;
                                break;
                            }
                    }
                    break;
                case BossPattern.Pound:
                    if (timer1 > 0) timer1--;
                    else
                    {
                        timer1 = (int)(10 / SpeedMultiplier);
                        for (int i = 0; i < projBuffer.Count; i++)
                            if (projBuffer[i] != null)
                            {
                                projList.Add(projBuffer[i]);
                                projBuffer[i] = null;
                                break;
                            }
                    }
                    break;
                case BossPattern.BigPound_Bottom:
                    break;
                case BossPattern.Move:
                    timer1++;
                    if (timer1 <= Math.Round(30 / (SpeedMultiplier / 2.0))) if (IsDown)
                            Velocity.Y -= 5.0f * ((float)SpeedMultiplier / 4.0f) / timer1;
                        else
                            Velocity.Y += 5.0f * ((float)SpeedMultiplier / 4.0f) / timer1;
                    else if (IsDown)
                            Velocity.Y += 10.0f * ((float)SpeedMultiplier / 4.0f) / (int)Math.Round(30 / (SpeedMultiplier / 2.0) - timer1);
                        else
                            Velocity.Y -= 10.0f * ((float)SpeedMultiplier / 4.0f) / (int)Math.Round(30 / (SpeedMultiplier / 2.0) - timer1);

                    break;
                case BossPattern.MoveForward:
                    timer1++;
                    if (timer1 <= Math.Round(60 / (SpeedMultiplier / 2.0)))
                        Velocity.X -= (float)SpeedMultiplier / 2.0f;
                    else
                    {
                        if (timer1 == Math.Round(60 / (SpeedMultiplier / 2.0)) + 1)
                            rect.X = (int)Math.Round(2000 / (SpeedMultiplier / 2.0));
                        Velocity.X += 1.5f * (float)SpeedMultiplier / 2.0f;
                    }
                    break;
            }
        }
        public void Draw(SpriteBatch sb)
        {
            if (IsDead) return;
            sb.Draw(CurrentTexture, drawRect, Color.White);

            sb.Draw(Game1.pixel, bossBarRect, Color.Red);

            foreach (ProjectileClump p in projList)
            {
                p.DrawAndUpdate(sb);
            }
            if (showWarning) 
                sb.Draw(warning, warningRect, new Color(255, 0, 0, 20));
        }
        public new Boss Clone() { return new Boss(Name, health, LastUsedSheet, graphics); }

        public static Rectangle Clone(Rectangle r) { return new Rectangle(r.X, r.Y, r.Width, r.Height); }

        public void Damage(int amount) { Health -= amount; }
        private void MakeAnimation(Bosses boss, Texture2D sheet, GraphicsDevice gd)
        {
            Rectangle[] rects = Player.LoadSheet(9, 9, 52, 52);
            switch (boss)
            {
                case Bosses.Demon:
                    AddAnimation("Attack", sheet, gd, 5, rects[18], rects[11], rects[20], rects[19]);
                    AddAnimation("MoveForward", sheet, gd, 10, rects[21], rects[27]);
                    AddAnimation("Idle", sheet, gd, 25, rects[10], rects[2]);
                    AddAnimation("Pound", sheet, gd, 10, rects[29], rects[28]);
                    AddAnimation("PoundUp", sheet, gd, 10, rects[12], rects[3]);
                    break;
                case Bosses.Yeti:
                    AddAnimation("Attack", sheet, gd, 5, rects[61], rects[52]);
                    AddAnimation("MoveForward", sheet, gd, 10, rects.Skip(57).Take(4).ToArray());
                    AddAnimation("Idle", sheet, gd, 15, rects.Skip(57).Take(4).ToArray());
                    AddAnimation("Pound", sheet, gd, 5, rects[7], rects[63], rects[7], rects[16], rects[25], rects[34]);
                    AddAnimation("PoundUp", sheet, gd, 10, rects[7], rects[63], rects[7], rects[16]);
                    break;
                case Bosses.Bat:
                    AddAnimation("Attack", sheet, gd, 5, rects.Skip(73).Take(8).ToArray());
                    AddAnimation("MoveForward", sheet, gd, 5, rects.Skip(64).Take(4).ToArray());
                    AddAnimation("Idle", sheet, gd, 15, rects[8], rects[68], rects[69], rects[70]);
                    AddAnimation("Pound", sheet, gd, 10, rects[17], rects[26], rects[35], rects[44]);
                    AddAnimation("PoundUp", sheet, gd, 10, rects[53], rects[62], rects[71], rects[72]);
                    break;
                case Bosses.Shark:
                    AddAnimation("Attack", sheet, gd, 5, rects[33], rects[42], rects[51], rects[54], rects[55], rects[56], rects[6], rects[15], rects[24], rects[48], rects[49], rects[50]);
                    AddAnimation("MoveForward", sheet, gd, 14, rects[30], rects[31], rects[36]);
                    AddAnimation("Idle", sheet, gd, 7, rects[4], rects[13], rects[22], rects[30], rects[31], rects[36]);
                    AddAnimation("Pound", sheet, gd, 10, rects[23], rects[32], rects[41], rects[45], rects[46], rects[47]);
                    AddAnimation("PoundUp", sheet, gd, 10, rects[37], rects[38], rects[39], rects[40], rects[5], rects[14]);
                    break;
            }
        }
        

    }
    public enum Bosses
    {
        Demon,
        Yeti,
        Bat,
        Shark
    }
    public enum BossPattern
    {
        Attack,
        Pound,
        BigPound_Bottom,
        BigPound_Top,
        Move,
        MoveForward
    }
}
