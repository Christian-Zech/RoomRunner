using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace RoomRunner
{
    public class Cutscene
    {
        public bool cutseneActive;
        public bool phase;
        public Rectangle screen;
        public int alpha;

        public Cutscene()
        {
            cutseneActive = false;
            phase = true;
            screen = new Rectangle(0, 0, 1900, 1000);
            alpha = 0;
        }
        public void updateCutsene()
        {
            if (cutseneActive)
            {
                if (alpha >= 255)
                    phase = false;
                if (phase)
                    alpha+=3;
                else
                    alpha-=3;
                Console.WriteLine(alpha);
            }

        }
        public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
        {
            if (cutseneActive)
                spriteBatch.Draw(pixel, screen, new Color(0, 0, 0, alpha));
            updateCutsene();
        }
    }
       
}
