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

        //for shop
        Texture2D collectableSheet, cosmeticSheet; 
        List<ShopItem> items;
        List<Rectangle> clock, skull, nuke, magnet, coin, skiMask, construction, hair, headphones, santa, headband, fire, army, redBand, blueBand;
        SpriteFont shopFont;
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
            graphics.PreferredBackBufferWidth = 1900;
            graphics.PreferredBackBufferHeight = 700;
            
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
            nuke = new List<Rectangle> { new Rectangle(96, 64, 32, 32), new Rectangle(128, 64, 32, 32), new Rectangle(0, 96, 32, 32), new Rectangle(32, 96, 32, 32), new Rectangle(64, 96, 32, 32), new Rectangle(96, 96, 32, 32), new Rectangle(128, 96, 32, 32), new Rectangle(0, 128, 32, 32) };
            magnet = new List<Rectangle> { new Rectangle(32, 128, 32, 32), new Rectangle(64, 128, 32, 32), new Rectangle(96, 128, 32, 32), new Rectangle(128, 128, 32, 32) };
            coin = new List<Rectangle> { new Rectangle(0, 160, 32, 32), new Rectangle(32, 160, 32, 32), new Rectangle(64, 160, 32, 32), new Rectangle(96, 160, 32, 32) };
            skiMask = new List<Rectangle> { new Rectangle(0, 0, 32, 32)};
            construction = new List<Rectangle> { new Rectangle(64, 0, 32, 32) };
            hair = new List<Rectangle> { new Rectangle(128, 0, 32, 32) };
            headphones = new List<Rectangle> { new Rectangle(32, 32, 32, 32) };
            santa = new List<Rectangle> { new Rectangle(96, 32, 32, 32) };
            headband = new List<Rectangle> { new Rectangle(0, 64, 32, 32) };
            fire = new List<Rectangle> { new Rectangle(64, 64, 32, 32), new Rectangle(128, 64, 32, 32), new Rectangle(32, 96, 32, 32) };
            army = new List<Rectangle> { new Rectangle(96, 96, 32, 32) };
            redBand = new List<Rectangle> { new Rectangle(0, 128, 32, 32) };
            blueBand = new List<Rectangle> { new Rectangle(64, 128, 32, 32) };
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
            shopFont = this.Content.Load<SpriteFont>("SpriteFont1");

            //for shop, textures have to be loaded first before they can be sent as parameters
            items.Add(new ShopItem(50, "Time Control", clock, collectableSheet));
            items.Add(new ShopItem(50, "Can't Die", skull, collectableSheet));
            items.Add(new ShopItem(50, "Instakill", nuke, collectableSheet));
            items.Add(new ShopItem(50, "Magnet", magnet, collectableSheet));
            items.Add(new ShopItem(50, "Coin", coin, collectableSheet));
            items.Add(new ShopItem(50, "Ski Mask", skiMask, cosmeticSheet));
            items.Add(new ShopItem(50, "Construction Hat", construction, cosmeticSheet));
            items.Add(new ShopItem(50, "Hair", hair, cosmeticSheet));
            items.Add(new ShopItem(50, "Headphones", headphones, cosmeticSheet));
            items.Add(new ShopItem(50, "Santa Hat", santa, cosmeticSheet));
            items.Add(new ShopItem(50, "Headband", headband, cosmeticSheet));
            items.Add(new ShopItem(50, "Fire", fire, cosmeticSheet));
            items.Add(new ShopItem(50, "Army Hat", army, cosmeticSheet));
            items.Add(new ShopItem(50, "Red Headband", redBand, cosmeticSheet));
            items.Add(new ShopItem(50, "Blue Headband", blueBand, cosmeticSheet));
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
            GraphicsDevice.Clear(Color.Gray);
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            if (gameState == GameState.Shop)
            {
                shop.Draw(gameTime, spriteBatch, shopFont);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
