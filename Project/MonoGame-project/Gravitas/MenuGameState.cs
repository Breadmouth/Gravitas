using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravitas
{
    public class MenuGameState : PlayingGameState
    {
        //Variables
        public Sprite m_exit;
        public Sprite m_level1;
        public Sprite m_level2;
        public Sprite m_level3;

        //
        public MenuGameState(
            GameStateManager a_gameStateManager,
            Rectangle a_screenRes)
            : base(a_gameStateManager, a_screenRes)
        {
            m_exit = new Sprite
                (
                m_world,
                ContentLibrary.platformTexture,
                new Vector2(100, 100),
                1,
                new Vector2(0, 700)
                );
            m_exit.m_body.UserData = "exit";
            m_exit.m_body.BodyType = BodyType.Static;

            m_level1 = new Sprite
                (
                m_world,
                ContentLibrary.platformTexture,
                new Vector2(100, 150),
                1,
                new Vector2(-500, 80)
                );
            m_level1.m_body.UserData = "level1";
            m_level1.m_body.BodyType = BodyType.Static;

            m_level2 = new Sprite
                (
                m_world,
                ContentLibrary.platformTexture,
                new Vector2(100, 150),
                1,
                new Vector2(0, -350)
                );
            m_level2.m_body.UserData = "level2";
            m_level2.m_body.BodyType = BodyType.Static;

            m_level3 = new Sprite
                (
                m_world,
                ContentLibrary.platformTexture,
                new Vector2(100, 150),
                1,
                new Vector2(450, 80)
                );
            m_level3.m_body.UserData = "level3";
            m_level3.m_body.BodyType = BodyType.Static;

            m_text.Add("EXIT");
            m_text.Add("LEVEL1");
            m_text.Add("LEVEL2");
            m_text.Add("LEVEL3");

            m_textPos["EXIT"] = new Vector2(-40, 280);
            m_textPos["LEVEL1"] = new Vector2(-270, 0);
            m_textPos["LEVEL2"] = new Vector2(-45, -145);

            m_textPos["LEVEL3"] = new Vector2(180, 0);
    
            //Camera
            m_camera.Zoom -= 0.2f;

            LevelIO.LoadLevel("Menu.xml", this);
            m_player.m_body.Position = m_playerSpawnLocation;

            m_player.m_body.OnCollision += SetState;

            m_currentLevel = "Menu";

            m_levelBoundary = 800;
        }

        public override void Update(GameTime a_gameTime)
        {
            base.Update(a_gameTime);
        }

        override public void Draw(SpriteBatch a_spriteBatch)
        {
            a_spriteBatch.Begin(0, null, null, null, null, null, m_camera.View);

            base.Draw(a_spriteBatch);

            a_spriteBatch.End();
        }

        private bool SetState(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if ((string)fixtureB.Body.UserData == "exit")
            {
                return m_enterExit = true;
            }
            else if ((string)fixtureB.Body.UserData == "level1")
            {
                return m_enterLevel1 = true;
            }
            else if ((string)fixtureB.Body.UserData == "level2")
            {
                return m_enterLevel2 = true;
            }
            return true;
        }

        public void ResetMenu()
        {
            m_player.m_body.Position = m_playerSpawnLocation;
            m_gravity.direction = new Vector2(0, 1);
        }
    }
}
