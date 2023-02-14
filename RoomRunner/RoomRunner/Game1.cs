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
        Texture2D collectableSheet, cosmeticSheet; //for shop
        List<ShopItem> items;
        List<Rectangle> clock, skull, nuke, magnet, coin;
        Shop shop;

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
            gameState = GameState.Shop;

            //for shop
            items = new List<ShopItem>();
            clock = new List<Rectangle> { new Rectangle(0, 0, 32, 32), new Rectangle(32, 0, 32, 32), new Rectangle(64, 0, 32, 32), new Rectangle(96, 0, 32, 32), new Rectangle(128, 0, 32, 32), new Rectangle(0, 32, 32, 32), new Rectangle(32, 32, 32, 32), new Rectangle(64, 32, 32, 32) };
            skull = new List<Rectangle> { new Rectangle(96, 32, 32, 32), new Rectangle(128, 32, 32, 32), new Rectangle(0, 64, 32, 32), new Rectangle(32, 64, 32, 32), new Rectangle(64, 64, 32, 32) };

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
            collectableSheet = this.Content.Load<Texture2D>("collectables");
            cosmeticSheet = this.Content.Load<Texture2D>("cosmetics");

            //for shop, textures have to be loaded first before they can be sent as parameters
            items.Add(new ShopItem(50, "Time Control", clock, collectableSheet));
            items.Add(new ShopItem(50, "Instakill", skull, collectableSheet));
            shop = new Shop(items);
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            if (gameState == GameState.Shop)
            {
                shop.Draw(gameTime, spriteBatch);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
