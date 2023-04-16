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
        public int BorderWidth, Insets;
        public bool Shown, MouseHovering;
        public Color BorderColor, BGColor;
        public virtual void DrawAndUpdate(SpriteBatch sb)
        {
            MouseHovering = MouseInBounds(Mouse.GetState());
            Rectangle DrawRect = new Rectangle(Rectangle.X - Insets, Rectangle.Y - Insets, Rectangle.Width + Insets * 2, Rectangle.Height + Insets * 2);
            if (BorderWidth > 0)
                sb.Draw(Game1.pixel, new Rectangle(DrawRect.X - BorderWidth, DrawRect.Y - BorderWidth, DrawRect.Width + BorderWidth * 2, DrawRect.Height + BorderWidth * 2), BorderColor);
            sb.Draw(Game1.pixel, DrawRect, BGColor);
        }

        protected MenuThingie(Rectangle r)
        {
            Rectangle = r;
            Position = new Vector2(r.X, r.Y);
            BorderWidth = 0;
            BorderColor = Color.Black;
            BGColor = Invisible;
            Shown = true;
            Insets = 0;
        }
        protected MenuThingie(Vector2 p)
        {
            Position = p;
            Rectangle = new Rectangle((int)p.X, (int)p.Y, 1, 1);
            BorderWidth = 0;
            BorderColor = Color.Black;
            BGColor = Invisible;
            Shown = true;
            Insets = 0;
        }
        protected MenuThingie()
        {
            Position = Vector2.Zero;
            Rectangle = new Rectangle();
            BorderWidth = 0;
            BorderColor = Color.Black;
            BGColor = Invisible;
            Shown = true;
            Insets = 0;
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
    public class SelectionGrid : MenuThingie
    {
        public readonly static Animation Pointer;

        public Button[][] Grid;
        public Button Current { get { return GetSelected(Selected); } }
        public Point Selected;
        public Button[] Butts;

        private Point[] RowSizes;
        private readonly Animation InstancePointer;
        private Rectangle PointerRect;
        private KeyboardState OldKb;
        

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
        public SelectionGrid(Button[][] g)
        {
            Grid = g;
            Selected = Point.Zero;
            InstancePointer = Pointer.Clone();
            PointerRect = new Rectangle(0, 0, 50, 50);
            OldKb = Keyboard.GetState();
            List<Button> hold = new List<Button>();
            foreach (Button[] a in g)
                hold.AddRange(a);
            Butts = hold.ToArray();
            GenBounds();
        }

        private void GenBounds()
        {
            RowSizes = new Point[Grid.Length];
            int x, y, MaxWidth, MaxHeight;
            x = y = -1;
            MaxWidth = MaxHeight = 0;
            for (int r = 0, h = 0; r < Grid.Length; r++)
            {
                int ww = 0, hh = 0, i = -1;
                for (int c = 0; c < Grid[r].Length; c++)
                {
                    if (Grid[r][c] == default) continue;
                    i = c;
                    Rectangle hold = Grid[r][c].Rectangle;
                    if (x == -1) x = hold.X;
                    if (y == -1) y = hold.Y;
                    if (hh < hold.Height) hh = hold.Height;
                    if (h < hold.Y) h = hold.Y;
                }
                ww = Grid[r][i].Rectangle.X + Grid[r][i].Rectangle.Width - x;
                RowSizes[r] = new Point(ww, hh);
                if (ww > MaxWidth) MaxWidth = ww;
                if (r == Grid.Length - 1)
                    MaxHeight = h + hh - y;
            }

            Rectangle = new Rectangle(x, y, MaxWidth, MaxHeight);
            Position = new Vector2(Rectangle.X, Rectangle.Y);
        }
        public override void DrawAndUpdate(SpriteBatch sb)
        {
            if (!Shown) return;
            base.DrawAndUpdate(sb);

            Button lastTouched = default;
            foreach (Button b in Butts) if (b != null && b.MouseHovering) lastTouched = b;
            KeyboardState kb = Keyboard.GetState();
            Point change = Point.Zero, newP;
            if (KeyPressed(Keys.Up, kb)) change.X -= 1;
            if (KeyPressed(Keys.Right, kb)) change.Y += 1;
            if (KeyPressed(Keys.Left, kb)) change.Y -= 1;
            if (KeyPressed(Keys.Down, kb)) change.X += 1;
            if (change != Point.Zero)
            {
                newP = new Point(Selected.X + change.X, Selected.Y + change.Y);
                if (!OutOfBounds(newP))
                    while (GetSelected(newP) == null)
                    {
                        newP.X += change.X;
                        newP.Y += change.Y;
                        if (OutOfBounds(newP)) break;
                    }
                if (!OutOfBounds(newP))
                    Selected = newP;
            }
            else if (lastTouched != default) Selected = FindSelected(lastTouched);

            foreach (MenuThingie[] a in Grid)
                foreach (MenuThingie b in a)
                    if (b != default) 
                        b.DrawAndUpdate(sb);
            Rectangle SelRect = Grid[Selected.X][Selected.Y].Rectangle;
            PointerRect.X = SelRect.X + SelRect.Width;
            PointerRect.Y = SelRect.Y + SelRect.Height;
            sb.Draw(InstancePointer.CurrentTexture, PointerRect, Color.White);
            InstancePointer.Update();

            OldKb = kb;
        }
        private bool KeyPressed(Keys k, KeyboardState kb) { return kb.IsKeyDown(k) && !OldKb.IsKeyDown(k); }
        private Button GetSelected(Point p) { return Grid[p.X][p.Y]; }
        private Point FindSelected(Button b)
        {
            for (int r = 0; r < Grid.Length; r++)
                for (int c = 0; c < Grid[r].Length; c++)
                    if (b.Equals(Grid[r][c]))
                        return new Point(r, c);
            throw new Exception("No Point Found For given Button");
        }
        private bool OutOfBounds(Point p)
        {
            bool a, b, c, d;
            a = p.X < 0;
            b = p.Y < 0;
            c = p.X >= Grid.Length;
            if (c || a) return true;
            d = p.Y >= Grid[p.X].Length;
            return b || d;
        }
    }
}
