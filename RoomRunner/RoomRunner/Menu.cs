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
        public List<MenuThingie> thingies;
        public MenuThingie LastTouched;
        
        public Menu(params MenuThingie[] butts)
        {
            thingies = new List<MenuThingie>(butts);
        }

        public void DrawAndUpdate(SpriteBatch sb)
        {
            foreach (MenuThingie b in thingies)
            {
                b.DrawAndUpdate(sb);
                if (b.MouseHovering) LastTouched = b;
            }
        }
    }
    public abstract class MenuThingie
    {
        public readonly static Color Invisible = new Color(0, 0, 0, 0);

        public Rectangle Rectangle;
        public Vector2 Position;
        public int BorderWidth;
        public bool Shown, MouseHovering;
        public Color BorderColor, BGColor;
        public virtual void DrawAndUpdate(SpriteBatch sb)
        {
            MouseHovering = MouseInBounds(Mouse.GetState());
            sb.Draw(Game1.pixel, Rectangle, BGColor);
            if (BorderWidth > 0)
                sb.Draw(Game1.pixel, new Rectangle(Rectangle.X - BorderWidth, Rectangle.Y - BorderWidth, Rectangle.Width + BorderWidth * 2, Rectangle.Height + BorderWidth * 2), BorderColor);
        }

        protected MenuThingie(Rectangle r)
        {
            Rectangle = r;
            Position = new Vector2(r.X, r.Y);
            BorderWidth = 0;
            BorderColor = Color.Black;
            BGColor = Invisible;
            Shown = true;
        }
        protected MenuThingie(Vector2 p)
        {
            Position = p;
            Rectangle = new Rectangle((int)p.X, (int)p.Y, 1, 1);
            BorderWidth = 0;
            BorderColor = Color.Black;
            BGColor = Invisible;
            Shown = true;
        }

        public bool MouseInBounds(MouseState ms)
        {
            bool a, b;
            a = ms.X >= Rectangle.X && ms.Y >= Rectangle.Y;
            b = ms.X <= Rectangle.X + Rectangle.Width && ms.Y <= Rectangle.Y + Rectangle.Height;
            return a && b;
        }
    }
    public class Button : MenuThingie
    {
        private string text;
        public SpriteFont Font;
        private Color col;
        public Color TextColor;
        public Color DrawColor { get { return col; }  set { col = value; } }
        private Texture2D txt;
        public Texture2D Texture { get { return txt; } set { txt = value; } }
        public bool MouseClicked, MouseClickedOnce;
        private MouseState oldms;
        private Vector2 txtPos;
        public Vector2 TextPosition { get { return txtPos; } set { txtPos = value; } }
        public Animation Animation;

        public Button(Rectangle rect, Color c) : base(rect)
        {
            col = c;
            txt = Game1.pixel;
            TextColor = Color.Black;
        }
        public Button(Rectangle rect, Texture2D txt) : base(rect)
        {
            this.txt = txt;
            col = Color.White;
            TextColor = Color.Black;
        }
        public Button(Rectangle rect, Animation anim) : base(rect)
        {
            Animation = anim;
            col = Color.White;
            TextColor = Color.Black;
        }
        public Button(Rectangle rect, Color c, SpriteFont sf, string text) : base(rect)
        {
            col = c;
            Font = sf;
            this.text = text;
            txtPos = calcTxt();
            txt = Game1.pixel;
            TextColor = Color.Black;
        }
        public Button(Rectangle rect, Texture2D txt, SpriteFont sf, string text) : base(rect)
        {
            this.txt = txt;
            this.text = text;
            Font = sf;
            txtPos = calcTxt();
            col = Color.White;
            TextColor = Color.Black;
        }
        public Button(Rectangle rect, Animation anim, SpriteFont sf, string text) : base(rect)
        {
            Animation = anim;
            this.text = text;
            Font = sf;
            txtPos = calcTxt();
            col = Color.White;
            TextColor = Color.Black;
        }

        public override void DrawAndUpdate(SpriteBatch sb)
        {
            if (!Shown) return;
            base.DrawAndUpdate(sb);
            MouseState ms = Mouse.GetState();
            if (MouseHovering)
            {
                MouseClicked = ms.LeftButton == ButtonState.Pressed;
                MouseClickedOnce = oldms.LeftButton == ButtonState.Released && ms.LeftButton == ButtonState.Pressed;
            }
            else MouseClicked = MouseClickedOnce = false;
            oldms = ms;

            if (Animation == default)
                sb.Draw(txt, Rectangle, col);
            else
            {
                sb.Draw(Animation.CurrentTexture, Rectangle, col);
                Animation.Update();
            }
            if (Font != default && text != "")
                sb.DrawString(Font, text, txtPos, TextColor); 
        }
        private bool MouseInBounds(MouseState ms)
        {
            bool a, b;
            a = ms.X >= Rectangle.X && ms.Y >= Rectangle.Y;
            b = ms.X <= Rectangle.X + Rectangle.Width && ms.Y <= Rectangle.Y + Rectangle.Height;
            return a && b;
        }
        private Vector2 calcTxt()
        {
            return new Vector2(Rectangle.Center.X, Rectangle.Center.Y) - Font.MeasureString(text) / 2;
        }
        public static Vector2 CalcTxt(SpriteFont font, Rectangle rect, string text)
        {
            return new Vector2(rect.Center.X, rect.Center.Y) - font.MeasureString(text) / 2;
        }
    }
    public class Box : MenuThingie
    {
        public Animation Animation;
        public SpriteFont Font;
        private Vector2 TextPos;
        public Vector2 TextPosition 
        {
            get 
            { 
                return TextPos; 
            } 
            set
            {
                Offset = value - TextPos;
                TextPos = value; 
            }
        }
        private Vector2 Offset;
        private string text;
        private readonly Func<string> TextGetter;
        public string Text 
        { 
            get
            {
                if (TextGetter != default) return TextGetter.Invoke();
                return text;
            }
        }
        public Color DrawColor, TextColor;

        public Box(Rectangle r, Animation a) : base(r)
        {
            Animation = a;
            DrawColor = Color.White;
            TextColor = Color.Black;
            Offset = Vector2.Zero;
        }
        public Box(Rectangle r, SpriteFont font, string text) : base(r)
        {
            Font = font;
            this.text = text;
            TextPos = Button.CalcTxt(font, r, text);
            DrawColor = Color.White;
            TextColor = Color.Black;
            Offset = Vector2.Zero;
        }
        public Box(Rectangle r, SpriteFont font, Func<string> dynamicText) : base(r)
        {
            Font = font;
            TextGetter = dynamicText;
            TextPos = Button.CalcTxt(font, r, Text);
            DrawColor = Color.White;
            TextColor = Color.Black;
            Offset = Vector2.Zero;
        }
        public Box(Rectangle r, Animation anim, SpriteFont font, string text) : base(r)
        {
            Animation = anim;
            Font = font;
            this.text = text;
            TextPos = Button.CalcTxt(font, r, text);
            DrawColor = Color.White;
            TextColor = Color.Black;
            Offset = Vector2.Zero;
        }
        public Box(Rectangle r, Animation anim, SpriteFont font, Func<string> dynamicText) : base(r)
        {
            Animation = anim;
            Font = font;
            TextGetter = dynamicText;
            TextPos = Button.CalcTxt(font, r, Text);
            DrawColor = Color.White;
            TextColor = Color.Black;
            Offset = Vector2.Zero;
        }

        public override void DrawAndUpdate(SpriteBatch sb)
        {
            if (!Shown) return;
            base.DrawAndUpdate(sb);
            UpdateText();
            if (Animation != null)
            {
                sb.Draw(Animation.CurrentTexture, Rectangle, DrawColor);
                Animation.Update();
            }
            if (Font != default)
                sb.DrawString(Font, text, TextPos, TextColor);
        }
        private void UpdateText()
        {
            if (TextGetter == default) return;
            string val = Text;
            if (val.Equals(text)) return;
            TextPos = Button.CalcTxt(Font, Rectangle, val) + Offset;
            text = val;
        }
    }
    public class MenuText : MenuThingie
    {
        public string Text { 
            get
            {
                return text;
            }
        }
        private string text;
        private readonly Func<string> TextGetter;
        public Vector2 TextPosition { get { return Position; } }
        public SpriteFont Font;
        public Color TextColor;

        public MenuText(SpriteFont font, string text, Vector2 TextPos) : base(TextPos)
        {
            Font = font;
            this.text = text;
            TextColor = Color.Black;
            Vector2 size = font.MeasureString(text);
            Rectangle.Width = (int)size.X;
            Rectangle.Height = (int)size.Y;
        }
        public MenuText(SpriteFont font, Func<string> text, Vector2 TextPos) : base(TextPos)
        {
            Font = font;
            TextGetter = text;
            TextColor = Color.Black;
        }

        public override void DrawAndUpdate(SpriteBatch sb)
        {
            if (!Shown) return;
            base.DrawAndUpdate(sb);
            UpdateText();
            sb.DrawString(Font, text, TextPosition, TextColor);
        }
        private void UpdateText()
        {
            if (TextGetter == default) return;
            string val = Text;
            if (val.Equals(text)) return;
            text = val;
            Vector2 size = Font.MeasureString(text);
            Rectangle.Width = (int)size.X;
            Rectangle.Height = (int)size.Y;
        }
    }
    public class SelectionGrid
    {
        public readonly static Animation Pointer;

        static SelectionGrid()
        {
            Texture2D arrow = Program.Game.Content.Load<Texture2D>("arrow");
            Color[] cols = new Color[arrow.Width * arrow.Height];
            arrow.GetData(cols);
            Color badCol = new Color(0, 255, 0, 255);
            for (int i = 0; i < cols.Length; i++)
                if (cols[i].Equals(badCol)) cols[i] = MenuThingie.Invisible;
            arrow.SetData(cols);
            Animation hold = new Animation("idle");
            hold.AddAnimation("idle", arrow, Program.Game.GraphicsDevice, 5, Player.LoadSheet(4, 3, 39, 30, 1));
            Pointer = hold;
        }
    }
}
