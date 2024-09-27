using Devcade;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace input
{

    public struct inputKey
    {
        #nullable enable //i have no clue what this does, but my ide likes it?
        public (int? playerNum, Devcade.Input.ArcadeButtons? key)[]? devcadeKeys;
        public Keys?[]? keyboardKeys;
        public inputKey((int? playerNum, Devcade.Input.ArcadeButtons? key)[]? devcadeKeys, Keys?[]? keyboardKeys)
        {
            this.devcadeKeys = devcadeKeys;
            this.keyboardKeys = keyboardKeys;
        }

        #nullable disable
    }

    public class InputHandler
    {
        //managing input required to play game:
        private Dictionary<string, inputKey> gameKeys;
        Dictionary<int, string> actionToKeyString;

        public InputHandler(Dictionary<string, inputKey> gameKeys, Dictionary<int, string> actionToKeyString)
        {
            this.gameKeys = gameKeys;
            this.actionToKeyString = actionToKeyString;
        }

        //keylist for only activating keys once per press;
        List<inputKey> keyList = new List<inputKey>();
        //returns true only on the first time a key is down
        public bool runOnKeyDown(int key)
        {
            inputKey inputKey = gameKeys[actionToKeyString[key]];
            
            foreach((int playerNum, Devcade.Input.ArcadeButtons? key) button in inputKey.devcadeKeys)
            {   
                if(button.key == null)
                {
                    continue;
                }

                //if the key is somehow null at this point it is defaulted to the menu button
                if(!Input.GetButton(button.playerNum, button.key ?? Input.ArcadeButtons.Menu))
                {
                    keyList.Remove(inputKey);
                    return false;
                }
            }

            foreach(Keys? button in inputKey.keyboardKeys)
            {
                if(button == null)
                {
                    continue;
                }

                //if the button is somehow null at this point it is defaulted to the enter button
                if(!Keyboard.GetState().IsKeyDown(button ?? Keys.Enter))
                {
                    keyList.Remove(inputKey);
                    return false;
                }
            }

            if(!keyList.Contains(inputKey)) 
            {
                keyList.Add(inputKey);
                return true;
            }
            
            return false;
        }
        public bool isKeyDown(int key)
        {
            inputKey inputKey = gameKeys[actionToKeyString[key]];
            
            foreach((int playerNum, Devcade.Input.ArcadeButtons key) button in inputKey.devcadeKeys)
            {
                if(!Input.GetButton(button.playerNum, button.key))
                {
                    return false;
                }
            }

            foreach(Keys button in inputKey.keyboardKeys)
            {
                if(!Keyboard.GetState().IsKeyDown(button))
                {
                    return false;
                }
            }
            
            return true;
        }  
    }

}