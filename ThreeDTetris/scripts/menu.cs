using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

//custom text rendering class
using TextRenderer;

namespace MenuClass
{
    public enum gameStates
    {
        Menu,
        Pause,
        Play,
        PlayerOneWon,
        PlayerTwoWon,
        Lost,
        Quit
    }

    public enum gameModes
    {
        SoloCustom,
        SoloStandard,
        SoloThreeD,
        OneVersesOneStandard,
        TwoVersesTwoThreeD,
    }

    public class Menu
    {
        internal GraphicsDevice _graphicsDevice;

        internal Rectangle _windowSize;

        internal int selectedOption;

        internal SpriteBatch spriteBatch;

        public int textSize;

        internal SpriteEffects _effect;

        //menu elements and corresponding menus
        internal List<string> MenuItems = new List<string>();
        internal Dictionary<string, Menu> menuItemsToStates = new Dictionary<string, Menu>();
        internal Dictionary<string, int[]> menuItemstoGameStates = new Dictionary<string, int[]>();

        public Menu(GraphicsDevice graphicsDevice, Rectangle windowSize)
        {
            selectedOption = 0;
            _graphicsDevice = graphicsDevice;
            _windowSize = windowSize;
        }

        //draws the text in MainMenuItems verticaly
        public virtual void draw(FontInterpreter fontInterpreter, Vector3 position, float scale)
        {
            string selectedCharacters = ">";
            int highestItemLength = 0;
            foreach (string item in MenuItems)
            {
                if(item.Length + selectedCharacters.Length > highestItemLength)
                {
                    highestItemLength = item.Length + selectedCharacters.Length;
                }
            }

            for (int i = 0; i < MenuItems.Count; i++)
            {
                int yPosition = i * -24 + 30;

                if (i == selectedOption)
                {
                    fontInterpreter.menuDrawStringInWorld(selectedCharacters + MenuItems[i], new Vector3(position.X, yPosition + position.Y, position.Z), 100, scale, Vector3.Zero, Vector3.One, highestItemLength);
                }
                else
                {
                    fontInterpreter.menuDrawStringInWorld(MenuItems[i], new Vector3(position.X, yPosition + position.Y, position.Z), 100, scale,  Vector3.Zero, new Vector3(0.4f, 0.4f, 0.4f), highestItemLength);
                }
            }
        }

        public void add(string name, Menu menu, int[] states) //cast to int
        {
            MenuItems.Add(name);
            menuItemsToStates.Add(name, menu);
            menuItemstoGameStates.Add(name, states);
        }

        public void add(string name, Menu menu, int state) //cast to int
        {
            MenuItems.Add(name);
            menuItemsToStates.Add(name, menu);
            menuItemstoGameStates.Add(name, new int[] {state});
        }

        public void add(string name, Menu menu, int state, int state2) //cast to int
        {
            MenuItems.Add(name);
            menuItemsToStates.Add(name, menu);
            menuItemstoGameStates.Add(name, new int[] {state, state2});
        }

        //return the currently selected Menu
        public virtual Menu selectOption(out int[] gameState)
        {
            gameState = menuItemstoGameStates[MenuItems[selectedOption]];

            return menuItemsToStates[MenuItems[selectedOption]];
        }

        public virtual void ChangeSelectedOptionUp()
        {
            selectedOption -= 1;

            if (selectedOption < 0)
            {
                selectedOption = 0;
            }
        }
        public virtual void ChangeSelectedOptionDown()
        {
            selectedOption += 1;

            if (selectedOption > MenuItems.Count - 1)
            {
                selectedOption = MenuItems.Count - 1;
            }
        }
    }


    public class ControlMenu : Menu
    {
        int charactersPerLine;

        public ControlMenu(GraphicsDevice graphicsDevice, Rectangle windowSize, int charactersPerLine) : base(graphicsDevice, windowSize)
        {
            this.charactersPerLine = charactersPerLine;
        }

        public override void draw(FontInterpreter fontInterpreter, Vector3 position, float scale)
        {
            string selectedCharacters = ">";
            int highestItemLength = charactersPerLine;

            int yDelta = -24;
            int yPosition = 200;

            for (int i = 0; i < MenuItems.Count; i++)
            {
                if (i == selectedOption)
                {
                    fontInterpreter.menuDrawStringInWorld(selectedCharacters + MenuItems[i], new Vector3(position.X, yPosition + position.Y, position.Z), charactersPerLine, scale, Vector3.Zero, Vector3.One, highestItemLength);
                }
                else
                {
                    fontInterpreter.menuDrawStringInWorld(MenuItems[i], new Vector3(position.X, yPosition + position.Y, position.Z), charactersPerLine, scale,  Vector3.Zero, new Vector3(0.4f, 0.4f, 0.4f), highestItemLength);
                }

                yPosition += yDelta * ((int)MathF.Floor(MenuItems[i].Length / charactersPerLine) + 2);
            }
        }
    }


    public class Integer
    {
        private Func<int> _get;
        private Action<int> _set;

        public Integer(Func<int> @get, Action<int> @set)
        {
            _get = @get;
            _set = @set;
        }

        public int Value
        {
            get { return _get(); }
            set { _set(value); }
        }
    }

    public class IntValueMenu : Menu
    {
        private Integer integer;
        private Menu parentMenu;
        private int changeAmount;

        public IntValueMenu(GraphicsDevice graphicDevice, Rectangle windowSize, Integer Value, int changeAmount, Menu parentMenu) : base(graphicDevice, windowSize)
        {
            selectedOption = 0;

            _graphicsDevice = graphicDevice;
            _windowSize = windowSize;

            spriteBatch = new SpriteBatch(graphicDevice);

            textSize = 3;

            _effect = new SpriteEffects();

            integer = Value;

            this.changeAmount = changeAmount;

            this.parentMenu = parentMenu;

            MenuItems.Add(integer.Value.ToString());
        }

        public override void ChangeSelectedOptionUp()
        {
            integer.Value += changeAmount;
            MenuItems[0] = integer.Value.ToString();
        }
        public override void ChangeSelectedOptionDown()
        {
            integer.Value -= changeAmount;
            MenuItems[0] = integer.Value.ToString();
        }

        public override Menu selectOption(out int[] gameState)
        {
            gameState = new int[]{ (int) gameStates.Menu };

            return parentMenu;
        }
    }
}
