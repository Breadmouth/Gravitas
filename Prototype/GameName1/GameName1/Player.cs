using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameName1;

namespace GameName1
{
    public class Player
    {
        //public Sprite torso;
        public Body torso;
        public Sprite thing;
        float speed = 1.0f;
        bool canJump = false;

        Vector2 m_size;
        public Texture2D m_texture;

        Gravity m_gravRef;

        public Player(World world, Texture2D torsoTexture, Texture2D wheelTexture, Vector2 size, float mass, Vector2 startPosition)
        {
            Vector2 torsoSize = new Vector2(size.X, size.Y - size.X / 2.0f);
            float wheelSize = size.X;

            thing = new Sprite(world, torsoTexture, torsoSize, mass / 2.0f);
            thing.m_Position = startPosition + new Vector2(0, 20);
            thing.m_body.BodyType = BodyType.Static;

            // Create the torso
            torso = BodyFactory.CreateCapsule(world, size.Y * 0.01f, size.X / 2 * 0.01f, 1);
            torso.BodyType = BodyType.Dynamic;

            m_size = torsoSize;
            m_texture = torsoTexture;
            torso.Position = new Vector2(2, 2);

            torso.CollidesWith = Category.All;
            thing.m_body.CollidesWith = Category.None;

            // Create a joint to keep the torso upright
            JointFactory.CreateAngleJoint(world, torso, thing.m_body);
        }

        bool Body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            canJump = true;
            return true;
        }

        void Body_OnRelease(Fixture fixtureA, Fixture fixtureB)
        {
            canJump = false;
        }

        public void Update(Gravity a_gravity)
        {
            thing.m_body.Position = torso.Position - new Vector2(0, 0.6f) + (torso.LinearVelocity * 0.5f);
            torso.OnCollision += Body_OnCollision;
            torso.OnSeparation += Body_OnRelease;



            m_gravRef = a_gravity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle destination = new Rectangle
            (
                (int)(torso.Position.X * 100),
                (int)(torso.Position.Y * 100),
                (int)m_size.X,
                (int)m_size.Y + (int)m_size.X
            );
            spriteBatch.Draw(m_texture, destination, null, Color.White, torso.Rotation, new Vector2(m_texture.Width / 2.0f, m_texture.Height / 2.0f), SpriteEffects.None, 0);
        }

        public void Move(Movement movement)
        {
            switch (movement)
            {
                case Movement.L:
                    torso.ApplyForce(new Vector2(-speed, 0), torso.Position);
                    break;
            
                case Movement.R:
                    torso.ApplyForce(new Vector2(speed, 0), torso.Position);
                    break;
            
                case Movement.U:
                    torso.ApplyForce(new Vector2(0, -speed), torso.Position);
                    break;
            
                case Movement.D:
                    torso.ApplyForce(new Vector2(0, speed), torso.Position);
                    break;
            }
            if (canJump)
                if (movement == Movement.J)
                {
                    torso.ApplyForce(new Vector2( -m_gravRef.m_downDirection.X, -m_gravRef.m_downDirection.Y) * 35, torso.Position);
                    canJump = false;
                }
        }
    }
}
