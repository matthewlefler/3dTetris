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

        public string devcadeToString()
        {
            string stringToReturn = "";

            int i = 0;
            foreach((int? playerNum, Devcade.Input.ArcadeButtons? key) value in devcadeKeys)
            {
                stringToReturn += value.key.ToString();

                i++;

                if(devcadeKeys.Length > 1 && i < devcadeKeys.Length)
                {
                    stringToReturn += " + ";
                }
            }

            return stringToReturn + "`"; 
        }

        public string keyboardToString()
        {
            string stringToReturn = "";

            int i = 0;
            foreach(Keys? value in keyboardKeys)
            {
                stringToReturn += "Key: " + value.ToString();

                i++;
                
                if(devcadeKeys.Length > 1 && i < keyboardKeys.Length)
                {
                    stringToReturn += " + ";
                }
            }

            return stringToReturn + "`";
        }
    }

    public class InputHandler
    {
        //managing input required to play game:
        private Dictionary<int, inputKey> gameKeys;
        Dictionary<int, string> actionToKeyString;

        public InputHandler(Dictionary<int, inputKey> gameKeys, Dictionary<int, string> actionToKeyString)
        {
            this.gameKeys = gameKeys;
            this.actionToKeyString = actionToKeyString;
        }

        //keylist for only activating keys once per press;
        List<inputKey> keyList = new List<inputKey>();
        //returns true only on the first time a key is down
        public bool runOnKeyDown(int key)
        {
            inputKey inputKey = gameKeys[key];

            bool DevcadeBool = true;
            foreach((int playerNum, Devcade.Input.ArcadeButtons? key) button in inputKey.devcadeKeys)
            {   
                if(button.key == null)
                {
                    continue;
                }

                //if the key is somehow null at this point it is defaulted to the menu button
                if(!Input.GetButton(button.playerNum, button.key ?? Input.ArcadeButtons.Menu))
                {
                    DevcadeBool = false;
                }
            }

            bool keyboardBool = true;
            foreach(Keys? button in inputKey.keyboardKeys)
            {
                if(button == null)
                {
                    continue;
                }

                //if the button is somehow null at this point it is defaulted to the enter button
                if(!Keyboard.GetState().IsKeyDown(button ?? Keys.Enter))
                {
                    keyboardBool = false;
                }
            }

            if(!(DevcadeBool || keyboardBool))
            {
                keyList.Remove(inputKey);
                return false;
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
            inputKey inputKey = gameKeys[key];
            
            bool DevcadeBool = true;
            foreach((int playerNum, Devcade.Input.ArcadeButtons key) button in inputKey.devcadeKeys)
            {
                if(!Input.GetButton(button.playerNum, button.key))
                {
                    DevcadeBool = false;
                }
            }

            bool keyboardBool = true;
            foreach(Keys button in inputKey.keyboardKeys)
            {
                if(!Keyboard.GetState().IsKeyDown(button))
                {
                    keyboardBool = false;
                }
            }
            
            if(!(DevcadeBool || keyboardBool))
            {
                return false;
            }

            return true;
        }  
    }

}