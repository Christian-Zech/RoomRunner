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
    class MusicScreen
    {
        FileDialogue fileExplorer;

        MouseState mouse;
        Rectangle sliderHandleMusic;
        Rectangle sliderLineMusic;
        Rectangle sliderHandleSound;
        Rectangle sliderLineSound;
        Rectangle exitRect;

        double dXMusic;
        double dXSound;

        public double musicVolume;
        public double soundVolume;

        Rectangle gameMusicButton;
        Rectangle customMusicButton;
        Rectangle selectedButtonBorder;

        public bool customMusic;
        Rectangle addMusic;

        Color gameColor;
        Color customColor;

        public List<string> customMusicNames;

        public MusicScreen()
        {
            fileExplorer = new FileDialogue();
            mouse = Mouse.GetState();
            sliderHandleMusic = new Rectangle(1174, 590, 30, 30);
            sliderLineMusic = new Rectangle(700, 600, 500, 8);
            sliderHandleSound = new Rectangle(1174, 790, 30, 30);
            sliderLineSound = new Rectangle(700, 800, 500, 8);
            gameMusicButton = new Rectangle(750, 300, 170, 100);
            selectedButtonBorder = new Rectangle(0, 0, 0, 0);
            customMusicButton = new Rectangle(1010, 300, 170, 100);
            addMusic = new Rectangle(1470, 300, 150, 100);
            exitRect = new Rectangle(200, 70, 250, 100);
            dXMusic = 0;
            dXSound = 0;
            musicVolume = 1;
            soundVolume = 1;
            customMusic = false;
            customMusicNames = new List<string>();
            gameColor = new Color(75, 75, 75);
            customColor = new Color(111, 111, 111);
        }
        public void GetInput()
        {
            mouse = Mouse.GetState();
            Rectangle mouseRect = new Rectangle(mouse.X-40, mouse.Y-40, 80, 80);
            if (mouseRect.Intersects(sliderHandleMusic) && mouse.LeftButton == ButtonState.Pressed)
            {
                if (mouse.X < 1175 && mouse.X > 675)
                {
                    double temp = Math.Abs(mouse.X - sliderHandleMusic.X);
                    if (mouse.X < sliderHandleMusic.X)
                    {
                        dXMusic += temp / 500;
                        
                    }

                    else
                    {
                        dXMusic -= temp / 500;
                    }
                        
                    sliderHandleMusic.X = mouse.X;
                    musicVolume = 1 - dXMusic;
                }
                
            }
            if (mouseRect.Intersects(sliderHandleSound) && mouse.LeftButton == ButtonState.Pressed)
            {
                if (mouse.X < 1175 && mouse.X > 675)
                {
                    double temp = Math.Abs(mouse.X - sliderHandleSound.X);
                    if (mouse.X < sliderHandleSound.X)
                    {
                        dXSound += temp / 500;

                    }

                    else
                    {
                        dXSound -= temp / 500;
                    }

                    sliderHandleSound.X = mouse.X;
                    soundVolume = 1 - dXSound;
                }

            }
            if (mouseRect.Intersects(addMusic) && customMusic && mouse.LeftButton == ButtonState.Pressed)
            {
                string temp = fileExplorer.Show();
                if (!temp.Equals(""))
                    customMusicNames.Add(temp);
            }
            else if (mouseRect.Intersects(addMusic) && customMusic)
            {
                selectedButtonBorder = new Rectangle(addMusic.X - 2, addMusic.Y - 2, addMusic.Width + 4, addMusic.Height + 4);
            }
            
            if (mouseRect.Intersects(gameMusicButton))
            {
                selectedButtonBorder = new Rectangle(748, 298, 174, 104);
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    customMusic = false;
                    gameColor = new Color(75, 75, 75);
                    customColor = new Color(111, 111, 111);
                }
            }
            
            else if (mouseRect.Intersects(customMusicButton))
            {
                selectedButtonBorder = new Rectangle(1008, 298, 174, 104);
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    customMusic = true;
                    customColor = new Color(75, 75, 75);
                    gameColor = new Color(111, 111, 111);
                }
            }
            if (!mouseRect.Intersects(customMusicButton) && !mouseRect.Intersects(addMusic) && !mouseRect.Intersects(gameMusicButton))
            {
                selectedButtonBorder = new Rectangle(0, 0, 0, 0);
            }
                
            //for exiting
            if (mouseRect.Intersects(exitRect) && mouse.LeftButton == ButtonState.Pressed)
            {
                Game1.gameState = Game1.GameState.Menu;
            }
        }
        
        public void Draw(SpriteBatch spriteBatch, Texture2D pixel, SpriteFont titleFont, SpriteFont boldFont, SpriteFont smallFont)
        {
            GetInput();
            double mVolume = Math.Round(musicVolume * 100);
            string strMVolume = mVolume + "%";
            double sVolume = Math.Round(soundVolume * 100);
            string strSVolume = sVolume + "%";
            //drawing title
            spriteBatch.DrawString(titleFont, "SOUNDS", new Vector2(770, 40), Color.Black);

            //drawing back button
            spriteBatch.Draw(pixel, exitRect, Color.Green);
            spriteBatch.DrawString(boldFont, "    Menu", new Vector2(exitRect.X + 55, exitRect.Y + 20), Color.White);

            //drawing music selection buttons
            spriteBatch.Draw(pixel, selectedButtonBorder, Color.Black);
            spriteBatch.Draw(pixel, gameMusicButton, gameColor);
            spriteBatch.Draw(pixel, customMusicButton, customColor);
            
            spriteBatch.DrawString(boldFont, "Game Music", new Vector2(gameMusicButton.X + 20, gameMusicButton.Y + 30), Color.Black);
            spriteBatch.DrawString(boldFont, "Custom Music", new Vector2(customMusicButton.X + 8, customMusicButton.Y + 30), Color.Black);

            //drawing music options
            Color musicColor = new Color((float)(2.0f * dXMusic), (float)(2.0f * (1 - dXMusic)), 0);
            spriteBatch.DrawString(boldFont, "Music Volume", new Vector2(900, 530), Color.Black);
            spriteBatch.Draw(pixel, sliderLineMusic, Color.Black);
            spriteBatch.Draw(pixel, sliderHandleMusic, musicColor);
            spriteBatch.DrawString(smallFont, strMVolume, new Vector2(sliderHandleMusic.X + 7 - 2*strMVolume.Length, sliderHandleMusic.Y + 35), Color.Black) ;

            //drawing sound effect options
            Color soundColor = new Color((float)(2.0f * dXSound), (float)(2.0f * (1 - dXSound)), 0);
            spriteBatch.DrawString(boldFont, "Sound Volume", new Vector2(900, 730), Color.Black);
            spriteBatch.Draw(pixel, sliderLineSound, Color.Black);
            spriteBatch.Draw(pixel, sliderHandleSound, soundColor);
            spriteBatch.DrawString(smallFont, strSVolume, new Vector2(sliderHandleSound.X + 7 - 2 * strSVolume.Length, sliderHandleSound.Y + 35), Color.Black);

            //displaying playlist and button to add additional songs
            if (customMusic)
            {
                spriteBatch.DrawString(boldFont, "Playlist", new Vector2(1500, 200), Color.Black);
                int y = 250;
                int count = 1;
                foreach (string name in customMusicNames){
                    string[] temp = name.Split('\\');
                    spriteBatch.DrawString(smallFont, count + ". " + temp[temp.Length-1], new Vector2(1500-4*temp[temp.Length-1].Length, y), Color.Black);
                    y += 30;
                    
                    count++;
                }
                addMusic.Y = 50 + y;
                spriteBatch.Draw(pixel, addMusic, new Color(75, 75, 75));
                spriteBatch.DrawString(boldFont, "Add Music", new Vector2(1490, 80+y), Color.Black);
            }
        }
    }
}
