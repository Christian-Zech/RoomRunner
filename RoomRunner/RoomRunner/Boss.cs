﻿using Microsoft.Xna.Framework;
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
        public const int TimeBetweenPatterns = 50; //in frames
        public const double SpeedMultiplier = 2;

        public int TimeBeforeNextPattern, TimeLeftInPattern;
        public bool DoingPattern;
        public BossPattern CurrentPattern;
        public static Dictionary<BossPattern, int> PatternTimes;

        private Rectangle rect, bossBarRect;
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
        private int health, maxHealth;
        private int timer1;
        private float BossBarPercent;
        public bool IsDead, FlipProjX, FlipProjY;
        public readonly Bosses Name;
        private GraphicsDevice graphics;
        public List<ProjectileClump> projList, projBuffer;

        static Boss()
        {
            PatternTimes = new Dictionary<BossPattern, int>
            {
                [BossPattern.Attack] = 90,
                [BossPattern.Pound] = 300,
                [BossPattern.BigPound_Bottom] = 300,
                [BossPattern.BigPound_Top] = 300
            };
        }
        public Boss(Bosses boss, int health, Texture2D sheet, GraphicsDevice gd) : base(new string[] { "Idle" })
        {
            Name = boss;
            TimeBeforeNextPattern = (int)(TimeBetweenPatterns / SpeedMultiplier);
            TimeLeftInPattern = 0;
            DoingPattern = false;
            projList = new List<ProjectileClump>();
            projBuffer = new List<ProjectileClump>();
            graphics = gd;
            rect = new Rectangle(1500,500,200,200);
            MakeAnimation(boss, sheet, gd);
            maxHealth = this.health = health;
            BossBarPercent = 1.0f;
            IsDead = false;
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
                    FinishPattern();
            }
            else if (TimeBeforeNextPattern-- <= 0)
            {
                TimeBeforeNextPattern = (int)(TimeBetweenPatterns / SpeedMultiplier);
                DoingPattern = true;
                CurrentPattern = (BossPattern)Program.Game.rand.Next(0, 4);
                TimeLeftInPattern = (int)(PatternTimes[CurrentPattern] / SpeedMultiplier);
                InitPattern();
            }
            if (DoingPattern)
                UpdatePattern();
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
            }
            projBuffer.Clear();
            projList.Clear();
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
                    numOfProjs = 3;
                    availablePoints = new List<int>();
                    for (int i = 10; i < Player.frameHeight - Player.floorHeight - 10; i++)
                        availablePoints.Add(i);

                    for (int i = 0; i < numOfProjs && availablePoints.Count > 0; i++)
                    {
                        int rdmNum = availablePoints.ElementAt(Program.Game.rand.Next(0, availablePoints.Count));
                        for (int ii = rdmNum - 100; ii <= rdmNum + 100; ii++)
                            availablePoints.Remove(ii);
                        projBuffer.Add(new ProjectileClump(FlipProjX, FlipProjY, new Projectile(new Rectangle(1500, rdmNum, 100, 100), 0, new Point((int)(-24 * SpeedMultiplier), 0), OnetimeAnimation.Anims[OnetimeAnims.Boss_Fireball].Clone(), false, true)));
                        Program.Game.rand.Next(DateTime.UtcNow.Millisecond);
                    }
                    break;
                case BossPattern.Pound:
                    timer1 = 0;
                    FlipProjX = false;
                    FlipProjY = false;
                    floor = Game1.window.Height - Player.floorHeight;
                    sheet = Program.Game.Content.Load<Texture2D>("Level1/Enemies/Obstacles");
                    rects = Player.LoadSheet(3, 3, 32, 32, 1);
                    frames = (int)(6 / SpeedMultiplier);
                    for (int x = Game1.window.Width; x > 0; x -= 100)
                    {
                        List<Projectile> toClump = new List<Projectile>
                        {
                            new Projectile(new Rectangle(x, floor - 30, 100, 30), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(new Rectangle(x, floor - 60, 100, 60), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(new Rectangle(x, floor - 90, 100, 90), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(new Rectangle(x, floor - 60, 100, 60), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(new Rectangle(x, floor - 30, 100, 30), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true)
                        };
                        projBuffer.Add(new ProjectileClump(FlipProjX, FlipProjY, toClump.ToArray()));
                    }
                    break;
                case BossPattern.BigPound_Bottom:
                    FlipProjX = false;
                    FlipProjY = false;
                    floor = Game1.window.Height - Player.floorHeight;
                    sheet = Program.Game.Content.Load<Texture2D>("Level1/Enemies/Obstacles");
                    rects = Player.LoadSheet(3, 3, 32, 32, 1);
                    frames = (int)(12 / SpeedMultiplier);
                    for (int x = Game1.window.Width; x >= 0; x -= 100)
                    {
                        List<Projectile> toClump = new List<Projectile>
                        {
                            new Projectile(new Rectangle(x, floor - 60, 100, 60), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(new Rectangle(x, floor - 120, 100, 120), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(new Rectangle(x, floor - 180, 100, 180), 0, frames * 10, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(new Rectangle(x, floor - 120, 100, 120), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(new Rectangle(x, floor - 60, 100, 60), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true)
                        };
                        projList.Add(new ProjectileClump(FlipProjX, FlipProjY, toClump.ToArray()));
                    }
                    break;
                case BossPattern.BigPound_Top:
                    FlipProjX = false;
                    FlipProjY = true;
                    ceiling = Game1.window.Height - Player.ceilingHeight + 20; //20 for inset
                    sheet = Program.Game.Content.Load<Texture2D>("Level1/Enemies/Obstacles");
                    rects = Player.LoadSheet(3, 3, 32, 32, 1);
                    frames = (int)(12 / SpeedMultiplier);
                    for (int x = Game1.window.Width; x >= 0; x -= 100)
                    {
                        List<Projectile> toClump = new List<Projectile>
                        {
                            new Projectile(new Rectangle(x, ceiling, 100, 60), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(new Rectangle(x, ceiling, 100, 120), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(new Rectangle(x, ceiling, 100, 180), 0, frames * 10, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(new Rectangle(x, ceiling, 100, 120), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true),
                            new Projectile(new Rectangle(x, ceiling, 100, 60), 0, frames, new OnetimeAnimation(10, Program.Game.GraphicsDevice, sheet, rects[5]), false, true)
                        };
                        projList.Add(new ProjectileClump(FlipProjX, FlipProjY, toClump.ToArray()));
                    }
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
            }
        }
        public void Draw(SpriteBatch sb)
        {
            if (IsDead) return;
            sb.Draw(CurrentTexture, rect, Color.White);

            sb.Draw(Game1.pixel, bossBarRect, Color.Red);

            foreach (ProjectileClump p in projList)
            {
                p.DrawAndUpdate(sb);
            }
        }
        public new Boss Clone() { return new Boss(Name, health, LastUsedSheet, graphics); }

        public void Damage(int amount) => Health -= amount;
        private void MakeAnimation(Bosses boss, Texture2D sheet, GraphicsDevice gd)
        {
            Rectangle[] rects = Player.LoadSheet(4, 5, 32, 32);
            switch (boss)
            {
                case Bosses.Demon:
                    AddAnimation("Idle", sheet, gd, 25, rects[0], rects[1]);
                    break;
                case Bosses.Yeti:
                    AddAnimation("Idle", sheet, gd, 5, rects[2], rects[3], rects[4], rects[5], rects[6]);
                    break;
                case Bosses.Bat:
                    AddAnimation("Idle", sheet, gd, 15, rects[7], rects[8], rects[9], rects[10]);
                    break;
                case Bosses.Shark:
                    AddAnimation("Idle", sheet, gd, 7, rects[14], rects[15], rects[16], rects[17], rects[18], rects[19]);
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
        BigPound_Top
    }
}
