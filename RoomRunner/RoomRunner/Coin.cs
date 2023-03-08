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

        public Rectangle Rectangle;
        public Texture2D Texture;

        


        public Coin(Rectangle rectangle, Texture2D texture, GraphicsDevice graphics) : base(new string[] {"coin"})
        {

            this.Rectangle = rectangle;
            this.Texture = texture;
            AddAnimation("Idle", texture, graphics, 5, Player.LoadSheet(2, 2, 32, 32));



        }

    }
}
