using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{
    class Coins : Animation
    {

        Rectangle coinRectangle;
        Texture2D coinTexture;

        


        public Coins(Rectangle rectangle, Texture2D texture, GraphicsDevice graphics) : base(new string[] {"coin"})
        {

            coinRectangle = rectangle;
            coinTexture = texture;
            AddAnimation("Idle", texture, graphics, 5, Player.LoadSheet(2, 2, 32, 32));



        }

    }
}
