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
        public static string[] States => statesstates;
        private static readonly string[] statesstates = new string[] { "Idle" }; //NEVER USE THIS VARIABLE, ONLY USE THE UPPERCASE ONE!!!!

        private Texture2D texture;
        public Rectangle BoundingBox;
        private int speed, timeUntilShown;

        public Obstacle(Texture2D txt, Rectangle bounding, int speed, int timeUntilShown) : base(States)
        {
            texture = txt;
            BoundingBox = bounding;
            this.speed = speed;
            this.timeUntilShown = timeUntilShown;
        }

        public new void Update()
        {
            BoundingBox.X -= speed;

            base.Update();
        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, BoundingBox, Color.White);
        }
        public bool DoesCollideWith(Rectangle other) => BoundingBox.Intersects(other);
    }
}
