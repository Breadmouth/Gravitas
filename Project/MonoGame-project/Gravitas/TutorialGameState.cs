using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gravitas
{
    class TutorialGameState : PlayingGameState
    {
        /// <summary>
        /// constructor for the tutorial state
        /// </summary>
        /// <param name="a_gameStateManager">The manager for the gamestate</param>
        /// <param name="a_screenRes">the resolution for the screen</param>
        public TutorialGameState(
            GameStateManager a_gameStateManager,
            Rectangle a_screenRes)
            : base(a_gameStateManager, a_screenRes)
        {    
            LevelIO.LoadLevel("tutorial.xml", this);
            m_player.m_body.Position = m_playerSpawnLocation;

            string buffer;

            buffer = "Move using the WASD keys, and know this";
            m_text.Add(buffer);
            m_textPos[buffer] = new Vector2(-260, -150);
            buffer = "You can always restart a level from the pause menu";
            m_text.Add(buffer);
            m_textPos[buffer] = new Vector2(-260, -125);
            buffer = "Jump with the SPACE key";
            m_text.Add(buffer);
            m_textPos[buffer] = new Vector2(750, 150);
            buffer = "To change the direction of gravity";
            m_text.Add(buffer);
            m_textPos[buffer] = new Vector2(1350, -50);
            buffer = "hold the RIGHT MOUSE BUTTON,";
            m_text.Add(buffer);
            m_textPos[buffer] = new Vector2(1350, -25);
            buffer = "then drag the MOUSE in the direction you";
            m_text.Add(buffer);
            m_textPos[buffer] = new Vector2(1350, 0);
            buffer = "want and release the RIGHT MOUSE BUTTON";
            m_text.Add(buffer);
            m_textPos[buffer] = new Vector2(1350, 25);
            buffer = "You can change gravity in the air";
            m_text.Add(buffer);
            m_textPos[buffer] = new Vector2(2200, 100);
            buffer = "but it will drain your GPE bar,";
            m_text.Add(buffer);
            m_textPos[buffer] = new Vector2(2200, 125);
            buffer = "which is located at the bottom";
            m_text.Add(buffer);
            m_textPos[buffer] = new Vector2(2200, 150);
            buffer = "of the screen";
            m_text.Add(buffer);
            m_textPos[buffer] = new Vector2(2200, 175);
            buffer = "Don't get shot!";
            m_text.Add(buffer);
            m_textPos[buffer] = new Vector2(3500, 0);
            buffer = "To finish a level you must reach";
            m_text.Add(buffer);
            m_textPos[buffer] = new Vector2(5000, 0);
            buffer = "the end goal. Finding it however";
            m_text.Add(buffer);
            m_textPos[buffer] = new Vector2(5000, 25);
            buffer = "may not be so easy...";
            m_text.Add(buffer);
            m_textPos[buffer] = new Vector2(5000, 50);

            m_goal.Position = new Vector2(5700, -80);
            m_goal.m_body.UserData = "menu";

            m_currentLevel = "Tutorial";

            m_levelBoundary = 7000;
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

            a_spriteBatch.End();
        }
    }
}
