using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Gravitas.Physics;
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
    /// <summary>
    /// Player Class: Made by Jackson
    /// Description:
    /// 
    /// </summary>
    public class Player
    {

        private Texture2D m_texture { get; set; }
        public Vector2 m_size { get; set; }
        public Sprite m_cameraFocus { get; private set; }
        public Body m_body { get; set; }
        public Body m_collisionBody;
        public Color m_colour { get; private set; }
        public float m_speed { get; private set; }
        private bool m_flipped { get; set; }
        public bool m_canJump { get; set; }

        /// <summary>
        ///  Player constructor
        /// </summary>
        /// <param name="a_world">World reference</param>          
        /// <param name="a_texture">Texture reference</param>        
        /// <param name="a_startPosition">Starting Position</param>  
        /// <param name="a_size">Sizing</param>           
        /// <param name="a_bodyType">Type of body? Dynamic, Static or Kenetic?</param>       
        /// <param name="mass">Weight of mass</param>             
        public Player(World a_world, Texture2D a_texture, Vector2 a_startPosition, Vector2 a_size, float mass)
        {
            Vector2 bodySize = new Vector2(a_size.X, a_size.Y);

            //Assigning arguments
            m_texture = a_texture;
            m_size = bodySize;
            m_colour = Color.White;
            m_speed = 10.0f;
            m_canJump = false;

            // Initalising camera Sprite
            m_cameraFocus = new Sprite(a_world, a_texture, bodySize, mass / 2.0f, a_startPosition);
            m_cameraFocus.Position = a_startPosition + new Vector2(0, 20);
            m_cameraFocus.m_body.BodyType = BodyType.Static;

            // Initalising body
            m_body = BodyFactory.CreateCapsule
                (
                a_world,
                ConvertUnits.ToSimUnits(a_size.Y),
                ConvertUnits.ToSimUnits(a_size.X / 2),
                5.0f,
                ConvertUnits.ToSimUnits(a_startPosition),
                0.0f,
                BodyType.Dynamic
                );

            Vector2 collisionBodyPosition = new Vector2(
                a_startPosition.X, a_startPosition.Y + m_texture.Height);

            // Collision body
            m_collisionBody = BodyFactory.CreateRectangle
                (
                a_world,
                ConvertUnits.ToSimUnits(a_size.X),
                ConvertUnits.ToSimUnits(5),
                5.0f,
                ConvertUnits.ToSimUnits(collisionBodyPosition),
                0.0f,
                BodyType.Dynamic
                );

            m_collisionBody.Friction = 5.0f;

            //Collisions
            m_collisionBody.CollidesWith = Category.All;
            m_collisionBody.IgnoreCollisionWith(m_body);
            m_cameraFocus.m_body.CollidesWith = Category.None;

            // Create a joint to keep the torso upright
            JointFactory.CreateAngleJoint(a_world, m_body, m_cameraFocus.m_body);
            JointFactory.CreateAngleJoint(a_world, m_body, m_collisionBody);

            //If collided, jumping possible
            m_collisionBody.OnCollision += Body_OnCollision;
            m_collisionBody.OnSeparation += Body_OnRelease;
        }

        /// <summary>
        /// Move activates and applies a switch that alters direction depending on the input key
        /// </summary>
        /// <param name="a_gravity">reference to main gravity</param> 
        /// <param name="a_key">key input alters direction of movement</param>     
        //private void Move(Gravity a_gravity, Keys a_key)
        //{
        //    if (Keyboard.GetState().IsKeyDown(a_key))
        //    {
        //        Vector2 directionChange = new Vector2();

        //        switch (a_key)
        //        {
        //            case Keys.A:
        //                directionChange = new Vector2(-m_speed, 0);
        //                break;
        //            case Keys.D:
        //                directionChange = new Vector2(m_speed, 0);
        //                break;
        //            case Keys.W:
        //                directionChange = new Vector2(0, -m_speed);
        //                break;
        //            case Keys.S:
        //                directionChange = new Vector2(0, m_speed);
        //                break;
        //            case Keys.Space:
        //                directionChange = new Vector2(-a_gravity.direction.X, -a_gravity.direction.Y) * 200;
        //                break;
        //        }

        //        //Algorithm - Andrew G
        //        Vector2 gravNorm = new Vector2(-a_gravity.direction.Y, a_gravity.direction.X);
        //        gravNorm.Normalize();

        //        float force = Math.Abs( Vector2.Dot(gravNorm, directionChange));

        //        m_body.ApplyForce(directionChange, m_body.Position);
        //    }
        //}

        /// <summary>
        /// Collection of if statements detecting input keys.
        /// As well as applying rotation to the player based on gravities angle
        /// </summary>
        /// <param name="a_gravity">reference to main gravity</param> 
        private void Controls(Gravity a_gravity)
        {

#if PSM
			GamePadState inputState = GamePad.GetState(PlayerIndex.One);
			
			Vector2 lStickInput = inputState.ThumbSticks.Left;
			lStickInput.Normalize();
				
            bool aDown = lStickInput.X < -0.1 ? true : false;
            bool dDown = lStickInput.X >  0.1 ? true : false;
            bool wDown = lStickInput.Y < -0.1 ? true : false;
            bool sDown = lStickInput.Y >  0.1 ? true : false;
			
			if (m_canJump)
            {
                if (inputState.Buttons.A == ButtonState.Pressed)
                {
                    m_body.ApplyForce(-a_gravity.direction * 170, m_body.Position);
                    m_canJump = false;
                }
            }
#else
            bool aDown = Keyboard.GetState().IsKeyDown(Keys.A);
            bool dDown = Keyboard.GetState().IsKeyDown(Keys.D);
            bool wDown = Keyboard.GetState().IsKeyDown(Keys.W);
            bool sDown = Keyboard.GetState().IsKeyDown(Keys.S);

            if (m_canJump)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    m_body.ApplyForce(-a_gravity.direction * 170, m_body.Position);
                    m_canJump = false;
                }
            }
#endif	
            float invSqrt2 = (float)(1.0f / Math.Sqrt(2));

            Vector2 keyForce = new Vector2(0, 0);

            if (aDown) // moving left
            {
                if (wDown)
                {
                    keyForce = new Vector2(-invSqrt2, -invSqrt2);
                }
                else if (sDown)
                {
                    keyForce = new Vector2(-invSqrt2, invSqrt2);
                }
                else if (dDown)
                {

                }
                else
                {
                    keyForce = new Vector2(-1, 0);
                }
            }
            else if (dDown) // moving right
            {
                if (wDown)
                {
                    keyForce = new Vector2(invSqrt2, -invSqrt2);
                }
                else if (sDown)
                {
                    keyForce = new Vector2(invSqrt2, invSqrt2);
                }
                else if (aDown)
                {

                }
                else
                {
                    keyForce = new Vector2(1, 0);
                }
            }
            else if (wDown)
            {
                if (aDown)
                {
                    keyForce = new Vector2(-invSqrt2, -invSqrt2);
                }
                else if (dDown)
                {
                    keyForce = new Vector2(invSqrt2, -invSqrt2);
                }
                else if (sDown)
                {

                }
                else
                {
                    keyForce = new Vector2(0, -1);
                }
            }
            else if (sDown)
            {
                keyForce = new Vector2(0, 1);
            }


            Vector2 gravNorm = new Vector2(-a_gravity.direction.Y, a_gravity.direction.X);
            gravNorm.Normalize();

            float force = Vector2.Dot(gravNorm, keyForce);

            m_body.ApplyForce(gravNorm * force * 5, m_body.Position);


            if (m_canJump)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    m_body.ApplyForce(-a_gravity.direction * 170, m_body.Position);
                    m_canJump = false;
                }
            }

            m_cameraFocus.m_body.Rotation = a_gravity.angle;
        }

        /// <summary>
        /// Body_OnCollision re-enables jumping capability if on an object
        /// </summary>
        /// <param name="fixtureA">fixture passed in from Body A</param> 
        /// <param name="fixtureB">fixture passed in from Body B</param> 
        /// <param name="contact">Checking contact between fixtures</param> 
        private bool Body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if ((string)fixtureB.Body.UserData == "platform")
            {
                return m_canJump = true;
            }
            return false;
        }

        /// <summary>
        /// If no collision is detected this code is ran.
        /// This function sets canJump to false
        /// </summary>
        /// <param name="fixtureA">fixture passed in from Body A</param> 
        /// <param name="fixtureB">fixture passed in from Body B</param> 
        private void Body_OnRelease(Fixture fixtureA, Fixture fixtureB)
        {
            m_canJump = false;
        }

        /// <summary>
        /// Updating players behaviours
        /// </summary>
        /// <param name="a_gravity">reference to main gravity source</param>  
        /// <param name="a_gameTime"reference to main gameTime></param> 
        public void Update(GameTime a_gameTime, Gravity a_gravity)
        {
            m_collisionBody.Position = m_body.Position + ConvertUnits.ToSimUnits(new Vector2((float)-Math.Sin(a_gravity.angle), (float)Math.Cos(a_gravity.angle)) * (m_size.Y / 2 + 5f));
         
            Vector2 tempRot = new Vector2(-(float)Math.Sin(m_body.Rotation), (float)Math.Cos(m_body.Rotation));

            m_cameraFocus.m_body.Position = m_body.Position - (tempRot/1.5f) + (m_body.LinearVelocity * 0.3f);

            Controls(a_gravity);
        }

        /// <summary>
        /// Drawing of the player
        /// </summary>
        /// <param name="a_spriteBatch">reference to the main spriteBatch</param> 
        public void Draw(SpriteBatch a_spriteBatch)
        {
            Rectangle destination = new Rectangle
            (
                (int)ConvertUnits.ToDisplayUnits((m_body.Position.X)),
                (int)ConvertUnits.ToDisplayUnits((m_body.Position.Y)),
                (int)m_size.X,
                (int)m_size.Y + (int)(m_size.X)
            );
            
            a_spriteBatch.Draw(m_texture,
               destination,
               null, Color.White,
               m_body.Rotation,
               new Vector2(m_texture.Width / 2.0f, m_texture.Height / 2.0f),
               m_flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
               0);

            //Rectangle destinationR = new Rectangle
            //(
            //    (int)ConvertUnits.ToDisplayUnits((m_collisionBody.Position.X)),
            //    (int)ConvertUnits.ToDisplayUnits((m_collisionBody.Position.Y)),
            //    (int)m_size.X,
            //    (int)5
            //);

            //a_spriteBatch.Draw(m_texture,
            //   destinationR,
            //   null, Color.LemonChiffon,
            //   m_body.Rotation,
            //   new Vector2(m_texture.Width / 2.0f, m_texture.Height / 2.0f),
            //   m_flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
            //   0);
        }
    }
}
