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
        List<Rectangle> multiplayerButtons;
        List<bool> multiplayerButtonStates;
        Texture2D[] iconTextures;
        private int CurrentPlayerInShop;

        Rectangle enemyHitBox;
        Rectangle playerHitBox;
        Rectangle coinHitBox;
        Rectangle obstacleHitBox;

        public static Rectangle window;
        public List<Player> players;
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
        public static int scrollSpeed;
        public bool transition;
        public bool endCurrentRoom;
        public static bool bossFight { get { return currentBoss != null && !currentBoss.IsDead; } }
        public Dictionary<Levels, Boss> bosses;
        public Dictionary<GameState, Menu> menus;
        private PlayerHats Pondering;
        private List<MenuThingie> Storage;

        public Random rand;

        private List<Texture2D> backgroundImages;

        //for shop
        public Texture2D collectableSheet, cosmeticSheet;
        public Rectangle[] collectableRect, cosmeticRect;
        public SpriteFont shopFont { get { return fonts[2]; } }
        public SpriteFont shopFontBold { get { return fonts[3]; } }
        public SpriteFont shopTitleFont { get { return fonts[4]; } }

        public int menuCoolDown;

        KeyboardState oldKB;
        MouseState oldMouse;

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
        bool debugMode = false;
        public static List<SoundEffect> soundEffects;

        //for cutsenes
        Cutscene cutscenes;
        GameState cutsceneDestination;

        //tutorial stuff
        Texture2D questionMark;
        Rectangle tutorialRect;
        public bool tutorialActive;
        public int textboxesIndex;
        public List<Textbox> textboxes;
        public Textbox textbox;

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

            graphics.ApplyChanges();
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
            var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
            form.Location = new System.Drawing.Point(0, 0);

            //for shop
            clock = new List<Rectangle> { new Rectangle(0, 0, 32, 32), new Rectangle(32, 0, 32, 32), new Rectangle(64, 0, 32, 32), new Rectangle(96, 0, 32, 32), new Rectangle(128, 0, 32, 32), new Rectangle(0, 32, 32, 32), new Rectangle(32, 32, 32, 32), new Rectangle(64, 32, 32, 32) };
            skull = new List<Rectangle> { new Rectangle(96, 32, 32, 32), new Rectangle(128, 32, 32, 32), new Rectangle(0, 64, 32, 32), new Rectangle(32, 64, 32, 32), new Rectangle(64, 64, 32, 32) };
            nuke = new List<Rectangle> { new Rectangle(96, 64, 32, 32), new Rectangle(128, 64, 32, 32), new Rectangle(0, 96, 32, 32), new Rectangle(32, 96, 32, 32), new Rectangle(64, 96, 32, 32), new Rectangle(96, 96, 32, 32), new Rectangle(128, 96, 32, 32), new Rectangle(0, 128, 32, 32) };
            magnet = new List<Rectangle> { new Rectangle(32, 128, 32, 32), new Rectangle(64, 128, 32, 32), new Rectangle(96, 128, 32, 32), new Rectangle(128, 128, 32, 32) };
            

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
            scrollSpeed = 10;
            menuCoolDown = 0;
            bossCooldown = 0;

            gameState = GameState.Menu;
            levels = Levels.Level1;
            gameTimer = 0;
            levelTimer = 0;
            currentRoomIndex = 10;

            multiplayerButtons = new List<Rectangle> { new Rectangle(550, 420, 80, 80), new Rectangle(550, 620, 80, 80), new Rectangle(550, 800, 80, 80) };
            multiplayerButtonStates = new List<bool> { true, false, false };
            iconTextures = new Texture2D[2];
            CurrentPlayerInShop = 0;
            Storage = new List<MenuThingie>();

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
            obstacleHitBox = new Rectangle(0, 0, 0, 0);
            playerHitBox = new Rectangle(35, 5, 60, 90);
            coinHitBox = new Rectangle(5, 5, 40, 40);

            cutscenes = new Cutscene();
            cutsceneDestination = GameState.Menu;
            oldMouse = Mouse.GetState();

            tutorialActive = false;
            tutorialRect = new Rectangle(window.Width/2, 280, 60, 60);
            textboxes = new List<Textbox>
            {
                new Textbox("Here you can press these buttons\nto select how many players \nyou'd like.", new Vector2(multiplayerButtons[1].X + multiplayerButtons[1].Width, multiplayerButtons[1].Y + multiplayerButtons[1].Width / 2)),
                new Textbox("This is the shop! You can use\nspace or click to buy \npowerups and cosmetics. You\nstart off with 100 coins,\nbut will collect more when\nyou play the game."),
                new Textbox("Here you can adjust sound and\nmusic! Moving these sliders\nwill change the volume\nto your desired level", new Vector2(musicScreen.sliderHandleMusic.X, musicScreen.sliderHandleMusic.Y)),
                new Textbox("If you'd rather listen to\nyour own music, you can\nselect the custom music\nbutton, and add .wav files\nfrom your own computer.\nYou can change this back\nto game music at any time.", new Vector2(musicScreen.customMusicButton.X+musicScreen.customMusicButton.Width/2, musicScreen.customMusicButton.Y+musicScreen.customMusicButton.Height)),
                new Textbox("Now to playing the game!\nWhat is the objective? Well,\nyour trying to get as far\nas you can without dying\nwhile collecting coins on the \nway. Use W to jump and S to \nfastfall.", new Vector2(window.Width-300, 200)),
                new Textbox("Find yourself in a pickle?\nNo worries, just use a powerup!\nThe powerups are as follows:\nslow time, invulnrability, \ninstakill, and a coin magnet.\nTo use them, press 1, 2, 3, \nor 4 respectively.", new Vector2(390, 200)),
                new Textbox("Finally, a boss battle will \noccur after a set time,\nin which you must dodge and\nattack with your fireballs\nby pressing D on your\nkeyboard. Once the boss\nis defeated, another one will\nappear after that same time\ninterval. That's it, have fun!")
            };
            textboxesIndex = 0;
            base.Initialize();

                                                                                                                                                                                    

        }

        private void GenMens()
        {
            
            List<MenuThingie> butts = new List<MenuThingie>
            {
                new Button(startButtonRectangle, new Color(0,255,0,255), menuFont, "Start")
                {
                    BorderWidth = 6,
                    TextColor = Color.White
                },
                new Button(shopButtonRectangle, Color.Green, menuFont, "Enter Shop")
                {
                    BorderWidth = 6,
                    TextColor = Color.White
                },
                new Button(MusicButtonRectangle, new Color(0,64,0,255), menuFont, "Settings")
                {
                    BorderWidth = 6,
                    TextColor = Color.White
                }
            };
            foreach (Rectangle r in multiplayerButtons)
                butts.Add(new Button(r, iconTextures[0]));
            (butts[3] as Button).Texture = iconTextures[1];

            MenuThingie hold = new SelectionGrid(new Button[][]
            {
                new Button[] {butts[3] as Button, butts[0] as Button},
                new Button[] {butts[4] as Button, butts[1] as Button},
                new Button[] {butts[5] as Button, butts[2] as Button}
            });
            butts.Clear();
            butts.Add(hold);

            menus[GameState.Menu] = new Menu(butts.ToArray());
            butts.Clear();

            Animation[] anims = GenShopButts();
            const int size = 75;
            string[] names = new string[] { "Time Control", "Can't Die", "Instakill", "Magnet", "Ski Mask", "Construction", "Hair", "Headphones", "Santa Hat", "Headband", "Fire", "Army Hat", "Red Headband", "Blue Headband" };
            for (int i = 0, j = 0, c = 0; c < 12; c++, i++)
            {
                if (i >= 4) { i = 0; j++; }
                butts.Add(new Button(new Rectangle(window.Width * (i + 5) / 16 + size * i, window.Height * (j * 3 + 5) / 18, size, size), anims[c], shopFont, names[c])
                {
                    HitboxInset = 20
                });
            }
            butts.Add(new Button(new Rectangle(window.Width * 6 / 16 + size, window.Height * 14 / 18, size, size), anims[12], shopFont, names[12])
            {
                HitboxInset = 20
            });
            butts.Add(new Button(new Rectangle(window.Width * 7 / 16 + size * 2, window.Height * 14 / 18, size, size), anims[13], shopFont, names[13])
            {
                HitboxInset = 20
            });
            foreach (MenuThingie a in butts)
            {
                Button b = a as Button;
                b.TextPosition = new Vector2(b.TextPosition.X, b.TextPosition.Y + b.Rectangle.Height);
            }
            hold = new SelectionGrid(new Button[][]
            {
                new Button[] { butts[0] as Button, butts[1] as Button, butts[2] as Button, butts[3] as Button },
                new Button[] { butts[4] as Button, butts[5] as Button, butts[6] as Button, butts[7] as Button },
                new Button[] { butts[8] as Button, butts[9] as Button, butts[10] as Button, butts[11] as Button },
                new Button[] {null, butts[12] as Button, butts[13] as Button, null }
            })
            {
                BGColor = new Color(192, 192, 192, 255),
                BorderWidth = 6,
                HitboxInset = 75
            };
            butts.Clear();
            butts.Add(hold);
            butts.Add(new Box(new Rectangle(window.Width * 13 / 16, window.Height * 2 / 18, size, size), anims[14], shopFont, () => "Coins: " + players[CurrentPlayerInShop].Coins));
            butts.Add(new Box(new Rectangle(window.Width * 5 / 32, window.Height * 8 / 18, size, size), anims[15], shopFont, () => "Equipped: " + players[CurrentPlayerInShop].currentHat));
            butts.Add(new MenuText(shopTitleFont, "SHOP", new Vector2(window.Width / 2.43f, window.Height / 25)));
            
            foreach (MenuThingie a in butts)
            {
                if (a is Button)
                {
                    Button b = a as Button;
                    b.TextPosition = new Vector2(b.TextPosition.X, b.TextPosition.Y + b.Rectangle.Height);
                    continue;
                }
                if (a is Box)
                {
                    Box b = a as Box;
                    b.TextPosition = new Vector2(b.TextPosition.X, b.TextPosition.Y + b.Rectangle.Height);
                    continue;
                }
            }
            butts.Add(new Button(new Rectangle(window.Width * 2 / 16, window.Height / 18, size * 3, size * 2), Color.Green, shopFont, "Go Back")
            {
                BorderWidth = 4,
                TextColor = Color.White
            });

            menus[GameState.Shop] = new Menu(butts.ToArray());
            butts.Clear();

            butts.AddRange(new MenuThingie[]
            {
                new SelectionGrid(new Button[][] { new Button[] {
                        new Button(startButtonRectangle, Color.Green, menuFont, "Play Again")
                        {
                            BorderWidth = 6,
                            TextColor = Color.White
                        }
                }, new Button[] {
                        new Button(menuButtonRectangle, Color.Green, menuFont, "Menu")
                        {
                            BorderWidth = 6,
                            TextColor = Color.White
                        }
                    }
                }),
                new MenuText(menuFont, "You Died! Whomp whomp", new Vector2(window.Width / 2 - window.Width * 2 / 19, window.Width * 2 / 19))
                {
                    TextColor = Color.White
                }
            });

            menus[GameState.GameOver] = new Menu(butts.ToArray());
            butts.Clear();

            butts.Add(new Button(new Rectangle(window.Width / 38 * 15 - window.Width / 76, window.Height / 10 * 3, window.Width / 190 * 17, window.Height / 10), Color.DarkGray, shopFontBold, "Game Music")
            {
                BorderWidth = 3
            });
            butts.Add(new Button(new Rectangle(window.Width / 190 * 101 - window.Width / 76, window.Height / 10 * 3, window.Width / 190 * 17, window.Height / 10), Color.DarkGray, shopFontBold, "Custom Music")
            {
                BorderWidth = 3
            });

            butts.Add(new Slider(new Rectangle(window.Width / 19 * 7, window.Height / 5 * 4, window.Width / 19 * 5, window.Height / 125)));
            butts.Add(new Slider(new Rectangle(window.Width / 19 * 7, window.Height / 5 * 3, window.Width / 19 * 5, window.Height / 125)));

            butts.Add(new MenuText(shopFontBold, "Music Volume", new Vector2(window.Width / 19 * 9 - window.Width / 76, window.Height / 100 * 53)));
            butts.Add(new MenuText(shopFontBold, "Sound Volume", new Vector2(window.Width / 19 * 9 - window.Width / 76, window.Height / 100 * 73)));




            menus[GameState.Music] = new Menu(butts.ToArray());
            butts.Clear();
        }
        private Animation[] GenShopButts()
        {
            List<Animation> outp = new List<Animation>();
            Texture2D sheet = collectableSheet;
            Rectangle[] rects = Player.LoadSheet(5, 6, 32, 32, 1);
            Animation hold = new Animation("idle");
            hold.AddAnimation("idle", sheet, GraphicsDevice, 10, rects.Take(8).ToArray());
            outp.Add(hold);
            hold = new Animation("idle");
            hold.AddAnimation("idle", sheet, GraphicsDevice, 10, rects.Skip(8).Take(5).ToArray());
            outp.Add(hold);
            hold = new Animation("idle");
            hold.AddAnimation("idle", sheet, GraphicsDevice, 10, rects.Skip(13).Take(8).ToArray());
            outp.Add(hold);
            hold = new Animation("idle");
            hold.AddAnimation("idle", sheet, GraphicsDevice, 10, rects.Skip(21).Take(4).ToArray());
            outp.Add(hold);

            sheet = cosmeticSheet;
            rects = Player.LoadSheet(5, 5, 32, 32, 1);
            for (int i = 0; i < 24; i+=2)
            {
                hold = new Animation("idle");
                if (i == 12) { hold.AddAnimation("idle", sheet, GraphicsDevice, 10, rects[12], rects[14], rects[16]); i += 4; }
                else hold.AddAnimation("idle", sheet, GraphicsDevice, 60, rects[i]);
                outp.Add(hold);
            }
            hold = new Animation("idle");
            rects = Player.LoadSheet(5, 6, 32, 32, 1);
            hold.AddAnimation("idle", collectableSheet, GraphicsDevice, 10, rects[25], rects[26], rects[27], rects[28], rects[27], rects[26]);
            outp.Add(hold);
            hold = new Animation("idle");
            rects = Player.LoadSheet(4, 3, 32, 32);
            hold.AddAnimation("idle", jebSheet, GraphicsDevice, 60, rects[10]);
            outp.Add(hold);
            

            return outp.ToArray();
        }
        private void UpdateShop()
        {
            //shopMenu length = 5
            if (menuCoolDown > 0) return;
            const int Price = 50;
            if (menus[GameState.Shop].LinkedMenu != default)
            {
                UpdatePopup(Price);
                return;
            }
            
            MenuThingie[] shopMenu = menus[GameState.Shop].thingies.ToArray();
            Player current = players[CurrentPlayerInShop];
            KeyboardState kb = Keyboard.GetState();
            Button b = (shopMenu[0] as SelectionGrid).Current;
            Point p = (shopMenu[0] as SelectionGrid).Selected;
            int c = p.X * 4 + p.Y - 4;

            if (b.BGColor.Equals(Color.Green))
            {
                if (b.MouseClickedOnce || KeyPressed(kb, Keys.Space, Keys.Enter))
                {
                    if (c <= 6)
                        current.currentHat = (PlayerHats)c + 1;
                    else if (c == 7)
                        current.currentHat = (PlayerHats)c + 3;
                    else
                        current.currentHat = (PlayerHats)c + 2;
                    (shopMenu[2] as Box).Animation = b.Animation;
                }
            }
            else if ((b.MouseClickedOnce || KeyPressed(kb, Keys.Space, Keys.Enter)) && current.Coins >= Price)
            {
                if (c >= 0)
                {
                    PlayerHats buying;
                    if (c <= 6)
                        buying = (PlayerHats)c + 1;
                    else if (c == 7)
                        buying = (PlayerHats)c + 3;
                    else
                        buying = (PlayerHats)c + 2;
                    Storage.AddRange(new MenuThingie[] { b, shopMenu[2] });
                    AddShopPopup(buying, menus[gameState], Price);
                }
                else
                {
                    powerups.quantities[c + 4]++;
                    current.Coins -= Price;
                }
            }
            if ((shopMenu[4] as Button).MouseClickedOnce) { gameState = GameState.Menu; menuCoolDown = 2; }
        }
        private void AddShopPopup(PlayerHats buying, Menu current, int Price)
        {
            menuCoolDown = 2;
            Pondering = buying;
            List<MenuThingie> elements = new List<MenuThingie>
            {
                new Box(new Rectangle(window.Width / 2 - window.Width * 7 / 38, window.Height / 2 - window.Height * 2 / 19, window.Width / 3, window.Height / 3))
                {
                    BGColor = new Color(255,253,219,255),
                    BorderWidth = 6
                },
                new MenuText(shopFont, "Buy "+buying+"?", new Vector2(window.Width / 2 - window.Width * 15 / 38 + window.Width / 3, window.Height / 2 - window.Height * 3 / 38)),
                new MenuText(shopFont, "Price: "+Price+" Coins", new Vector2(window.Width / 2 - window.Width * 15 / 38 + window.Width / 3, window.Height / 2 - window.Height / 38)),
                new SelectionGrid(
                    new Button[][] {
                        new Button[] {
                            new Button(new Rectangle(window.Width / 2 - window.Width * 6 / 38, window.Height / 2 + window.Height / 19, (int)Math.Round(window.Width / 9.5), window.Height / 9), Color.Red, shopFontBold, "No")
                            {
                                BorderWidth = 6
                            },
                            new Button(new Rectangle(window.Width / 2 - window.Width * 12 / 38 + window.Width / 3, window.Height / 2 + window.Height / 19, (int)Math.Round(window.Width / 9.5), window.Height / 9), Color.Green, shopFontBold, "Yes")
                            {
                                BorderWidth = 6
                            }
                            
                        }
                    }
                )
            };
            Menu popup = new Menu(elements.ToArray());

            current.LinkedMenu = popup;
        }
        private void UpdatePopup(int Price)
        {
            if (menuCoolDown > 0) return;
            Button b = (menus[gameState].LinkedMenu.thingies[3] as SelectionGrid).Current;
            KeyboardState kb = Keyboard.GetState();
            if (b.MouseClickedOnce || KeyPressed(kb, Keys.Enter, Keys.Space))
            {
                if (b.DrawColor.Equals(Color.Green))
                {
                    players[CurrentPlayerInShop].ownedHats.Add((int)Pondering);
                    players[CurrentPlayerInShop].currentHat = Pondering;
                    players[CurrentPlayerInShop].Coins -= Price;
                    Storage[0].BGColor = Color.Green;
                    (Storage[1] as Box).Animation = (Storage[0] as Button).Animation;
                }
                menus[gameState].LinkedMenu = default;
                Pondering = default;
                menuCoolDown = 2;
                Storage.Clear();
            }
        }


        private void CreateBosses()
        {
            Texture2D sheet = loadImage("Enemies/EnemiesButBetter", Content);
            List<Boss> bos = new List<Boss>
            {
                new Boss(Bosses.Bat, 200, sheet, GraphicsDevice),
                new Boss(Bosses.Demon, 300, sheet, GraphicsDevice),
                new Boss(Bosses.Yeti, 500, sheet, GraphicsDevice),
                new Boss(Bosses.Shark, 1000, sheet, GraphicsDevice)
            };

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


            jebSheet = this.Content.Load<Texture2D>("jeb");
            iconTextures[0] = Content.Load<Texture2D>("Icons/personIcon");
            iconTextures[1] = Content.Load<Texture2D>("Icons/personIconSelected-removebg-preview");
            questionMark = Content.Load<Texture2D>("Icons/questionMark");
            backgroundImages = loadTextures("Background", Content);

            players = new List<Player> {
                new Player(new Vector2(700, 500))
                {
                    Invulnerable = false,
                    Up = new List<Keys> { Keys.W },
                    Down = new List<Keys> { Keys.S },
                    Left = new List<Keys> { Keys.A },
                    Shoot = new List<Keys> { Keys.D }
                }
            };

            menus = new Dictionary<GameState, Menu>();
            GenMens();

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
        private void GeneratePlayers(int amount)
        {
            if (amount <= 1) return;
            players.Add(new Player(new Vector2(900, 500))
            {
                Invulnerable = false,
                Up = new List<Keys> { Keys.Up },
                Down = new List<Keys> { Keys.Down },
                Left = new List<Keys> { Keys.Left },
                Shoot = new List<Keys> { Keys.Right, Keys.NumPad0 }
            });
            if (amount == 2) return;
            players.Add(new Player(new Vector2(500, 500))
            {
                Invulnerable = false,
                Up = new List<Keys> { Keys.I },
                Down = new List<Keys> { Keys.K },
                Left = new List<Keys> { Keys.J },
                Shoot = new List<Keys> { Keys.L }
            });
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || KeyPressed(keyboard, Keys.Escape))
            {
                if (menuCoolDown == 0)
                    switch (gameState)
                    {
                        case GameState.Shop:
                            gameState = GameState.Menu;
                            menuCoolDown = 2;
                            break;
                        case GameState.Music:
                            gameState = GameState.Menu;
                            menuCoolDown = 2;
                            break;
                        default:
                            this.Exit();
                            break;
                    }
            }
            Menu currentMenu = getCurrentMenu();

            // controls the main menu with each gamestate representing a different portion of the game

            if (menuCoolDown == 0 && currentMenu != default)
            {
                Button b = default;
                if (currentMenu.thingies[0] is SelectionGrid sg)
                    b = sg.Current;
                if (currentMenu.LastTouched is Button button)
                    b = button;
                if (b == default)
                    goto SkipInputs;

                if ((b.MouseClickedOnce || KeyPressed(keyboard, Keys.Space, Keys.Enter)) && b.Text != default)
                {
                    if ((gameState == GameState.Menu && b.Text.Equals("Start")) || (gameState == GameState.GameOver && b.Text.Equals("Play Again")))
                    {

                        cutsceneDestination = GameState.Play;
                        gameState = GameState.Cutscene;
                        Reset();
                        menuCoolDown = 2;
                        GeneratePlayers(multiplayerButtonStates.FindAll(a => a).Count);
                    }

                    if ((gameState == GameState.Menu || gameState == GameState.GameOver) && b.Text.Equals("Settings"))
                    {
                        gameState = GameState.Music;
                    }

                    if (gameState == GameState.GameOver && b.Text.Equals("Menu"))
                    {
                        gameState = GameState.Menu;
                        menuCoolDown = 2;
                    }


                    if (gameState == GameState.Menu && b.Text.Equals("Enter Shop"))
                    {
                        gameState = GameState.Shop;
                        Button[] arr = (menus[gameState].thingies[0] as SelectionGrid).Butts;
                        for (int c = 4, i = 1; i < arr.Length; i++,c++)
                            if (i <= 7)
                            {
                                if (players[0].ownedHats.Contains(i))
                                    arr[c].BGColor = Color.Green;
                            }
                            else
                                if (players[0].ownedHats.Contains(i + 2))
                                arr[c].BGColor = Color.Green;
                        menuCoolDown = 2;
                    }
                    if (gameState == GameState.Music && b.Text.Equals("Game Music"))
                    {
                        foreach (MenuThingie mt in currentMenu.thingies.Skip(2).Take(4))
                            mt.Shown = true;
                    }
                    if (gameState == GameState.Music && b.Text.Equals("Custom Music"))
                    {
                        foreach (MenuThingie mt in currentMenu.thingies.Skip(2).Take(4))
                            mt.Shown = false;
                    }
                }
            }
            if (gameState == GameState.Menu && mouse.LeftButton == ButtonState.Pressed && CheckForCollision(mouse.X, mouse.Y, tutorialRect) && menuCoolDown == 0 && !tutorialActive)
            {
                tutorialActive = true;
                cutsceneDestination = GameState.Menu;
                gameState = GameState.Cutscene;
            }
            if (gameState == GameState.Cutscene)
            {
                if (cutscenes.phase == false)
                {
                    gameState = cutsceneDestination;
                }
            }
            SkipInputs:

            if (menuCoolDown > 0)
                menuCoolDown--;

            if (gameState == GameState.Menu && menuCoolDown == 0)
            {
                if (musicScreen.customMusic)
                    LoadCustomSongs();

                gameSongListInstance[3].Volume = (float)musicVolume / 5;
                if (gameSongListInstance[3].State != SoundState.Playing)
                    gameSongListInstance[3].Play();

                if (currentMenu != default)
                {
                    SelectionGrid grid = currentMenu.thingies[0] as SelectionGrid;
                    Button b = grid.Current;
                    if (b.MouseClickedOnce || KeyPressed(keyboard, Keys.Enter, Keys.Space))
                    {
                        int i = grid.Butts.ToList().IndexOf(grid.Current);
                        if (i % 2 == 0)
                        {
                            i /= 2;
                            bool toSet = !multiplayerButtonStates[i];
                            if (toSet)
                                for (int j = i; j >= 0; j--)
                                    multiplayerButtonStates[j] = true;
                            else
                                for (int j = 2; j >= i; j--)
                                    multiplayerButtonStates[j] = false;
                            for (int ii = 0; ii < 3; ii++)
                                if (multiplayerButtonStates[ii])
                                    grid.Butts[ii * 2].Texture = iconTextures[1];
                                else
                                    grid.Butts[ii * 2].Texture = iconTextures[0];
                        }
                    }
                }
            }
            

            if (gameState == GameState.Play)
            {
                gameSongListInstance[3].Stop();
                musicVolume = musicScreen.musicVolume;
                soundVolume = musicScreen.soundVolume;
                if (musicScreen.customMusic) //if custom music is selected
                {
                    if (customSongList.Count != 0)
                    {
                        if (songTimeElapsed == 0 && customSongIndex == 0)
                            customSongList[customSongIndex].Play(volume: (float)musicVolume / 3, pitch: 0.0f, pan: 0.0f);
                        if (songTimeElapsed / 60 > customSongList[customSongIndex].Duration.TotalSeconds)
                        {
                            customSongIndex++;
                            if (customSongIndex >= customSongList.Count)
                            {
                                customSongIndex = 0;
                            }
                            songTimeElapsed = 0;
                            customSongList[customSongIndex].Play(volume: (float)musicVolume / 3, pitch: 0.0f, pan: 0.0f);
                        }
                        else
                        {
                            songTimeElapsed++;
                        }

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
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].IsAlive)
                        players[i].distanceTraveled += (int)Math.Ceiling((decimal)scrollSpeed / 15);
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


                // damages player if enemy intercepts them
                foreach (Enemy enemy in roomList[currentRoomIndex].enemyArray)
                {
                    if (activePowerupIndex != 1)
                        if (enemy != null)
                            foreach (Player p in players)
                            if (new Rectangle(p.PlayerRectangle.X + playerHitBox.X, p.PlayerRectangle.Y + playerHitBox.Y, playerHitBox.Width, playerHitBox.Height).Intersects(new Rectangle(enemy.rectangle.X + enemyHitBox.X, enemy.rectangle.Y + enemyHitBox.Y, enemyHitBox.Width, enemyHitBox.Height)))
                                p.Damage();
                }

                // player coin collection
                foreach (Coin[,] coinGrid in roomList[currentRoomIndex].coinsGridList)
                {
                    foreach(Coin coin in coinGrid)
                    {
                        foreach (Player p in players)
                        if (coin != null && new Rectangle(coin.rectangle.X + coinHitBox.X, coin.rectangle.Y + coinHitBox.Y, coinHitBox.Width, coinHitBox.Height).Intersects(new Rectangle(p.PlayerRectangle.X + playerHitBox.X, p.PlayerRectangle.Y + playerHitBox.Y, playerHitBox.Width, playerHitBox.Height)))
                        {
                            coin.Destroy();
                            p.Coins++;
                            soundEffects[0].Play(volume: (float)soundVolume/180, pitch: 0.0f, pan: 0.0f);
                        }
                    }
                }


               // damages player if obstacle intercepts them
               foreach(ProjectileClump obstacle in roomList[currentRoomIndex].obstacleList)
                {

                    foreach(Player jeb in players)
                    {
                        if (new Rectangle(jeb.PlayerRectangle.X + playerHitBox.X, jeb.PlayerRectangle.Y + playerHitBox.Y, playerHitBox.Width, playerHitBox.Height)
                            .Intersects(new Rectangle(obstacle.Current.Rectangle.X + obstacleHitBox.X, obstacle.Current.Rectangle.Y + obstacleHitBox.Y,
                            obstacle.Current.Rectangle.Width + obstacleHitBox.Width, obstacle.Current.Rectangle.Height + obstacleHitBox.Height)))
                            jeb.Damage();
                    }
                        
                }

                    if (bossFight)
                {
                    if (roomList[currentRoomIndex].enemyArray.Count > 0) roomList[currentRoomIndex].enemyArray.Clear();
                    goto Jeb;
                }

                foreach (Enemy enemy in roomList[currentRoomIndex].enemyArray)
                    foreach (Player p in players)
                        if (activePowerupIndex != 1 && enemy != null && p.PlayerRectangle.Intersects(enemy.rectangle))
                            p.Damage();


                        Jeb:

                bool weLiving = false;
                foreach (Player p in players)
                {
                    p.Idle = gameState != GameState.Play;
                    p.Update();

                    if (p.IsAlive)
                        weLiving = true;
                }
                if (!weLiving)
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
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].distanceTraveled = 0;
                }

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
            if (gameState == GameState.Shop)
                UpdateShop();
            oldMouse = mouse;
            oldKB = Keyboard.GetState();
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

            if (p.DamagesBoss && bossFight && p.Rectangle.Intersects(currentBoss.Rectangle))
            {
                currentBoss.Damage(p.BossDamage);
                p.DeltDamage = true;
            }
            foreach (Player pp in players)
                if (p.DamagesPlayer && p.Rectangle.Intersects(pp.PlayerRectangle))
                {
                    pp.Damage();
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
            foreach (Player p in players)
            {
                p.Health = Player.MaxHealth;
                p.IsAlive = true;
                p.Position.Y = Player.floorHeight + p.PlayerRectangle.Height;
                p.delayLeft = Player.InputDelay;
            }

            transition = false;
            endCurrentRoom = false;

            projectileList.Clear();
            currentBoss = null;

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
                if (!tutorialActive)
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

                    spriteBatch.Draw(questionMark, tutorialRect, Color.White);

                    for (int i = 0; i < multiplayerButtons.Count; i++)
                    {
                        if (multiplayerButtonStates[i])
                            spriteBatch.Draw(iconTextures[1], multiplayerButtons[i], Color.White);
                        else
                            spriteBatch.Draw(iconTextures[0], multiplayerButtons[i], Color.White);
                    }

                    cutscenes.Draw(spriteBatch, pixel);
                    if (cutscenes.alpha < 1 && !cutscenes.phase)
                    {
                        cutscenes = new Cutscene();
                    }
                }
                else
                {
                    switch (textboxesIndex)
                    {
                        case 0:
                            //menu.draw once samuel does it
                            break;
                        case 1:
                            //shop.Draw(gameTime, spriteBatch, shopFont, shopFontBold, shopTitleFont, pixel);
                            break;
                        case 2:
                            musicScreen.Draw(spriteBatch, pixel, shopTitleFont, shopFontBold, shopFont);
                            break;
                        case 3:
                            musicScreen.Draw(spriteBatch, pixel, shopTitleFont, shopFontBold, shopFont);
                            break;
                        
                        default:
                            if (!transition)
                                levelTimer++;
                            Rectangle roomRectangle = roomList[currentRoomIndex].backgroundRectangle;

                            spriteBatch.Draw(roomList[currentRoomIndex].background1, roomRectangle, Color.White);
                            spriteBatch.Draw(roomList[currentRoomIndex].background2, new Rectangle(roomRectangle.Right, 0, roomRectangle.Width, roomRectangle.Height), Color.White);


                            powerups.Draw(spriteBatch, collectableSheet, pixel, clock, skull, nuke, magnet, shopFontBold, shopFont);

                            //score and coins
                            int y = 70;
                            for (int i = 0; i < players.Count; i++)
                            {
                                spriteBatch.Draw(pixel, new Rectangle(window.Width - 300, y, 300, 130), Color.Black * .3f);
                                spriteBatch.Draw(collectableSheet, new Rectangle(window.Width - 285, y + 15, 40, 40), collectableRect[25], Color.White);
                                spriteBatch.DrawString(fonts[3], "    : " + players[i].Coins + "\n\n Distance: " + players[i].distanceTraveled, new Vector2(window.Width - 295, y + 20), Color.White);
                                //spriteBatch.DrawString(fonts[3], "Player " + i, new Vector2(window.Width - 120, y + 40), Color.White);
                                y += 140;
                            }
                            break;
                    }

                    textboxes[textboxesIndex].Draw(spriteBatch, pixel, fonts[3]);
                    textboxes[textboxesIndex].Update();
                    if (textboxes[textboxesIndex].exited)
                        if (textboxesIndex == 6)
                        {
                            textboxesIndex = 0;
                            tutorialActive = false;
                            cutsceneDestination = GameState.Menu;
                            gameState = GameState.Cutscene;
                            for (int i = 0; i < textboxes.Count; i++)
                            {
                                textboxes[i].exited = false;
                            }
                        }

                        else
                            textboxesIndex++;
                    cutscenes.Draw(spriteBatch, pixel);
                    if (cutscenes.alpha < 1 && !cutscenes.phase)
                    {
                        cutscenes = new Cutscene();
                    }
                }

            }
            if (gameState == GameState.Cutscene)
            {
                
                    
                cutscenes.cutseneActive = true;
                cutscenes.Draw(spriteBatch, pixel);
            }

            // shop
            if (gameState == GameState.Shop)
            {
                if (gameSongListInstance[3].State != SoundState.Playing)
                    gameSongListInstance[3].Play();

            }
            if (gameState == GameState.Music)
            {
                //musicScreen.Draw(spriteBatch, pixel, shopTitleFont, shopFontBold, shopFont);
                musicVolume = musicScreen.musicVolume;
                gameSongListInstance[3].Volume = (float)musicVolume / 5;
                if (gameSongListInstance[3].State != SoundState.Playing)
                    gameSongListInstance[3].Play();
            }
            if (gameState == GameState.Play)
            {

                



                if (!transition)
                    levelTimer++;

                int levelSeconds = levelTimer / 60;


                if (bossCooldown > 0 && !bossFight) bossCooldown--;
                if (levelSeconds > 999 && !bossFight && bossCooldown == 0)
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

                //Console.WriteLine("LoopImage: " + loopImage + "\ntransition: " + transition + "\nendCurrentRoom: " + endCurrentRoom);

                

                // draws the room

                Rectangle roomRectangle = roomList[currentRoomIndex].backgroundRectangle;

                spriteBatch.Draw(roomList[currentRoomIndex].background1, roomRectangle, Color.White);
                spriteBatch.Draw(roomList[currentRoomIndex].background2, new Rectangle(roomRectangle.Right, 0, roomRectangle.Width, roomRectangle.Height), Color.White);


                if (transition)
                {
                    //draws the obstacles in the next room
                    foreach (ProjectileClump obstacle in roomList[currentRoomIndex + 1].obstacleList)
                    {
                        obstacle.Current.Velocity.X = scrollSpeed;
                        if (obstacle.Current.Rectangle.Intersects(window))
                            obstacle.Current.anim.Idle = false;
                        obstacle.DrawAndUpdate(spriteBatch);
                    }
                }

                // draws the boss
                if (!bossFight) roomList[currentRoomIndex].Draw(spriteBatch);

                if(bossFight)
                    spriteBatch.DrawString(menuFont, "BOSS FIGHT!", new Vector2(window.Width / 2 - 100, 300), Color.Red);

                foreach (Player p in players)
                    p.Draw(spriteBatch);
                if (currentBoss != null) currentBoss.Draw(spriteBatch);
                foreach (Projectile p in projectileList)
                    p.Draw(spriteBatch);

                powerups.Draw(spriteBatch, collectableSheet, pixel, clock, skull, nuke, magnet, shopFontBold, shopFont);
                
                //score and coins
                int y = 70;
                for (int i = 0; i < players.Count; i++)
                {
                    spriteBatch.Draw(pixel, new Rectangle(window.Width - 300, y, 300, 130), Color.Black * .3f);
                    spriteBatch.Draw(collectableSheet, new Rectangle(window.Width - 285, y + 15, 40, 40), collectableRect[25], Color.White);
                    spriteBatch.DrawString(fonts[3], "    : " + players[i].Coins + "\n\n Distance: " + players[i].distanceTraveled, new Vector2(window.Width - 295, y + 20), Color.White);
                    //spriteBatch.DrawString(fonts[3], "Player " + i, new Vector2(window.Width - 120, y + 40), Color.White);
                    y += 140;
                }

                // draws hitboxes to help debug them if debugMode is active

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
                    foreach (Player p in players)
                        spriteBatch.Draw(pixel, new Rectangle(p.PlayerRectangle.X + playerHitBox.X, p.PlayerRectangle.Y + playerHitBox.Y, playerHitBox.Width, playerHitBox.Height), Color.Black);
                }

                
                

                cutscenes.Draw(spriteBatch, pixel);
                if (cutscenes.alpha < 1 && !cutscenes.phase)
                {
                    cutscenes = new Cutscene();
                }

            }

            Menu val = getCurrentMenu();
            if (val != null)
                val.DrawAndUpdate(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private Menu getCurrentMenu()
        {
            menus.TryGetValue(gameState, out Menu val);
            return val;
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
        public static bool KeyPressed(Keys k, KeyboardState kb) { return kb.IsKeyDown(k) && !Program.Game.oldKB.IsKeyDown(k); }
        public static bool KeyPressed(KeyboardState kb, params Keys[] k)
        {
            foreach (Keys kk in k) if (KeyPressed(kk, kb)) return true;
            return false;
        }


    }
}
