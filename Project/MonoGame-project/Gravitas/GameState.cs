using FarseerPhysics.Dynamics;
using Gravitas.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Gravitas
{
    /// <summary>
    /// Author: Jackson Luff
    /// </summary>
    abstract public class GameState
    {
        //Add BaseUpdate() to Update() by default
        //Add BaseDraw()   to Draw()   by default

        protected GameStateManager m_gameStateManager;
        protected Game m_game;
        public World m_world { get; protected set; }
        public Gravity m_gravity { get; protected set; }
        public Camera m_camera { get; protected set; }
        public Rectangle m_windowRes { get; protected set; }
        public List<Platform> m_platformList;
        public List<Particle> m_particleList;
        public Vector2 m_playerSpawnLocation;
        public bool m_gravCanChange = true;
        public string m_currentLevel;
        public Vector2 m_particleSpawnPosition;

        private Vector2 renderPadding;

#if PSM
		public GamePadState m_inputStateP1 = GamePad.GetState(PlayerIndex.One);
#else
        private Vector2 m_gravityBegin;
        public Vector2 m_mousePos;
        private bool m_mouseDragging = false;
#endif

        /// <summary>
        /// Constructor for the Gamestate class
        /// </summary>
        /// <param name="a_gameStateManager">The manager for the gamestate</param>
        /// <param name="a_windowRes">the resolution for the screen</param>
        public GameState(GameStateManager a_gameStateManager, Rectangle a_windowRes)
        {
            m_platformList = new List<Platform>();
            m_particleList = new List<Particle>();

            //Set Resoultion
            m_windowRes = a_windowRes;

            //World
            m_world = new World(new Vector2(0, 0));

            //Gravity
            m_gravity = new Gravity(m_world, new Vector2(0, 1), 7.0f);

            m_gameStateManager = a_gameStateManager;
            m_game = m_gameStateManager.m_game;
            m_camera = new Camera(m_game.GraphicsDevice);

            m_playerSpawnLocation = new Vector2(0, 0);

            //Particle stuff
            m_particleSpawnPosition = new Vector2();
        }

        /// <summary>
        /// abstract update for all classes inheriting from gamestate to implement
        /// </summary>
        /// <param name="a_gameTime"></param>
        abstract public void Update(GameTime a_gameTime);

        /// <summary>
        /// abstract draw for all classes inheriting from gamestate to implement
        /// </summary>
        /// <param name="a_spritebatch"></param>
        abstract public void Draw(SpriteBatch a_spritebatch);

        /// <summary>
        /// This Update function is cycled through for all game states.
        /// The use of this function is so that the world updates it's placements
        /// and to run the particle function
        /// 
        /// Cross Platform Conditional Symbols are used to define update types
        /// depending on the platform
        /// </summary>
        /// <param name="a_gameTime"></param>
        protected void BaseUpdate(GameTime a_gameTime)
        {
            //World
            m_world.Step((float)a_gameTime.ElapsedGameTime.TotalSeconds);

            //Camera stuff
            m_camera.Update(a_gameTime);

            renderPadding = new Vector2(
                m_camera.Position.X - (m_windowRes.X / 100),
                m_camera.Position.Y - (m_windowRes.Y / 100)
                );

            PlatformSysUpdate(a_gameTime);
            ParticleSysUpdate(a_gameTime);
#if PSM
			PSMUpdate(a_gameTime);
#else
            WindowsUpdate(a_gameTime);
#endif
        }

        /// <summary>
        /// Platforms the sys update.
        /// </summary>
        /// <param name='a_gameTime'>
        /// A_game time.
        /// </param>
        private void PlatformSysUpdate(GameTime a_gameTime)
        {
            //Platforms
            for (int i = 0; i < m_platformList.Count; ++i)
            {
                m_platformList[i].Update();
            }
        }

        /// <summary>
        /// Particles the sys update.
        /// </summary>
        /// <param name='a_gameTime'>
        /// A_game time.
        /// </param>
        private void ParticleSysUpdate(GameTime a_gameTime)
        {
            if(m_particleList.Count < 150)
            {
                m_particleList.Add(new Particle(
                           m_world,
                           ContentLibrary.platformTexture,
                           m_particleSpawnPosition,
                           5, 5.5f, 1000));
            }

            for (int i = 0; i < m_particleList.Count; ++i)
            {
                Particle piece = m_particleList[i];

                piece.Update(a_gameTime);

                if (piece.m_LivingTime > piece.m_TTL)
                {
                    m_world.RemoveBody(piece.m_body);
                    m_particleList.RemoveAt(i--);
                    break;
                }
            }
        }

        /// <summary>
        /// This method is to run updates only necessary to Windows.
        /// The features inside this method provide camera zooming -
        /// ('O' to increase, 'P' to decrease)
        /// 
        /// To allow the click and drag capability for the right 
        /// button on the mouse so that the user can manipulate 
        /// their direction of gravity based on their desired dragging direction.
        /// 
        /// The method also updates the mouse position but modified
        /// with the consideration of zooming of the camera to provide
        /// effective placement
        /// </summary>
        /// <param name="a_gameTime"></param>
        private void WindowsUpdate(GameTime a_gameTime)
        {
            //Zooming features
            if (Keyboard.GetState().IsKeyDown(Keys.O))
            {
                m_camera.Zoom += 0.01f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                m_camera.Zoom -= 0.01f;
            }

            //Enabling 'click n drag' for gravity on PC
            ButtonState rmb = Mouse.GetState().RightButton;

            if (!m_mouseDragging && rmb == ButtonState.Pressed && m_gravCanChange)
            {
                m_mouseDragging = true;
                m_gravityBegin = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            }
            else if (m_mouseDragging && rmb == ButtonState.Released && m_gravCanChange)
            {
                if (m_gravityBegin != new Vector2(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    m_mouseDragging = false;
                    Vector2 gravityDirection = (new Vector2(Mouse.GetState().X, Mouse.GetState().Y)) - m_gravityBegin;
                    m_gravity.direction = gravityDirection;
                }
            }

            //Scaling mousePosition with consideration of camera zooming
            m_mousePos = (new Vector2(Mouse.GetState().X, Mouse.GetState().Y) -
                 new Vector2(m_windowRes.Width / 2, m_windowRes.Height / 2))
                 * (1 / m_camera.Zoom)
                 + m_camera.Position;

            //HotKeys
            if (Keyboard.GetState().IsKeyDown(Keys.Y))
            {
                m_gameStateManager.PopState();
                m_gameStateManager.PushState("editor");
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (m_currentLevel != "Pause")
                    m_gameStateManager.PushState("pause");
            }
        }

        /// <summary>
        /// This method is to run all the 'Base' processing for PSM
        /// The process returns the vector of the right thumbstick
        /// and then normalises it's values to receive it's direction
        /// and thus with this unit Vector2 we apply the direction
        /// of gravity with a hardcoded force
        /// </summary>
        /// <param name="a_gameTime"></param>
        private void PSMUpdate(GameTime a_gameTime)
        {
            // Updating the direction of the gravity (and it's force) -
            // dependant on the direction of the right thumb stick for PSM
            GamePadState playerInput = GamePad.GetState(PlayerIndex.One);
            Vector2 rStickInput = playerInput.ThumbSticks.Right;
            rStickInput.Normalize();
            m_gravity.direction = rStickInput * 50;
        }

        /// <summary>
        /// Draw code that all classes inheriting from GameState should implement
        /// </summary>
        /// <param name="a_spriteBatch"></param>
        protected void BaseDraw(SpriteBatch a_spriteBatch)
        {
            /*
            for (int i = 0; i < m_platformList.Count; ++i )
            {
                float padding = (float)((m_windowRes.Height / 100) / 1.5);

                float X1 =  (float)(m_camera.Position.X - padding);
                float X2 =  (float)(m_camera.Position.X + padding);
                float Y1 =  (float)(m_camera.Position.Y - padding);
                float Y2 =  (float)(m_camera.Position.Y + padding);

                if (   m_platformList[i].m_sprite.m_body.Position.X > X1
                    && m_platformList[i].m_sprite.m_body.Position.X < X2
                    && m_platformList[i].m_sprite.m_body.Position.Y > Y1
                    && m_platformList[i].m_sprite.m_body.Position.Y < Y2)
                {
                    m_platformList[i].Draw(a_spriteBatch);
                }
            }
            */

            //Drawing particles
            foreach (Particle piece in m_particleList)
            {
                piece.Draw(a_spriteBatch);
            }

            //Drawing platforms
            foreach (Platform plank in m_platformList)
            {
                //if (plank.m_sprite.m_body.Position.X < renderPadding.X
                //    && plank.m_sprite.m_body.Position.X > -renderPadding.X)
                {
                    plank.Draw(a_spriteBatch);
                }
            }
        }
    }
}
