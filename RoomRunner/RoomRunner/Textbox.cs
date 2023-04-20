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
    public class Textbox
    {
        public Rectangle rect;
        public Rectangle exitButton;
        public string message;
        public Vector2 arrowEndPoint;
        public bool exited;
        public float angle;
        public int distance;
        public Textbox(string text, Vector2 relevantPoint)
        {
            arrowEndPoint = relevantPoint;
            message = text;
            if (relevantPoint.X > Game1.window.Width - 310)
                relevantPoint.X = Game1.window.Width/ 2 + 200;
            if (relevantPoint.X < 400)
                relevantPoint.X = Game1.window.Width / 2 ;
            if (relevantPoint.Y < 205)
                relevantPoint.Y = Game1.window.Height/2 +150;
            rect = new Rectangle(Game1.window.Width - (int)relevantPoint.X, Game1.window.Height - (int)relevantPoint.Y, 400, 300);
            exitButton = new Rectangle(rect.X + rect.Width - 60, rect.Y + rect.Height - 40, 60, 40);
            exited = false;
            distance = (int)Math.Sqrt(Math.Pow(rect.X+rect.Width/2 - arrowEndPoint.X, 2) + Math.Pow(rect.Y + rect.Height / 2 - arrowEndPoint.Y, 2));
            angle = (float)Math.Atan2(rect.Y + rect.Height / 2 - arrowEndPoint.Y, rect.X + rect.Width / 2 - arrowEndPoint.X);
        }
        public Textbox(string text)
        {
            message = text;
            rect = new Rectangle(Game1.window.Width / 2 - 250, Game1.window.Height / 2 - 200, 400, 300);
            exitButton = new Rectangle(rect.X + rect.Width - 60, rect.Y + rect.Height - 40, 60, 40);
            exited = false;
        }
        public void Update()
        {
            MouseState mouse = Mouse.GetState();
            Rectangle mouseRect = new Rectangle(mouse.X - 1, mouse.Y - 1, 2, 2);
            if (mouse.LeftButton == ButtonState.Pressed && mouseRect.Intersects(exitButton))
            {
                exited = true;
            }
        }
        public void Draw(SpriteBatch spriteBatch, Texture2D pixel, SpriteFont font)
        {
            if (arrowEndPoint != null)
                spriteBatch.Draw(pixel, new Rectangle((int)arrowEndPoint.X, (int)arrowEndPoint.Y, distance, 3), null, Color.White, angle, new Vector2(0, 0), SpriteEffects.None, 0);
            spriteBatch.Draw(pixel, Game1.window, Color.Black * .3f);
            spriteBatch.Draw(pixel, rect, Color.White);
            spriteBatch.DrawString(font, message, new Vector2(rect.X + 5, rect.Y + 5), Color.Black);
            spriteBatch.DrawString(font, "Okay", new Vector2(exitButton.X, exitButton.Y), Color.Green);
            
        }
    }
}
