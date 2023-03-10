using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoomRunner
{
    class Shop
    {
        public List<ShopItem> items;
        public List<bool> selectedItem;
        public Rectangle[,] grid;
        public Rectangle selection; //highlights background of currently selected item
        public Rectangle backButton;
        public bool leave;
        int mouseX, mouseY, selectionIndexX, selectionIndexY;
        List<Keys> pressedKeys, oldKeys;

        public Shop(List<ShopItem> itemList)
        {
            items = itemList;
            leave = false;
            selectedItem = new List<bool> { true, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
            grid = new Rectangle[4, 4];
            int x = 650;
            int y = 250;
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    grid[r, c] = new Rectangle(x, y, 80, 80);
                    x += 150;
                }
                y += 170;
                x = 650;
            }
            backButton = new Rectangle(180, 120, 130, 80);
            pressedKeys = new List<Keys>();
            oldKeys = new List<Keys>();
            selection = grid[0, 0];
            selectionIndexX = 0;
            selectionIndexY = 0;
        }
        //public void Buy(ShopItem item, Player jeb)
        //{
        //if jeb's money is greater than or equal to the items price
        //add it to players item list and subtract their money
        //}
        public void updateSelection()
        {
            if (leave)
            {
                leave = false;
            }
            if (pressedKeys.Contains(Keys.Back))
            {
                leave = true;
                pressedKeys.Clear();
            }
                
            oldKeys = pressedKeys;
            MouseState mouse = Mouse.GetState();
            KeyboardState kb = Keyboard.GetState();
            mouseX = mouse.X;
            mouseY = mouse.Y;
            pressedKeys = kb.GetPressedKeys().ToList();
            Rectangle temp = new Rectangle(mouseX, mouseY, 35, 35);
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    if (r != 3)
                    {
                        if (temp.Intersects(grid[r, c]))
                        {
                            selection = grid[r, c];
                            selectionIndexX = r;
                            selectionIndexY = c;
                        }
                    }
                    else if (r == 3 && c != 0)
                    {
                        if (c != 3)
                        {
                            if (temp.Intersects(grid[r, c]))
                            {
                                selection = grid[r, c];
                                selectionIndexX = r;
                                selectionIndexY = c;
                            }
                        }

                    }
                }
            }
            if (pressedKeys.Contains(Keys.A) || pressedKeys.Contains(Keys.S) || pressedKeys.Contains(Keys.W) || pressedKeys.Contains(Keys.D) || pressedKeys.Contains(Keys.Right) || pressedKeys.Contains(Keys.Up) || pressedKeys.Contains(Keys.Down) || pressedKeys.Contains(Keys.Left))
            {
                for (int i = 0; i < pressedKeys.Count; i++)
                {
                    if (pressedKeys[i] == Keys.A && !oldKeys.Contains(Keys.A))
                    {
                        selectionIndexY--;
                        if (selectionIndexY == 0 && selectionIndexX == 3)
                            selectionIndexY++;
                        if (selectionIndexY < 0)
                            selectionIndexY = 3;
                        selection = grid[selectionIndexX, selectionIndexY];
                    }
                    if (pressedKeys[i] == Keys.Left && !oldKeys.Contains(Keys.Left))
                    {
                        selectionIndexY--;
                        if (selectionIndexY == 0 && selectionIndexX == 3)
                            selectionIndexY++;
                        if (selectionIndexY < 0)
                            selectionIndexY = 3;
                        selection = grid[selectionIndexX, selectionIndexY];
                    }
                    if (pressedKeys[i] == Keys.D && !oldKeys.Contains(Keys.D))
                    {
                        selectionIndexY++;
                        if (selectionIndexY == 3 && selectionIndexX == 3)
                            selectionIndexY--;
                        if (selectionIndexY > 3)
                            selectionIndexY = 0;
                        selection = grid[selectionIndexX, selectionIndexY];
                    }
                    if (pressedKeys[i] == Keys.Right && !oldKeys.Contains(Keys.Right))
                    {
                        selectionIndexY++;
                        if (selectionIndexY == 3 && selectionIndexX == 3)
                            selectionIndexY--;
                        if (selectionIndexY > 3)
                            selectionIndexY = 0;
                        selection = grid[selectionIndexX, selectionIndexY];
                    }
                    if (pressedKeys[i] == Keys.W && !oldKeys.Contains(Keys.W))
                    {
                        selectionIndexX--;

                        if (selectionIndexX < 0)
                            selectionIndexX = 3;
                        if (selectionIndexX == 3 && (selectionIndexY == 0 || selectionIndexY == 3))
                            selectionIndexX = 0;
                        selection = grid[selectionIndexX, selectionIndexY];
                    }
                    if (pressedKeys[i] == Keys.Up && !oldKeys.Contains(Keys.Up))
                    {
                        selectionIndexX--;

                        if (selectionIndexX < 0)
                            selectionIndexX = 3;
                        if (selectionIndexX == 3 && (selectionIndexY == 0 || selectionIndexY == 3))
                            selectionIndexX = 0;
                        selection = grid[selectionIndexX, selectionIndexY];
                    }
                    if (pressedKeys[i] == Keys.S && !oldKeys.Contains(Keys.S))
                    {
                        selectionIndexX++;
                        if (selectionIndexX == 3 && (selectionIndexY == 0 || selectionIndexY == 3))
                            selectionIndexX--;
                        if (selectionIndexX > 3)
                            selectionIndexX = 0;
                        selection = grid[selectionIndexX, selectionIndexY];
                    }
                    if (pressedKeys[i] == Keys.Down && !oldKeys.Contains(Keys.Down))
                    {
                        selectionIndexX++;
                        if (selectionIndexX == 3 && (selectionIndexY == 0 || selectionIndexY == 3))
                            selectionIndexX--;
                        if (selectionIndexX > 3)
                            selectionIndexX = 0;
                        selection = grid[selectionIndexX, selectionIndexY];
                    }
                }
            }
            selectedItem = new List<bool> { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
            selectedItem[(4 * selectionIndexX) + selectionIndexY] = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font, SpriteFont bold, SpriteFont title, Texture2D pixel)
        {
            updateSelection();
            spriteBatch.Draw(pixel, new Rectangle(mouseX, mouseY, 5, 5), Color.Black);
            spriteBatch.Draw(pixel, new Rectangle(selection.X - 5, selection.Y - 10, 90, 100), new Color(200, 200, 200, 255));

            spriteBatch.Draw(pixel, backButton, Color.Red);
            spriteBatch.DrawString(bold, "Back", new Vector2(backButton.X + 40, backButton.Y + 15), Color.Black);
            spriteBatch.DrawString(font, "(backspace)", new Vector2(backButton.X + 5, backButton.Y + 40), Color.Black);

            int x = 0;
            int y = 0;
            int temp = 0;
            spriteBatch.DrawString(title, "SHOP", new Vector2(780, 40), Color.Black);
            spriteBatch.DrawString(font, "(space/click to buy)", new Vector2(790, 170), Color.Black);
            for (int i = 0; i < items.Count; i++)
            {
                temp = 0;
                if (items[i].name.Length <= 4)
                    temp += 20;
                if (i % 4 == 0 && i != 0)
                {

                    y = 0;
                    x++;

                    if (x == 3)
                        y = 1;
                }
                if (items[i].name.Equals("Coin"))
                {
                    spriteBatch.Draw(items[i].tex, new Rectangle(1650, 120, 60, 60), items[i].sourceRects[(int)items[i].currentFrameIndex], Color.White);
                    spriteBatch.DrawString(font, "Current coins:", new Vector2(1600, 180), Color.Black);
                }
                else
                {

                    spriteBatch.Draw(items[i].tex, grid[x, y], items[i].sourceRects[(int)items[i].currentFrameIndex], Color.White);
                    if (items[i].name.Equals("Blue Headband") && selection == grid[3, 2])
                    {
                        spriteBatch.DrawString(bold, items[i].name, new Vector2(grid[x, y].X - items[i].name.Length * 2, grid[x, y].Y + 110), Color.Black);
                    }
                    else if (items[i].name.Equals("Red Headband") && selection == grid[3, 1])
                    {
                        spriteBatch.DrawString(bold, items[i].name, new Vector2(grid[x, y].X - items[i].name.Length * 2, grid[x, y].Y + 110), Color.Black);
                    }
                    else if (selectedItem[i] != true)
                        spriteBatch.DrawString(font, items[i].name, new Vector2(grid[x, y].X - items[i].name.Length * 2 + temp, grid[x, y].Y + 110), Color.Black);
                    else
                    {
                        if (!items[i].name.Equals("Blue Headband"))
                            spriteBatch.DrawString(bold, items[i].name, new Vector2(grid[x, y].X - items[i].name.Length * 2 + temp, grid[x, y].Y + 110), Color.Black);
                        else
                            spriteBatch.DrawString(font, items[i].name, new Vector2(grid[x, y].X - items[i].name.Length * 2, grid[x, y].Y + 110), Color.Black);
                    }

                }
                y++;



                if (items[i].name.Equals("Can't Die") || items[i].name.Equals("Coin"))
                {
                    items[i].AnimateReverse();
                }
                else
                {
                    items[i].AnimateLinear();
                }

            }




        }
    }
}
