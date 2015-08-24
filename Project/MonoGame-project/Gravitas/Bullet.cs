using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravitas
{
    /// <summary>
    /// Created by Iain Dowling
    /// </summary>
    public class Bullet
    {
        public Sprite m_sprite;
        PlayingGameState m_playingGameState;
        private float m_lifeTime = 5;

        /// <summary>
        /// Constructor for the bullet class
        /// </summary>
        /// <param name="a_world">World to create the bullet in</param>
        /// <param name="a_texture">Texture to draw for the bullet</param>
        /// <param name="a_position">where to create the bullet initially</param>
        /// <param name="a_velocity">initial velocity to assign the bullet</param>
        public Bullet(World a_world, Texture2D a_texture, Vector2 a_position, Vector2 a_velocity, PlayingGameState a_playingGameState)
        {
            m_sprite = new Sprite(a_world, a_texture, 5, 1, a_position);
            m_sprite.m_body.LinearVelocity = a_velocity;
            m_playingGameState = a_playingGameState;
            m_sprite.m_body.UserData = "bullet";
        }

        public void Update(GameTime a_gameTime)
        {
            m_lifeTime -= (float)a_gameTime.ElapsedGameTime.TotalSeconds;

            if (m_lifeTime <= 0)
            {
                m_playingGameState.RemoveBullet(this);
            }
        }

        /// <summary>
        /// draw the bullet
        /// </summary>
        /// <param name="a_spriteBatch">SpriteBatch to use to draw the texture</param>
        public void Draw(SpriteBatch a_spriteBatch)
        {
            m_sprite.Draw(a_spriteBatch, Color.Magenta);
        }
    }
}
