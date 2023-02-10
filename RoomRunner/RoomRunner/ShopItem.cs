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
    class ShopItem
    {
        public int price;
        public string name;
        public Rectangle rect;
        public Texture2D tex;
        public ShopItem(int p, string n, Rectangle r, Texture2D t)
        {
            price = p;
            name = n;
            rect = r;
            tex = t;
        }
    }
}
