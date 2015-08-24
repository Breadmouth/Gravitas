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
    public class SplashScreen : GameState
    {
        private Sprite m_background;
        private float m_countdown = 2.0f;

        public SplashScreen(
            GameStateManager a_gameStateManager,
            Rectangle a_windowRes) 
            : base(a_gameStateManager,
            a_windowRes)
        {
            m_currentLevel = "splashScreen";

            m_background = new Sprite(m_world,
                ContentLibrary.teamLogoTexture,
                new Vector2(
                    ContentLibrary.teamLogoTexture.Width / 2,
                    ContentLibrary.teamLogoTexture.Height/ 2
                    ),
                1,
                new Vector2(0, 0)
                );

            m_background.m_body.BodyType = BodyType.Static;

            m_camera.TrackingBody = m_background.m_body;
        }

        public override void Update(GameTime a_gameTime)
        {
            BaseUpdate(a_gameTime);
            m_camera.Update(a_gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Y))
            {
                m_gameStateManager.PopState();
                m_gameStateManager.PushState("editor");
            }

            if (m_countdown < 0.0f && m_countdown > -3.0f)
            {
                if (m_camera.TrackingBody != null)
                    m_camera.TrackingBody = null;

                m_camera.MoveCamera(new Vector2(0, 0.03f));
                m_countdown -= (float)a_gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if(m_countdown < -3.0f)
            {
                m_gameStateManager.PopState();
                m_gameStateManager.PushState("tutorial");
            }
            else
            {
                m_countdown -= (float)a_gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public override void Draw(SpriteBatch a_spriteBatch)
        {
            a_spriteBatch.Begin(0, null, null, null, null, null, m_camera.View);

            BaseDraw(a_spriteBatch);

            m_background.Draw(a_spriteBatch);

            a_spriteBatch.End();
        }
    }
}
