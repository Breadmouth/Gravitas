using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project1
{
    public class GameStateManager
    {
        public Game1 _game; 

        protected enum CommandType
        {
            PUSH,
            POP,
        };

        protected struct Command
        {
            public string _name;
            public CommandType _command;
        };

        protected Dictionary<string, GameState> _avaliableStates = new Dictionary<string,GameState>();
        protected List<GameState> _stateList   = new List<GameState>();
        protected List<Command>   _commandList = new List<Command>();
        

        public GameStateManager( Game1 a_game )
        {
            _game = a_game;
        }

        public void SetState(string a_name, GameState a_state)
        {
            _avaliableStates[a_name] = a_state;
        }

        public void PushState(string a_name)
        {
            Command newCommand;
            newCommand._name = a_name;
            newCommand._command = CommandType.PUSH;
            _commandList.Add(newCommand);
        }

        public void PopState()
        {
            Command newCommand;
            newCommand._name = "0";
            newCommand._command = CommandType.POP;

            _commandList.Add(newCommand);
        }

        public void Update()
        {
            ProcessCommands();

            for (int i = 0; i < _stateList.Count(); ++i)
            {
                _stateList[i].Update();
            }
        }

        public void Draw()
        {
            for (int i = 0; i < _stateList.Count(); ++i)
            {
                _stateList[i].Draw();
            }
        }

        protected void ProcessCommands()
        {
            for (int i = 0; i < _commandList.Count; i++)
            {
                //if PUSH...
                if (_commandList[i]._command == CommandType.PUSH)
                {
                    _stateList.Add(_avaliableStates[_commandList[i]._name]);
                }

               //else if POP...
                if (_commandList[i]._command == CommandType.POP)
                {
                    _stateList.RemoveAt(0);
                }
            }

            _commandList.Clear();
        }
    }
}
