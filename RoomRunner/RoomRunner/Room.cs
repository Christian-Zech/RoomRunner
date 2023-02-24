using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{
    class Room
    {


        public Texture2D background1;
        public Texture2D background2;
        public int numberOfEnemies;
        public Enemy[] enemyArray;

        public Rectangle backgroundRectangle;


        // intended for single images
        public Room(Texture2D background, Rectangle backgroundRectangle, int numberOfEnemies, GraphicsDevice graphics)
        {
            background1 = background;
            background2 = background;
            this.backgroundRectangle = backgroundRectangle;
            this.numberOfEnemies = numberOfEnemies;
            enemyArray = new Enemy[numberOfEnemies];

            for(int i = 0; i < enemyArray.Length; i++)
            {
                enemyArray[i] = new Enemy();
            }

        }




        public void Update(int scrollSpeed)
        {
            backgroundRectangle.X -= scrollSpeed;

            foreach(Enemy enemy in enemyArray)
            {
                enemy.Update();
            }

        }

        
        



    }
}
