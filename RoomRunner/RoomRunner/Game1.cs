using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RoomRunner
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D pixel;
        Texture2D jebSheet;

        List<Texture2D> backgroundImages = new List<Texture2D>();

        SpriteFont menuFont;
        SpriteFont buttonFont;

        List<Rectangle> jebList;
        List<Rectangle> idleAnimationRectangles;
        Rectangle startButtonRectangle;
        Rectangle shopButtonRectangle;

        Rectangle window;
        private Player jeb;

        List<Room> roomList;
        int amountOfRooms;

        int gameTimer;
        int levelTimer;
        int currentRoom;
        int scrollSpeed;

        Random rand;

        string[] backgroundFiles;


        enum GameState
        {
            Menu,
            Shop,
            Play,
            GameOver
        }
        
        enum Levels
        {
            Level1,
            Level2,
            Level3
        }

        Levels levels;
        GameState gameState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1900;
            graphics.PreferredBackBufferHeight = 1000;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            roomList = new List<Room>();
            jebList = new List<Rectangle>();
            idleAnimationRectangles = new List<Rectangle>();
            rand = new Random();


            amountOfRooms = 5;
            scrollSpeed = 5;
            

            gameState = GameState.Menu;
            levels = Levels.Level1;
            gameTimer = 0;
            levelTimer = 0;
            currentRoom = 0;

            this.IsMouseVisible = true;

            window = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            jebList.Add(new Rectangle(0, 0, 32, 32));
            jebList.Add(new Rectangle(32, 0, 32, 32));
            jebList.Add(new Rectangle(0, 32, 32, 32));
            jebList.Add(new Rectangle(32, 32, 32, 32));
            jebList.Add(new Rectangle(0, 64, 32, 32));

            idleAnimationRectangles.Add(jebList[3]);
            idleAnimationRectangles.Add(jebList[4]);

            jeb = new Player(new Vector2(900, 500), Content, GraphicsDevice);

            startButtonRectangle = new Rectangle(window.Width / 2 - 140, 400, 350, 100);
            shopButtonRectangle = new Rectangle(startButtonRectangle.X, startButtonRectangle.Y + 200, startButtonRectangle.Width, startButtonRectangle.Height);



            // reads background images
            backgroundFiles = Directory.GetFiles(@"Content\" + levels + "/Background/", "*");

            int i = 0;

            foreach(var File in backgroundFiles)
            {
                string[] Temp;
                Temp = File.Split('.');
                string NameMinus = Temp[0];
                int Index = NameMinus.LastIndexOf('\\') + 1;
                NameMinus = NameMinus.Substring(Index);
                


                backgroundFiles[i] = NameMinus;
                i++;

            }


            base.Initialize();
            


        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            pixel = this.Content.Load<Texture2D>("Level1/Background/pixel");
            jebSheet = this.Content.Load<Texture2D>("jeb");
            menuFont = this.Content.Load<SpriteFont>("SpriteFonts/menuFont");
            buttonFont = this.Content.Load<SpriteFont>("SpriteFonts/buttonFont");


            
            foreach (string file in backgroundFiles)
            {
                backgroundImages.Add(this.Content.Load<Texture2D>(@".\" + file));
            }

            GenerateRoom(amountOfRooms, backgroundImages, window);


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                this.Exit();


            if (mouse.LeftButton == ButtonState.Pressed && CheckForCollision(mouse.X, mouse.Y, startButtonRectangle))
                gameState = GameState.Play;

            if (mouse.LeftButton == ButtonState.Pressed && CheckForCollision(mouse.X, mouse.Y, shopButtonRectangle))
                gameState = GameState.Shop;



            if(gameState == GameState.Play)
                roomList[currentRoom].Update(scrollSpeed);

            // TODO: Add your update logic here

            jeb.Update();
            gameTimer++;
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            jeb.Draw(spriteBatch);


            


            if(gameState == GameState.Menu)
            {
                int halfSeconds = gameTimer / 30;
                Rectangle playerIdleDimensions = new Rectangle(window.Width / 2 - 20, 100, 100, 100);
                
                Vector2 titlePosition = new Vector2(window.Width / 2 - 220, 200);


                // animation
                if (halfSeconds % 2 == 0)
                    spriteBatch.Draw(jebSheet, playerIdleDimensions, idleAnimationRectangles[0], Color.White);
                else
                    spriteBatch.Draw(jebSheet, playerIdleDimensions, idleAnimationRectangles[1], Color.White);
        
                spriteBatch.DrawString(menuFont, "Welcome to Room Runner!", titlePosition, Color.White);


                // menu buttons

                
                spriteBatch.Draw(pixel, startButtonRectangle, Color.Green);
                spriteBatch.DrawString(buttonFont, "Start", new Vector2(startButtonRectangle.X + 110, startButtonRectangle.Y + 20), Color.White);


                spriteBatch.Draw(pixel, shopButtonRectangle, Color.Green);
                spriteBatch.DrawString(buttonFont, "Enter Shop", new Vector2(shopButtonRectangle.X + 50, shopButtonRectangle.Y + 20), Color.White);



            }

            if(gameState == GameState.Play)
            {
                levelTimer++;
                int levelSeconds = levelTimer / 60;
                
                // advances to next room every 10 seconds

                if (levelTimer > 60 && levelSeconds % 10 == 0)
                {
                    //currentRoom++;
                }


                // scrolling calculations

                Rectangle roomRectangle = roomList[currentRoom].backgroundRectangle;

                if (roomRectangle.X < -((window.Width * 2) - window.Right - 10))
                    roomList[currentRoom].backgroundRectangle.X = 0;


                

                spriteBatch.Draw(roomList[currentRoom].background1, roomRectangle, Color.White);
                spriteBatch.Draw(roomList[currentRoom].background2, new Rectangle(roomRectangle.Right, 0, roomRectangle.Width, roomRectangle.Height), Color.White);


            }

            


            spriteBatch.End();

            base.Draw(gameTime);
        }


        public bool CheckForCollision(int x, int y, Rectangle inputRectangle)
        {
            if (x < inputRectangle.Right && x > inputRectangle.Left && y < inputRectangle.Bottom && y > inputRectangle.Top)
                return true;

            return false;
        }

        public void GenerateRoom(int amountOfRooms, List<Texture2D> textures, Rectangle dimensions)
        {

            roomList.Clear();

            for(int i = 0; i < amountOfRooms; i++)
            {

                roomList.Add(new Room(textures[rand.Next(0, textures.Count)], dimensions));
            }

            
        }


    }
}
