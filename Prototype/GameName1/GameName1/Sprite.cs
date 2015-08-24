using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameName1
{
    public class Sprite
    {
        public const float m_unitToPixel = 100.0f;
        public const float m_pixelToUnit = 1 / m_unitToPixel;

        public Body m_body;
        public Vector2 m_Position
        {
            get { return m_body.Position * m_unitToPixel; }
            set { m_body.Position = value * m_pixelToUnit; }
        }

        public Texture2D m_texture;

        private Vector2 m_size;
        public Vector2 m_Size
        {
            get { return m_size * m_unitToPixel; }
            set { m_size = value * m_pixelToUnit; }
        }

        public Sprite(World a_world, Texture2D a_texture, Vector2 a_size, float a_mass)
        {
            m_body = BodyFactory.CreateRectangle(a_world, a_size.X * m_pixelToUnit, a_size.Y * m_pixelToUnit, 1);
            m_body.BodyType = BodyType.Dynamic;

            this.m_Size = a_size;
            this.m_texture = a_texture;
        }

        public Sprite(World a_world, Texture2D a_texture, float a_diameter, float a_mass)
        {
            m_size = new Vector2(a_diameter, a_diameter);
            m_body = BodyFactory.CreateCircle(a_world, (a_diameter / 2.0f) * CoordinateHelper.pixelToUnit, 1);

            m_body.BodyType = BodyType.Dynamic;

            this.m_Size = m_size;
            this.m_texture = a_texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle destination = new Rectangle
            (
                (int)m_Position.X,
                (int)m_Position.Y,
                (int)m_Size.X,
                (int)m_Size.Y
            );
            spriteBatch.Draw(m_texture, destination, null, Color.White, m_body.Rotation, new Vector2(m_texture.Width / 2.0f, m_texture.Height / 2.0f), SpriteEffects.None, 0);
        }
    }
}
