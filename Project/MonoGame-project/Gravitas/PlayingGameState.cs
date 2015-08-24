using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Gravitas.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravitas
{
    /// <summary>
    /// Author: Jackson Luff, Iain Dowling
    /// </summary>
    public class PlayingGameState : GameState
    {
        //Variables
        public Player m_player { get; protected set; }
        public Sprite m_goal;
        public Vector2 m_goalLocation;
        public GPE m_gpe;
        public Score m_score;
        public List<Turret> m_turretList;
        public List<Bullet> m_bulletList;
        public float m_levelBoundary;

        protected static bool m_restartLevel = false;

        protected bool m_enterMenu = false;
        protected bool m_enterLevel1 = false;
        protected bool m_enterLevel2 = false;
        protected bool m_enterLevel3 = false;
        protected bool m_enterExit = false;  

        protected List<string> m_text;
        protected Dictionary<string, Vector2> m_textPos;

        public bool m_sortLTH = true;
        //

        /// <summary>
        /// PlayingGameState constructor
        /// </summary>
        /// <param name="a_gameStateManager">The manager for the gamestate</param>
        /// <param name="a_screenRes">the resolution for the screen</param>
        public PlayingGameState(
            GameStateManager a_gameStateManager,
            Rectangle a_screenRes)
            : base(a_gameStateManager, a_screenRes)
        {
            m_turretList = new List<Turret>();
            m_bulletList = new List<Bullet>();

            m_text = new List<string>();
            m_textPos = new Dictionary<string, Vector2>();

            m_player = new Player
                (
                    m_world,
                    ContentLibrary.platformTexture,
                    new Vector2(200, 200),
                    new Vector2(20, 50),
                    50
                );

            m_gpe = new GPE(m_world, this, ContentLibrary.platformTexture, m_camera, m_player);

            m_score = new Score();

            m_goal = new Sprite
                (
                m_world,
                ContentLibrary.platformTexture, 
                new Vector2(20, 20), 
                0.0f, 
                new Vector2(1000, 1000)
                );
            m_goal.m_body.BodyType = BodyType.Static;

            //Camera
            m_camera.TrackingBody = m_player.m_cameraFocus.m_body;

            m_player.m_body.OnCollision += ChangeLevel;
            m_player.m_body.OnCollision += KillPlayer;
        }

        /// <summary>
        /// This function checks for an existing file depending
        /// on it's filePath and resets all attributes
        /// </summary>
        protected void LoadLevel()
        {
            Console.WriteLine("Please enter a filename to load from: ");
            string filePath = Console.ReadLine();
            if (filePath != null)
            {
                LevelIO.LoadLevel(filePath, this);
            }
            m_player.m_body.Position = m_playerSpawnLocation;
            m_player.m_body.LinearVelocity = new Vector2(0, 0);

            m_goal.m_body.Position = m_goalLocation;

            
        }

        /// <summary>
        /// checks the collision between the player and the level changing sprites
        /// </summary>
        /// <param name="fixtureA">the player body</param>
        /// <param name="fixtureB">the colliding object's body</param>
        /// <param name="contact">Not used, but required by delegate</param>
        private bool ChangeLevel(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if ((string)fixtureB.Body.UserData == "menu")
            {
                return m_enterMenu = true;
            }
            else if ((string)fixtureB.Body.UserData == "level2")
            {
                return m_enterLevel2 = true;
            }
            else if ((string)fixtureB.Body.UserData == "level3")
            {
                return m_enterLevel3 = true;
            }
            return true;
        }

        /// <summary>
        /// This function is called when the player is killed
        /// due to being shot by a bullet.
        /// This bool checks the collision of the player
        /// with the bullet via the bullet's UserData and
        /// by doing so resets the players position, 
        /// resets gravity and adds 1 to the death counter
        /// 
        /// essentially if the player is colliding with the
        /// bullet, reset player's pos, gravity's dir' and
        /// add 1 to deaths, but if the player isn't colliding
        /// with the bullet then the resultant is false.
        /// </summary>
        /// <param name="fixtureA">The testing body fixture (in this case it's player)</param>
        /// <param name="fixtureB">Conducted fixture (in this case it's a bullet)</param>
        /// <param name="contact">Required by delegate</param>
        /// <returns></returns>
        private bool KillPlayer(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if ((string)fixtureB.Body.UserData == "bullet")
            {
                m_player.m_body.Position = m_playerSpawnLocation;
                m_gravity.direction = new Vector2(0, 1);
                m_score.m_deaths += 1;
            }
            return true;
            
        }

        /// <summary>
        /// This update function runs the BaseUpdate() from GameState.cs
        /// as well as all the necessary updating data for this state 
        /// like score accumulation and some levelIO stuff
        /// </summary>
        /// <param name="a_gameTime">Reference to GameTime</param>
        public override void Update(GameTime a_gameTime)
        {
            //World 
            BaseUpdate(a_gameTime);

            if (m_restartLevel)
            {
                ResetLevel();
                m_restartLevel = false;
            }

            m_score.m_time += (float)a_gameTime.ElapsedGameTime.TotalSeconds;

            m_gpe.Update(a_gameTime);

            for (int i = 0; i < m_turretList.Count; ++i)
            {
                m_turretList[i].Update(a_gameTime);
            }

            for (int i = 0; i < m_bulletList.Count; ++i)
            {
                m_bulletList[i].Update(a_gameTime);
            }

            //Player
            m_player.Update(a_gameTime, m_gravity);

            if (m_player.m_body.Position.X * 100 > m_levelBoundary || m_player.m_body.Position.X * 100 < -m_levelBoundary)
            {
                ResetLevel();
                m_score.m_deaths += 1;
            }
            else if (m_player.m_body.Position.Y * 100 > m_levelBoundary || m_player.m_body.Position.Y * 100 < -m_levelBoundary)
            {
                ResetLevel();
                m_score.m_deaths += 1;
            }

            //Particle spawn position
            m_particleSpawnPosition = m_player.m_collisionBody.Position * 100;

            if (Keyboard.GetState().IsKeyDown(Keys.Tab))
            {
                LoadLevel();
            }

            if (m_enterMenu)
            {
                ResetLevel();
                AddScore();
                m_gameStateManager.PopState();
                m_gameStateManager.PushState("menu");
            }
            if (m_enterExit)
            {
                ResetLevel();
                LevelIO.SaveScore("scores.bin", m_gameStateManager.m_game.m_scoreList);
                m_gameStateManager.PopState();
                m_game.Exit();
            }
            else if (m_enterLevel1)
            {
                ResetLevel();
                AddScore();
                m_gameStateManager.PopState();
                m_gameStateManager.PushState("level1");
            }
            else if (m_enterLevel2)
            {
                ResetLevel();
                AddScore();
                m_gameStateManager.PopState();
                m_gameStateManager.PushState("level2");
            }
            else if (m_enterLevel3)
            {
                ResetLevel();
                AddScore();
                m_gameStateManager.PopState();
                m_gameStateManager.PushState("level3");
            }
        }

        /// <summary>
        /// Resets the level by setting all important values to default
        /// </summary>
        protected void ResetLevel()
        {
            m_enterMenu = false;
            m_enterLevel1 = false;
            m_enterLevel2 = false;
            m_enterLevel3 = false;
            m_enterExit = false;
            m_player.m_body.Position = m_playerSpawnLocation;
            m_gravity.direction = new Vector2(0, 1);
            m_player.m_body.LinearVelocity = new Vector2(0, 0);
            m_player.m_body.AngularVelocity = 0;
            m_gpe.m_gpeRemaining = m_gpe.m_maxGPE;
        }

        /// <summary>
        /// Adds the score from the level into the score List
        /// </summary>
        protected void AddScore()
        {
            m_score.m_level = m_currentLevel;
            m_gameStateManager.m_game.m_scoreList.Add(m_score);
            m_score = new Score();
            if (m_sortLTH)
                m_gameStateManager.m_game.m_scoreList.Sort(CompareScoresLTH);
            else
                m_gameStateManager.m_game.m_scoreList.Sort(CompareScoresHTL);
        }

        /// <summary>
        /// a comparison delegate to sort the scores from lowest to highest
        /// </summary>
        /// <param name="x">first score to compare</param>
        /// <param name="y">second score to compare</param>
        /// <returns></returns>
        protected static int CompareScoresLTH(Score x, Score y)
        {
            if (x.m_time > y.m_time)
                return 1;
            else return - 1;
        }

        /// <summary>
        /// a comparison delegate to sort the scores from highest to lowest
        /// </summary>
        /// <param name="x">first score to compare</param>
        /// <param name="y">second score to compare</param>
        /// <returns></returns>
        protected static int CompareScoresHTL(Score x, Score y)
        {
            if (x.m_time > y.m_time)
                return -1;
            else return 1;
        }

        /// <summary>
        /// Handles all the drawing
        /// </summary>
        /// <param name="a_spriteBatch"></param>
        override public void Draw(SpriteBatch a_spriteBatch)
        {
            //TODO: Remove shader from effecting splashScreen
            
            BaseDraw(a_spriteBatch);

            for (int i = 0; i < m_turretList.Count; ++i)
            {
                m_turretList[i].Draw(a_spriteBatch);
            }

            for (int i = 0; i < m_bulletList.Count; ++i)
            {
                m_bulletList[i].Draw(a_spriteBatch);
            }

            //Drawing Temp_Player
            m_player.Draw(a_spriteBatch);

            for (int i = 0; i < m_text.Count; i++)
            {
                a_spriteBatch.DrawString
                    (
                    ContentLibrary.consolasFont,
                    m_text[i],
                    m_textPos[m_text[i]],
                    Color.White
                    );
            }

            m_goal.Draw(a_spriteBatch);

            m_gpe.Draw(a_spriteBatch);
        }

        /// <summary>
        /// Remove a bullet from the bullet list
        /// </summary>
        /// <param name="a_bullet">the bullet to be removed</param>
        public void RemoveBullet(Bullet a_bullet)
        {
            m_bulletList.Remove(a_bullet);
            m_world.RemoveBody(a_bullet.m_sprite.m_body);
        }
    }
}
