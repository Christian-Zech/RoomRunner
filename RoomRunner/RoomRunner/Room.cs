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


        public List<Texture2D> animationTextures;
        public Texture2D background;



        public Room(Texture2D background)
        {
            this.background = background;
            animationTextures = new List<Texture2D>();





        }

        public void AddAnimationTexture(Texture2D texture)
        {

        }

        





    }
}
