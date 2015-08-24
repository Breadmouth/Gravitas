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
    /// Author: Jackson Luff
    /// </summary>
    public class Particle : Sprite
    {
        public Vector2 m_position { get; private set; }
        public Vector2 m_velocity { get; private set; }
        public float m_TTL        { get; private set; }
        public float m_LivingTime { get; private set; }

        private Random m_rand;
        private float m_speed;
        private float m_direction;
        private Color m_colour;
        /// <summary>
        /// Body_OnCollision re-enables jumping capability if on an object
        /// </summary>
        /// <param name="a_fixtureA">fixture passed in from Body A</param> 
        /// <param name="a_fixtureB">fixture passed in from Body B</param> 
        /// <param name="a_contact">Checking contact between fixtures</param> 
        private bool Body_OnCollision(Fixture a_fixtureA, Fixture a_fixtureB, FarseerPhysics.Dynamics.Contacts.Contact a_contact)
        {
            if ((string)a_fixtureB.Body.UserData == "platform")
            {
                m_body.CollisionCategories = Category.None;
                m_body.OnCollision -= Body_OnCollision;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Particle is an effect object that is emitted through a Particle System
        /// </summary>
        /// <param name="a_world">Ref' to the GameState world</param> 
        /// <param name="a_texture">Prefered texture for the particle</param> 
        /// <param name="a_position">Prefered position for the particle</param> 
        /// <param name="a_radius">Prefered radius for the particle</param> 
        /// <param name="a_speed">Prefered speed for the particle</param> 
        /// <param name="a_timeToLive">Prefered TTL for the particle</param> 
        public Particle(World a_world, Texture2D a_texture, Vector2 a_position, float a_radius, float a_speed, float a_timeToLive)
            : base(a_world, a_texture, a_radius, 1, a_position)
        {
            m_rand = new Random();
            m_colour = new Color();
            m_colour.PackedValue = (uint)m_rand.Next(int.MaxValue);

            m_position = a_position;

            m_speed = a_speed;
            //m_speed = (float)(m_rand.NextDouble() * m_speed * m_position.Length());
            m_speed = (float)(m_rand.NextDouble() * m_speed);
            m_direction = (float)(m_rand.NextDouble() * (Math.PI * 2));

            m_velocity += new Vector2(
                (float)(m_speed * Math.Sin(Math.PI * (m_speed * m_direction))),
                (float)(m_speed * Math.Cos(Math.PI * (m_speed * m_direction))));

            m_TTL = a_timeToLive;

            m_body.IgnoreGravity = true;
            m_body.CollisionCategories = Category.All;

            m_body.OnCollision += Body_OnCollision;

            m_body.Rotation += m_rand.Next(360);
        }

        /// <summary>
        /// The update function for the particles applies a constant velocity
        /// and applies the elapsed milliseconds to livingTime to eventually
        /// exceed TTL (Time To Live)
        /// </summary>
        /// <param name="a_gameTime">GameTime reference</param> 
        public void Update(GameTime a_gameTime)
        {
            m_LivingTime += (float)a_gameTime.ElapsedGameTime.Milliseconds;

            m_body.ApplyForce(m_velocity);
        }

        /// <summary>
        /// Draw is used to project the texture of the particle
        /// </summary>
        /// <param name="a_spriteBatch">spriteBatch reference</param> 
        public new void Draw(SpriteBatch a_spriteBatch)
        {
            float fadeVar = m_LivingTime / m_TTL;
            Draw(a_spriteBatch, Color.Lerp(m_colour, Color.Transparent, fadeVar ));
        }
    }
}
