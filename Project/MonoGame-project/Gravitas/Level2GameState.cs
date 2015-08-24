using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravitas
{
    class Level2GameState : PlayingGameState
    {
        /// <summary>
        /// constructor for the Level2 Gamestate
        /// </summary>
        /// <param name="a_gameStateManager">The manager for the gamestate</param>
        /// <param name="a_screenRes">the resolution for the screen</param>
        public Level2GameState(
            GameStateManager a_gameStateManager,
            Rectangle a_screenRes)
            : base(a_gameStateManager, a_screenRes)
        {
            LevelIO.LoadLevel("Level2.xml", this);
            m_player.m_body.Position = m_playerSpawnLocation;

            m_goal.Position = new Vector2(359.2854f, 32.14273f); 
            m_goal.m_body.UserData = "menu";

            m_currentLevel = "Level 2";

            m_levelBoundary = 3600;
        }

        /// <summary>
        /// Handles all the updates
        /// </summary>
        /// <param name="a_gameTime"></param>
        public override void Update(GameTime a_gameTime)
        {
            base.Update(a_gameTime);
        }

        /// <summary>
        /// handles all the drawing code
        /// </summary>
        /// <param name="a_spriteBatch">Spritebatch used to draw the textures</param>
        override public void Draw(SpriteBatch a_spriteBatch)
        {
            a_spriteBatch.Begin(0, null, null, null, null, null, m_camera.View);

            base.Draw(a_spriteBatch);
            m_goal.Draw(a_spriteBatch);

            a_spriteBatch.End();
        }
    }
}
