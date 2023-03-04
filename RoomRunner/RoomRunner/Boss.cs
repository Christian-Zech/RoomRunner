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
        public Boss(Bosses boss, int health, Texture2D sheet, GraphicsDevice gd) : base(new string[] { "Idle" })
        {
            Name = boss;
            graphics = gd;
            rect = new Rectangle(1000,500,200,200);
            MakeAnimation(boss, sheet, gd);
            maxHealth = this.health = health;
            BossBarPercent = 1.0f;
            IsDead = false;
            bossBarRect = new Rectangle(Insets, 900, 1900 - Insets * 2, 50);
        }

        public new void Update()
        {
            base.Update();
        }
        public void Draw(SpriteBatch sb)
        {
            if (IsDead) return;
            sb.Draw(CurrentTexture, rect, Color.White);

            sb.Draw(Game1.pixel, bossBarRect, Color.Red);
        }
        public Boss Clone() => new Boss(Name, health, LastUsedSheet, graphics);

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
}
