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
    
    class PowerupItem
    {
        public string id;
        public int duration;
        int durationTemp;
        public bool active;
        public int count;
        public PowerupItem(string name, int duration, bool active)
        {
            id = name;
            this.duration = duration;
            durationTemp = duration;
            this.active = active;
            count = 0;
        }
        public void Activate()
        {
            if (active = true)
                duration--;
            else
                active = true;

            if (duration <= 0)
            {
                active = false;
                duration = durationTemp;
            }
                
        }
        public double AnimateLinear(List<Rectangle> frames, double currentFrameIndex)
        {
            currentFrameIndex += 0.09;
            if (currentFrameIndex > frames.Count)
            {
                currentFrameIndex = 0;
            }
            return currentFrameIndex;
        }
        public double AnimateReverse(List<Rectangle> frames, double currentFrameIndex)
        {
            if (count == 0)
            {
                currentFrameIndex += 0.09;
                if (currentFrameIndex + .6 > frames.Count)
                {
                    count = 1;
                }
            }
            else if (count == 1)
            {
                currentFrameIndex -= 0.09;
                if (currentFrameIndex - .6 < 0)
                {
                    count = 0;
                }
            }
            return currentFrameIndex;

        }
    }
}
