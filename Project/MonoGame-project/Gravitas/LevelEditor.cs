using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Gravitas.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
    /// <Author>Andrew Giannopoulos</Author>
    /// <Description>Game state allowing for the editing, loading, and saving of level data to XML files</Description>
    /// </summary>
    public class LevelEditor : PlayingGameState
    {
        Vector2 m_leftposition;
        Vector2 m_rightposition;

        Sprite m_playerSpawnIndicator;
        Sprite m_goalIndicator;

        private bool m_dragging = false;

        private bool m_holding = false;

        /// <summary>
        /// Constructor for level editor gamestate
        /// </summary>
        /// <param name="a_gameStateManager">The Gamestatemanager that manages the level editor</param>
        /// <param name="a_screenRes">the resolution for the screen</param>
        public LevelEditor(
            GameStateManager a_gameStateManager,
            Rectangle a_screenRes)
            : base(a_gameStateManager, a_screenRes)
        {
            m_platformList = new List<Platform>();

            m_playerSpawnLocation = new Vector2(0, 0);
            m_playerSpawnIndicator = new Sprite(m_world, ContentLibrary.platformTexture, new Vector2(10, 50), 0, m_playerSpawnLocation);
            m_playerSpawnIndicator.m_body.BodyType = BodyType.Static;

            m_goalLocation = new Vector2(100, 0);
            m_goalIndicator = new Sprite(m_world, ContentLibrary.platformTexture, new Vector2(100, 100), 0, m_goalLocation);
            m_goalIndicator.m_body.BodyType = BodyType.Static;

            m_camera.EnableTracking = false;
        }

        /// <summary>
        /// Method used for calculating the mouse snap when selecting start/end points of platforms
        /// </summary>
        /// <param name="a_mousePosition">The input mouse position given for checking against established platform endpoints.</param>
        /// <returns>Either the input mouse position if it's far enough away from all platform endpoints, otherwise returns the location of the closest endpoint</returns>
        protected Vector2 CalculateSnap(Vector2 a_mousePosition)
        {
            Vector2 closestPoint = new Vector2(0, 0);
            for (int i = 0; i < m_platformList.Count; i++)
            {
                if (m_platformList[i] != null)
                {
                    if (Vector2.Distance(m_platformList[i].m_leftPoint, a_mousePosition) < Vector2.Distance(closestPoint, a_mousePosition))
                    {
                        closestPoint = m_platformList[i].m_leftPoint;
                    }

                    if (Vector2.Distance(m_platformList[i].m_rightPoint, a_mousePosition) < Vector2.Distance(closestPoint, a_mousePosition))
                    {
                        closestPoint = m_platformList[i].m_rightPoint;
                    }
                }
            }

            if (Vector2.Distance(closestPoint, a_mousePosition) < 60)
            {
                return closestPoint;
            }

            return a_mousePosition;
        }

        /// <summary>
        /// Generates a turret attached to the platform that the mouse is currently on top of
        /// </summary>
        protected void GenerateTurret()
        {
            Fixture f = m_world.TestPoint(ConvertUnits.ToSimUnits(m_mousePos));
            if (f != null)
            {
                Body body = f.Body;
                for (int i = 0; i < m_platformList.Count; i++)
                {
                    if (m_platformList[i] != null && body == m_platformList[i].m_sprite.m_body)
                    {
                        m_turretList.Add(
                            new Turret(
                                this,
                                ContentLibrary.turretTopTexture,
                                ContentLibrary.turretBaseTexture,
                                ContentLibrary.circleTexture,
                                m_platformList[i],
                                m_player
                            )
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Overload of the gamestate update that updates camera position, as well as the platforms.
        /// </summary>
        /// <param name="a_gameTime"></param>
        public override void Update(GameTime a_gameTime)
        {
            //World
            BaseUpdate(a_gameTime);

            m_camera.Update(a_gameTime);

            for (int i = 0; i < m_platformList.Count; ++i)
            {
                m_platformList[i].Update();
            }

            LevelEditUpdate();
        }

        /// <summary>
        /// Overload of the gamestate draw that draws platforms, turrets, player spawn indicator, and endpoint indicator to the screen.
        /// </summary>
        /// <param name="a_spriteBatch"></param>
        override public void Draw(SpriteBatch a_spriteBatch)
        {
            a_spriteBatch.Begin(0, null, null, null, null, null, m_camera.View);

            for (int i = 0; i < m_platformList.Count; ++i)
            {
                m_platformList[i].Draw(a_spriteBatch);
            }

            for (int i = 0; i < m_turretList.Count; ++i)
            {
                m_turretList[i].Draw(a_spriteBatch);
            }

            m_playerSpawnIndicator.Draw(a_spriteBatch);

            m_goalIndicator.Draw(a_spriteBatch);

            a_spriteBatch.End();
        }

        /// <summary>
        /// Updates the main functions of the level editor including click and keypress detection for the sake of user input.
        /// </summary>
        protected void LevelEditUpdate()
        {
            ButtonState leftClick = Mouse.GetState().LeftButton;
            if (!m_dragging)
            {
                if (leftClick == ButtonState.Pressed)
                {
                    m_dragging = true;
                    //leftposition = CalculateSnap(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
                    m_leftposition = CalculateSnap(m_mousePos);
                }
            }

            if (leftClick == ButtonState.Released && m_dragging)
            {
                if (m_leftposition != new Vector2(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    m_dragging = false;
                    //rightposition = CalculateSnap(new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
                    m_rightposition = CalculateSnap(m_mousePos);
                    if (m_rightposition == m_leftposition)
                    {

                    }
                    else
                    {
                        m_platformList.Add(new Platform(m_world, m_leftposition, m_rightposition, ContentLibrary.platformTexture));
                    }
                }
            }

            ButtonState rightClick = Mouse.GetState().RightButton;
            if (rightClick == ButtonState.Pressed)
            {
                Fixture f = m_world.TestPoint(ConvertUnits.ToSimUnits(m_mousePos));
                if (f != null)
                {
                    Body body = f.Body;
                    for (int i = 0; i < m_platformList.Count; i++)
                    {
                        if (m_platformList[i] != null && body == m_platformList[i].m_sprite.m_body)
                        {
                            m_world.RemoveBody(m_platformList[i].m_sprite.m_body);
                            m_platformList.RemoveAt(i--);
                        }
                    }

                    for (int i = 0; i < m_turretList.Count; i++)
                    {
                        if (body == m_turretList[i].m_base.m_body)
                        {
                            m_world.RemoveBody(m_turretList[i].m_base.m_body);
                            m_world.RemoveBody(m_turretList[i].m_top.m_body);
                            m_turretList.RemoveAt(i--);
                        }
                    }
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                m_holding = true;
            }

            if (Keyboard.GetState().IsKeyUp(Keys.K) && m_holding)
            {
                m_holding = false;
                GenerateTurret();
            }

            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {

                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    //m_goal = new Sprite(m_world, ContentLibrary.crateTexture, new Vector2(100, 100), 0, m_mousePos);
                    //m_goal.m_body.BodyType = BodyType.Static;

                    m_goalLocation = m_mousePos;
                    m_goalIndicator = new Sprite(m_world, ContentLibrary.platformTexture, new Vector2(100, 100), 0, m_goalLocation);
                    m_goalIndicator.m_body.BodyType = BodyType.Static;
                    m_goalLocation = ConvertUnits.ToSimUnits(m_goalLocation);
                    Console.WriteLine("Goal X: " + m_goalLocation.X + ", Y: " + m_goalLocation.Y);

                }
                else
                {
                    m_playerSpawnLocation = m_mousePos;
                    m_playerSpawnIndicator = new Sprite(m_world, ContentLibrary.platformTexture, new Vector2(10, 50), 0, m_playerSpawnLocation);
                    m_playerSpawnIndicator.m_body.BodyType = BodyType.Static;
                    m_playerSpawnLocation = ConvertUnits.ToSimUnits(m_playerSpawnLocation);
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                Console.WriteLine("Please enter a file to save to: ");
                string filePath = Console.ReadLine();
                if (filePath != null)
                {
                    LevelIO.SaveLevel(filePath, this);
                }

            }

            if (Keyboard.GetState().IsKeyDown(Keys.H))
            {

                Console.WriteLine("Please enter a file to load from: ");
                string filePath = Console.ReadLine();
                if (filePath != null)
                {
                    LevelIO.LoadLevel(filePath, this);

                    Console.WriteLine("Goal X: " + m_goalLocation.X + ", Y: " + m_goalLocation.Y);
                    Console.WriteLine("Player X: " + m_playerSpawnLocation.X + ", Y: " + m_playerSpawnLocation.Y);

                    m_playerSpawnIndicator = new Sprite(m_world, ContentLibrary.platformTexture, new Vector2(10, 50), 0, ConvertUnits.ToDisplayUnits(m_playerSpawnLocation));
                    m_playerSpawnIndicator.m_body.BodyType = BodyType.Static;
                    m_goalIndicator = new Sprite(m_world, ContentLibrary.platformTexture, new Vector2(100, 100), 0, ConvertUnits.ToDisplayUnits(m_goalLocation));
                    m_goalIndicator.m_body.BodyType = BodyType.Static;
                }

            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                m_camera.MoveCamera(new Vector2(0, -0.1f) * 1/m_camera.Zoom);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                m_camera.MoveCamera(new Vector2(-0.1f, 0) * 1/m_camera.Zoom);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                m_camera.MoveCamera(new Vector2(0, 0.1f) * 1/m_camera.Zoom);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                m_camera.MoveCamera(new Vector2(0.1f, 0) * 1/m_camera.Zoom);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.T))
            {
                m_gameStateManager.PopState();
                m_gameStateManager.PushState("tutorial");
            }
        }

    }
}
