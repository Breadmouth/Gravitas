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
    public class GPE
    {
        public Sprite m_bar;
        public Sprite m_barBackground;
        public Camera m_camera;
        public GameState m_gameState;

        private Player m_player;
        
        private float m_timer = 1.0f;
        public float m_maxGPE = 1.0f;
        public float m_gpeRemaining = 1.0f;

        /// <summary>
        /// Constructor for the GPE
        /// </summary>
        /// <param name="a_world">World for the GPE to exist in</param>
        /// <param name="a_gameState">gamestate to manage the gravity of</param>
        /// <param name="a_texture">texture for the gravity bar</param>
        /// <param name="a_camera">camera that provides the viewport to draw in</param>
        /// <param name="a_player">player that handles the gravity bar</param>
        public GPE(World a_world, GameState a_gameState, Texture2D a_texture, Camera a_camera, Player a_player)
        {
            m_camera = a_camera;
            m_gameState = a_gameState;
            m_bar = new Sprite(a_world, a_texture, new Vector2(450, 40), 0, m_camera.Position + new Vector2(0, 200));
            m_bar.m_body.BodyType = BodyType.Static;
            m_barBackground = new Sprite(a_world, a_texture, new Vector2(450, 40), 0, m_camera.Position + new Vector2(0, 200));
            m_barBackground.m_body.BodyType = BodyType.Static;
            m_bar.m_body.CollisionCategories = Category.None;
            m_barBackground.m_body.CollisionCategories = Category.None;

            m_player = a_player;
        }

        /// <summary>
        /// updates the GPE bar
        /// </summary>
        /// <param name="a_gameTime">GameTime of the game</param>
        public void Update(GameTime a_gameTime)
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                m_timer += (float)(a_gameTime.ElapsedGameTime.TotalSeconds * 5);

                if (!m_player.m_canJump)
                    m_gpeRemaining -= (float)a_gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Mouse.GetState().RightButton == ButtonState.Released)
            {
                m_timer -= (float)(a_gameTime.ElapsedGameTime.TotalSeconds * 5);
                if (m_player.m_canJump && m_gpeRemaining < m_maxGPE)
                    m_gpeRemaining += (float)a_gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (m_timer > 5)
            {
                m_timer = 5;
            }
            else if (m_timer < 1)
            {
                m_timer = 1;
            }

            if (m_gpeRemaining > m_maxGPE)
            {
                m_gpeRemaining = m_maxGPE;
            }
            else if (m_gpeRemaining <= 0)
            {
                m_gpeRemaining = 0;
                m_gameState.m_gravCanChange = false;
            }
            else
            {
                m_gameState.m_gravCanChange = true;
            }

            m_bar.Size = new Vector2(0 + (m_gpeRemaining * 450), 40);

            m_bar.Position = m_camera.Position + new Vector2(0, 200);
            m_barBackground.Position = m_bar.Position;
        }

        /// <summary>
        /// Draws the GPE bar
        /// </summary>
        /// <param name="a_spriteBatch">Gametime of the game</param>
        public void Draw(SpriteBatch a_spriteBatch)
        {
            m_bar.Draw(a_spriteBatch, Color.White * 0.2f * m_timer);
            m_barBackground.Draw(a_spriteBatch, Color.White * 0.1f);
        }
    }
}
