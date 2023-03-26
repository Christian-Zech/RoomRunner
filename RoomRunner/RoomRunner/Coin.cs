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
        public Vector2 Position;
        public Vector2 Velocity;

        public Coin(Rectangle rectangle, Texture2D texture, GraphicsDevice graphics) : base(new string[] { "Coin" })
        {
            this.rectangle = rectangle;
            this.texture = texture;
            Position = new Vector2(rectangle.X, rectangle.Y);
            Velocity = Vector2.Zero;
            Rectangle[] collectablesRectangleArray = Player.LoadSheet(5, 6, 32, 32);


            AddAnimation("Coin", texture, graphics, 5, collectablesRectangleArray[25], collectablesRectangleArray[26], collectablesRectangleArray[27], collectablesRectangleArray[28]);
        }

        public new void Update()
        {
            base.Update();
            
            rectangle.X = (int)Position.X;
            rectangle.Y = (int)Position.Y;
        }

        public void ApplyMagnetForce(GameTime gameTime)
        {
            float magnetForce = 0.1f;
            Vector2 direction = Player.Position - Position;
            direction.Normalize();

            Velocity += direction * magnetForce;
            Velocity *= 0.98f;
            Position += Velocity;
        }


        public void Destroy()
        {
            rectangle = Rectangle.Empty;
            Position = Vector2.Zero;
        }

    }
}