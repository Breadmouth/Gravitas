using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project1
{
    public class SplashScreen : GameState
    {
        public SplashScreen(GameStateManager a_gameStateManager) : base(a_gameStateManager)
        {
            
        }

        override public void Init()
        {
            
        }
        override public void DeInit()
        {

        }

        override public void Update()
        {

        }
        override public void Draw()
        {
            _game.GraphicsDevice.Clear(Color.Blue);
        }
    }
}
