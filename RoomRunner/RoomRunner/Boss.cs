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
                BossBarPercent = health / maxHealth;
                bossBarRect.Width = (int)((1900 - Insets * 2) / (BossBarPercent / 100.0f));
            }
        }
        private int health, maxHealth;
        private float BossBarPercent;
        public bool IsDead;
        public Boss(Rectangle rect, int health) : base(new string[] { "Idle" })
        {
            this.rect = rect;
            maxHealth = Health = health;
            BossBarPercent = 100.0f;
            IsDead = false;
            bossBarRect = new Rectangle(Insets, 900, 1900 - Insets * 2, 50);
        }

        public new void Update()
        {
            base.Update();
        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(CurrentTexture, rect, Color.White);

            sb.Draw(Game1.pixel, bossBarRect, Color.Red);
        }

        public void Damage(int amount) => health -= amount;

        

    }
    public enum Bosses
    {

    }
}
