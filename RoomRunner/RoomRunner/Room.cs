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
        public const int minimumNumOfEnemies = 5;

        public static int ceilingHeight, floorHeight;

        public Texture2D background1;
        public Texture2D background2;
        public int numberOfEnemies;
        public List<Enemy> enemyList;
        public List<Coin> coinsList;
        public Random rand;
        public Coin[,] coinsGrid;

        public Rectangle backgroundRectangle;

        private GraphicsDevice graphics;
        private ContentManager content;
        private string collectablesPath = "collectables";


        public enum CoinPattern
        {
            Straight,
            Zigzag,
            Column,
            Block
        }

        static Room()
        {
            ceilingHeight = Player.frameHeight;
            floorHeight = 0;
        }


        // intended for single images
        public Room(Texture2D background, Rectangle backgroundRectangle, int numberOfEnemies, GraphicsDevice graphics, ContentManager content, Rectangle window)
        {
            background1 = background;
            background2 = background;
            this.backgroundRectangle = backgroundRectangle;
            this.numberOfEnemies = numberOfEnemies;
            this.graphics = graphics;
            this.content = content;
            enemyList = new List<Enemy>();
            coinsList = new List<Coin>();
            rand = new Random(DateTime.Now.Millisecond);
            this.content = content;
            this.graphics = graphics;

            generateEnemies(numberOfEnemies);
            generateCoins(rand.Next(5, 30), CoinPattern.Straight, window);

        }

        private void generateEnemies(int amount)
        {
            Enemy.totalEnemyCount += amount;
            while (amount-- > 0) //same thing as: for(int i=0;i<amount;i++)
                enemyList.Add(new Enemy((EnemyName)rand.Next(0, 5), content, graphics, new Rectangle(rand.Next(2000, 4000), rand.Next(Player.frameHeight - ceilingHeight, Player.frameHeight - floorHeight - 150), 100, 100)));
            RemoveOverlap();
        }

        private void generateCoins(int amount, CoinPattern pattern, Rectangle window)
        {
            Rectangle startRectangle = new Rectangle(rand.Next(window.Width, window.Width + 1000), rand.Next(Player.frameHeight - ceilingHeight, Player.frameHeight - floorHeight - 100), 50, 50);
            coinsGrid = new Coin[amount, amount];
            int coinGap = 50; // seperation between coins (pixels)

            // generates each coin pattern. For future reference, GetLength(0) = rows and GetLength(1) = columns
            switch(pattern)
            {
                case CoinPattern.Straight:

                    for(int column = 0; column < coinsGrid.GetLength(1); column++)
                    {
                        coinsGrid[0, column] = new Coin(new Rectangle(startRectangle.X + (column * coinGap), startRectangle.Y, startRectangle.Width, startRectangle.Height), content.Load<Texture2D>(collectablesPath), graphics);
                    }
                    break;

                case CoinPattern.Zigzag:
                    for (int row = 0; row < coinsGrid.GetLength(0); row++)
                    {
                        for (int column = 0; column < coinsGrid.GetLength(1); column++)
                        {
                            
                        }
                    }
                    break;

                case CoinPattern.Column:
                    for (int row = 0; row < coinsGrid.GetLength(0); row++)
                    {
                        for (int column = 0; column < coinsGrid.GetLength(1); column++)
                        {
                            coinsGrid[coinsGrid.GetLength(0) / 2, column] = new Coin(Rectangle.Empty, content.Load<Texture2D>(collectablesPath), graphics);
                        }
                    }
                    break;

                case CoinPattern.Block:
                    for (int row = 0; row < coinsGrid.GetLength(0); row++)
                    {
                        for (int column = 0; column < coinsGrid.GetLength(1); column++)
                        {
                            coinsGrid[coinsGrid.GetLength(0) / 2, column] = new Coin(Rectangle.Empty, content.Load<Texture2D>(collectablesPath), graphics);
                        }
                    }
                    break;
            }





        }

        public void InheritEnemies(List<Enemy> toInherit)
        {
            enemyList.AddRange(toInherit);
        }
        private void RemoveOverlap()
        {
            List<Enemy> hasOverlap = new List<Enemy>();
            for (int i1 = 0; i1 < enemyList.Count; i1++)
                for (int i2 = i1 + 1; i2 < enemyList.Count; i2++)
                    if (enemyList[i1].rectangle.Intersects(enemyList[i2].rectangle))
                    {
                        hasOverlap.Add(enemyList[i1]);
                        hasOverlap.Add(enemyList[i2]);
                    }
            if (hasOverlap.Count == 0) return;
            foreach (Enemy e in hasOverlap)
                enemyList.Remove(e);
            generateEnemies(hasOverlap.Count);
        }


        public void Update(int scrollSpeed)
        {
            backgroundRectangle.X -= scrollSpeed;
            List<Enemy> toRemove = new List<Enemy>();

            foreach(Enemy enemy in enemyList)
            {
                enemy.Update();
                enemy.rectangle.X -= scrollSpeed;
                if(enemy.rectangle.X + enemy.rectangle.Width < 0)
                {
                    toRemove.Add(enemy);
                    Enemy.totalEnemyCount--;
                }

            }

            foreach (Coin coin in coinsGrid)
            {
                if (coin != null)
                {
                    coin.Update();
                    coin.rectangle.X -= scrollSpeed;
                }

            }

            foreach (Enemy e in toRemove)
                enemyList.Remove(e);
            generateEnemies(toRemove.Count);
            if (enemyList.Count < minimumNumOfEnemies) 
                generateEnemies(minimumNumOfEnemies - enemyList.Count);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Enemy enemy in enemyList)
            {
                spriteBatch.Draw(enemy.CurrentTexture, enemy.rectangle, Color.White);
            }
            foreach(Coin coin in coinsGrid)
            {
                if(coin != null)
                {
                    spriteBatch.Draw(coin.CurrentTexture, coin.rectangle, Color.White);
                }
                    
            }


        }
        



    }
}
