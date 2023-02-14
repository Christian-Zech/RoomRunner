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
        public Vector2 roomSize;



        public Room(Texture2D background, Vector2 roomSize)
        {
            this.background = background;
            this.roomSize = roomSize;
            animationTextures = new List<Texture2D>();





        }

        public void AddAnimationTexture(Texture2D texture)
        {

        }

        public void Draw(int gameTimer)
        {

            


        }





    }
}
