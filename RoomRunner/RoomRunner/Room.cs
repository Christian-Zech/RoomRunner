using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{
    class Room
    {


        public Texture2D background1;
        public Texture2D background2;

        public Rectangle backgroundRectangle;


        // intended for single images
        public Room(Texture2D background, Rectangle backgroundRectangle)
        {
            background1 = background;
            background2 = background;
            this.backgroundRectangle = backgroundRectangle;
        }


        // intended for 2 part images
        public Room(Texture2D background1, Texture2D background2, Rectangle backgroundRectangle)
        {
            this.background1 = background1;
            this.background2 = background2;
            this.backgroundRectangle = backgroundRectangle;
        }


        public void Update(int scrollSpeed)
        {
            backgroundRectangle.X -= scrollSpeed;


        }

        
        



    }
}
