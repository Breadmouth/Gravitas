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
    public class Platform
    {
        Sprite m_sprite;
        public float m_rotation;

        public Platform(World a_world, Texture2D a_texture, Vector2 a_size, Vector2 a_startPosition)
        {
            m_sprite = new Sprite(a_world, a_texture, a_size, 0.0f);
            m_sprite.m_Position = a_startPosition;
            m_sprite.m_body.BodyType = BodyType.Static;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            m_sprite.Draw(spriteBatch);
        }

        public void Update()
        {
            //m_rotation += 0.05f;
            m_sprite.m_body.Rotation = m_rotation;
        }
    }
}
