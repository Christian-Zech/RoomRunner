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
    class Powerups
    {
        List<PowerupItem> powerups;
        List<int> quantities;

        public Powerups()
        {
            powerups = new List<PowerupItem> { new PowerupItem("Time Control", 10, false), new PowerupItem("Can't Die", 10, false), new PowerupItem("Instakill", 1, false), new PowerupItem("Magnet", 10, false) };
            quantities = new List<int> { 0, 0, 0, 0 };
        }
        public void AddPowerup(int index)
        {
            quantities[index]++;
        }
        public void UsePowerup(int index)
        {
            if (quantities[index] > 0 && !ActivePowerups())
            {
                quantities[index]--;
                powerups[index].Activate();
            }
        }

        public bool ActivePowerups()
        {
            for (int i = 0; i < 4; i++)
            {
                if (powerups[i].active)
                    return false;
            }
            return true;
        }
    }
}
