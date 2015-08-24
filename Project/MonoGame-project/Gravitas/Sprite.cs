using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
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
    /// Created/Handled by Iain Dowling
    /// </summary>
    public class Sprite
    {
        public Texture2D m_texture { get; private set; }
        public Body m_body;
        public Vector2 Position
        {
            get { return ConvertUnits.ToDisplayUnits(m_body.Position); }
            set { m_body.Position = ConvertUnits.ToSimUnits(value); }
        }
        private Vector2 m_size;
        public Vector2 Size
        {
            get { return m_size; }
            set { m_size = value; }
        }

        /// <summary>
        /// Sprite Constructor that creates a rectangle
        /// </summary>
        /// <param name="a_world"></param>World for the Sprite to exist in
        /// <param name="a_texture"></param>Texture to draw for the Sprite
        /// <param name="a_size"></param>Size of the Sprite
        /// <param name="a_mass"></param>Mass of the Sprite
        /// <param name="a_position"></param>Starting position of the Sprite
        public Sprite(World a_world, Texture2D a_texture, Vector2 a_size, float a_mass, Vector2 a_position)
        {
            m_body = BodyFactory.CreateRectangle(a_world, ConvertUnits.ToSimUnits(a_size.X), ConvertUnits.ToSimUnits(a_size.Y), 1);
            m_body.BodyType = BodyType.Dynamic;
            m_body.Position = a_position;

            m_texture = a_texture;
            m_body.Mass = a_mass;
            m_size = a_size;
            Position = a_position;
        }

        /// <summary>
        /// Sprite Constructor that creates a Capsule
        /// </summary>
        /// <param name="a_world"></param>World for the Sprite to exist in
        /// <param name="a_texture"></param>Texture to draw for the Sprite
        /// <param name="a_size"></param>Size of the Sprite
        /// <param name="a_mass"></param>Mass of the Sprite
        /// <param name="a_position"></param>Starting position of the Sprite
        public Sprite(World a_world, Texture2D a_texture, float a_height, float a_radius, float a_mass, Vector2 a_position)
        {
            m_body = BodyFactory.CreateCapsule(a_world, ConvertUnits.ToSimUnits(a_height), ConvertUnits.ToSimUnits(a_radius), 1);
            m_body.BodyType = BodyType.Dynamic;
            m_body.Position = a_position;

            m_texture = a_texture;
            m_body.Mass = a_mass;
            m_size.X = a_radius * 2;
            m_size.Y = a_height;
            Position = a_position;
        }

        /// <summary>
        /// Sprite constructor that creates a circle
        /// </summary>
        /// <param name="a_world"></param>World for the Sprite to exist in
        /// <param name="a_texture"></param>Texture to draw for the Sprite
        /// <param name="a_radius"></param>Size of the Sprite
        /// <param name="a_mass"></param>Mass of the Sprite
        /// <param name="a_position"></param>Starting position of the Sprite
        public Sprite(World a_world, Texture2D a_texture, float a_radius, float a_mass, Vector2 a_position)
        {
            m_body = BodyFactory.CreateCircle(a_world, ConvertUnits.ToSimUnits(a_radius), 1);
            m_body.BodyType = BodyType.Dynamic;
            m_body.Position = a_position;

            m_texture = a_texture;
            m_body.Mass = a_mass;
            m_size.X = a_radius * 2;
            m_size.Y = a_radius * 2;
            Position = a_position;
        }
        /// <summary>
        /// Draw the Sprite to the screen
        /// </summary>
        /// <param name="a_spriteBatch"></param>SpriteBatch that will handle drawing the Sprite texture
        public void Draw(SpriteBatch a_spriteBatch)
        {
            Rectangle _rectangle = new Rectangle
            (
                (int)Position.X,
                (int)Position.Y,
                (int)m_size.X,
                (int)m_size.Y
            );
            a_spriteBatch.Draw(m_texture, _rectangle, null, Color.White, m_body.Rotation, new Vector2(m_texture.Width / 2.0f, m_texture.Height / 2.0f), SpriteEffects.None, 0);
        }

        public void Draw(SpriteBatch a_spriteBatch, Color a_color)
        {
            Rectangle _rectangle = new Rectangle
            (
                (int)Position.X,
                (int)Position.Y,
                (int)m_size.X,
                (int)m_size.Y
            );
            a_spriteBatch.Draw(m_texture, _rectangle, null, a_color, m_body.Rotation, new Vector2(m_texture.Width / 2.0f, m_texture.Height / 2.0f), SpriteEffects.None, 0);
        }
    }
}
