using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TextRenderer;

namespace MenuClass
{
    public enum gameStates
    {
        Menu,
        Pause,
        SoloCustom,
        SoloStandard,
        SoloThreeD,
        OneVersesOneStandard,
        TwoVersesTwoThreeD,
        PlayerOneWon,
        PlayerTwoWon,
        Lost,
        Quit
    }

    public class Menu
    {
        internal GraphicsDevice _graphicsDevice;
        internal SpriteFont _font;
        internal Rectangle _windowSize;

        internal int selectedOption;

        internal SpriteBatch spriteBatch;

        public int textSize;

        internal SpriteEffects _effect;

        //menu elements and corresponding menus
        internal List<string> MenuItems = new List<string>();
        internal Dictionary<string, Menu> menuItemsToStates = new Dictionary<string, Menu>();
        internal Dictionary<string, gameStates> menuItemstoGameState = new Dictionary<string, gameStates>();

        public Menu(GraphicsDevice graphicsDevice, SpriteFont font, Rectangle windowSize)
        {
            selectedOption = 0;
            _graphicsDevice = graphicsDevice;
            _font = font;
            _windowSize = windowSize;
        }

        //draws the text in MainMenuItems verticaly
        public void draw(FontInterpreter fontInterpreter, Vector3 position)
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
                    fontInterpreter.menuDrawStringInWorld(selectedCharacters + MenuItems[i], new Vector3(position.X, yPosition + position.Y, position.Z), 100, 1f, Vector3.Zero, Vector3.One, highestItemLength);
                }
                else
                {
                    fontInterpreter.menuDrawStringInWorld(MenuItems[i], new Vector3(position.X, yPosition + position.Y, position.Z), 100, 1f, Vector3.Zero, new Vector3(0.4f, 0.4f, 0.4f), highestItemLength);
                }
            }
        }

        public void add(string name, Menu menu, gameStates state)
        {
            MenuItems.Add(name);
            menuItemsToStates.Add(name, menu);
            menuItemstoGameState.Add(name, state);
        }

        //return the currently selected Menu
        public virtual Menu selectOption(out gameStates gameState)
        {
            gameState = menuItemstoGameState[MenuItems[selectedOption]];

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


    public class MainMenu : Menu
    {
        public MainMenu(GraphicsDevice graphicDevice, SpriteFont font, Rectangle windowSize) : base(graphicDevice, font, windowSize)
        {
            selectedOption = 0;

            _graphicsDevice = graphicDevice;
            _font = font;

            _windowSize = windowSize;

            spriteBatch = new SpriteBatch(graphicDevice);

            textSize = 3;

            _effect = new SpriteEffects();
        }
    }

    public class SettingsMenu : Menu
    {

        public SettingsMenu(GraphicsDevice graphicDevice, SpriteFont font, Rectangle windowSize) : base(graphicDevice, font, windowSize)
        {
            selectedOption = 0;

            _graphicsDevice = graphicDevice;
            _font = font;
            _windowSize = windowSize;

            spriteBatch = new SpriteBatch(graphicDevice);

            textSize = 3;

            _effect = new SpriteEffects();
        }
    }

    public class BoardSizeMenu : Menu
    {
        public BoardSizeMenu(GraphicsDevice graphicDevice, SpriteFont font, Rectangle windowSize) : base(graphicDevice, font, windowSize)
        {
            selectedOption = 0;

            _graphicsDevice = graphicDevice;
            _font = font;
            _windowSize = windowSize;

            spriteBatch = new SpriteBatch(graphicDevice);

            textSize = 3;

            _effect = new SpriteEffects();
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

        public IntValueMenu(GraphicsDevice graphicDevice, SpriteFont font, Rectangle windowSize, Integer Value, int changeAmount, Menu parentMenu) : base(graphicDevice, font, windowSize)
        {
            selectedOption = 0;

            _graphicsDevice = graphicDevice;
            _font = font;
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

        public override Menu selectOption(out gameStates gameState)
        {
            gameState = gameStates.Menu;

            return parentMenu;
        }
    }
}
