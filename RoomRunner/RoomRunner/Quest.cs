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
        public bool completedAnim;
        public bool completedQuest;
        public Random rnd;
        public int showing;
        public Rectangle rect;
        public Rectangle shadowRect;
        public int id;
        public int amnt;
        public int dist;
        public Quest(int i)
        {
            rnd = new Random();
            if (i == 1)
            {
                amnt = rnd.Next(50, 100);
                text = "Collect " + amnt + " coins for a chance to revive once you die!";
            }
            else
            {
                dist = rnd.Next(1500, 2500);
                text = "Run " + dist + " meters for a chance to revive once you die!";
            }
            completedAnim = false;
            completedQuest = false;
            showing = 200;
            rect = new Rectangle(Game1.window.Width / 2 - 375, -70, 750, 70);
            shadowRect = new Rectangle(rect.X - 3, rect.Y - 3, rect.Width + 6, rect.Height + 6);
            id = i;
        }
        public void Update()
        {
            showing--;
            if (showing > 140)
            {
                shadowRect.Y += 2;
                rect.Y+=2;
            }
            else if (showing < 60)
            {
                shadowRect.Y -= 2;
                rect.Y-=2;
            }
            if (showing < 0)
            {
                completedAnim = true;
                showing = 200;
                text = "Quest Completed! You will revive once you die";
            }

        }
        public void completedUpdate()
        {
            showing--;
            if (showing > 140)
            {
                shadowRect.Y += 2;
                rect.Y += 2;
            }
            else if (showing < 60)
            {
                shadowRect.Y -= 2;
                rect.Y -= 2;
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Texture2D img, Rectangle source, Texture2D pixel)
        {
            spriteBatch.Draw(pixel, shadowRect, Color.Black * .5f);
            spriteBatch.Draw(pixel, rect, Color.White);

            spriteBatch.DrawString(font, text, new Vector2(rect.X + 20, rect.Y + 20), Color.Black);
            if (id == 1)
                spriteBatch.Draw(img, new Rectangle(rect.X + rect.Width - 60, rect.Y + 18, 30, 30), source, Color.White);
            else
                spriteBatch.Draw(img, new Rectangle(rect.X + rect.Width - 80, rect.Y + 10, 45, 45), source, Color.White);
        }
    }
}
