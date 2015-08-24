using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Factories;

namespace FarseerPhysics.Tools
{
    class Animation
    {
        public Body _body { get; private set; }
        public Texture2D _texture { get; private set; }
        public Rectangle _rectangle { get; private set; }
        public Vector2 _origin { get; private set; }
        public Vector2 _scale { get; private set; }

        /*
        public Texture2D _texture   { get; private set; }
        public Rectangle _rectangle { get; private set; }
        public Vector2 _position    { get; private set; }
        public Vector2 _origin      { get; private set; }
        public Vector2 _scale       { get; private set; }
        */

        protected int _currentFrameX;
        protected int _frameHeight;
        protected int _frameWidth;
        protected float _timer;
        protected float _interval = 65;
        protected float _rotation = 0;

        private void CreatePolygon(Texture2D a_texture, World a_world)
        {
            //Create an array to hold the data from the texture
            uint[] data = new uint[a_texture.Width * a_texture.Height];

            //Transfer the texture data to the array
            a_texture.GetData(data);

            //Find the vertices that makes up the outline of the shape in the texture
            Vertices textureVertices = PolygonTools.CreatePolygon(data, a_texture.Width, false);

            //The tool return vertices as they were found in the texture.
            //We need to find the real center (centroid) of the vertices for 2 reasons:

            //1. To translate the vertices so the polygon is centered around the centroid.
            Vector2 centroid = -textureVertices.GetCentroid();
            textureVertices.Translate(ref centroid);

            //2. To draw the texture the correct place.
            _origin = -centroid;

            //We simplify the vertices found in the texture.
            textureVertices = SimplifyTools.ReduceByDistance(textureVertices, 4f);

            //Since it is a concave polygon, we need to partition it into several smaller convex polygons
            List<Vertices> list = Triangulate.ConvexPartition(textureVertices, TriangulationAlgorithm.Bayazit);


            //Adjust the scale of the object for WP7's lower resolution
            _scale = new Vector2(1f, 1f);

            //scale the vertices from graphics space to sim space
            Vector2 vertScale = new Vector2(ConvertUnits.ToSimUnits(1)) * _scale;
            foreach (Vertices vertices in list)
            {
                vertices.Scale(ref vertScale);
            }

            //Create a single body with multiple fixtures
            _body = BodyFactory.CreateCompoundPolygon(a_world, list, 1f, BodyType.Dynamic);
            _body.BodyType = BodyType.Dynamic;
        }

        public Animation(World a_world, Texture2D a_newTexture, Vector2 a_newPosition, int a_newFrameWidth, int a_newFrameHeight)
        {
            _texture = a_newTexture;
            

            _frameWidth = a_newFrameWidth;
            _frameHeight = a_newFrameHeight;

            CreatePolygon(a_newTexture, a_world);

            _body.Position = a_newPosition;
        }

        public void UpdateAnimation(int a_limit, int a_restart, int a_currentYFrame, GameTime a_gameTime)
        {
            _rectangle = new Rectangle(_currentFrameX * _frameWidth, a_currentYFrame * _frameHeight, _frameWidth, _frameHeight);
           
            float _vertIndex = (_texture.Height / _frameHeight);
            float _horiIndex = (_texture.Width / _frameWidth);

            _origin = new Vector2(_rectangle.Width / _horiIndex, _rectangle.Height / _vertIndex);
            
            if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Animate(a_limit, a_restart, a_gameTime);
            }
            else
                _currentFrameX = 0;
        }

        public void Animate(int a_lim, int a_restart, GameTime a_gameTime)
        {
            _timer += (float)a_gameTime.ElapsedGameTime.TotalMilliseconds / 2;
            if(_timer > _interval)
            {
                _currentFrameX++;
                _timer = 0;

                if (_currentFrameX > a_lim)
                {
                    _currentFrameX = a_restart;
                }
            }
        }
    }
}
