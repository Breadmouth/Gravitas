#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Joints;
using GameName1;
#endregion

namespace GameName1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        World world;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D texture;
        List<Sprite> crateList;
        KeyboardState prevKeyboardState;
        Random random;
        Player player;
        Gravity m_gravity;

        Vector2 gravityBegin;
        Vector2 gravityEnd;

        List<Platform> m_platforms;

        Camera2D m_camera;

        private bool dragging = false;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            world = new World(new Vector2(0,0));
            m_camera = new Camera2D(graphics.GraphicsDevice);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            texture = Content.Load<Texture2D>("GroundSprite");

            // TODO: use this.Content to load your game content here
            Vector2 size = new Vector2(50, 50);
            m_platforms = new List<Platform>();

            player = new Player
                (
                    world,
                    Content.Load<Texture2D>("GroundSprite"),
                    Content.Load<Texture2D>("GroundSprite"),
                    new Vector2(20, 50),
                    50,
                    new Vector2(50, 100)
                );

            random = new Random();

            ///////////////////////////////////////////////////////////
            m_gravity = new Gravity(world, new Vector2(0, 1), 9.8f);
            ///////////////////////////////////////////////////////////

            crateList = new List<Sprite>();
            prevKeyboardState = Keyboard.GetState();
            m_camera.TrackingBody = player.thing.m_body;

            CreatePlatform(new Vector2(0, 0), new Vector2(0, 300));
            CreatePlatform(new Vector2(0, 300), new Vector2(400, 300));
            CreatePlatform(new Vector2(400, 300), new Vector2(400, 350));
            CreatePlatform(new Vector2(400, 350), new Vector2(500, 350));
            CreatePlatform(new Vector2(500, 350), new Vector2(500, 300));
            CreatePlatform(new Vector2(500, 300), new Vector2(600, 300));
            CreatePlatform(new Vector2(600, 300), new Vector2(600, 250));
            CreatePlatform(new Vector2(600, 250), new Vector2(700, 250));
            CreatePlatform(new Vector2(700, 250), new Vector2(700, 300));
            CreatePlatform(new Vector2(700, 300), new Vector2(800, 300));
            CreatePlatform(new Vector2(0, 0), new Vector2(800, 0));
            CreatePlatform(new Vector2(800, 0), new Vector2(800, 300));


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        /// 
        private void SpawnCrate()
        {
            Sprite crate;
            crate = new Sprite(world, texture, new Vector2(50.0f, 50.0f), 0.1f);
            crate.m_Position = new Vector2(random.Next(50, GraphicsDevice.Viewport.Width - 50), 1);
         
            crateList.Add(crate);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
            // TODO: Add your update logic here
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Space) && !prevKeyboardState.IsKeyDown(Keys.Space))
            {
                SpawnCrate();
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                player.Move(Movement.L);
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                player.Move(Movement.R);
            }
            if (keyboardState.IsKeyDown(Keys.W))
            {
                player.Move(Movement.U);
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                player.Move(Movement.D);
            }
            if (keyboardState.IsKeyDown(Keys.E))
            {
                player.Move(Movement.J);
            }

            player.thing.m_body.Rotation = m_gravity.angle;

            player.Update(m_gravity);

            prevKeyboardState = keyboardState;

            ButtonState b = Mouse.GetState().RightButton;

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
                    Vector2 gravityDirection = gravityEnd - gravityBegin;
                    m_gravity.direction = gravityDirection;
                }
            }

            m_camera.Update(gameTime);

            for (int i = 0; i < m_platforms.Count; i++)
            {
                m_platforms[i].Update();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(0, null, null, null, null, null, m_camera.View);
            foreach (Sprite crate in crateList)
            {
                crate.Draw(spriteBatch);
            }
            player.Draw(spriteBatch);

            for (int i = 0; i < m_platforms.Count; i++)
            {
                m_platforms[i].Draw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public float GetAngleBetween(Vector2 lhs, Vector2 rhs)
        {
            float xDiff = rhs.X - lhs.X;
            float yDiff = rhs.Y - lhs.Y; 
            return (float)(Math.Atan2(yDiff, xDiff) * (180 / Math.PI));
        }

        public float LineLength(Vector2 lhs, Vector2 rhs)
        {
            return (float)Math.Sqrt(((rhs.X - lhs.X) * (rhs.X - lhs.X)) + ((rhs.Y - lhs.Y) * (rhs.Y - lhs.Y)));
        }

        public Vector2 Midpoint(Vector2 lhs, Vector2 rhs)
        {
            float x = (lhs.X + rhs.X) / 2;
            float y = (lhs.Y + rhs.Y) / 2;
            return new Vector2(x, y);
        }

        public void CreatePlatform(Vector2 lhs, Vector2 rhs)
        {
            float width = LineLength(lhs, rhs);
            Vector2 midPos = Midpoint(lhs, rhs);
            float angle = GetAngleBetween(lhs, rhs);
            Platform newPlatform = new Platform(world, texture, new Vector2(width + 10, 10), midPos);
            newPlatform.m_rotation = angle * (float)(Math.PI/180);
            m_platforms.Add(newPlatform);
        }
    }
}

public static class CoordinateHelper
{
    // Because Farseer uses 1 unit = 1 meter we need to convert
    // between pixel coordinates and physics coordinates.
    // I've chosen to use the rule that 100 pixels is one meter.
    // We have to take care to convert between these two
    // coordinate-sets wherever we mix them!

    public const float unitToPixel = 100.0f;
    public const float pixelToUnit = 1 / unitToPixel;

    public static Vector2 ToScreen(Vector2 worldCoordinates)
    {
        return worldCoordinates * unitToPixel;
    }

    public static Vector2 ToWorld(Vector2 screenCoordinates)
    {
        return screenCoordinates * pixelToUnit;
    }
}

public enum Movement
{
    L, R , U, D, J
}