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
        public Rectangle selection; //highlights background of currently selected item

        public Shop(List<ShopItem> itemList)
        {
            items = itemList;
        }
        //public void Buy(ShopItem item, Player jeb)
        //{
            //if jeb's money is greater than or equal to the items price
            //add it to players item list and subtract their money
        //}
        public void updateSelection()
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState kb = Keyboard.GetState();
        }
        
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font)
        {
            //Title

            //Current Coins, powerups, etc.

            //Drawing Items
            int x = 50;
            int y = 50;
            int count = 0;
            for (int i = 0; i < items.Count; i++)
            {
                
                if (items[i].name.Equals("Coin"))
                {
                    spriteBatch.Draw(items[i].tex, new Rectangle(1700, 100, 60, 60), items[i].sourceRects[(int)items[i].currentFrameIndex], Color.White);
                    spriteBatch.DrawString(font, "Current coins:", new Vector2(1700, 170), Color.Black);
                }
                else
                {
                    spriteBatch.Draw(items[i].tex, new Rectangle(x, y, 80, 80), items[i].sourceRects[(int)items[i].currentFrameIndex], Color.White);
                    spriteBatch.DrawString(font, items[i].name, new Vector2(x, y + 110), Color.Black);
                }
                if (i % 4 == 0 && i != 0)
                {
                    y += 150;
                    x = 50;
                    count++;
                    if (count == 3)
                        x = 150;
                }
                else
                {
                    if (count == 3)
                        x += 300;
                    else
                        x += 150;
                }
                
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
