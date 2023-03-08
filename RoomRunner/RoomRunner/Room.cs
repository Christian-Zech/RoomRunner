using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoomRunner
{
    public class Room
    {
        public const int minimumNumOfEnemies = 5;

        public static int ceilingHeight { get { return Player.ceilingHeight; } }
        public static int floorHeight { get { return Player.floorHeight; } }

        public Texture2D background1;
        public Texture2D background2;
        public int numberOfEnemies;
        public List<Enemy> enemyArray;
        public Random rand;

        public Rectangle backgroundRectangle;

        private GraphicsDevice graphics;
        private ContentManager content;




        // intended for single images
        public Room(Texture2D background, Rectangle backgroundRectangle, int numberOfEnemies, GraphicsDevice graphics, ContentManager content)
        {
            background1 = background;
            background2 = background;
            this.backgroundRectangle = backgroundRectangle;
            this.numberOfEnemies = numberOfEnemies;
            this.graphics = graphics;
            this.content = content;
            enemyArray = new List<Enemy>();
            rand = new Random(DateTime.Now.Millisecond);

            generateEnemies(numberOfEnemies);

        }

        private void generateEnemies(int amount)
        {
            Enemy.totalEnemyCount += amount;
            while (amount-- > 0) //same thing as: for(int i=0;i<amount;i++)
            enemyArray.Add(new Enemy((EnemyName)rand.Next(0, Enemy.EnemyNames), content, graphics, new Rectangle(rand.Next(2000, 4000), rand.Next(Player.frameHeight - ceilingHeight, Player.frameHeight - floorHeight - 100), 100, 100)));
            RemoveOverlap();
        }
        public void InheritEnemies(List<Enemy> toInherit)
        {
            enemyArray.AddRange(toInherit);
        }
        private void RemoveOverlap()
        {
            List<Enemy> hasOverlap = new List<Enemy>();
            for (int i1 = 0; i1 < enemyArray.Count; i1++)
                for (int i2 = i1 + 1; i2 < enemyArray.Count; i2++)
                    if (enemyArray[i1].rectangle.Intersects(enemyArray[i2].rectangle))
                    {
                        hasOverlap.Add(enemyArray[i1]);
                        hasOverlap.Add(enemyArray[i2]);
                    }
            if (hasOverlap.Count == 0) return;
            foreach (Enemy e in hasOverlap)
                enemyArray.Remove(e);
            generateEnemies(hasOverlap.Count);
        }

        // makes the game scroll by moving the background to the left. Also controls enemies.
        public void Update(int scrollSpeed)
        {
            backgroundRectangle.X -= scrollSpeed;
            if (Game1.bossFight) 
                return;
            List<Enemy> toRemove = new List<Enemy>();

            foreach(Enemy enemy in enemyArray)
            {
                enemy.Update();
                enemy.rectangle.X -= scrollSpeed;
                if(enemy.rectangle.X + enemy.rectangle.Width < 0)
                {
                    toRemove.Add(enemy);
                    Enemy.totalEnemyCount--;
                }

            }
            foreach (Enemy e in toRemove)
                enemyArray.Remove(e);
            generateEnemies(toRemove.Count);
            if (enemyArray.Count < minimumNumOfEnemies) 
                generateEnemies(minimumNumOfEnemies - enemyArray.Count);

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
