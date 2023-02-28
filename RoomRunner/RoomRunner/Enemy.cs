using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{

    class Enemy : Animation
    {
        public const string EnemySpritesheet = "Enemies";

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
                case EnemyName.Demon:
                    AddAnimation("Idle", txt, gd, 25, rects[0], rects[1]);
                    break;
                case EnemyName.Yeti:
                    AddAnimation("Idle", txt, gd, 5, rects[2], rects[3], rects[4], rects[5], rects[6]);
                    break;
                case EnemyName.Bat:
                    AddAnimation("Idle", txt, gd, 15, rects[7], rects[8], rects[9], rects[10]);
                    break;
                case EnemyName.Bot:
                    AddAnimation("Idle", txt, gd, 12, rects[11], rects[12], rects[13]);
                    break;
                case EnemyName.Shark:
                    AddAnimation("Idle", txt, gd, 7, rects[14], rects[15], rects[16], rects[17], rects[18], rects[19]);
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
        Demon,
        Yeti,
        Bat,
        Bot,
        Shark
    }
}
