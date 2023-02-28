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
        public bool active;
        public PowerupItem(string name, int duration, bool active)
        {
            id = name;
            this.duration = duration;
            this.active = active;
        }
        public void Activate()
        {
            if (active = true)
                duration--;
            else
                active = true;

            if (duration <= 0)
                active = false;
        }
    }
}
