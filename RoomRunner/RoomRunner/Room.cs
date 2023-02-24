using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        public Random rand;

        public Rectangle backgroundRectangle;


        // intended for single images
        public Room(Texture2D background, Rectangle backgroundRectangle, int numberOfEnemies, GraphicsDevice graphics, ContentManager content)
        {
            background1 = background;
            background2 = background;
            this.backgroundRectangle = backgroundRectangle;
            this.numberOfEnemies = numberOfEnemies;
            enemyArray = new Enemy[numberOfEnemies];
            rand = new Random();

            for(int i = 0; i < enemyArray.Length; i++)
            {
                enemyArray[i] = new Enemy(Game1.loadImage("Enemies/Jeb", content), new Rectangle(rand.Next(1000, 1500), rand.Next(300, 600), 100, 100), graphics);
            }

        }




        public void Update(int scrollSpeed, Rectangle window)
        {
            backgroundRectangle.X -= scrollSpeed;

            

            foreach(Enemy enemy in enemyArray)
            {
                enemy.Update(scrollSpeed);
                if (enemy.rectangle.X < 0)
                {
                    enemy.rectangle.X = rand.Next(2000, 2500);
                    enemy.rectangle.Y = rand.Next(300, 600);
                }

                

            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Enemy enemy in enemyArray)
            {
                spriteBatch.Draw(enemy.CurrentTexture, enemy.rectangle, Color.White);
            }
        }
        
        


    }
}
