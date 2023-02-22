using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{
    public class Obstacle : Animation
    {
        public Rectangle BoundingBox;
        private int speed;

        public Obstacle(Texture2D[] animation, Rectangle bounding, int speed, int animSpeed = 15) : base(new string[] { "Idle" })
        {
            BoundingBox = bounding;
            this.speed = speed;
            AddAnimation("Idle", animSpeed, animation);
        }

        public new void Update()
        {
            BoundingBox.X -= speed;

            base.Update();
        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(CurrentTexture, BoundingBox, Color.White);
        }
        public bool DoesCollideWith(Rectangle other) => BoundingBox.Intersects(other);

        public static Obstacle GenerateObstacle(Random rdm = default)
        {
            if (rdm == default) rdm = new Random(4298523);
            return null;
        }
        public static void MakeObstacles(Texture2D enemySpriteSheet, GraphicsDevice gd)
        {
            List<Texture2D[]> obstacles = new List<Texture2D[]>();
            Rectangle[] rects = Player.LoadSheet(4, 5, 32, 32);

            //obstacles.Add()
        }
    }
}
