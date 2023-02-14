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
        public List<Rectangle> sourceRects;
        public Texture2D tex;
        public double currentFrameIndex;
        int count;

        public ShopItem(int p, string n, List<Rectangle> r, Texture2D t)
        {
            price = p;
            name = n;
            sourceRects = r;
            tex = t;
            currentFrameIndex = 0;
            count = 0;
        }
        public void AnimateLinear()
        {
            currentFrameIndex += 0.09;
            if (currentFrameIndex > sourceRects.Count-1)
            {
                currentFrameIndex = 0;
            }
        }
        public void AnimateReverse()
        {
            if (count == 0)
            {
                currentFrameIndex += 0.09;
                if (currentFrameIndex > sourceRects.Count - 1)
                {
                    count = 1;
                }
            }
            else if (count == 1)
            {
                currentFrameIndex -= 0.09;
                if (currentFrameIndex < 1)
                {
                    count = 0;
                }
            }
            
        }
    }
}
