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
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Texture2D pixel;
        public Texture2D jebSheet;

        public SpriteFont menuFont { get { return fonts[1]; } }
        public SpriteFont buttonFont { get { return fonts[0]; } }

        List<Rectangle> jebList;
        List<Rectangle> idleAnimationRectangles;
        List<Rectangle> clock, skull, nuke, magnet;
        Rectangle startButtonRectangle;
        Rectangle shopButtonRectangle;
        Rectangle MusicButtonRectangle;
        Rectangle menuButtonRectangle;

        Rectangle enemyHitBox;
        Rectangle playerHitBox;
        Rectangle coinHitBox;

        public static Rectangle window;
        public Player jeb;
        public static Boss currentBoss;
        public SpriteFont[] fonts;

        List<Room> roomList;
        public List<Projectile> projectileList;
        private int amountOfRooms;
        public static Powerups powerups;
        public int activePowerupIndex;
        int slowTimeTemp;



        private int gameTimer;
        private int levelTimer;
        private int bossCooldown;
        public int currentRoomIndex;
        public int scrollSpeed;
        public bool transition;
        public bool endCurrentRoom;
        public static bool bossFight { get { return currentBoss != null && !currentBoss.IsDead; } }
        public Dictionary<Levels, Boss> bosses;

        public Random rand;

        private List<Texture2D> backgroundImages;

        //for shop
        public Texture2D collectableSheet, cosmeticSheet;
        private List<ShopItem> items;
        public Rectangle[] collectableRect, cosmeticRect;
        public SpriteFont shopFont { get { return fonts[2]; } }
        public SpriteFont shopFontBold { get { return fonts[3]; } }
        public SpriteFont shopTitleFont { get { return fonts[4]; } }
        private Shop shop;

        public int menuCoolDown;

        KeyboardState oldKB;

        //for music and sounds
        FileDialogue files;
        MusicScreen musicScreen;
        List<SoundEffect> customSongList;
        int customSongIndex;
        List<SoundEffect> gameSongList;
        List<SoundEffectInstance> gameSongListInstance;
        public static double musicVolume;
        public static double soundVolume;
        int songTimeElapsed;
        int gameSongListIndex;
        int fileOpenCount = 0;
        bool debugMode = false;
        public static List<SoundEffect> soundEffects;

        //for cutsenes
        Cutscene cutscenes;

        public enum GameState
        {
            Menu,
            Shop,
            Play,
            Music,
            GameOver,
            Cutscene
        }
        
        public enum Levels
        {
            Level1,
            Level2,
            Level3
        }

        public static Levels levels;
        public static GameState gameState;

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

            //for shop
            clock = new List<Rectangle> { new Rectangle(0, 0, 32, 32), new Rectangle(32, 0, 32, 32), new Rectangle(64, 0, 32, 32), new Rectangle(96, 0, 32, 32), new Rectangle(128, 0, 32, 32), new Rectangle(0, 32, 32, 32), new Rectangle(32, 32, 32, 32), new Rectangle(64, 32, 32, 32) };
            skull = new List<Rectangle> { new Rectangle(96, 32, 32, 32), new Rectangle(128, 32, 32, 32), new Rectangle(0, 64, 32, 32), new Rectangle(32, 64, 32, 32), new Rectangle(64, 64, 32, 32) };
            nuke = new List<Rectangle> { new Rectangle(96, 64, 32, 32), new Rectangle(128, 64, 32, 32), new Rectangle(0, 96, 32, 32), new Rectangle(32, 96, 32, 32), new Rectangle(64, 96, 32, 32), new Rectangle(96, 96, 32, 32), new Rectangle(128, 96, 32, 32), new Rectangle(0, 128, 32, 32) };
            magnet = new List<Rectangle> { new Rectangle(32, 128, 32, 32), new Rectangle(64, 128, 32, 32), new Rectangle(96, 128, 32, 32), new Rectangle(128, 128, 32, 32) };
            

            //I'm fixing you're stupid hard-coded mess, Owen - Samuel
            items = new List<ShopItem>();
            collectableRect = Player.LoadSheet(5, 6, 32, 32, 1);
            cosmeticRect = Player.LoadSheet(5, 5, 32, 32, 1);

            bosses = new Dictionary<Levels, Boss>();
            CreateBosses();


            roomList = new List<Room>();
            jebList = new List<Rectangle>();
            projectileList = new List<Projectile>();
            idleAnimationRectangles = new List<Rectangle>();
            rand = new Random();


            amountOfRooms = 5;
            scrollSpeed = 0;
            menuCoolDown = 0;
            bossCooldown = 0;

            gameState = GameState.Menu;
            levels = Levels.Level1;
            gameTimer = 0;
            levelTimer = 0;
            currentRoomIndex = 0;

            transition = false;
            endCurrentRoom = false;

            this.IsMouseVisible = true;

            window = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            jebList.Add(Player.LoadSheet(4, 3, 32, 32)[10]);
            jebList.Add(Player.LoadSheet(4, 3, 32, 32)[11]);

            idleAnimationRectangles.Add(jebList[0]);
            idleAnimationRectangles.Add(jebList[1]);

            Player.floorHeight = 220;

            startButtonRectangle = new Rectangle(window.Width / 2 - 140, 400, 350, 100);
            shopButtonRectangle = new Rectangle(startButtonRectangle.X, startButtonRectangle.Y + 200, startButtonRectangle.Width, startButtonRectangle.Height);
            menuButtonRectangle = new Rectangle(window.Width / 2 - 140, 600, 350, 100);
            MusicButtonRectangle = new Rectangle(window.Width / 2 - 140, 800, 350, 100);

            powerups = new Powerups();
            activePowerupIndex = -1;
            slowTimeTemp = 0;

            files = new FileDialogue();
            musicScreen = new MusicScreen();
            musicVolume = 1;
            soundVolume = 1;
            customSongList = new List<SoundEffect>();
            customSongIndex = 0;
            songTimeElapsed = 0;
            gameSongList = new List<SoundEffect>();
            soundEffects = new List<SoundEffect>();
            gameSongListInstance = new List<SoundEffectInstance>();
            gameSongListIndex = 0;

            oldKB = Keyboard.GetState();


            enemyHitBox = new Rectangle(30, 10, 40, 50);
            playerHitBox = new Rectangle(25, 0, 80, 110);
            coinHitBox = new Rectangle(-20, -15, 35, 35);


            cutscenes = new Cutscene();

            base.Initialize();
            


        }

        private void CreateBosses()
        {
            Texture2D sheet = loadImage("Enemies/Enemies", Content);
            List<Boss> bos = new List<Boss>();

            bos.Add(new Boss(Bosses.Bat, 200, sheet, GraphicsDevice));
            bos.Add(new Boss(Bosses.Demon, 300, sheet, GraphicsDevice));

            for (int i = 0; i < bos.Count; i++)
                bosses.Add((Levels)i, bos[i]);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadFonts();
            pixel = this.Content.Load<Texture2D>("pixel");
            collectableSheet = this.Content.Load<Texture2D>("Shop/collectables");
            cosmeticSheet = this.Content.Load<Texture2D>("Shop/cosmetics");

            //for shop, textures have to be loaded first before they can be sent as parameters
            int[] collectNums = new int[] { 8, 5, 8, 4 };
            string[] itemNames = new string[] { "Time Control", "Can't Die", "Instakill", "Magnet", "Ski Mask", "Construction", "Hair", "Headphones", "Santa Hat", "Headband", "Army Hat", "Red Headband", "Blue Headband" };
            for (int i = 0, c = 0; i < collectNums.Length; c += collectNums[i], i++)
                items.Add(new ShopItem(50, itemNames[i], collectableRect.Skip(c).Take(collectNums[i]).ToList(), collectableSheet));
            for (int i = collectNums.Length, c = 0; i < itemNames.Length; i++, c += 2)
            {
                if (c == 12) //fire has multiple frames, so had to add a specific case for it
                {
                    items.Add(new ShopItem(50, "Fire", new List<Rectangle> { cosmeticRect[12], cosmeticRect[14], cosmeticRect[16] }, cosmeticSheet));
                    c += 6;
                }
                items.Add(new ShopItem(50, itemNames[i], new List<Rectangle> { cosmeticRect[c] }, cosmeticSheet));
            }
            items.Add(new ShopItem(50, "Coin", new List<Rectangle> { collectableRect[25], collectableRect[26], collectableRect[27], collectableRect[28] }, collectableSheet));
            

            jebSheet = this.Content.Load<Texture2D>("jeb");
            
            backgroundImages = loadTextures("Background", Content);

            jeb = new Player(new Vector2(900, 500), this);
            jeb.Invulnerable = false;
            shop = new Shop(items, jeb, jebSheet, idleAnimationRectangles[0]);

            loadGameSongs(0);
            loadSoundEffects();

            GenerateRooms(amountOfRooms, backgroundImages, window);


        }
        private void LoadFonts()
        {
            string fontFolder = "SpriteFonts/";

            FileInfo[] fontFiles = new DirectoryInfo("Content/"+fontFolder).GetFiles();
            fonts = new SpriteFont[fontFiles.Length];
            for (int i = 0; i < fontFiles.Length; i++)
                fonts[i] = Content.Load<SpriteFont>(fontFolder + Path.GetFileNameWithoutExtension(fontFiles[i].FullName));
        }
        public void LoadCustomSongs()
        {
            while (customSongList.Count != 0)
            {
                customSongList[0].Dispose();
                customSongList.RemoveAt(0);
            }
            List<string> names = musicScreen.customMusicNames;
            foreach (string name in names)
            {
                System.IO.FileStream fs = new System.IO.FileStream(name, System.IO.FileMode.Open);
                SoundEffect temp = SoundEffect.FromStream(fs);
                customSongList.Add(temp);
                Console.WriteLine(name);
            }
        }

        public void loadGameSongs(int instance)
        {
            if (instance == 0)
            {
                gameSongList.Add(Content.Load<SoundEffect>("Sounds/8bitMusic"));
                gameSongList.Add(Content.Load<SoundEffect>("Sounds/8bitMusic2"));
                gameSongList.Add(Content.Load<SoundEffect>("Sounds/8bitMusic3"));
                gameSongList.Add(Content.Load<SoundEffect>("Sounds/menuMusic"));
                gameSongList.Add(Content.Load<SoundEffect>("Sounds/BossMusic"));
                for (int i = 0; i < gameSongList.Count; i++)
                    gameSongListInstance.Add(gameSongList[i].CreateInstance());
            }
            else
            {
                for (int i = 0; i < gameSongList.Count; i++)
                {
                    if (gameSongListInstance[i].State == SoundState.Playing)
                        gameSongListInstance[i].Pause();
                }
            }
            
        }
        public void loadSoundEffects()
        {
            soundEffects.Add(Content.Load<SoundEffect>("Sounds/coinSound"));
            soundEffects.Add(Content.Load<SoundEffect>("Sounds/jump"));
            soundEffects.Add(Content.Load<SoundEffect>("Sounds/powerUp (1)"));

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



            // controls the main menu with each gamestate representing a different portion of the game

            if ((gameState == GameState.Menu || gameState == GameState.GameOver) && mouse.LeftButton == ButtonState.Pressed && CheckForCollision(mouse.X, mouse.Y, startButtonRectangle) && menuCoolDown == 0)
            {
                
                gameState = GameState.Cutscene;
                Reset();
                menuCoolDown = 60;
            }

            if ((gameState == GameState.Menu || gameState == GameState.GameOver) && mouse.LeftButton == ButtonState.Pressed && CheckForCollision(mouse.X, mouse.Y, MusicButtonRectangle) && menuCoolDown == 0)
            {
                gameState = GameState.Music;
            }


            if (gameState == GameState.GameOver && mouse.LeftButton == ButtonState.Pressed && CheckForCollision(mouse.X, mouse.Y, menuButtonRectangle) && menuCoolDown == 0)
            {
                gameState = GameState.Menu;
                menuCoolDown = 60;
            }
                

            if (gameState == GameState.Menu && mouse.LeftButton == ButtonState.Pressed && CheckForCollision(mouse.X, mouse.Y, shopButtonRectangle) && menuCoolDown == 0)
            {
                gameState = GameState.Shop;
                menuCoolDown = 60;
            }


            if (menuCoolDown > 0)
                menuCoolDown--;

            if (gameState == GameState.Menu)
            {
                if (musicScreen.customMusic)
                    LoadCustomSongs();

                gameSongListInstance[3].Volume = (float)musicVolume/5;
                if (gameSongListInstance[3].State != SoundState.Playing)
                    gameSongListInstance[3].Play();
            }
                
            

            if (gameState == GameState.Play)
            {
                gameSongListInstance[3].Stop();
                musicVolume = musicScreen.musicVolume;
                soundVolume = musicScreen.soundVolume;
                if (musicScreen.customMusic) //if custom music is selected
                {
                    if (songTimeElapsed == 0 && customSongIndex == 0)
                        customSongList[customSongIndex].Play(volume: (float)musicVolume/3, pitch: 0.0f, pan: 0.0f);
                    if (songTimeElapsed/60 > customSongList[customSongIndex].Duration.TotalSeconds)
                    {
                        customSongIndex++;
                        if (customSongIndex >= customSongList.Count)
                        {
                            customSongIndex = 0;
                        }
                        songTimeElapsed = 0;
                        customSongList[customSongIndex].Play(volume: (float)musicVolume/3, pitch: 0.0f, pan: 0.0f);
                    }
                    else
                    {
                        songTimeElapsed++;
                    }
                }
                else //regular game music
                {
                    gameSongListInstance[gameSongListIndex].Volume = (float)(musicVolume/15);
                    if (songTimeElapsed == 0 && gameSongListIndex == 0)
                        gameSongListInstance[gameSongListIndex].Play();
                    if (songTimeElapsed / 60 > gameSongList[gameSongListIndex].Duration.TotalSeconds)
                    {
                        gameSongListIndex++;
                        if (gameSongListIndex >= 3)
                        {
                            gameSongListIndex = 0;
                        }
                        songTimeElapsed = 0;
                        gameSongListInstance[gameSongListIndex].Play();
                    }
                    else
                    {
                        songTimeElapsed++;
                    }
                }

                if (activePowerupIndex == 0)
                {
                    slowTimeTemp++;
                    if (slowTimeTemp % 2 == 0)
                        return;

                }

                if (bossFight && currentBoss.IsDead)
                    currentBoss = null;
                if (bossFight) currentBoss.Update();

                scrollSpeed = currentRoomIndex + 10;

                roomList[currentRoomIndex].Update(scrollSpeed);

                




                if (bossFight)
                {
                    if (roomList[currentRoomIndex].enemyArray.Count > 0) 
                        roomList[currentRoomIndex].enemyArray.Clear();

                    goto Jeb;
                }


                // kills player if enemy intercepts them
                foreach (Enemy enemy in roomList[currentRoomIndex].enemyArray)
                {
                    if (activePowerupIndex != 1)
                        if (enemy != null)
                            if (new Rectangle(jeb.PlayerRectangle.X + playerHitBox.X, jeb.PlayerRectangle.Y + playerHitBox.Y, playerHitBox.Width, playerHitBox.Height).Intersects(new Rectangle(enemy.rectangle.X + enemyHitBox.X, enemy.rectangle.Y + enemyHitBox.Y, enemyHitBox.Width, enemyHitBox.Height)))
                                jeb.Damage();
                }

                // player coin collection
                foreach (Coin[,] coinGrid in roomList[currentRoomIndex].coinsGridList)
                {
                    foreach(Coin coin in coinGrid)
                    {
                        if (coin != null && new Rectangle(coin.rectangle.X + coinHitBox.X, coin.rectangle.Y + coinHitBox.Y, coinHitBox.Width, coinHitBox.Height).Intersects(new Rectangle(jeb.PlayerRectangle.X + playerHitBox.X, jeb.PlayerRectangle.Y + playerHitBox.Y, playerHitBox.Width, playerHitBox.Height)))
                        {
                            coin.Destroy();
                            jeb.Coins++;
                            soundEffects[0].Play(volume: (float)soundVolume/180, pitch: 0.0f, pan: 0.0f);
                        }
                    }
                }




                    if (bossFight)
                {
                    if (roomList[currentRoomIndex].enemyArray.Count > 0) roomList[currentRoomIndex].enemyArray.Clear();
                    goto Jeb;
                }

                foreach (Enemy enemy in roomList[currentRoomIndex].enemyArray)
                    if (activePowerupIndex != 1 && enemy != null && jeb.PlayerRectangle.Intersects(enemy.rectangle))
                        jeb.Damage();


                Jeb:

                jeb.Idle = gameState != GameState.Play;
                jeb.Update();

                if (!jeb.IsAlive)
                    gameState = GameState.GameOver;

                UpdateProjList(projectileList);


                
                



                if (keyboard.IsKeyDown(Keys.D1) && oldKB.IsKeyUp(Keys.D1))
                {
                    powerups.UsePowerup(0);
                }
                if (keyboard.IsKeyDown(Keys.D2) && oldKB.IsKeyUp(Keys.D2))
                {
                    powerups.UsePowerup(1);
                }
                if (keyboard.IsKeyDown(Keys.D3) && oldKB.IsKeyUp(Keys.D3))
                {
                    powerups.UsePowerup(2);
                }
                if (keyboard.IsKeyDown(Keys.D4) && oldKB.IsKeyUp(Keys.D4))
                {
                    powerups.UsePowerup(3);
                }
                if (powerups.ActivePowerups())
                {
                    activePowerupIndex = powerups.ActivePowerupsIndex();
                    
                    
                    if (activePowerupIndex == 2)
                    {

                        roomList[currentRoomIndex].enemyArray.Clear();
                        
                    }
                    if (activePowerupIndex == 3)
                    {
                        // pulls coins toward player
                        foreach (Coin[,] coinsGrid in roomList[currentRoomIndex].coinsGridList)
                        {
                            foreach (Coin coin in coinsGrid)
                            {
                                if (coin != null)
                                {
                                    coin.ApplyMagnetForce(gameTime);
                                }
                            }
                        }
                    }
                }
                else
                {
                    activePowerupIndex = -1;
                }

                powerups.Update();
            }
            
            gameTimer++;
            if (gameState == GameState.GameOver)
            {
                if (musicScreen.customMusic)
                {
                    LoadCustomSongs();
                    loadSoundEffects();
                    customSongIndex = 0;
                    songTimeElapsed = 0;
                }
                else
                {
                    loadGameSongs(1);
                    loadSoundEffects();
                    gameSongListIndex = 0;
                    songTimeElapsed = 0;
                }

                activePowerupIndex = -1;
                powerups.RemovePowerups();
            }

            base.Update(gameTime);
        }
        public void UpdateProjList(List<Projectile> list)
        {
            List<Projectile> toRemove = new List<Projectile>();
            foreach (Projectile p in list)
            {
                UpdateProjectile(p);

                if (p.ToRemove) toRemove.Add(p);
            }
            foreach (Projectile p in toRemove)
                list.Remove(p);
        }
        public void UpdateProjectile(Projectile p)
        {
            p.Update();

            if (p.DamagesBoss && bossFight && p.Rect.Intersects(currentBoss.Rectangle))
            {
                currentBoss.Damage(p.BossDamage);
                p.DeltDamage = true;
            }
            if (p.DamagesPlayer && p.Rect.Intersects(jeb.PlayerRectangle))
            {
                jeb.Damage();
                p.DeltDamage = true;
            }
        }

        

        private void Reset()
        {
            levels = Levels.Level1;
            gameTimer = 0;
            levelTimer = 0;
            currentRoomIndex = 0;
            scrollSpeed = 0;
            jeb.Health = Player.MaxHealth;
            jeb.IsAlive = true;

            transition = false;
            endCurrentRoom = false;

            projectileList.Clear();
            currentBoss = null;
            jeb.Position.Y = Player.floorHeight + jeb.PlayerRectangle.Height;
            jeb.delayLeft = Player.InputDelay;

            GenerateRooms(amountOfRooms, backgroundImages, window);
        }

        /// This is called when the game should draw itself.
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin();
            


            

            // menu
            if(gameState == GameState.Menu)
            {
                int halfSeconds = gameTimer / 30;
                Rectangle playerIdleDimensions = new Rectangle(window.Width / 2 - 20, 100, 100, 100);
                
                Vector2 titlePosition = new Vector2(window.Width / 2 - 220, 200);


                // Title screen animation
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

                spriteBatch.Draw(pixel, MusicButtonRectangle, Color.Green);
                spriteBatch.DrawString(buttonFont, "Music + Sound", new Vector2(MusicButtonRectangle.X+20, MusicButtonRectangle.Y+20), Color.White);

            }
            if (gameState == GameState.Cutscene)
            {
                if (cutscenes.alpha < 0)
                {
                    cutscenes.phase = true;
                    cutscenes.alpha = 0;
                }
                    
                cutscenes.cutseneActive = true;
                cutscenes.Draw(spriteBatch, pixel);
                
                if (cutscenes.phase == false)
                {
                    gameState = GameState.Play;
                }
            }

            // shop
            if (gameState == GameState.Shop)
            {
                shop.Draw(gameTime, spriteBatch, shopFont, shopFontBold, shopTitleFont, pixel);
                
                    
            }
            if (gameState == GameState.Music)
            {
                musicScreen.Draw(spriteBatch, pixel, shopTitleFont, shopFontBold, shopFont);
                musicVolume = musicScreen.musicVolume;
                gameSongListInstance[3].Volume = (float)musicVolume / 5;
            }
            if (gameState == GameState.Play)
            {

                



                if (!transition)
                    levelTimer++;

                int levelSeconds = levelTimer / 60;


                if (bossCooldown > 0 && !bossFight) bossCooldown--;
                if (levelSeconds > 10 && !bossFight && bossCooldown == 0)
                    SummonBoss();
                // tries to advance to next room every 10 seconds
                if (currentRoomIndex < roomList.Count - 1 && levelSeconds > 10 && !bossFight)
                {
                    transition = true;
                    levelTimer = 0;
                    levels++;
                }
                


                // scrolling calculations
                
                bool loopImage = roomList[currentRoomIndex].backgroundRectangle.X < -((window.Width * 2) - window.Right - 10);



                
                


                // checks if the transition period is over so we can move on to the next room
                if (currentRoomIndex < roomList.Count - 1 && loopImage && endCurrentRoom)
                {
                    currentRoomIndex++;
                    roomList[currentRoomIndex].InheritEnemies(roomList[currentRoomIndex - 1].enemyArray);
                    endCurrentRoom = false;
                    transition = false;
                }


                // checks if we are currently transitioning to next room so it can draw the transition from the current room to the next room
                if (loopImage && transition)
                {
                    roomList[currentRoomIndex].background2 = roomList[currentRoomIndex + 1].background1;
                    endCurrentRoom = true;
                }

                // checks if we have undergone a full loop of the background
                if (loopImage)
                {
                    roomList[currentRoomIndex].backgroundRectangle.X = 0;
                }


                // draws the room

                Rectangle roomRectangle = roomList[currentRoomIndex].backgroundRectangle;

                spriteBatch.Draw(roomList[currentRoomIndex].background1, roomRectangle, Color.White);
                spriteBatch.Draw(roomList[currentRoomIndex].background2, new Rectangle(roomRectangle.Right, 0, roomRectangle.Width, roomRectangle.Height), Color.White);

                // draws the boss
                if (!bossFight) roomList[currentRoomIndex].Draw(spriteBatch);

                if(bossFight)
                    spriteBatch.DrawString(menuFont, "BOSS FIGHT!", new Vector2(window.Width / 2 - 100, 300), Color.Red);


                jeb.Draw(spriteBatch);
                if (currentBoss != null) currentBoss.Draw(spriteBatch);
                foreach (Projectile p in projectileList)
                    p.Draw(spriteBatch);

                powerups.Draw(spriteBatch, collectableSheet, pixel, clock, skull, nuke, magnet, shopFontBold, shopFont);


                // draws hitboxes to help debug them

                if (debugMode)
                {
                    foreach (Enemy enemy in roomList[currentRoomIndex].enemyArray)
                    {
                        spriteBatch.Draw(pixel, new Rectangle(enemy.rectangle.X + enemyHitBox.X, enemy.rectangle.Y + enemyHitBox.Y, enemyHitBox.Width, enemyHitBox.Height), Color.Black);
                    }

                    foreach (Coin[,] coinGrid in roomList[currentRoomIndex].coinsGridList)
                    {
                        foreach (Coin coin in coinGrid)
                        {
                            if (coin != null)
                                spriteBatch.Draw(pixel, new Rectangle(coin.rectangle.X + coinHitBox.X, coin.rectangle.Y + coinHitBox.Y, coinHitBox.Width, coinHitBox.Height), Color.Black);
                        }
                    }
                    spriteBatch.Draw(pixel, new Rectangle(jeb.PlayerRectangle.X + playerHitBox.X, jeb.PlayerRectangle.Y + playerHitBox.Y, playerHitBox.Width, playerHitBox.Height), Color.Black);
                }

                cutscenes.Draw(spriteBatch, pixel);
                if (cutscenes.alpha > 255)
                {
                    cutscenes.alpha = 0;
                    cutscenes.cutseneActive = false;
                }

            }
            // game over screen and meny
            if(gameState == GameState.GameOver)
            {
                spriteBatch.DrawString(menuFont, "You Died! Whomp whomp", new Vector2(window.Width / 2 - 200, 200), Color.White);

                spriteBatch.Draw(pixel, startButtonRectangle, Color.Green);
                spriteBatch.DrawString(buttonFont, "Play Again", new Vector2(startButtonRectangle.X + 50, startButtonRectangle.Y + 20), Color.White);

                spriteBatch.Draw(pixel, menuButtonRectangle, Color.Green);
                spriteBatch.DrawString(buttonFont, "Menu", new Vector2(menuButtonRectangle.X + 120, menuButtonRectangle.Y + 20), Color.White);

            }
            



            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        

        private void SummonBoss()
        {
            currentBoss = bosses[levels].Clone();
            bossCooldown = 300;
        }
        public bool CheckForCollision(int x, int y, Rectangle inputRectangle)
        {
            if (x < inputRectangle.Right && x > inputRectangle.Left && y < inputRectangle.Bottom && y > inputRectangle.Top)
                return true;

            return false;
        }

        public void GenerateRooms(int amountOfRooms, List<Texture2D> textures, Rectangle dimensions)
        {
            currentRoomIndex = 0;
            roomList.Clear();

            for(int i = 0; i < amountOfRooms; i++)
            {

                roomList.Add(new Room(textures[rand.Next(0, textures.Count)], dimensions, rand.Next(1,Enemy.EnemyNames), GraphicsDevice, Content, window));
            }

            
        }

        // directory should be the name of a FOLDER with the images you want to load.
        // This assumes that the stuff you want to load is in the correct level folder
        public static List<Texture2D> loadTextures(string directory, ContentManager content)
        {
            string[] files = Directory.GetFiles(@"Content\" + levels + "/" + directory + "/", "*");
            List<Texture2D> images = new List<Texture2D>();

            int i = 0;

            foreach (var File in files)
            {
                string[] Temp;
                Temp = File.Split('.');
                string NameMinus = Temp[0];
                int Index = NameMinus.LastIndexOf('\\') + 1;
                NameMinus = NameMinus.Substring(Index);



                files[i] = NameMinus;
                i++;

            }


            foreach (string file in files)
            {
                images.Add(content.Load<Texture2D>(@".\" + file));
            }

            return images;
        }

        // directory should be a a single IMAGE that you want to load.
        // This assumes that the stuff you want to load is in the correct level folder 
        public static Texture2D loadImage(string directory, ContentManager content)
        {
            return content.Load<Texture2D>(@".\" + levels + "/" + directory);
        }

        public static Color GetAverageColor(Texture2D texture)
        {
            double r, g, b;
            r = g = b = 0;
            Color[] pixels = new Color[texture.Width * texture.Height];
            texture.GetData(pixels);
            foreach (Color c in pixels)
            {
                r += c.R;
                g += c.G;
                b += c.B;
            }
            return new Color((int)r / pixels.Length, (int)g / pixels.Length, (int)b / pixels.Length);
        }



    }
}
