using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{
    public class Obstacle
    {
        private Texture2D texture;
        public Rectangle BoundingBox;
        private int speed, timeUntilShown;

        public Obstacle(Texture2D txt, Rectangle bounding, int speed, int timeUntilShown)
        {
            texture = txt;
            BoundingBox = bounding;
            this.speed = speed;
            this.timeUntilShown = timeUntilShown;
        }

        public void Update()
        {
            BoundingBox.X -= speed;
        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, BoundingBox, Color.White);
        }
        public bool DoesCollideWith(Rectangle other) => BoundingBox.Intersects(other);
    }
}
