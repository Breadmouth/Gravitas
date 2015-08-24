using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;

namespace Gravitas
{

    /// <summary>
    /// Platform class: Andrew Giannopoulos
    /// </summary>
    public class Platform
    {
        public enum PlatformState
        {
            GROUND,
            SLOPE,
            ROOF
        }

        public  Sprite          m_sprite;
        public  Sprite          m_connection;
        public  Sprite          m_connection2;
        private Vector2         m_normalVector;
        private float           m_rotation;
        private PlatformState   m_currentState;
        private World           m_world;
        public  Vector2         m_leftPoint;
        public  Vector2         m_rightPoint;

        /// <summary>
        /// Constructor for platform
        /// </summary>
        /// <param name="a_world"></param>World in which the platform is created and drawn
        /// <param name="a_leftPoint"></param>Left end point of the platform
        /// <param name="a_rightPoint"></param>Right end point of the platform
        /// <param name="a_texture"></param>Platform texture
        public Platform(World a_world, Vector2 a_leftPoint, Vector2 a_rightPoint, Texture2D a_texture)
        {
            m_world = a_world;

            m_leftPoint = a_leftPoint;
            m_rightPoint = a_rightPoint;

            // Initializing platform position and dimensions
            float width = MathX.LineLength(a_leftPoint, a_rightPoint);
            Vector2 midPoint = MathX.Midpoint(a_leftPoint, a_rightPoint);
            m_sprite = new Sprite(a_world, a_texture, new Vector2(width, 10), 0.0f, midPoint);
            m_rotation = MathX.GetAngleBetween(a_leftPoint, a_rightPoint) * (float)(Math.PI / 180);
            m_sprite.m_body.Rotation = m_rotation;
            m_sprite.m_body.BodyType = BodyType.Static;
            m_sprite.m_body.UserData = "platform";
            m_currentState = PlatformState.ROOF;

            // Initializing normal vector
            Vector2 difference = a_leftPoint - a_rightPoint;
            m_normalVector = new Vector2(-difference.Y, difference.X);
            m_normalVector.Normalize();

            m_connection = new Sprite(a_world, ContentLibrary.circleTexture, 5, 0.0f, a_leftPoint);
            m_connection.m_body.BodyType = BodyType.Static;
            m_connection2 = new Sprite(a_world, ContentLibrary.circleTexture, 5, 0.0f, a_rightPoint);
            m_connection2.m_body.BodyType = BodyType.Static;
            //m_connection.m_body.UserData = "platform";
        }

        /// <summary>
        /// The normal vector of the platform
        /// </summary>
        public Vector2 Normal
        {
            get { return m_normalVector; }
        }

        /// <summary>
        /// The state of the platform. Will return either GROUND (if it's a green walkable surface), ROOF (if it's a blue roof region), or SLOPE (if it's a red unwalkable slope)
        /// </summary>
        public PlatformState State
        {
            get { return m_currentState; }
        }

        /// <summary>
        /// draws platform based on what its state is.
        /// TODO: implement a way to separate sprites (possibly draw with different colour overlays)
        /// </summary>
        /// <param name="a_spriteBatch"></param>Spritebatch that is used when drawing the platform sprite
        public void Draw(SpriteBatch a_spriteBatch)
        {
            switch (m_currentState)
            {
                case PlatformState.GROUND:
                    m_sprite.Draw(a_spriteBatch, Color.Green);
                    break;
                case PlatformState.ROOF:
                    m_sprite.Draw(a_spriteBatch, Color.Blue);
                    break;
                case PlatformState.SLOPE:
                    m_sprite.Draw(a_spriteBatch, Color.Red);
                    break;
            }

            m_connection.Draw(a_spriteBatch, Color.White);
            m_connection2.Draw(a_spriteBatch, Color.White);
        }

        /// <summary>
        /// Updates the platform's current state and makes sure it's rotated correctly
        /// </summary>
        public void Update()
        {
            m_sprite.m_body.Rotation = m_rotation;
            Vector2 grav = m_world.Gravity;
            grav.Normalize();

            float dotProduct = Vector2.Dot(m_normalVector, grav);

            if (dotProduct > 0)
            {
                m_currentState = PlatformState.ROOF;
            }
            else if (dotProduct < -0.8f)
            {
                m_currentState = PlatformState.GROUND;
            }
            else
            {
                m_currentState = PlatformState.SLOPE;
            }


        }

    }
}
