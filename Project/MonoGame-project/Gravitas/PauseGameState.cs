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
    public class PauseGameState : PlayingGameState
    {

        //Variables
        public Sprite m_resume;
        public Sprite m_restart;
        public Sprite m_menu;

        private bool m_enterResume = false;
        private bool m_enterRestart = false;
        private bool m_enterMenu = false;

        private float m_switchSortMode = 3.0f;

        Score[] m_highScores;

        public PauseGameState(
            GameStateManager a_gameStateManager,
            Rectangle a_screenRes)
            : base(a_gameStateManager, a_screenRes)
        {
            m_currentLevel = "Pause";

            //GameState Portals
            m_menu = new Sprite
                (
                m_world,
                ContentLibrary.platformTexture,
                new Vector2(150, 110),
                1,
                new Vector2(-25, 450)
                );
            m_menu.m_body.UserData = "exit";
            m_menu.m_body.BodyType = BodyType.Static;
            m_menu.m_body.Rotation = 0;

            m_restart = new Sprite
                (
                m_world,
                ContentLibrary.platformTexture,
                new Vector2(110, 110),
                1,
                new Vector2(250, -200)
                );
            m_restart.m_body.UserData = "restart";
            m_restart.m_body.BodyType = BodyType.Static;
            m_restart.m_body.Rotation = -45;

            m_resume = new Sprite
                (
                m_world,
                ContentLibrary.platformTexture,
                new Vector2(110, 110),
                1,
                new Vector2(-300, -200)
                );
            m_resume.m_body.UserData = "resume";
            m_resume.m_body.BodyType = BodyType.Static;
            m_resume.m_body.Rotation = 45;

            m_text.Add("RESUME");
            m_text.Add("RESTART");
            m_text.Add("MENU");
            m_text.Add("BEST TIMES");

            m_textPos["RESUME"] = new Vector2(-230, -100);
            m_textPos["RESTART"] = new Vector2(140, -100);
            m_textPos["MENU"] = new Vector2(-50, 220);
            m_textPos["BEST TIMES"] = new Vector2(-60, 10);


            //NOTE::Rotation might actually be in radians
            
            m_player.m_body.OnCollision += SetState;

            LevelIO.LoadLevel("Pause.xml", this);

            m_player.m_body.Position = m_playerSpawnLocation;

            m_highScores = new Score[3];
            if (m_gameStateManager.m_game.m_scoreList.Count >= 3)
            {
                m_highScores[0] = m_gameStateManager.m_game.m_scoreList[0];
                m_highScores[1] = m_gameStateManager.m_game.m_scoreList[1];
                m_highScores[2] = m_gameStateManager.m_game.m_scoreList[2];
            }
            else if (m_gameStateManager.m_game.m_scoreList.Count == 2)
            {
                m_highScores[0] = m_gameStateManager.m_game.m_scoreList[0];
                m_highScores[1] = m_gameStateManager.m_game.m_scoreList[1];
                m_highScores[2] = new Score();
            }
            else if (m_gameStateManager.m_game.m_scoreList.Count == 1)
            {
                m_highScores[0] = m_gameStateManager.m_game.m_scoreList[0];
                m_highScores[1] = new Score();
                m_highScores[2] = new Score();
            }
            else
            {
                m_highScores[0] = new Score();
                m_highScores[1] = new Score();
                m_highScores[2] = new Score();
            }

            m_levelBoundary = 600;
        }

        private void UpdateScores()
        {
            if (m_gameStateManager.m_game.m_scoreList.Count >= 3)
            {
                m_highScores[0] = m_gameStateManager.m_game.m_scoreList[0];
                m_highScores[1] = m_gameStateManager.m_game.m_scoreList[1];
                m_highScores[2] = m_gameStateManager.m_game.m_scoreList[2];
            }
            else if (m_gameStateManager.m_game.m_scoreList.Count == 2)
            {
                m_highScores[0] = m_gameStateManager.m_game.m_scoreList[0];
                m_highScores[1] = m_gameStateManager.m_game.m_scoreList[1];
            }
            else if (m_gameStateManager.m_game.m_scoreList.Count == 1)
            {
                m_highScores[0] = m_gameStateManager.m_game.m_scoreList[0];
            }
        }

        private void CreatePlatforms(Texture2D a_texture)
        {
            float posX = m_windowRes.Width / 2;
            float posY = m_windowRes.Height / 2;

            //Bottom platform
            CreatePlatform(m_world,
                new Vector2(-posX, posY),
                new Vector2(posX, posY), a_texture);

            //Right platform
            CreatePlatform(m_world,
                new Vector2(posX, posY),
                new Vector2(posX, -posY), a_texture);

            //Top platform
            CreatePlatform(m_world,
                new Vector2(posX, -posY),
                new Vector2(-posX, -posY), a_texture);

            //Left platform
            CreatePlatform(m_world,
                new Vector2(-posX, -posY),
                new Vector2(-posX, posY - 150), a_texture);
        }

        public void CreatePlatform(World a_world, Vector2 lhs, Vector2 rhs, Texture2D a_texture)
        {
            Platform newPlatform = new Platform(a_world, lhs, rhs, a_texture);
            m_platformList.Add(newPlatform);
        }

        private bool SetState(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if ((string)fixtureB.Body.UserData == "resume")
            {
                return m_enterResume = true;
            }
            else if ((string)fixtureB.Body.UserData == "restart")
            {
                return m_enterRestart = true;
            }
            else if ((string)fixtureB.Body.UserData == "exit")
            {
                return m_enterMenu = true;
            }
            return true;
        }

        private void GameStateEnterChecks()
        {
            if (m_enterResume)
            {
                m_enterResume = false;
                ResetLevel();
                m_player.m_body.Position = m_playerSpawnLocation;
                m_gameStateManager.PopBackState();
            }
            else if (m_enterRestart)
            {
                m_enterRestart = false;
                ResetLevel();
                m_player.m_body.Position = m_playerSpawnLocation;
                m_gameStateManager.PopBackState();
                m_restartLevel = true;
            }
            else if (m_enterMenu)
            {
                m_enterMenu = false;
                ResetLevel();
                m_player.m_body.Position = m_playerSpawnLocation;
                m_gameStateManager.PopState();
                m_gameStateManager.PopState();
                m_gameStateManager.PushState("menu");
            }
        }

        public override void Update(GameTime a_gameTime)
        {
            //World
            base.Update(a_gameTime);

            UpdateScores();

            //Platforms
            foreach (Platform platform in m_platformList)
            {
                platform.Update();
            }

            //Player
            m_player.Update(a_gameTime, m_gravity);

            m_switchSortMode -= (float)a_gameTime.ElapsedGameTime.TotalSeconds;

            if (m_switchSortMode < 0)
            {
                if (m_sortLTH)
                {
                    m_sortLTH = false;
                    m_gameStateManager.m_game.m_scoreList.Sort(CompareScoresHTL);
                }
                else
                {
                    m_sortLTH = true;
                    m_gameStateManager.m_game.m_scoreList.Sort(CompareScoresLTH);
                }
                m_switchSortMode = 3.0f;
            }

            UpdateScores();
            GameStateEnterChecks();


        }

        override public void Draw(SpriteBatch a_spriteBatch)
        {
            a_spriteBatch.Begin(0, null, null, null, null, null, m_camera.View);

            base.Draw(a_spriteBatch);

            for (int i = 0; i < 3; i++)
            {
                a_spriteBatch.DrawString(
                    ContentLibrary.consolasFont,
                    m_highScores[i].m_time.ToString(),
                    new Vector2(-60, 50 + (i * 40)),
                    Color.White);
            }

            a_spriteBatch.End();
        }
    }
}
