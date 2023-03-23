using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{
    class Coin : Animation
    {
        public Rectangle rectangle;
        public Texture2D texture;
        public Vector2 velocity;
        public Vector2 acceleration;

        public Coin(Rectangle rectangle, Texture2D texture, GraphicsDevice graphics) : base(new string[] { "Coin" })
        {
            this.rectangle = rectangle;
            this.texture = texture;
            velocity = Vector2.Zero;
            acceleration = new Vector2(10, 10);
            Rectangle[] collectablesRectangleArray = Player.LoadSheet(5, 6, 32, 32);


            AddAnimation("Coin", texture, graphics, 5, collectablesRectangleArray[25], collectablesRectangleArray[26], collectablesRectangleArray[27], collectablesRectangleArray[28]);
        }

        public new void Update()
        {
            base.Update();
        }


        public void Destroy()
        {
            rectangle = Rectangle.Empty;
        }

    }
}