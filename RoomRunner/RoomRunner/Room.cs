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
        public List<Enemy> enemyArray;
        public Random rand;

        public Rectangle backgroundRectangle;


        // intended for single images
        public Room(Texture2D background, Rectangle backgroundRectangle, int numberOfEnemies, GraphicsDevice graphics, ContentManager content)
        {
            background1 = background;
            background2 = background;
            this.backgroundRectangle = backgroundRectangle;
            this.numberOfEnemies = numberOfEnemies;
            enemyArray = new List<Enemy>();
            rand = new Random(DateTime.Now.Millisecond);

            for(int i = 0; i < numberOfEnemies; i++)
            {
                enemyArray.Add(new Enemy((EnemyName)rand.Next(0, 5), content, graphics, new Rectangle(rand.Next(1500, 3000), rand.Next(200, 500), 100, 100)));
            }

        }

        //private void generateEnemies(int amount, )


        public void Update(int scrollSpeed)
        {
            backgroundRectangle.X -= scrollSpeed;
            List<Enemy> toRemove = new List<Enemy>();

            foreach(Enemy enemy in enemyArray)
            {
                enemy.Update();
                enemy.rectangle.X -= scrollSpeed;
                if(enemy.rectangle.X + enemy.rectangle.Width < 0)
                {
                    toRemove.Add(enemy);
                }

            }
            foreach (Enemy e in toRemove)
                enemyArray.Remove(e);
            


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
