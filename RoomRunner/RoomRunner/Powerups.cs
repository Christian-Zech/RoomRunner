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
        public List<PowerupItem> items;
        public List<int> quantities;

        public Powerups()
        {
            items = new List<PowerupItem> { new PowerupItem("Time Control", 400, false), new PowerupItem("Can't Die", 600, false), new PowerupItem("Instakill", 10, false), new PowerupItem("Magnet", 600, false) };
            quantities = new List<int> { 1, 1, 1, 1 };
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
                items[index].Activate();
                
            }
        }
        public void Update()
        {
            if (ActivePowerups())
            {
                items[ActivePowerupsIndex()].Activate();
                Console.WriteLine("powerup is Active");
            }
        }

        public bool ActivePowerups()
        {
            for (int i = 0; i < 4; i++)
            {
                if (items[i].active)
                    return true;
            }
            return false;
        }
        public int ActivePowerupsIndex()
        {
            for (int i = 0; i < 4; i++)
            {
                if (items[i].active)
                    return i;
            }
            return -1;
        }
        public void RemovePowerups()
        {
            for (int i = 0; i < 4; i++)
            {
                if (items[i].active)
                    items[i].active = false;
            }
        }
        public void Draw(SpriteBatch spriteBatch, Texture2D textures, List<Rectangle> clock, List<Rectangle> skull, List<Rectangle> nuke, List<Rectangle> magnet, SpriteFont font)
        {
            spriteBatch.Draw(textures, new Rectangle(50, 40, 70, 70), clock[0], Color.White);
            spriteBatch.Draw(textures, new Rectangle(150, 40, 70, 70), skull[0], Color.White);
            spriteBatch.Draw(textures, new Rectangle(250, 40, 70, 70), nuke[0], Color.White);
            spriteBatch.Draw(textures, new Rectangle(350, 40, 70, 70), magnet[0], Color.White);
        }
    }
}
