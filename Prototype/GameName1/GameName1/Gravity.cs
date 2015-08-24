using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameName1
{
    public class Gravity
    {
        public Vector2 m_downDirection;
        private World m_world;
        public float m_acceleration;
        private float m_angle;

        public Vector2 direction
        {
            get { return m_downDirection; }
            set 
            { 
                m_downDirection = value;
                m_downDirection.Normalize();
                m_world.Gravity = m_downDirection * m_acceleration;
                m_angle = (float)(Math.Atan2((double)m_downDirection.Y, (double)m_downDirection.X) - Math.PI/2);
            }
        }

        public float angle
        {
            get { return m_angle; }
        }

        public Gravity (World a_world, Vector2 a_direction, float a_acceleration)
        {
            m_world = a_world;
            m_acceleration = a_acceleration;
            direction = a_direction;
        }

    }
}
