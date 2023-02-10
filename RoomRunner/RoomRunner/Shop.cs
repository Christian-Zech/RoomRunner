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
        public Shop(List<ShopItem> itemList)
        {
            items = itemList;
        }
        public void Buy(ShopItem item, Player jeb)
        {
            //if jeb's money is greater than or equal to the items price
            //add it to players item list and subtract their money
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //will make this in a 2d grid style, just can't do it without being able to test what it looks like
            for (int i = 0; i < items.Count; i++)
            {
                spriteBatch.Draw(items[i].tex, items[i].rect, Color.White);
            }
        }
    }
}
