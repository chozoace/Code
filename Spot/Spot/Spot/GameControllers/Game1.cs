using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Spot
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        EffectComponent effect;
        EffectController controlEffects;

        public enum GameState
        {
            Gameplay,
            MainMenu,
            Pause,
            GameOver
        }
        public GameState gameState = GameState.MainMenu;//should be main menu

        LevelManager levelManager;
        MainMenu mainMenu;
        GameOverScreen gameOverScreen;
        static Game1 instance;
        //bool singletonEnforcer = false;

        public bool EffectsOn = false;
        public bool usingController;
        bool songStarted = false;
        public Song song;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 480;
            graphics.PreferredBackBufferWidth = 640;
            effect = new EffectComponent(this);
            controlEffects = new EffectController();
            Components.Add(effect);
            usingController = false;

            instance = this;
        }

        public static Game1 Instance()
        {
            return instance;
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            mainMenu = new MainMenu();
            gameOverScreen = new GameOverScreen();
            levelManager = new LevelManager(Content, spriteBatch);


            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            song = Content.Load<Song>("Music/EpicSong");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            //Debug.WriteLine(gameState);
            if (gameState == GameState.Gameplay)
            {
                levelManager.Update();

                if (!songStarted)
                {
                    //MediaPlayer.Play(song);
                    songStarted = true;
                }
            }
            else if (gameState == GameState.MainMenu)
            {
                mainMenu.Update();
            }
            else if (gameState == GameState.GameOver)
            {
                gameOverScreen.Update();
            }



            if (EffectsOn)
                controlEffects.Update();

            base.Update(gameTime);
        }

        public ContentManager getContent()
        {
            return Content;
        }

        public SpriteBatch getSpriteBatch()
        {
            return spriteBatch;
        }

        public void gameOver()
        {
            gameState = GameState.GameOver;
            gameOverScreen.visible = true;
        }

        protected override void Draw(GameTime gameTime)
        {
            if (EffectsOn)
            {
                GraphicsDevice device = graphics.GraphicsDevice;
                Viewport viewPort = device.Viewport;

                effect.BeginDraw();

                device.Clear(Color.CornflowerBlue);

                spriteBatch.Begin();

                if (gameState == GameState.Gameplay)
                {
                    levelManager.drawGame(spriteBatch);
                }
                else if (gameState == GameState.MainMenu)
                {
                    mainMenu.Draw(spriteBatch);
                }

                spriteBatch.End();

                device.DepthStencilState = DepthStencilState.Default;

            }
            else
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);

                spriteBatch.Begin();

                if (gameState == GameState.Gameplay)
                {
                    levelManager.drawGame(spriteBatch);
                }
                else if (gameState == GameState.MainMenu)
                {
                    mainMenu.Draw(spriteBatch);
                }
                else if (gameState == GameState.GameOver)
                {
                    gameOverScreen.Draw(spriteBatch);
                }

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
