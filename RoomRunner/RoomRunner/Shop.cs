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
        int timeBuffer;
        public List<ShopItem> items;
        public List<bool> selectedItem;
        public Rectangle[,] grid;
        public int[,] boughtItems; //0 for nothing, 1 for bought, 2 for equipped
        public Rectangle selection; //highlights background of currently selected item
        public Rectangle backButton;
        public bool leave;
        int mouseX, mouseY, selectionIndexX, selectionIndexY;
        List<Keys> pressedKeys, oldKeys;
        public MouseState oldMouse;
        Texture2D texture;
        Rectangle source;
        string currentHatName;
        Texture2D equippedHat;
        Rectangle equippedRect;
        Player jeb;

        public Shop(List<ShopItem> itemList, Player j, Texture2D tex, Rectangle s)
        {
            equippedRect = new Rectangle(300, 410, 100, 100);
            source = s;
            texture = tex;
            boughtItems = new int[4, 4];
            timeBuffer = 30;
            currentHatName = "None";
            jeb = j;
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
                    boughtItems[r, c] = 0;
                    grid[r, c] = new Rectangle(x, y, 80, 80);
                    x += 150;
                }
                y += 170;
                x = 650;
            }
            backButton = new Rectangle(200, 70, 250, 100);
            pressedKeys = new List<Keys>();
            oldKeys = new List<Keys>();
            selection = grid[0, 0];
            selectionIndexX = 0;
            selectionIndexY = 0;
            oldMouse = Mouse.GetState();
            
        }
        public void updateJeb(Player j) //keeping the shop's version of jeb up to date
        {
            jeb = j;
        }
        public void BuyOrEquip(ShopItem item)
        {
            if (item.name.Equals("Time Control") || item.name.Equals("Can't Die") || item.name.Equals("Instakill") || item.name.Equals("Magnet")) //powerups can only be bought
            {
                if (jeb.Coins >= item.price) //all of this works
                {
                    if (item.name.Equals("Time Control"))
                        Game1.powerups.quantities[0]++;
                    if (item.name.Equals("Can't Die"))
                        Game1.powerups.quantities[1]++;
                    if (item.name.Equals("Instakill"))
                        Game1.powerups.quantities[2]++;
                    if (item.name.Equals("Magnet"))
                        Game1.powerups.quantities[3]++;
                    jeb.Coins -= item.price;
                }
                
                return;
            }
            else //it's a cosmetic
            {
                if ((selectedItem.IndexOf(true) < 11 && jeb.ownedHats.Contains(selectedItem.IndexOf(true)-3)) || (selectedItem.IndexOf(true) == 11 && jeb.ownedHats.Contains(selectedItem.IndexOf(true) - 1)) || (selectedItem.IndexOf(true) == 13 && jeb.ownedHats.Contains(selectedItem.IndexOf(true) - 2)) || (selectedItem.IndexOf(true) == 14 && jeb.ownedHats.Contains(selectedItem.IndexOf(true) - 2))) //already owned
                {
                    for (int r = 0; r < 4; r++)
                    {
                        for (int c = 0; c < 4; c++)
                        {
                            if (boughtItems[r,c] == 2)
                            {
                                boughtItems[r, c] = 1;
                                break;
                            }
                        }
                    }
                    boughtItems[selectionIndexX, selectionIndexY] = 2;
                    if (selectedItem.IndexOf(true) < 11)
                        jeb.currentHat = (PlayerHats)selectedItem.IndexOf(true)-3;
                    else if (selectedItem.IndexOf(true) == 11)
                        jeb.currentHat = (PlayerHats)selectedItem.IndexOf(true) - 1;
                    else
                        jeb.currentHat = (PlayerHats)selectedItem.IndexOf(true) - 2;
                }
                else
                {
                    if (jeb.Coins >= item.price) //has enough coins to buy
                    {
                        for (int r = 0; r < 4; r++)
                        {
                            for (int c = 0; c < 4; c++)
                            {
                                if (boughtItems[r, c] == 2)
                                {
                                    boughtItems[r, c] = 1;
                                }
                            }
                        }
                        boughtItems[selectionIndexX, selectionIndexY] = 2;
                        jeb.Coins -= item.price;
                        //special cases because when i first set up the selection index i was stupid
                        if (selectedItem.IndexOf(true) == 11)
                        {
                            jeb.ownedHats.Add(selectedItem.IndexOf(true) - 1);
                            jeb.currentHat = (PlayerHats)selectedItem.IndexOf(true) - 1;
                        }
                        else if (selectedItem.IndexOf(true) == 13)
                        {
                            jeb.ownedHats.Add(selectedItem.IndexOf(true) - 2);
                            jeb.currentHat = (PlayerHats)selectedItem.IndexOf(true) - 2;
                        }
                        else if (selectedItem.IndexOf(true) == 14)
                        {
                            jeb.ownedHats.Add(selectedItem.IndexOf(true) - 2);
                            jeb.currentHat = (PlayerHats)selectedItem.IndexOf(true) - 2;
                        }
                        else
                        {
                            jeb.ownedHats.Add(selectedItem.IndexOf(true) - 3);
                            jeb.currentHat = (PlayerHats)selectedItem.IndexOf(true) - 3;
                        }
                            

                    }
                }
            }
            equippedHat = Player.Hats[jeb.currentHat];
            
        }
        
        public void updateSelection()
        {
            if (timeBuffer != 0)
                timeBuffer--;
                
            oldKeys = pressedKeys;
            MouseState mouse = Mouse.GetState();
            KeyboardState kb = Keyboard.GetState();
            mouseX = mouse.X;
            mouseY = mouse.Y;
            pressedKeys = kb.GetPressedKeys().ToList();
            
            Rectangle temp = new Rectangle(mouseX-15, mouseY-15, 30, 30);
            if (temp.Intersects(backButton) && mouse.LeftButton == ButtonState.Pressed)
            {
                timeBuffer = 30;
                Game1.gameState = Game1.GameState.Menu;
            }
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
                            if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released && timeBuffer == 0)
                            {
                                int index = selectedItem.IndexOf(true);
                                BuyOrEquip(items[index]);
                            }
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
                                if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                                {
                                    int index = selectedItem.IndexOf(true);
                                    BuyOrEquip(items[index]);
                                }
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

            
            if (pressedKeys.Contains(Keys.Space) && !oldKeys.Contains(Keys.Space))
            {
                int index = selectedItem.IndexOf(true);
                BuyOrEquip(items[index]);
            }

            oldMouse = mouse;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font, SpriteFont bold, SpriteFont title, Texture2D pixel)
        {
            updateSelection();
            spriteBatch.Draw(texture, new Rectangle(300, 410, 100, 100), source, Color.White);
            if (equippedHat != null)
                spriteBatch.Draw(equippedHat, equippedRect, new Rectangle(0, 0, 32, 32), Color.White);
            string[] name = currentHatName.Split('_');
            if (name[0].Contains("1") || name[0].Contains("2") || name[0].Contains("3"))
            {
                name[0] = name[0].Substring(0, name[0].Length - 1);
            }
            if (name.Length > 1)
                spriteBatch.DrawString(font, "Equipped: " + name[0] + " " + name[1], new Vector2(290-(3*(name[0].Length+name[1].Length)), 540), Color.Black);
            else
                spriteBatch.DrawString(font, "Equipped: " + name[0], new Vector2(290-3*name[0].Length, 540), Color.Black);
            spriteBatch.Draw(pixel, new Rectangle(selection.X - 10, selection.Y - 10, 90, 100), new Color(200, 200, 200, 255));

            spriteBatch.Draw(pixel, backButton, Color.Green);
            spriteBatch.DrawString(bold, "Menu", new Vector2(backButton.X + 95, backButton.Y + 30), Color.White);

            int x = 0;
            int y = 0;
            int temp = 0;
            spriteBatch.DrawString(title, "SHOP", new Vector2(780, 40), Color.Black);
            spriteBatch.DrawString(font, "(space/click to buy)", new Vector2(790, 170), Color.Black);

            //drawing green background for owned items
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    if (boughtItems[r, c] == 1)
                        spriteBatch.Draw(pixel, new Rectangle(grid[r, c].X-10, grid[r,c].Y-10, grid[r,c].Width+10, grid[r,c].Height+20), new Color(0, 60, 0, 110));
                    if (boughtItems[r, c] == 2)
                        spriteBatch.Draw(pixel, new Rectangle(grid[r, c].X - 10, grid[r, c].Y - 10, grid[r, c].Width + 10, grid[r, c].Height + 20), new Color(0, 60, 0, 160));
                    Console.WriteLine(boughtItems[r, c]);
                }
            }

            //drawing items n stuff
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
                    spriteBatch.Draw(items[i].tex, new Rectangle(1500, 120, 60, 60), items[i].sourceRects[(int)items[i].currentFrameIndex], Color.White);
                    spriteBatch.DrawString(font, "   Coins: " + jeb.Coins, new Vector2(1450, 200), Color.Black);
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
