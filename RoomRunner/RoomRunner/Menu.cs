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
        public Menu LinkedMenu;
        public Menu(params MenuThingie[] butts)
        {
            thingies = new List<MenuThingie>(butts);
        }

        public void DrawAndUpdate(SpriteBatch sb)
        {
            bool hasLink = LinkedMenu != default;
            foreach (MenuThingie b in thingies)
            {
                b.Disabled = hasLink;
                b.DrawAndUpdate(sb);
                if (b.MouseHovering) LastTouched = b;
            }
            if (hasLink) LinkedMenu.DrawAndUpdate(sb);
        }
    }
    public abstract class MenuThingie
    {
        public readonly static Color Invisible = new Color(0, 0, 0, 0);

        public Rectangle Rectangle;
        public Rectangle DrawRectangle;
        public Vector2 Position;
        public int BorderWidth;
        public bool Shown, MouseHovering, Disabled;
        public Color BorderColor, BGColor;
        public int HitboxInset
        {
            set
            {
                DrawRectangle = new Rectangle(DrawRectangle.X - value, DrawRectangle.Y - value, DrawRectangle.Width + value * 2, DrawRectangle.Height + value * 2);
            }
        }
        public int Inset
        {
            set
            {
                Rectangle = new Rectangle(Rectangle.X - value, Rectangle.Y - value, Rectangle.Width + value * 2, Rectangle.Height + value * 2);
            }
        }
        public virtual void DrawAndUpdate(SpriteBatch sb)
        {
            MouseHovering = MouseInBounds(Mouse.GetState());
            if (BorderWidth > 0)
                sb.Draw(Game1.pixel, new Rectangle(DrawRectangle.X - BorderWidth, DrawRectangle.Y - BorderWidth, DrawRectangle.Width + BorderWidth * 2, DrawRectangle.Height + BorderWidth * 2), BorderColor);
            sb.Draw(Game1.pixel, DrawRectangle, BGColor);
        }

        protected MenuThingie(Rectangle r)
        {
            Rectangle = r;
            Position = new Vector2(r.X, r.Y);
            BorderWidth = 0;
            BorderColor = Color.Black;
            BGColor = Invisible;
            Shown = true;
            DrawRectangle = Rectangle;
        }
        protected MenuThingie(Vector2 p)
        {
            Position = p;
            Rectangle = new Rectangle((int)p.X, (int)p.Y, 1, 1);
            BorderWidth = 0;
            BorderColor = Color.Black;
            BGColor = Invisible;
            Shown = true;
            DrawRectangle = Rectangle;
        }
        protected MenuThingie()
        {
            Position = Vector2.Zero;
            Rectangle = new Rectangle();
            BorderWidth = 0;
            BorderColor = Color.Black;
            BGColor = Invisible;
            Shown = true;
            DrawRectangle = Rectangle;
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
        private readonly string text;
        public string Text { get { return text; } }
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
                sb.Draw(txt, DrawRectangle, col);
            else
            {
                sb.Draw(Animation.CurrentTexture, DrawRectangle, col);
                Animation.Update();
            }
            if (Font != default && text != "")
                sb.DrawString(Font, text, txtPos, TextColor); 
        }
        private Vector2 calcTxt()
        {
            return new Vector2(DrawRectangle.Center.X, DrawRectangle.Center.Y) - Font.MeasureString(text) / 2;
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
        public Box(Rectangle r) : base(r)
        {
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
                sb.Draw(Animation.CurrentTexture, DrawRectangle, DrawColor);
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
            TextPos = Button.CalcTxt(Font, DrawRectangle, val) + Offset;
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
            DrawRectangle = Rectangle;
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
            DrawRectangle = Rectangle;
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
        private Point OldMousePos;
        private sbyte lastUsed; //0 == Mouse, 1 == Keyboard
        

        static SelectionGrid()
        {
            Texture2D arrow = Program.Game.Content.Load<Texture2D>("arrow");
            Color[] cols = new Color[arrow.Width * arrow.Height];
            arrow.GetData(cols);
            Color badCol = new Color(0, 255, 0, 255);
            for (int i = 0; i < cols.Length; i++)
                if (cols[i].Equals(badCol)) cols[i] = Invisible;
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
            lastUsed = -1;
            OldMousePos = new Point(-1, -1);
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
                int hh = 0, i = -1;
                for (int c = 0; c < Grid[r].Length; c++)
                {
                    if (Grid[r][c] == default) continue;
                    i = c;
                    Rectangle hold = Grid[r][c].DrawRectangle;
                    if (x == -1) x = hold.X;
                    if (y == -1) y = hold.Y;
                    if (hh < hold.Height) hh = hold.Height;
                    if (h < hold.Y) h = hold.Y;
                }
                int ww = 0;
                if (i != -1)
                    ww = Grid[r][i].DrawRectangle.X + Grid[r][i].DrawRectangle.Width - x;
                RowSizes[r] = new Point(ww, hh);
                if (ww > MaxWidth) MaxWidth = ww;
                if (r == Grid.Length - 1)
                    MaxHeight = h + hh - y;
            }

            Rectangle = new Rectangle(x, y, MaxWidth, MaxHeight);
            DrawRectangle = Rectangle;
            Position = new Vector2(Rectangle.X, Rectangle.Y);
        }
        public override void DrawAndUpdate(SpriteBatch sb)
        {
            if (!Shown) return;
            base.DrawAndUpdate(sb);

            if (Disabled)
            {
                foreach (MenuThingie[] a in Grid)
                    foreach (MenuThingie b in a)
                        if (b != default)
                            b.DrawAndUpdate(sb);
                return;
            }

            Button lastTouched = default;
            foreach (Button b in Butts) if (b != null && b.MouseHovering) lastTouched = b;
            KeyboardState kb = Keyboard.GetState();
            Point change = Point.Zero, newP;
            //ignore this part, just hijacking variables to save a bit of memory :)
            newP = new Point(Mouse.GetState().X, Mouse.GetState().Y);
            if (!newP.Equals(OldMousePos)) lastUsed = 0;
            OldMousePos = newP;
            bool ignoreMouse = lastUsed != 0;
            //---------------------------------------------------------------------
            if (KeyPressed(Keys.Up, kb)) change.X -= 1;
            if (KeyPressed(Keys.Right, kb)) change.Y += 1;
            if (KeyPressed(Keys.Left, kb)) change.Y -= 1;
            if (KeyPressed(Keys.Down, kb)) change.X += 1;
            if (change != Point.Zero)
            {
                lastUsed = 1;
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
            else if (lastTouched != default && !ignoreMouse) Selected = FindSelected(lastTouched);

            foreach (MenuThingie[] a in Grid)
                foreach (MenuThingie b in a)
                    if (b != default) 
                        b.DrawAndUpdate(sb);
            if (Grid[Selected.X].Length > Selected.Y && Grid[Selected.X][Selected.Y] != null)
            {
                Rectangle SelRect = Grid[Selected.X][Selected.Y].DrawRectangle;
                PointerRect.X = SelRect.X + SelRect.Width;
                PointerRect.Y = SelRect.Y + SelRect.Height;
                sb.Draw(InstancePointer.CurrentTexture, PointerRect, Color.White);
            }
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
    public class Slider : MenuThingie
    {
        public static Button Default
        {
            get
            {
                return new Button(new Rectangle(0, 0, Game1.window.Width / 38, Game1.window.Height / 18), Color.Green)
                {
                    BorderWidth = 3
                };
            }
        }

        public Button Knob;
        public float Percent;
        private bool Held;

        public Slider(Rectangle r) : base(r)
        {
            Knob = new Button(new Rectangle(r.X - r.Width / 100, r.Y - r.Height + Rectangle.Height / 2, r.Width / 50, r.Height * 2), Color.Green)
            {
                BorderWidth = 3
            };
            Inset = r.Height;
            Rectangle.X -= r.Height;
            Rectangle.Width += r.Height * 2;
            Percent = 0.0f;
            BGColor = Color.Black;
            BorderColor = Color.Silver;
            BorderWidth = 3;
        }

        public override void DrawAndUpdate(SpriteBatch sb)
        {
            if (!Shown) return;
            base.DrawAndUpdate(sb);

            MouseState ms = Mouse.GetState();

            if (Held || (ms.LeftButton == ButtonState.Pressed && MouseInBounds(ms)))
            {
                Held = true;
                if (ms.X >= DrawRectangle.X && ms.X <= DrawRectangle.X + DrawRectangle.Width)
                    Knob.DrawRectangle.X = ms.X - Knob.DrawRectangle.Width / 2;
                else
                    if (ms.X < DrawRectangle.X)
                        Knob.DrawRectangle.X = DrawRectangle.X - Knob.DrawRectangle.Width / 2;
                    else
                        Knob.DrawRectangle.X = DrawRectangle.X + DrawRectangle.Width - Knob.DrawRectangle.Width / 2;
            }
            if (Held && ms.LeftButton != ButtonState.Pressed) Held = false;
            Knob.DrawAndUpdate(sb);
        }

    }
}
