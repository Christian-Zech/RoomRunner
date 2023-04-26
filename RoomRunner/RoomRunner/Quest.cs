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
    public class Quest
    {
        public string text;
        public bool completed;
        public Random rnd;
        public int showing;
        public Rectangle rect;
        public Quest(int i)
        {
            rnd = new Random();
            if (i == 1)
            {
                int amnt = rnd.Next(100, 200);
                text = "Collect " + amnt + " coins for a chance to revive once you die!";
            }
            else
            {
                int dist = rnd.Next(1500, 2500);
                text = "Run " + dist + " meters for a chance to revive once you die!";
            }
            completed = false;
            showing = 200;
            rect = new Rectangle(Game1.window.Width / 2 - 375, -70, 750, 70);
        }
        public void Update(int i)
        {
            showing--;
            if (showing > 140)
            {
                rect.Y+=2;
            }
            else if (showing < 60)
            {
                rect.Y-=2;
            }
            if (showing < 0)
            {
                completed = true;
            }

        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Texture2D img, Texture2D pixel)
        {
            spriteBatch.Draw(pixel, rect, Color.White);
            spriteBatch.DrawString(font, text, new Vector2(rect.X+40, rect.Y+5), Color.Black);
            spriteBatch.Draw(img, new Rectangle(rect.X+5, rect.Y+5, 30, 30), Color.White);
        }
    }
}
