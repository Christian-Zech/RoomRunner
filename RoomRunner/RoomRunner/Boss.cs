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
        public const int TimeBetweenPatterns = 30; //in frames

        public int TimeBeforeNextPattern, TimeLeftInPattern;
        public bool DoingPattern;
        public BossPattern CurrentPattern;
        public static Dictionary<BossPattern, int> PatternTimes;

        private Rectangle rect, bossBarRect;
        public Rectangle Rectangle => rect;
        public Point Position => new Point(rect.X, rect.Y);
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
        private float BossBarPercent;
        public bool IsDead;
        public readonly Bosses Name;
        private GraphicsDevice graphics;
        public List<Projectile> projList;

        static Boss()
        {
            PatternTimes = new Dictionary<BossPattern, int>
            {
                [BossPattern.Attack] = 90
            };
        }
        public Boss(Bosses boss, int health, Texture2D sheet, GraphicsDevice gd) : base(new string[] { "Idle" })
        {
            Name = boss;
            TimeBeforeNextPattern = TimeBetweenPatterns;
            TimeLeftInPattern = 0;
            DoingPattern = false;
            projList = new List<Projectile>();
            graphics = gd;
            rect = new Rectangle(1500,500,200,200);
            MakeAnimation(boss, sheet, gd);
            maxHealth = this.health = health;
            BossBarPercent = 1.0f;
            IsDead = false;
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
            if (TimeBeforeNextPattern-- <= 0)
            {
                TimeBeforeNextPattern = TimeBetweenPatterns;
                DoingPattern = true;
                CurrentPattern = BossPattern.Attack;
                TimeLeftInPattern = PatternTimes[CurrentPattern];
                InitPattern();
            }
            if (DoingPattern)
                UpdatePattern();
        }
        private void FinishPattern()
        {

        }
        private void InitPattern()
        {
            switch (CurrentPattern)
            {
                case BossPattern.Attack:
                    const int numOfProjs = 10;
                    HashSet<int> availablePoints = new HashSet<int>();
                    for (int i = 0; i < Player.frameHeight - Player.floorHeight - 100; i++)
                        availablePoints.Add(i);

                    for (int i = 0; i < numOfProjs; i++)
                    {
                        int rdmNum = -1;
                        while (!availablePoints.Contains(rdmNum))
                            rdmNum = Program.Game.rand.Next(0, Player.frameHeight - Player.floorHeight - 100);
                        for (int ii = rdmNum - 100; ii < rdmNum; ii++)
                            availablePoints.Remove(ii);
                        projList.Add(new Projectile(new Rectangle(1500, rdmNum, 100, 100), 0, new Point(-8, 0), OnetimeAnimation.Anims[OnetimeAnims.Boss_Fireball].Clone(), false, true));
                        Program.Game.rand.Next(DateTime.UtcNow.Millisecond);
                    }
                    break;
            }
        }
        private void UpdatePattern()
        {
            switch (CurrentPattern)
            {
                case BossPattern.Attack:
                    Program.Game.UpdateProjList(projList);
                    break;
            }
        }
        public void Draw(SpriteBatch sb)
        {
            if (IsDead) return;
            sb.Draw(CurrentTexture, rect, Color.White);

            sb.Draw(Game1.pixel, bossBarRect, Color.Red);

            foreach (Projectile p in projList)
            {
                p.Draw(sb, true);
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
        Attack
    }
}
