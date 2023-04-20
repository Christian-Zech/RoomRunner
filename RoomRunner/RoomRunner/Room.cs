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
        public List<Coin[,]> coinsGridList;
        public List<ProjectileClump> obstacleList;

        public Rectangle backgroundRectangle;

        private GraphicsDevice graphics;
        private ContentManager content;
        public string collectablesPath = "Shop/collectables";


        public enum CoinPattern
        {
            Straight,
            Zigzag,
            Column,
            Block
        }
        

        // Each room operates as its own entity where it will have its own background, enemies, and coins. Game1 has a roomList variable that stores all of the rooms.
        public Room(Texture2D background, Rectangle backgroundRectangle, int numberOfEnemies, GraphicsDevice graphics, ContentManager content, Rectangle window)
        {
            background1 = background;
            background2 = background;
            this.backgroundRectangle = backgroundRectangle;
            this.numberOfEnemies = numberOfEnemies;
            this.graphics = graphics;
            this.content = content;
            enemyArray = new List<Enemy>();
            obstacleList = new List<ProjectileClump>();

            rand = new Random();

            // determines amount of coin patches in the room
            coinsGridList = new List<Coin[,]>();
            for(int i = 0; i < rand.Next(2,6); i++)
            {
                int amountOfCoins = rand.Next(5, 10);
                coinsGridList.Add(new Coin[amountOfCoins,amountOfCoins]);
            }

            generateEnemies(numberOfEnemies);

            // generates random amount of coins in a random pattern per coin patch
            for(int i = 0; i < coinsGridList.Count; i++)
            {
                generateCoins(coinsGridList[i], (CoinPattern)rand.Next(0, 5), window);
            }

            RemoveCoinOverLap();

            // generates room obstacles
            GenerateObstacles(rand.Next(4, 8));

        }

        // generates enemies. Call this once and the room will have enemies.
        private void generateEnemies(int amount)
        {
            Enemy.totalEnemyCount += amount;
            while (amount-- > 0) //same thing as: for(int i=0;i<amount;i++)
                enemyArray.Add(new Enemy((EnemyName)rand.Next(0, Enemy.EnemyNames), content, graphics, new Rectangle(rand.Next(2000, 4000), rand.Next(Player.frameHeight - ceilingHeight, Player.frameHeight - floorHeight - 100), 100, 100)));
            RemoveOverlap();
        }

        // generates 1 coin patch. Call this multiple times to get lots of coins
        private void generateCoins(Coin[,] coinsGrid, CoinPattern pattern, Rectangle window)
        {
            int coinGap = 50; // seperation between coins (pixels)

            Rectangle startRectangle = new Rectangle(rand.Next(window.Width, window.Width + 5000), rand.Next(100, Player.frameHeight - floorHeight - (coinsGrid.GetLength(0) * coinGap)), 50, 50);

            // generates each coin pattern. For future reference, GetLength(0) = rows and GetLength(1) = columns
            switch (pattern)
            {
                case CoinPattern.Straight:

                    for (int i = 0; i < coinsGrid.GetLength(1); i++)
                    {
                        coinsGrid[0, i] = new Coin(new Rectangle(startRectangle.X + (i * coinGap), startRectangle.Y, startRectangle.Width, startRectangle.Height), content.Load<Texture2D>(collectablesPath), graphics);
                    }
                    break;

                case CoinPattern.Zigzag:
                    for (int i = 0; i < coinsGrid.GetLength(0); i++)
                    {
                        coinsGrid[i, i] = new Coin(new Rectangle(startRectangle.X + (i * coinGap), startRectangle.Y + (i * coinGap), startRectangle.Width, startRectangle.Height), content.Load<Texture2D>(collectablesPath), graphics);
                        coinsGrid[coinsGrid.GetLength(0) - i - 1, i] = new Coin(new Rectangle(startRectangle.X + (i * coinGap), startRectangle.Y + ((coinsGrid.GetLength(0) - i - 1) * coinGap), startRectangle.Width, startRectangle.Height), content.Load<Texture2D>(collectablesPath), graphics);
                    }
                    break;

                case CoinPattern.Column:
                    for (int i = 0; i < coinsGrid.GetLength(1); i++)
                    {
                        coinsGrid[i, coinsGrid.GetLength(1) - i - 1] = new Coin(new Rectangle(startRectangle.X, startRectangle.Y + (i * coinGap), startRectangle.Width, startRectangle.Height), content.Load<Texture2D>(collectablesPath), graphics);
                    }
                    break;

                case CoinPattern.Block:
                    for (int row = 0; row < coinsGrid.GetLength(0); row++)
                    {
                        for (int column = 0; column < coinsGrid.GetLength(1); column++)
                        {
                            coinsGrid[row, column] = new Coin(new Rectangle(startRectangle.X + (column * coinGap), startRectangle.Y + (row * coinGap), startRectangle.Width, startRectangle.Height), content.Load<Texture2D>(collectablesPath), graphics);
                        }
                    }
                    break;
            }
        }


        // generates obstacles for the room. Call once and forget.
        private void GenerateObstacles(int amountOfObstacles)
        {


            Rectangle[] frameRectangles = Player.LoadSheet(3, 3, 32, 32);
            for (int i = 0; i < amountOfObstacles; i++)
            {
                int height = rand.Next(100, 300);
                int width = (int)(height / 2.5);
                obstacleList.Add(new ProjectileClump(false, false, new Projectile(true, new Rectangle(rand.Next(Game1.window.Width, Game1.window.Width * 4), rand.Next(Player.frameHeight - ceilingHeight, Player.frameHeight - floorHeight - height), width, height), 1, new Point(0, 0),
                    new OnetimeAnimation(15, graphics, Program.Game.Content.Load<Texture2D>("Level1/Enemies/Obstacles"), frameRectangles.Take(5).ToArray())
                    {
                        Next = new OnetimeAnimation(1, graphics, Program.Game.Content.Load<Texture2D>("Level1/Enemies/Obstacles"), frameRectangles[4])
                    })));
                   

                obstacleList[i].Current.anim.Idle = true;
            }
        }



        // fixes an issue where enemies can "despawn" if the room count progresses.
        public void InheritEnemies(List<Enemy> toInherit)
        {
            enemyArray.AddRange(toInherit);
        }

        // fixes an issue where enemies can be spawned right on top of each other. Fixes the issue by making a list of problematic enemies and then deleting them from the main enemy list.
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

        // fixes an issue where coincs can spawn on top of each other
        public void RemoveCoinOverLap()
        {
            for(int i = 0; i < coinsGridList.Count; i++)
            {
                for(int j = i+1; j < coinsGridList.Count; j++)
                {
                    foreach(Coin coin in coinsGridList[i])
                    {
                        foreach(Coin coin2 in coinsGridList[j])
                        {
                            if(coin != null && coin2 != null && coin.rectangle.Intersects(coin2.rectangle))
                            {
                                coin.Destroy();

                            }
                        }
                    }
                }

                
            }
        }

        public void RemoveObstacleOverLap()
        {
            for(int i = 0; i < obstacleList.Count; i++)
            {
                for(int j = i+1; j < obstacleList.Count; j++)
                {
                    if (obstacleList[i].Current.Rectangle.Intersects(obstacleList[j].Current.Rectangle))
                        obstacleList.RemoveAt(i);
                }
            }
        }

        // makes the game scroll by moving the background to the left. Also controls enemies and coins.
        public void Update(int scrollSpeed)
        {
            backgroundRectangle.X -= scrollSpeed;
            if (Game1.bossFight)
                return;
            List<Enemy> toRemove = new List<Enemy>();

            

            foreach (Enemy enemy in enemyArray)
            {
                enemy.Update();
                enemy.rectangle.X -= scrollSpeed;
                if (enemy.rectangle.X + enemy.rectangle.Width < 0)
                {
                    toRemove.Add(enemy);
                    Enemy.totalEnemyCount--;
                }

            }

            foreach (Coin[,] coinGrid in coinsGridList)
            {
                foreach (Coin coin in coinGrid)
                {
                    if (coin != null)
                    {
                        if (Program.Game.activePowerupIndex != 3) // 3rd index = magnet powerup
                            coin.Position.X -= scrollSpeed;
                        coin.Update();
                    }
                }
            }


            for(int i = 0; i < obstacleList.Count; i++)
            {
                ProjectileClump obstacle = obstacleList[i];

                if(obstacle.Delete)
                {
                    obstacleList.RemoveAt(i);
                    i--;
                    continue;
                }


                obstacle.Current.Velocity.X = -scrollSpeed;
                if (obstacle.Current.Rectangle.Intersects(Game1.window))
                    obstacle.Current.anim.Idle = false;
                
            }



            foreach (Enemy e in toRemove)
                enemyArray.Remove(e);
            generateEnemies(toRemove.Count);
            if (enemyArray.Count < minimumNumOfEnemies)
                generateEnemies(minimumNumOfEnemies - enemyArray.Count);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Enemy enemy in enemyArray)
            {
                spriteBatch.Draw(enemy.CurrentTexture, enemy.rectangle, Color.White);
            }
            foreach(Coin[,] coinGrid in coinsGridList)
            {
                foreach(Coin coin in coinGrid)
                {
                    if (coin != null)
                        spriteBatch.Draw(coin.CurrentTexture, coin.rectangle, Color.White);
                }
            }

            foreach (ProjectileClump obstacle in obstacleList)
            {
                obstacle.DrawAndUpdate(spriteBatch);
            }



        }




    }
}