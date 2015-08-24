using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gravitas;
using Gravitas.Physics;

namespace Gravitas
{
    public class GameStateManager
    {
        public Game1 m_game; 

        protected enum CommandType
        {
            PUSH,
            POP,
            POPBACK
        };

        protected struct Command
        {
            public string m_name;
            public CommandType m_command;
        };

        protected Dictionary<string, GameState> m_avaliableStates = new Dictionary<string,GameState>();
        public List<GameState> m_stateList   = new List<GameState>();
        protected List<Command>   m_commandList = new List<Command>();        

        public GameStateManager( Game1 a_game )
        {
            m_game = a_game;
        }

        public void SetState(string a_name, GameState a_state)
        {
            m_avaliableStates[a_name] = a_state;
        }

        public void PushState(string a_name)
        {
            Command newCommand;
            newCommand.m_name = a_name;
            newCommand.m_command = CommandType.PUSH;
            m_commandList.Add(newCommand);
        }

        public void PopState()
        {
            Command newCommand;
            newCommand.m_name = "0";
            newCommand.m_command = CommandType.POP;

            m_commandList.Add(newCommand);
        }

        public void PopBackState()
        {
            Command newCommand;
            newCommand.m_name = "0";
            newCommand.m_command = CommandType.POPBACK;

            m_commandList.Add(newCommand);
        }

        public void Update(GameTime a_gameTime)
        {
            ProcessCommands();

            for (int i = m_stateList.Count() - 1; i >= 0; --i)
            {              
                m_stateList[i].Update(a_gameTime);
                if (m_stateList[i].m_currentLevel == "Pause")
                    break;
            }
        }

        public void Draw(SpriteBatch a_spriteBatch)
        {
            for (int i = m_stateList.Count() -1; i >= 0; --i)
            {
                m_stateList[i].Draw(a_spriteBatch);
                if (m_stateList[i].m_currentLevel == "Pause")
                    break;
            }
        }

        protected void ProcessCommands()
        {
            for (int i = 0; i < m_commandList.Count; i++)
            {
                //if PUSH...
                if (m_commandList[i].m_command == CommandType.PUSH)
                {
                    m_stateList.Add(m_avaliableStates[m_commandList[i].m_name]);
                }

               //else if POP...
                if (m_commandList[i].m_command == CommandType.POP)
                {
                    m_stateList.RemoveAt(0);
                }

                if (m_commandList[i].m_command == CommandType.POPBACK)
                {
                    m_stateList.RemoveAt(m_stateList.Count - 1);
                }
            }

            m_commandList.Clear();
        }
    }
}
