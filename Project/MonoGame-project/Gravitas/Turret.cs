using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Gravitas.Physics;
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
    public class Turret
    {

        private Texture2D m_bulletTexture;

        public PlayingGameState m_state;

        public Sprite m_top;
        public Sprite m_base;
        public Vector2 m_angle;
        public Player m_player;
        public float m_cooldown;

        /// <summary>
        /// Constructor for the Turret class
        /// </summary>
        /// <param name="a_world">World for the turret to exist in</param>
        /// <param name="a_topTexture">Texture for the top of the turret</param>
        /// <param name="a_baseTexture">Texture for the base of the turret</param>
        /// <param name="a_bulletTexture">Texture for the bullet</param>
        /// <param name="a_position">Position to create the turret at !(warning, turret draws from centre of base)!</param>
        /// <param name="a_player">Target for the turret to follow</param>
        /// <param name="a_rotation">Initial rotation for the turret</param>
        public Turret(
            PlayingGameState a_state, 
            Texture2D a_topTexture,
            Texture2D a_baseTexture,
            Texture2D a_bulletTexture,
            Vector2 a_position, 
            Player a_player,
            float a_rotation)
        {
            Vector2 tempRot = new Vector2(-(float)Math.Sin(a_rotation), (float)Math.Cos(a_rotation));

            m_bulletTexture = a_bulletTexture;

            m_state = a_state;

            m_base = new Sprite(
                m_state.m_world,
                a_baseTexture,
                new Vector2(80, 40),
                50,
                a_position);
            m_base.m_body.BodyType = BodyType.Static;
            m_base.m_body.Rotation = (float)(Math.Atan2(tempRot.Y, tempRot.X) - (Math.PI / 2));

            m_top = new Sprite(
                m_state.m_world, 
                a_topTexture, 
                15, 
                50,
                m_base.Position + (tempRot * 15));
            m_top.m_body.BodyType = BodyType.Static;
  
            m_angle = new Vector2(-1, 0);
            m_player = a_player;
            m_cooldown = 0;
        }

        /// <summary>
        /// Constructor for the turret class, passing in a platform for level editing purposes
        /// </summary>
        /// <param name="a_state">World for the turret to exist in</param>
        /// <param name="a_topTexture">Texture for the top of the turret</param>
        /// <param name="a_baseTexture">Texture for the base of the turret</param>
        /// <param name="a_bulletTexture">Texture for the bullet</param>
        /// <param name="a_platform">Platform provides position and rotation to create the turret</param>
        /// <param name="a_player">Target for the turret to follow</param>
        public Turret(
            PlayingGameState a_state, 
            Texture2D a_topTexture,
            Texture2D a_baseTexture,
            Texture2D a_bulletTexture,
            Platform a_platform, 
            Player a_player)
        {
            m_bulletTexture = a_bulletTexture;

            m_state = a_state;

            Vector2 tempRot = new Vector2(-(float)Math.Sin(a_platform.m_sprite.m_body.Rotation - (Math.PI)), (float)Math.Cos(a_platform.m_sprite.m_body.Rotation - (Math.PI)));

            m_base = new Sprite(
                m_state.m_world,
                a_baseTexture,
                new Vector2(80, 40),
                50,
                a_platform.m_sprite.Position + (tempRot * 26));
            m_base.m_body.BodyType = BodyType.Static;
            m_base.m_body.Rotation = a_platform.m_sprite.m_body.Rotation - (float)(Math.PI);

            m_top = new Sprite(
                m_state.m_world, 
                a_topTexture, 
                15, 
                50,
                m_base.Position + (tempRot * 15));
            m_top.m_body.BodyType = BodyType.Static;
  
            m_angle = new Vector2(-1, 0);
            m_player = a_player;
            m_cooldown = 0;
        }

        /// <summary>
        /// Updates the turret tracking and shooting
        /// </summary>
        /// <param name="a_gameTime"></param>
        public void Update(GameTime a_gameTime)
        {
            Vector2 projection;
            float projectionLength = 0;

            projectionLength = MathX.LineLength(m_player.m_body.Position, m_top.m_body.Position) / 3.5f;

            projection = (Vector2.Normalize(-m_state.m_world.Gravity) * projectionLength) + m_player.m_body.Position;

            m_angle = projection - m_top.m_body.Position;

            float rot = (float)(Math.Atan2(m_angle.Y, m_angle.X) - (Math.PI / 2));

            if (rot > 2 * Math.PI) rot = 0;

            m_top.m_body.Rotation = rot;

            m_cooldown -= (float)a_gameTime.ElapsedGameTime.TotalSeconds;
            if (m_cooldown >= 0)
            {
                m_cooldown -= (float)a_gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                //check distance between turret and player
                if (MathX.LineLength(m_player.m_body.Position, m_top.m_body.Position) < 3)
                {
                    //check if the target is within rotation range
                    float upperRange = m_base.m_body.Rotation + (float)(Math.PI / 3);
                    float lowerRange = m_base.m_body.Rotation - (float)(Math.PI / 3);

                    if (upperRange > (float)(Math.PI / 2) && m_top.m_body.Rotation < 0)
                    {
                        upperRange -= 2 * (float)Math.PI;
                        lowerRange -= 2 * (float)Math.PI;
                    }

                    if (lowerRange < (float)(-3 * (Math.PI / 2)) && m_top.m_body.Rotation > 0)
                    {
                        lowerRange += 2 * (float)Math.PI;
                        upperRange += 2 * (float)Math.PI;
                    }

                    if (m_top.m_body.Rotation > lowerRange && m_top.m_body.Rotation < upperRange)
                    {
                        List<Fixture> collision = m_state.m_world.RayCast(ConvertUnits.ToSimUnits(m_top.Position), m_player.m_body.Position);
                        Fixture platformFound = collision.Find(x => (string)x.Body.UserData == "platform");
                        if(platformFound == null)
                        {
                            Shoot(m_state.m_world);
                            m_cooldown = 2;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draw the base and top
        /// </summary>
        /// <param name="a_spriteBatch">Spritebatch used to draw the textures</param>
        public void Draw(SpriteBatch a_spriteBatch)
        {
            m_top.Draw(a_spriteBatch, Color.Purple);
            m_base.Draw(a_spriteBatch, Color.Purple);
        }

        /// <summary>
        /// Shoots a bullet towards the player
        /// </summary>
        private void Shoot(World a_world)
        {
            m_state.m_bulletList.Add(
                new Bullet
                    (
                    a_world,
                    m_bulletTexture, 
                    ConvertUnits.ToDisplayUnits(m_top.m_body.Position) + (m_angle * 10), 
                    m_angle * 2,
                    m_state)
                    );
        }
    }
}
