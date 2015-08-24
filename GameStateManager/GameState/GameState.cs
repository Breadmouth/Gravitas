using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project1
{
    abstract public class GameState
    {
        public GameState(GameStateManager a_gameStateManager)
        {
            _gameStateManager = a_gameStateManager;
            _game = _gameStateManager._game;
        }
        abstract public void Init();
        abstract public void DeInit();

        abstract public void Update();
        abstract public void Draw();

        protected GameStateManager _gameStateManager;
        protected Game _game;
    }
}
