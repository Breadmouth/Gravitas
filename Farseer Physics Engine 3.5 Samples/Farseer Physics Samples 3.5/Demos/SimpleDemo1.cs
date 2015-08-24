using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Samples.Demos.Prefabs;
using FarseerPhysics.Samples.DrawingSystem;
using FarseerPhysics.Samples.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace FarseerPhysics.Samples.Demos
{
    internal class SimpleDemo1 : PhysicsGameScreen, IDemoScreen
    {
        private Border _border;
        private Body _rectangle;
        private Sprite _rectangleSprite;

        private bool dragging = false;
        private Vector2 gravityBegin = new Vector2(0, 0);
        private Vector2 gravityEnd = new Vector2(0, 20);

        bool canJump = false;

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Body with a single fixture";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("This demo shows a single body with one attached fixture and shape.");
            sb.AppendLine("A fixture binds a shape to a body and adds material");
            sb.AppendLine("properties such as density, friction, and restitution.");
            sb.AppendLine(string.Empty);
            sb.AppendLine("GamePad:");
            sb.AppendLine("  - Rotate object: left and right triggers");
            sb.AppendLine("  - Move object: right thumbstick");
            sb.AppendLine("  - Move cursor: left thumbstick");
            sb.AppendLine("  - Grab object (beneath cursor): A button");
            sb.AppendLine("  - Drag grabbed object: left thumbstick");
            sb.AppendLine("  - Exit to menu: Back button");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Keyboard:");
            sb.AppendLine("  - Rotate Object: left and right arrows");
            sb.AppendLine("  - Move Object: A,S,D,W");
            sb.AppendLine("  - Exit to menu: Escape");
            sb.AppendLine(string.Empty);
            sb.AppendLine("Mouse / Touchscreen");
            sb.AppendLine("  - Grab object (beneath cursor): Left click");
            sb.AppendLine("  - Drag grabbed object: move mouse / finger");
            return sb.ToString();
        }

        #endregion

        public override void LoadContent()
        {
            base.LoadContent();

            World.Gravity = (gravityEnd - gravityBegin);

            _border = new Border(World, ScreenManager, Camera);

            _rectangle = BodyFactory.CreateRectangle(World, 5f, 5f, 1f);
            _rectangle.BodyType = BodyType.Dynamic;
           
            Camera.TrackingBody = _rectangle;
            Camera.EnablePositionTracking = true;

            SetUserAgent(_rectangle, 100f, 100f);

            // create sprite based on body
            _rectangleSprite = new Sprite(ScreenManager.Assets.TextureFromShape(_rectangle.FixtureList[0].Shape, MaterialType.Squares, Color.White, 1f));
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

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
           // World.Gravity = new Vector2(20 * (float)Math.Sin(Camera.Rotation), 20 * (float)Math.Cos(Camera.Rotation));
            ButtonState b = Mouse.GetState().RightButton;

            _rectangle.OnCollision += Body_OnCollision;
            _rectangle.OnSeparation += Body_OnRelease;

            if (canJump)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    _rectangle.ApplyForce(new Vector2(-30, 0) * 30, _rectangle.Position);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    _rectangle.ApplyForce(new Vector2(30, 0) * 30, _rectangle.Position);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    _rectangle.ApplyForce(new Vector2(0, -30) * 30, _rectangle.Position);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    _rectangle.ApplyForce(new Vector2(0, 30) * 30, _rectangle.Position);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    _rectangle.ApplyForce(new Vector2(-World.Gravity.X, -World.Gravity.Y) * 1200, _rectangle.Position);
                    canJump = false;
                }
            }
            

            if (!dragging)
            {
                if (b == ButtonState.Pressed)
                {
                    dragging = true;
                    gravityBegin = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                }
            }

            if (b == ButtonState.Released && dragging)
            {
                if (gravityBegin != new Vector2(Mouse.GetState().X, Mouse.GetState().Y))
                {
                    dragging = false;
                    gravityEnd = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                    World.Gravity = (gravityEnd - gravityBegin);
                    World.Gravity.Normalize();
                    World.Gravity *= 20;
                }
            }

        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(0, null, null, null, null, null, Camera.View);
            ScreenManager.SpriteBatch.Draw(_rectangleSprite.Texture, ConvertUnits.ToDisplayUnits(_rectangle.Position), null, Color.White, _rectangle.Rotation, _rectangleSprite.Origin, 1f, /*SpriteEffects.None*/SpriteEffects.FlipHorizontally, 0f);
            ScreenManager.SpriteBatch.End();

            _border.Draw();
            base.Draw(gameTime);
        }
    }
}