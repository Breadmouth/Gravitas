#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Gravitas;
using FarseerPhysics.Dynamics;
using BloomPostprocess;
#endregion

namespace Gravitas
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private BloomComponent bloomShader;
        public List<Score> m_scoreList;

        /// <summary>
        /// windowRes: Override the default window resolution to preffered
        /// bloomShader: Shader override of drawing
        /// m_gameStateManager: GameState is responsible for switching between GameStates
        /// </summary>
        Rectangle windowRes;
        GameStateManager m_gameStateManager;
        
        public Game1()
            : base()
        {
            windowRes = new Rectangle(
                0, 0,
                960, 544);

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content/";

            graphics.PreferredBackBufferWidth = windowRes.Width;
            graphics.PreferredBackBufferHeight = windowRes.Height;
            IsFixedTimeStep = false;

            bloomShader = new BloomComponent(this);
            Components.Add(bloomShader);
            bloomShader.Settings = new BloomSettings(null, 0.25f, 1.25f, 2.6f, 2f, 2.5f, 1f);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            ContentLibrary.LoadContentLibrary(Content);

            m_scoreList = new List<Score>();
            m_scoreList = LevelIO.LoadScore("scores.bin");

            //GameState Manager Stuff
            m_gameStateManager = new GameStateManager(this);
            
            PlayingGameState m_playingGameState =
                new PlayingGameState(m_gameStateManager, windowRes);
            
            LevelEditor m_editor =
                new LevelEditor(m_gameStateManager, windowRes);

            SplashScreen m_splash =
                new SplashScreen(m_gameStateManager, windowRes);

            MenuGameState m_menu =
                new MenuGameState(m_gameStateManager, windowRes);

            PauseGameState m_pause =
                new PauseGameState(m_gameStateManager, windowRes);

            TutorialGameState m_tutorial =
                new TutorialGameState(m_gameStateManager, windowRes);

            Level1GameState m_level1 =
                new Level1GameState(m_gameStateManager, windowRes);

            Level2GameState m_level2 =
                new Level2GameState(m_gameStateManager, windowRes);

            Level3GameState m_level3 =
                new Level3GameState(m_gameStateManager, windowRes);

            m_gameStateManager.SetState("playingGS", m_playingGameState);
            m_gameStateManager.SetState("editor", m_editor);
            m_gameStateManager.SetState("splashScreen", m_splash);
            m_gameStateManager.SetState("menu", m_menu);
            m_gameStateManager.SetState("pause", m_pause);
            m_gameStateManager.SetState("tutorial", m_tutorial);
            m_gameStateManager.SetState("level1", m_level1);
            m_gameStateManager.SetState("level2", m_level2);
            m_gameStateManager.SetState("level3", m_level3);

            m_gameStateManager.PushState("splashScreen");
            base.Initialize();

            for (int i = 0; i < m_scoreList.Count; i++)
            {
                Console.WriteLine(m_scoreList[i].m_level);
                Console.WriteLine(m_scoreList[i].m_time);
                Console.WriteLine(m_scoreList[i].m_deaths);
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            Console.WriteLine( 1 / (float)gameTime.ElapsedGameTime.TotalSeconds);
            m_gameStateManager.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            bloomShader.BeginDraw();

            graphics.GraphicsDevice.Clear(Color.Black);

            m_gameStateManager.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}
