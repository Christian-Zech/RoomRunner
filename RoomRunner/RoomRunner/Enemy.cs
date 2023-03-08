using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{

    public class Enemy : Animation
    {
        public const int EnemyNames = 1;
        public static string EnemySpritesheet = Game1.levels + "/Enemies/Enemies";

        public Texture2D texture;
        public Rectangle rectangle;
        public static int totalEnemyCount;

        static Enemy()
        {
            totalEnemyCount = 0;
        }
        public Enemy(Texture2D texture, Rectangle rectangle, GraphicsDevice graphics) : base(new string[] {"Idle"})
        {
            this.texture = texture;
            this.rectangle = rectangle;
            AddAnimation("Idle", texture, graphics, 5, Player.LoadSheet(2, 2, 32, 32));
            totalEnemyCount++;

        }
        public Enemy(EnemyName name, ContentManager cm, GraphicsDevice gd, Rectangle rect) : base(new string[] { "Idle" })
        {
            Texture2D sheet = cm.Load<Texture2D>(EnemySpritesheet);
            rectangle = rect;
            MakeAnimation(name, sheet, gd);
            totalEnemyCount++;
        }

        private void MakeAnimation(EnemyName n, Texture2D txt, GraphicsDevice gd)
        {
            Rectangle[] rects = Player.LoadSheet(4, 5, 32, 32);
            switch (n)
            {
                case EnemyName.Bot:
                    AddAnimation("Idle", txt, gd, 12, rects[11], rects[12], rects[13]);
                    break;
            }
        }


        public new void Update() 
        {
            base.Update();
        }

    }
    public enum EnemyName
    {
        Bot
    }
}
