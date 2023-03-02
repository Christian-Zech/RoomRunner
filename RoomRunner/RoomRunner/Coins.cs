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

        public Coins(Rectangle rectangle, Texture2D texture) : base(new string[] {"coin"})
        {

            coinRectangle = rectangle;
            coinTexture = texture;


        }

    }
}
