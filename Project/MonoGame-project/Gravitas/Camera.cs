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
    public class Camera
     {
        private static GraphicsDevice m_graphics;

        private const float MinZoom = 0.02f;
        private const float MaxZoom = 20f;

        private Vector2 m_currentPosition;
        private float m_currentZoom;

        private bool m_positionTracking;

        private Vector2 m_targetPosition;

        private Body m_trackingBody;

        private Vector2 m_translateCenter;

        public Matrix SimProjection;
        public Matrix SimView;
        public Matrix View;


        /// <summary>
        /// The constructor for the Camera class.
        /// </summary>
        /// <param name="graphics"></param>
        public Camera(GraphicsDevice graphics)
        {
            m_graphics = graphics;
            SimProjection = Matrix.CreateOrthographicOffCenter(0f, ConvertUnits.ToSimUnits(m_graphics.Viewport.Width), ConvertUnits.ToSimUnits(m_graphics.Viewport.Height), 0f, 0f, 1f);
            SimView = Matrix.Identity;
            View = Matrix.Identity;

            m_translateCenter = new Vector2(ConvertUnits.ToSimUnits(m_graphics.Viewport.Width / 2f), ConvertUnits.ToSimUnits(m_graphics.Viewport.Height / 2f));

            ResetCamera();
        }

        /// <summary>
        /// Current position of the camera
        /// </summary>
        public Vector2 Position
        {
            get { return ConvertUnits.ToDisplayUnits(m_currentPosition); }
            set
            {
                m_targetPosition = ConvertUnits.ToSimUnits(value);
            }
        }

        /// <summary>
        /// Current zoom of the camera
        /// </summary>
        public float Zoom
        {
            get { return m_currentZoom; }
            set
            {
                m_currentZoom = value;
                m_currentZoom = MathHelper.Clamp(m_currentZoom, MinZoom, MaxZoom);
            }
        }

        /// <summary>
        /// the body that this camera is currently tracking. 
        /// </summary>
        public Body TrackingBody
        {
            get { return m_trackingBody; }
            set
            {
                m_trackingBody = value;
                if (m_trackingBody != null)
                {
                    m_positionTracking = true;
                }
            }
        }

        public bool EnablePositionTracking
        {
            get { return m_positionTracking; }
            set
            {
                if (value && m_trackingBody != null)
                {
                    m_positionTracking = true;
                }
                else
                {
                    m_positionTracking = false;
                }
            }
        }

        public bool EnableTracking
        {
            set
            {
                EnablePositionTracking = value;
            }
        }

        public void MoveCamera(Vector2 amount)
        {
            m_currentPosition += amount;
            m_targetPosition = m_currentPosition;
            m_positionTracking = false;
        }

        /// <summary>
        /// resets the camera
        /// </summary>
        public void ResetCamera()
        {
            m_currentPosition = Vector2.Zero;
            m_targetPosition = Vector2.Zero;

            m_positionTracking = false;

            m_currentZoom = 1f;

            SetView();
        }

        public void Jump2Target()
        {
            m_currentPosition = m_targetPosition;

            SetView();
        }

        private void SetView()
        {
            Matrix matZoom = Matrix.CreateScale(m_currentZoom);
            Vector3 translateCenter = new Vector3(m_translateCenter, 0f);
            Vector3 translateBody = new Vector3(-m_currentPosition, 0f);

            SimView = Matrix.CreateTranslation(translateBody) * matZoom * Matrix.CreateTranslation(translateCenter);

            translateCenter = ConvertUnits.ToDisplayUnits(translateCenter);
            translateBody = ConvertUnits.ToDisplayUnits(translateBody);

            View = Matrix.CreateTranslation(translateBody) * matZoom * Matrix.CreateTranslation(translateCenter);
        }

        /// <summary>
        /// moves the camera forward one timestep
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (m_trackingBody != null)
            {
                if (m_positionTracking)
                {
                    m_targetPosition = m_trackingBody.Position;
                }
            }
            Vector2 delta = m_targetPosition - m_currentPosition;
            float distance = delta.Length();
            if (distance > 0f)
            {
                delta /= distance;
            }
            float inertia;
            if (distance < 10f)
            {
                inertia = (float)Math.Pow(distance / 10.0, 2.0);
            }
            else
            {
                inertia = 1f;
            }

            m_currentPosition += 100f * delta * inertia * (float)gameTime.ElapsedGameTime.TotalSeconds;

            SetView();
        }

        public Vector2 ConvertScreenToWorld(Vector2 location)
        {
            Vector3 temp = new Vector3(location, 0);
            temp = m_graphics.Viewport.Unproject(temp, SimProjection, SimView, Matrix.Identity);
            return new Vector2(temp.X, temp.Y);
        }

        public Vector2 ConvertWorldToScreen(Vector2 location)
        {
            Vector3 temp = new Vector3(location, 0);
            temp = m_graphics.Viewport.Project(temp, SimProjection, SimView, Matrix.Identity);
            return new Vector2(temp.X, temp.Y);
        }
    }
}
