using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{
    public class Menu
    {
        public Button[] thingies;
        
        public Menu(params Button[] butts)
        {
            thingies = butts;
        }

        public void DrawAndUpdate(SpriteBatch sb)
        {
            foreach (Button b in thingies)
                b.DrawAndUpdate(sb);
        }
    }
    public struct Button
    {
        private string text;
        private SpriteFont font;
        private Rectangle rect;
        private Color col;
        public Color TextColor, BorderColor;
        private Texture2D txt;
        public int BorderWidth; //in px
        public bool MouseClicked, MouseClickedOnce, Shown;
        private MouseState oldms;
        private Vector2 txtPos;
        public Button(Rectangle rect, Color c) : this()
        {
            this.rect = rect;
            col = c;
            BorderWidth = 2;
            txt = Game1.pixel;
            TextColor = Color.Black;
            BorderColor = Color.Black;
            Shown = true;
        }
        public Button(Rectangle rect, Texture2D txt) : this()
        {
            this.rect = rect;
            this.txt = txt;
            BorderWidth = 2;
            col = Color.White;
            TextColor = Color.Black;
            BorderColor = Color.Black;
            Shown = true;
        }
        public Button(Rectangle rect, Color c, SpriteFont sf, string text) : this()
        {
            this.rect = rect;
            col = c;
            font = sf;
            this.text = text;
            txtPos = calcTxt();
            BorderWidth = 2;
            txt = Game1.pixel;
            TextColor = Color.Black;
            BorderColor = Color.Black;
            Shown = true;
        }
        public Button(Rectangle rect, Texture2D txt, SpriteFont sf, string text) : this()
        {
            this.rect = rect;
            this.txt = txt;
            this.text = text;
            font = sf;
            txtPos = calcTxt();
            BorderWidth = 2;
            col = Color.White;
            TextColor = Color.Black;
            BorderColor = Color.Black;
            Shown = true;
        }

        public void DrawAndUpdate(SpriteBatch sb)
        {
            if (!Shown) return;
            MouseState ms = Mouse.GetState();
            if (MouseInBounds(ms))
            {
                MouseClicked = ms.LeftButton == ButtonState.Pressed;
                MouseClickedOnce = oldms.LeftButton != ButtonState.Pressed && ms.LeftButton == ButtonState.Pressed;
            }
            else MouseClicked = MouseClickedOnce = false;
            oldms = ms;

            sb.Draw(Game1.pixel, new Rectangle(rect.X - BorderWidth / 2, rect.Y - BorderWidth / 2, rect.Width + BorderWidth, rect.Height + BorderWidth), BorderColor);
            sb.Draw(txt, rect, col);
            if (font != default && text != "")
                sb.DrawString(font, text, txtPos, TextColor);
        }
        private bool MouseInBounds(MouseState ms)
        {
            bool a, b;
            a = ms.X >= rect.X && ms.Y >= rect.Y;
            b = ms.X <= rect.X + rect.Width && ms.Y >= rect.Y + rect.Height;
            return a && b;
        }
        private Vector2 calcTxt()
        {
            return new Vector2(rect.Center.X, rect.Center.Y) - font.MeasureString(text) / 2;
        }
    }
}
