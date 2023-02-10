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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D pixel;
        Texture2D jebSheet;

        SpriteFont menuFont;
        SpriteFont buttonFont;

        List<Rectangle> jebList = new List<Rectangle>();
        List<Rectangle> idleAnimationRectangles = new List<Rectangle>();
        Rectangle startButtonRectangle;

        Rectangle window;


        int count;


        enum GameState
        {
            Menu,
            Shop,
            Play,
            GameOver

        }

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
            gameState = GameState.Menu;
            count = 0;

            this.IsMouseVisible = true;

            window = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            jebList.Add(new Rectangle(0, 0, 32, 32));
            jebList.Add(new Rectangle(32, 0, 32, 32));
            jebList.Add(new Rectangle(0, 32, 32, 32));
            jebList.Add(new Rectangle(32, 32, 32, 32));
            jebList.Add(new Rectangle(0, 64, 32, 32));

            idleAnimationRectangles.Add(jebList[3]);
            idleAnimationRectangles.Add(jebList[4]);

            startButtonRectangle = new Rectangle(window.Width / 2 - 100, 400, 350, 100);

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
            pixel = this.Content.Load<Texture2D>("pixel");
            jebSheet = this.Content.Load<Texture2D>("jeb");
            menuFont = this.Content.Load<SpriteFont>("SpriteFonts/menuFont");
            buttonFont = this.Content.Load<SpriteFont>("SpriteFonts/buttonFont");
            
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


            if (mouse.LeftButton == ButtonState.Pressed && mouse.X < startButtonRectangle.Right && mouse.X > startButtonRectangle.Left && mouse.Y < startButtonRectangle.Bottom && mouse.Y > startButtonRectangle.Top)
                gameState = GameState.Play;

            // TODO: Add your update logic here

            count++;
            

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


            


            if(gameState == GameState.Menu)
            {
                int halfSeconds = count / 30;
                Rectangle playerIdleDimensions = new Rectangle(window.Width / 2 + 20, 100, 100, 100);
                Vector2 titlePosition = new Vector2(window.Width / 2 - 200, 200);


                // animation
                if (halfSeconds % 2 == 0)
                    spriteBatch.Draw(jebSheet, playerIdleDimensions, idleAnimationRectangles[0], Color.White);
                else
                    spriteBatch.Draw(jebSheet, playerIdleDimensions, idleAnimationRectangles[1], Color.White);
        
                spriteBatch.DrawString(menuFont, "Welcome to Room Runner!", titlePosition, Color.White);


                // menu buttons

                
                spriteBatch.Draw(pixel, startButtonRectangle, Color.Green);
                spriteBatch.DrawString(buttonFont, "Start", new Vector2(startButtonRectangle.X + 100, startButtonRectangle.Y + 20), Color.White);



            }






            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
