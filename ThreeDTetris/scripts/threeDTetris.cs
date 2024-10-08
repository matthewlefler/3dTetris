﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Security.Principal;
using System.Linq.Expressions;
using Microsoft.Xna.Framework.Audio;

//creator made classes
//game parts classes
using MenuClass;
using CameraClass;
using CubeClass;
using PieceClass;
using BoardClass;
using SimpleAnimation;
using TextRenderer;
using input;

// MAKE SURE YOU RENAME ALL PROJECT FILES FROM DevcadeGame TO YOUR YOUR GAME NAME
namespace ThreeDTetris
{
    public class Game1 : Game
    {

        private static GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont _font;
        private static Color backgroundColor = Color.Black;

        //Menus
        private Menu _mainMenu;
        private Menu _pauseMenu;
        private Menu _continueMenu;
        private Menu selectedMenu;

        private static gameStates gameState = new gameStates();
        private static gameModes gameMode = new MenuClass.gameModes();

        private List<piecePrefab> piecesPresets = new List<piecePrefab>
        {
                IPiece,
                JPiece,
                LPiece,
                OPiece,
                SPiece,
                TPiece,
                ZPiece
        };

        //camera objects
        private static OrbitCamera player1Camera;
        private static OrbitCamera player2Camera;
        //camera objects animations
        private static AnimationFloat camera1Animation;
        private static AnimationFloat camera2Animation;

        private static Vector3 mainMenuPosition;

        private static FontInterpreter _fontInterpreter;


        //input handler
        private enum gameActions
        {
            //camera actions
            moveCameraUp,
            moveCameraDown,
            moveCameraLeft,
            moveCameraRight,

            moveCameraIn,
            moveCameraOut,

            //piece actions
            translatePieceLeft,
            translatePieceRight,
            translatePieceForward,
            translatePieceBackward,

            rotatePieceClockwise,
            rotatePieceCounterClockwise,
            rotatePieceAwayFromCamera,
            rotatePieceTowardsCamera,

            movePieceDownFaster,
            dropPieceImmediately,

            //menu actions
            menuSelectionDown,
            menuSelectionUp,
            menuSelectionLeft,
            menuSelectionRight,

            selectMenuSelection,

            //pause
            pause,
        }

        private static Dictionary<int, string> actionToInputString = new Dictionary<int, string>
        {
            {(int)gameActions.moveCameraUp, "move camera up"},
            {(int)gameActions.moveCameraDown, "move camera down"},
            {(int)gameActions.moveCameraLeft, "move camera left"},
            {(int)gameActions.moveCameraRight, "move camera right"},
            {(int)gameActions.moveCameraIn, "move camera in"},
            {(int)gameActions.moveCameraOut, "move camera out"},
            {(int)gameActions.translatePieceLeft, "translate piece left"},
            {(int)gameActions.translatePieceRight, "translate piece right"},
            {(int)gameActions.translatePieceForward, "translate piece away from camera"},
            {(int)gameActions.translatePieceBackward, "translate piece towards camera"},
            {(int)gameActions.rotatePieceClockwise, "rotate piece clockwise"},
            {(int)gameActions.rotatePieceCounterClockwise, "rotate piece counterclockwise"},
            {(int)gameActions.rotatePieceAwayFromCamera, "rotate piece away from camera"},
            {(int)gameActions.rotatePieceTowardsCamera, "rotate piece towards from camera"},
            {(int)gameActions.movePieceDownFaster, "move piece down faster"},
            {(int)gameActions.dropPieceImmediately, "drop piece immediately"},
            {(int)gameActions.menuSelectionDown, "menu selection down"},
            {(int)gameActions.menuSelectionUp, "menu selection up"},
            {(int)gameActions.menuSelectionLeft, "menu selection left"},
            {(int)gameActions.menuSelectionRight, "menu selection right"},
            {(int)gameActions.selectMenuSelection, "select menu selection"},
            {(int)gameActions.pause, "pause game"},
        };

        private static Dictionary<int, inputKey> defaultGameKeys = new Dictionary<int, inputKey>
        {
            //(int  , Input.ArcadeButtons )
            {(int)gameActions.moveCameraUp, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.StickUp ), ( 1 , Input.ArcadeButtons.A1 )}, new Keys?[]{Keys.W})},
            {(int)gameActions.moveCameraDown, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.StickDown ), ( 1 , Input.ArcadeButtons.A1 )}, new Keys?[]{Keys.S})},
            {(int)gameActions.moveCameraLeft, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.StickLeft ), ( 1 , Input.ArcadeButtons.A1 )}, new Keys?[]{Keys.A})},
            {(int)gameActions.moveCameraRight, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.StickRight ), ( 1 , Input.ArcadeButtons.A1 )}, new Keys?[]{Keys.D})},

            {(int)gameActions.moveCameraIn, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.B3 )}, new Keys?[]{Keys.Q})},
            {(int)gameActions.moveCameraOut, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.B4 )}, new Keys?[]{Keys.E})},

            {(int)gameActions.translatePieceLeft, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.StickLeft ), ( 1 , Input.ArcadeButtons.A2 )}, new Keys?[]{Keys.Left})},
            {(int)gameActions.translatePieceRight, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.StickRight ), ( 1 , Input.ArcadeButtons.A2 )}, new Keys?[]{Keys.Right})},
            {(int)gameActions.translatePieceForward, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.StickUp ), ( 1 , Input.ArcadeButtons.A2 )}, new Keys?[]{Keys.Up})},
            {(int)gameActions.translatePieceBackward, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.StickDown ), ( 1 , Input.ArcadeButtons.A2 )}, new Keys?[]{Keys.Down})},
            
            {(int)gameActions.rotatePieceClockwise, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.StickRight ), ( 1 , Input.ArcadeButtons.A3 )}, new Keys?[]{Keys.L})},
            {(int)gameActions.rotatePieceCounterClockwise, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.StickLeft ), ( 1 , Input.ArcadeButtons.A3 )}, new Keys?[]{Keys.J})},
            {(int)gameActions.rotatePieceAwayFromCamera, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.StickUp ), ( 1 , Input.ArcadeButtons.A3 )}, new Keys?[]{Keys.I})},
            {(int)gameActions.rotatePieceTowardsCamera, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.StickDown ), ( 1 , Input.ArcadeButtons.A3 )}, new Keys?[]{Keys.K})},
            
            {(int)gameActions.movePieceDownFaster, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.B1 )}, new Keys?[]{Keys.Space})},
            {(int)gameActions.dropPieceImmediately, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.B2 )}, new Keys?[]{Keys.M})},
            
            {(int)gameActions.menuSelectionDown, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.StickDown )}, new Keys?[]{Keys.Down})},
            {(int)gameActions.menuSelectionUp, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.StickUp )}, new Keys?[]{Keys.Up})},
            {(int)gameActions.menuSelectionLeft, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.StickLeft )}, new Keys?[]{Keys.Left})},
            {(int)gameActions.menuSelectionRight, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.StickRight )}, new Keys?[]{Keys.Right})},
            
            {(int)gameActions.selectMenuSelection, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.A1 )}, new Keys?[]{Keys.Enter})},
            
            {(int)gameActions.pause, new inputKey(new (int? playerNum, Input.ArcadeButtons? key)[] {( 1 , Input.ArcadeButtons.Menu )}, new Keys?[]{Keys.P})},
        };

        private static InputHandler inputHandler = new InputHandler(defaultGameKeys, actionToInputString);

        //the models:
        //all 3d models are to be imported in the .fbx format

        //piece cube lists
#region pieces

        static List<Vector3> cube2by2by2Cubes = new List<Vector3>()
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 1)
        };

        static List<Vector3> singleCubes = new List<Vector3>()
        {
            new Vector3(0, 0, 0),
        };

        static List<Vector3> ICubes = new List<Vector3>()
        {
            new Vector3(-1, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(2, 0, 0),
        };

        static List<Vector3> JCubes = new List<Vector3>()
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(2, 0, 0),
            new Vector3(0, 1, 0),
        };

        static List<Vector3> LCubes = new List<Vector3>()
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(2, 0, 0),
            new Vector3(2, 1, 0),
        };

        static List<Vector3> OCubes = new List<Vector3>()
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
        };

        static List<Vector3> SCubes = new List<Vector3>()
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(2, 1, 0),
        };

        static List<Vector3> TCubes = new List<Vector3>()
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(2, 0, 0),
            new Vector3(1, 1, 0),
        };

        static List<Vector3> ZCubes = new List<Vector3>()
        {
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 0, 0),
            new Vector3(2, 0, 0),
        };

        //kicking offsets
        private static (int x, int y)[] mainKickOffsets = new (int x, int y)[] //40 long, 8 sets of 5; J, L, T, S, Z Tetromino Wall Kick Data
        {
            (0, 0), (-1, 0), (-1, 1), ( 0,-2), (-1,-2),
            (0, 0), ( 1, 0), ( 1,-1), ( 0, 2), ( 1, 2),
            (0, 0), ( 1, 0), ( 1,-1), ( 0, 2), ( 1, 2),
            (0, 0), (-1, 0), (-1, 1), ( 0,-2), (-1,-2),
            (0, 0), ( 1, 0), ( 1, 1), ( 0,-2), ( 1,-2),
            (0, 0), (-1, 0), (-1,-1), ( 0, 2), (-1, 2),
            (0, 0), (-1, 0), (-1,-1), ( 0, 2), (-1, 2),
            (0, 0), ( 1, 0), ( 1, 1), ( 0,-2), ( 1,-2),
        };

        private static (int x, int y)[] IPieceKickOffsets = new (int x, int y)[] //40 long, 8 sets of 5; J, L, T, S, Z Tetromino Wall Kick Data
        {
            (0, 0), (-2, 0), ( 1, 0), (-2,-1), ( 1, 2),
            (0, 0), ( 2, 0), (-1, 0), ( 2, 1), (-1,-2),
            (0, 0), (-1, 0), ( 2, 0), (-1, 2), ( 2,-1),
            (0, 0), ( 1, 0), (-2, 0), ( 1,-2), (-2, 1),
            (0, 0), ( 2, 0), (-1, 0), ( 2, 1), (-1,-2),
            (0, 0), (-2, 0), ( 1, 0), (-2,-1), ( 1, 2),
            (0, 0), ( 1, 0), (-2, 0), ( 1,-2), (-2, 1),
            (0, 0), (-1, 0), ( 2, 0), (-1, 2), ( 2,-1),
        };

        //test pieces
        static piecePrefab cube2by2by2 = new piecePrefab(new Vector3(0.5f, 0.5f, 0.5f), cube2by2by2Cubes, Color.AliceBlue, mainKickOffsets);
        static piecePrefab singlePiece = new piecePrefab(new Vector3(0.5f, 0.5f, 0.5f), singleCubes, Color.BlueViolet, mainKickOffsets);

        //standard tetris pieces (all made of 4 blocks)
        static piecePrefab IPiece = new piecePrefab(new Vector3(2.0f, 0f, 0f), ICubes, Color.Aqua, IPieceKickOffsets);
        static piecePrefab JPiece = new piecePrefab(new Vector3(1.5f, 0.5f, 0.5f), JCubes, Color.MediumBlue, mainKickOffsets);
        static piecePrefab LPiece = new piecePrefab(new Vector3(1.5f, 0.5f, 0.5f), LCubes, Color.DarkOrange, mainKickOffsets);
        static piecePrefab OPiece = new piecePrefab(new Vector3(0.5f, 0.5f, 0.5f), OCubes, Color.Yellow, mainKickOffsets);
        static piecePrefab SPiece = new piecePrefab(new Vector3(1.5f, 0.5f, 0.5f), SCubes, Color.Lime, mainKickOffsets);
        static piecePrefab TPiece = new piecePrefab(new Vector3(1.5f, 0.5f, 0.5f), TCubes, Color.Magenta, mainKickOffsets);
        static piecePrefab ZPiece = new piecePrefab(new Vector3(1.5f, 0.5f, 0.5f), ZCubes, Color.Crimson, mainKickOffsets);

#endregion

        static float newPieceTime;
        static float newPieceTimeCounter;

        //tetris game boards
        private static Board board1;
        private static Board board2;

        /// <summary>
        /// Stores the window dimensions in a rectangle object for easy use
        /// </summary>
        private Rectangle windowSize;

        /// <summary>
        /// Game constructor
        /// </summary>
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        /// <summary>
        /// init the main game loop state and related varibles
        /// </summary>
        void gameInit()
        {
            player1Camera = new OrbitCamera(new Vector2((1f/2f) * MathF.PI, 0), _graphics, 10f);
            player2Camera = new OrbitCamera(new Vector2((1f/2f) * MathF.PI, 0), _graphics, 10f);

            board1 = new BoardClass.Board(5, 20, 5, _graphics.GraphicsDevice, piecesPresets, player1Camera);
            board2 = new BoardClass.Board(5, 20, 5, _graphics.GraphicsDevice, piecesPresets, player2Camera);

            gameState = gameStates.Menu;

            newPieceTime = 20f;
            newPieceTimeCounter = 0f;
        }

        /// <summary>
        /// Performs any setup that doesn't require loaded content before the first frame.
        /// </summary>
        protected override void Initialize()
        {
            // Sets up the input library
            Input.Initialize();
            //Persistence.Init(); Uncomment if using the persistence section for save and load

            _graphics.GraphicsProfile = GraphicsProfile.HiDef; 

            // Set window size if running debug (in release it will be fullscreen)
            #region
#if DEBUG
			_graphics.PreferredBackBufferWidth = 594; //594 on framework 16    devcade monitor is 9 wide by 21 tall or an aspect ration of 9/21
			_graphics.PreferredBackBufferHeight = 1386; //1386 on framework 16 
			_graphics.ApplyChanges();
#else
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            _graphics.ApplyChanges();
#endif
            #endregion

            // TODO: Add your initialization logic here

            //sets up the basic tetris componets
            gameInit();

            windowSize = new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rs;

            basicEffect = new BasicEffect(_graphics.GraphicsDevice);

            //basicEffect: effect setup
            basicEffect.VertexColorEnabled = false;
            basicEffect.TextureEnabled = true;

            //lighting
            basicEffect.LightingEnabled = true;
            basicEffect.AmbientLightColor = new Vector3(0.6f, 0.6f, 0.6f);

            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight0.DiffuseColor = new Vector3(0.35f, 0.35f, 0.35f);
            basicEffect.DirectionalLight0.Direction = Vector3.Left + Vector3.Down + Vector3.Backward;

            base.Initialize();
        }



        /// <summary>
        /// Performs any setup that requires loaded content before the first frame.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Content to load your game content here:
            _font = Content.Load<SpriteFont>("upHeaval");

            /// <summary>
            /// this is the menu structure; 
            /// has to be in LoadContent due to font requirement;
            /// </summary>
            _mainMenu = new Menu(_graphics.GraphicsDevice, windowSize);

            _pauseMenu = new Menu(_graphics.GraphicsDevice, windowSize);

            _continueMenu = new Menu(_graphics.GraphicsDevice, windowSize);

            Menu playMenu = new Menu(_graphics.GraphicsDevice, windowSize);
            
            Menu settingsMenu = new Menu(_graphics.GraphicsDevice, windowSize);

            Menu boardSizeMenu = new Menu(_graphics.GraphicsDevice, windowSize);

            Menu controls = new ControlMenu(_graphics.GraphicsDevice, windowSize, 22);


            int getBoardWidth()
            {
                return board1.width;
            }

            void setBoardWidth(int value)
            {
                board1.width = value;
            }

            Integer boardWidthValue = new Integer(new Func<int>(getBoardWidth), new Action<int>(setBoardWidth));

            int getBoardHeight()
            {
                return board1.height;
            }

            void setBoardHeight(int value)
            {
                board1.height = value;
            }

            Integer boardHeightValue = new Integer(new Func<int>(getBoardHeight), new Action<int>(setBoardHeight));

            int getBoardDepth()
            {
                return board1.depth;
            }

            void setBoardDepth(int value)
            {
                board1.depth = value;
            }

            Integer boardDepthValue = new Integer(new Func<int>(getBoardDepth), new Action<int>(setBoardDepth));

            //main menu
            IntValueMenu boardWidth = new IntValueMenu(_graphics.GraphicsDevice, windowSize, boardWidthValue, 2, settingsMenu);
            IntValueMenu boardHeight = new IntValueMenu(_graphics.GraphicsDevice, windowSize, boardHeightValue, 1, settingsMenu);
            IntValueMenu boardDepth = new IntValueMenu(_graphics.GraphicsDevice, windowSize, boardDepthValue, 2, settingsMenu);

            boardSizeMenu.add("Width", boardWidth, (int)gameStates.Menu);
            boardSizeMenu.add("Depth", boardDepth, (int)gameStates.Menu);
            boardSizeMenu.add("Height", boardHeight, (int)gameStates.Menu);
            boardSizeMenu.add("Back", settingsMenu, (int)gameStates.Menu);

            settingsMenu.add("Board Size", boardSizeMenu, (int)gameStates.Menu);
            settingsMenu.add("Back", _mainMenu, (int)gameStates.Menu);

            List<string> controlsItems = new List<string>();

            foreach(var action in defaultGameKeys)
            {
                controlsItems.Add(actionToInputString[action.Key]);
                controlsItems.Add(action.Value.devcadeToString());
            }   

            foreach(string item in controlsItems)
            {
                controls.add(item, controls, (int)gameStates.Menu);
            }
            controls.add("Back", _mainMenu, (int)gameStates.Menu);

            playMenu.add("Solo Custom", playMenu, (int)gameStates.Play, (int)gameModes.SoloCustom);
            playMenu.add("Solo Standard", playMenu, (int)gameStates.Play, (int)gameModes.SoloStandard);
            playMenu.add("Solo 3D", playMenu, (int)gameStates.Play, (int)gameModes.SoloThreeD);
            playMenu.add("Back", _mainMenu, (int)gameStates.Menu);

            _mainMenu.add("Play", playMenu, (int)gameStates.Menu);
            _mainMenu.add("Controls", controls, (int)gameStates.Menu);
            _mainMenu.add("Settings", settingsMenu, (int)gameStates.Menu);
            _mainMenu.add("Quit", settingsMenu, (int)gameStates.Quit);

            #region pauseMenu
            //pause menu            
            Menu pauseBoardSizeMenu = new Menu(_graphics.GraphicsDevice, windowSize);

            Menu pauseSettingsMenu = new Menu(_graphics.GraphicsDevice, windowSize);
            
            Menu pauseControls = new ControlMenu(_graphics.GraphicsDevice, windowSize, 22);

            IntValueMenu pauseBoardWidth = new IntValueMenu(_graphics.GraphicsDevice, windowSize, boardWidthValue, 2, pauseSettingsMenu);
            IntValueMenu pauseBoardHeight = new IntValueMenu(_graphics.GraphicsDevice, windowSize, boardHeightValue, 1, pauseSettingsMenu);
            IntValueMenu pauseBoardDepth = new IntValueMenu(_graphics.GraphicsDevice, windowSize, boardDepthValue, 2, pauseSettingsMenu);
            
            pauseBoardSizeMenu.add("Width", pauseBoardWidth, (int)gameStates.Pause);
            pauseBoardSizeMenu.add("Depth", pauseBoardDepth, (int)gameStates.Pause);
            pauseBoardSizeMenu.add("Height", pauseBoardHeight, (int)gameStates.Pause);
            pauseBoardSizeMenu.add("Back", pauseSettingsMenu, (int)gameStates.Pause);

            pauseSettingsMenu.add("Board Size", pauseBoardSizeMenu, (int)gameStates.Pause);
            pauseSettingsMenu.add("Back", _pauseMenu, (int)gameStates.Pause);

            foreach(string item in controlsItems)
            {
                pauseControls.add(item, pauseControls, (int)gameStates.Pause);
            }
            pauseControls.add("Back", _pauseMenu, (int)gameStates.Pause);

            _pauseMenu.add("Resume", _pauseMenu, (int)gameStates.Play);
            _pauseMenu.add("Settings", pauseSettingsMenu, (int)gameStates.Pause);
            _pauseMenu.add("Controls", pauseControls, (int)gameStates.Pause);
            _pauseMenu.add("Quit", _mainMenu, (int)gameStates.Menu);
            #endregion

            
            _continueMenu.add("Restart?", _continueMenu, (int)gameStates.Play);
            _continueMenu.add("Main Menu?", _mainMenu, (int)gameStates.Menu);

            selectedMenu = _mainMenu;

            BasicEffect fontEffect = new BasicEffect(_graphics.GraphicsDevice);

            fontEffect.LightingEnabled = true;
            fontEffect.VertexColorEnabled = true;
            fontEffect.TextureEnabled = false;

            fontEffect.EmissiveColor = new Vector3(1f, 1f, 1f);
            
            _fontInterpreter = new FontInterpreter(_font, GraphicsDevice, windowSize, fontEffect, player1Camera);
        }



        /// <summary>
        /// Your main update loop. This runs once every frame, over and over.
        /// </summary>
        /// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
        float time = 0;
        protected override void Update(GameTime gameTime)
        {
            float secondsAfterLastFrame = (float)gameTime.ElapsedGameTime.TotalSeconds;
            time += secondsAfterLastFrame;
            Input.Update(); // Updates the state of the input library

            // Exit when both menu buttons are pressed (or escape for keyboard debugging)
            // You can change this but it is suggested to keep the keybind of both menu
            // buttons at once for a graceful exit.
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) ||
                Input.GetButton(1, Input.ArcadeButtons.Menu) &&
                Input.GetButton(2, Input.ArcadeButtons.Menu))
            {
                Exit();
            }

            switch (gameState)
            {
                case gameStates.Menu:
                    if (inputHandler.runOnKeyDown((int)gameActions.menuSelectionUp))
                    {
                        selectedMenu.ChangeSelectedOptionUp();
                    }
                    if (inputHandler.runOnKeyDown((int)gameActions.menuSelectionDown))
                    {
                        selectedMenu.ChangeSelectedOptionDown();
                    }
                    if (inputHandler.runOnKeyDown((int)gameActions.selectMenuSelection))
                    {
                        selectedMenu = selectedMenu.selectOption(out int[] states);
                        if(states.Length == 1)
                        {
                            gameState = (gameStates)states[0]; //cast from int back to gamestate
                        }
                        else if(states.Length == 2) 
                        {
                            gameState = (gameStates)states[0]; //cast from int back to gamestate
                            gameMode = (gameModes)states[1];
                        }

                    }

                    player1Camera.distanceFromMiddle = 10000;

                    break;



                case gameStates.Pause:
                    if(inputHandler.runOnKeyDown((int)gameActions.pause))
                    {
                        gameState = gameStates.Play;
                    }
                    
                    if (inputHandler.runOnKeyDown((int)gameActions.menuSelectionUp))
                    {
                        selectedMenu.ChangeSelectedOptionUp();
                    }
                    if (inputHandler.runOnKeyDown((int)gameActions.menuSelectionDown))
                    {
                        selectedMenu.ChangeSelectedOptionDown();
                    }

                    if (inputHandler.runOnKeyDown((int)gameActions.selectMenuSelection))
                    {
                        selectedMenu = selectedMenu.selectOption(out int[] states);
                        if(states.Length == 1)
                        {
                            gameState = (gameStates)states[0]; //cast from int back to gamestate

                            //reset board
                            if(gameState == gameStates.Menu)
                            {
                                board1.zoomInAnimation = true;
                                board1.reset();
                                player1Camera.distanceAnimation = null;
                            }
                        }
                        else if(states.Length == 2) 
                        {
                            gameState = (gameStates)states[0]; //cast from int back to gamestate
                            gameMode = (gameModes)states[1];

                            //reset board
                            if(gameState == gameStates.Menu)
                            {
                                board1.zoomInAnimation = true;
                                board1.reset();
                                player1Camera.distanceAnimation = null;
                            }
                        }

                    }

                    break;



                case gameStates.Quit:
                    Exit();
                    break;



                case gameStates.Lost:
                    if (inputHandler.runOnKeyDown((int)gameActions.selectMenuSelection))
                    {
                        gameState = gameStates.Menu;
                        selectedMenu = _mainMenu;
                        player1Camera.absoluteMove((1f/2f) * MathF.PI,0);
                        player2Camera.absoluteMove((1f/2f) * MathF.PI,0);
                    }
                    break;



                case gameStates.Play:
                    if(inputHandler.runOnKeyDown((int)gameActions.pause))
                    {
                        gameState = gameStates.Pause;
                        selectedMenu = _pauseMenu;
                    }
                    
                    //anim to zoom in from menu position
                    if (board1.zoomInAnimation == true)
                    {
                        if (player1Camera.distanceAnimation == null)
                        {
                            switch(gameMode)
                            {
                                case gameModes.SoloCustom:
                                    board1.reset();
                                    break;

                                case gameModes.SoloStandard:
                                    board1 = new BoardClass.Board(10, 20, 1, _graphics.GraphicsDevice, piecesPresets, player1Camera);
                                    break;

                                case gameModes.SoloThreeD:
                                    board1 = new BoardClass.Board(6, 20, 6, _graphics.GraphicsDevice, piecesPresets, player1Camera);
                                    break;

                                default:
                                    board1 = new BoardClass.Board(10, 20, 10, _graphics.GraphicsDevice, piecesPresets, player1Camera);
                                    break;
                            }

                            float startDistance = 10000;
                            player1Camera.distanceFromMiddle = startDistance;
                            player1Camera.distanceAnimation = new AnimationFloat(startDistance, player1Camera._finalDistanceFromMiddle * board1.boardDistanceFromMiddle, startDistance);
                            mainMenuPosition = player1Camera.position - Vector3.Normalize(player1Camera.position) * 400;
                        }
                        else
                        {
                            if (player1Camera.distanceAnimation.done == true)
                            {
                                player1Camera.distanceFromMiddle = player1Camera._finalDistanceFromMiddle * board1.boardDistanceFromMiddle;
                                player1Camera.distanceAnimation = null;
                                board1.startAnimation = true;
                                board1.zoomInAnimation = false;
                            }
                            else
                            {
                                player1Camera.distanceFromMiddle = player1Camera.distanceAnimation.easeInEaseOutBump(secondsAfterLastFrame/2f);
                            }
                        }
                    }

                    //start anim
                    if (board1.startAnimation == true)
                    {
                        board1.startTimer += secondsAfterLastFrame * board1.startTimerSpeed;
                        if (board1.startTimer > 1)
                        {
                            if (board1.startText == 1)
                            {
                                board1.enabled = true;
                                board1.startTimer = 0;
                                board1.startText = 3;
                                board1.startAnimation = false;
                                newPieceTimeCounter = newPieceTime;
                                break;
                            }
                            board1.startTimer -= 1;
                            board1.startText -= 1;

                            board1.startTextScale = 0;
                            board1.startTextColor = Vector3.One;
                        }
                        board1.startTextScale += -1 * (3f/4f) * secondsAfterLastFrame * board1.startTimerSpeed;
                        board1.startTextColor -= new Vector3(0.3f * secondsAfterLastFrame, 0.3f * secondsAfterLastFrame, 0.3f * secondsAfterLastFrame) * board1.startTimerSpeed;
                    }

                    //start anim
                    if (board1.startAnimation == true)
                    {
                        board1.startTimer += secondsAfterLastFrame * board1.startTimerSpeed;
                        if (board1.startTimer > 1)
                        {
                            if (board1.startText == 1)
                            {
                                board1.enabled = true;
                                board1.startTimer = 0;
                                board1.startText = 3;
                                board1.startAnimation = false;
                                newPieceTimeCounter = newPieceTime;
                                break;
                            }
                            board1.startTimer -= 1;
                            board1.startText -= 1;

                            board1.startTextScale = 0;
                            board1.startTextColor = Vector3.One;
                        }
                        board1.startTextScale += -1 * (3f/4f) * secondsAfterLastFrame * board1.startTimerSpeed;
                        board1.startTextColor -= new Vector3(0.3f * secondsAfterLastFrame, 0.3f * secondsAfterLastFrame, 0.3f * secondsAfterLastFrame) * board1.startTimerSpeed;
                    }

                    //main game state
                    if (board1.enabled == true)
                    {
                        //key inputs to manipulate camera
                        if (inputHandler.isKeyDown((int)gameActions.moveCameraUp))
                        {
                            player1Camera.move(0, player1Camera.speed * secondsAfterLastFrame);
                        }
                        if (inputHandler.isKeyDown((int)gameActions.moveCameraDown))
                        {
                            player1Camera.move(0, -player1Camera.speed * secondsAfterLastFrame);
                        }

                        if (inputHandler.isKeyDown((int)gameActions.moveCameraOut))
                        {
                            player1Camera.distanceFromMiddle += player1Camera.speed * 20f * secondsAfterLastFrame;
                            if(player1Camera.distanceFromMiddle > player1Camera._finalDistanceFromMiddle * 1000f)
                            {
                                player1Camera.distanceFromMiddle = player1Camera._finalDistanceFromMiddle * 1000f;
                            }
                        }
                        if (inputHandler.isKeyDown((int)gameActions.moveCameraIn))
                        {
                            player1Camera.distanceFromMiddle -= player1Camera.speed * 20f * secondsAfterLastFrame;

                            if(player1Camera.distanceFromMiddle < 3f)
                            {
                                player1Camera.distanceFromMiddle = 3f;
                            }
                        }

                        if (inputHandler.runOnKeyDown((int)gameActions.moveCameraRight) && camera1Animation == null)
                        {
                            camera1Animation = new AnimationFloat(0, -MathHelper.PiOver2, 0);
                        }
                        if (inputHandler.runOnKeyDown((int)gameActions.moveCameraLeft) && camera1Animation == null)
                        {
                            camera1Animation = new AnimationFloat(0, MathHelper.PiOver2, 0);
                        }
                        //camera x rotation animation
                        if (camera1Animation != null)
                        {
                            player1Camera.move(camera1Animation.deltaEaseInEaseOut(secondsAfterLastFrame * (player1Camera.speed + 3)), 0);

                            if (camera1Animation.done)
                            {
                                camera1Animation = null;
                            }
                        }

                        //key inputs to move and rotate selected piece
                        if (board1.selectedPiece != null)
                        {
                            //translate piece
                            if (inputHandler.runOnKeyDown((int)gameActions.translatePieceForward))
                            {
                                board1.moveSelectedPieceRelativeToCamera(Vector3.Forward, secondsAfterLastFrame);
                            }

                            if (inputHandler.runOnKeyDown((int)gameActions.translatePieceBackward))
                            {
                                board1.moveSelectedPieceRelativeToCamera(Vector3.Backward, secondsAfterLastFrame);
                            }

                            if (inputHandler.runOnKeyDown((int)gameActions.translatePieceRight))
                            {
                                board1.moveSelectedPieceRelativeToCamera(Vector3.Right, secondsAfterLastFrame);
                            }

                            if (inputHandler.runOnKeyDown((int)gameActions.translatePieceLeft))
                            {
                                board1.moveSelectedPieceRelativeToCamera(Vector3.Left, secondsAfterLastFrame);
                            }

                            //move piece down faster
                            if (inputHandler.isKeyDown((int)gameActions.movePieceDownFaster))
                            {
                                board1.moveSelectedPieceRelativeToCamera(Vector3.Down * secondsAfterLastFrame * 30f, secondsAfterLastFrame);
                            }

                            //rotate piece
                            if (inputHandler.runOnKeyDown((int)gameActions.rotatePieceAwayFromCamera))
                            {
                                board1.rotateSelectedPieceRelativeToCamera(new Vector3(1, 0, 0));
                            }
                            if (inputHandler.runOnKeyDown((int)gameActions.rotatePieceCounterClockwise))
                            {
                                board1.rotateSelectedPieceRelativeToCamera(new Vector3(0, 0, 1));
                            }
                            if (inputHandler.runOnKeyDown((int)gameActions.rotatePieceTowardsCamera))
                            {
                                board1.rotateSelectedPieceRelativeToCamera(new Vector3(-1, 0, 0));
                            }
                            if (inputHandler.runOnKeyDown((int)gameActions.rotatePieceClockwise))
                            {
                                board1.rotateSelectedPieceRelativeToCamera(new Vector3(0, 0, -1));
                            }

                        }


                        //increment the time since last piece
                        newPieceTimeCounter += secondsAfterLastFrame;

                        //animate the size of each block in the selected piece when it is spawned in from 0 to 1f
                        if (board1.addPieceAnimation != null)
                        {
                            if (board1.selectedPiece.position.Y < board1.height - 2 || board1.selectedPiece.grounded == true)
                            {
                                board1.selectedPiece.blockSize = board1.addPieceAnimation.Lerp(3f);
                            }
                            else
                            {
                                board1.selectedPiece.blockSize = board1.addPieceAnimation.Lerp(secondsAfterLastFrame * 2f);
                            }

                            if (board1.addPieceAnimation.done)
                            {
                                board1.addPieceAnimation = null;
                            }
                        }

                        //add the new piece if suffencient time has passed
                        if (board1.selectedPiece != null)
                        {
                            if (board1.selectedPiece.grounded == true)
                            {
                                newPieceTimeCounter = 0;

                                board1.addNextPiece();
                            }
                        }

                        if (newPieceTimeCounter > newPieceTime)
                        {
                            newPieceTimeCounter = 0;

                            board1.addNextPiece();
                        }

                        //move each piece in the board down.
                        //and lose game if piece gets grounded higher than board height
                        foreach (Piece piece in board1.pieceList)
                        {
                            piece.move(new Vector3(0f, board1.downSpeed * secondsAfterLastFrame, 0f), secondsAfterLastFrame);
                            if (piece.position.Y > board1.height && piece.grounded == true)
                            {
                                gameState = gameStates.Lost;
                                board1.zoomInAnimation = true;
                            }
                        }

                        board1.lineClear();
                    }

                    break;


            }

            base.Update(gameTime);
        }


        private static BasicEffect basicEffect;
        /// <summary>
        /// Your main draw loop. This runs once every frame, over and over.
        /// </summary>
        /// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Batches all the draw calls for this frame, and then performs them all at once
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            // TODO: Add your drawing code here

            basicEffect.VertexColorEnabled = true;

            basicEffect.LightingEnabled = true;

            basicEffect.TextureEnabled = true;

            basicEffect.FogEnabled = false;

            //matrices
            basicEffect.View = player1Camera.viewMatrix;
            basicEffect.Projection = player1Camera.projectionMatrix;

            switch (gameState)
            {
                case gameStates.Menu:
                    selectedMenu.draw(_fontInterpreter, player1Camera.position, 1f);
                    break;



                case gameStates.Pause:
                    basicEffect.FogColor = Vector3.Zero;

                    basicEffect.FogStart = player1Camera._finalDistanceFromMiddle - 20f;
                    basicEffect.FogEnd = 140f;

                    basicEffect.FogEnabled = true;

                    board1.draw(basicEffect);
                    selectedMenu.draw(_fontInterpreter, player1Camera.position, 1f);
                    break;



                case gameStates.Lost:
                    basicEffect.FogColor = Vector3.Zero;

                    basicEffect.FogStart = player1Camera._finalDistanceFromMiddle - 20f;
                    basicEffect.FogEnd = 140f;

                    basicEffect.FogEnabled = true;
                    board1.draw(basicEffect);
                    _continueMenu.draw(_fontInterpreter, player1Camera.position, 1f);

                    break;

                

                case gameStates.Play:
                    if(board1.zoomInAnimation == true)
                    {
                        selectedMenu.draw(_fontInterpreter, mainMenuPosition, 1f);
                    }
                    if (board1.startAnimation)
                    {
                        _fontInterpreter.drawStringRelativeToCamera(((int)board1.startText).ToString(), new Vector2(0, 2f), 6, 100, 1f/2f, new Vector3(MathF.PI - (2 * MathF.PI * board1.startTextScale),0,0), board1.startTextColor);
                    }
                    board1.draw(basicEffect);
                    _fontInterpreter.drawStringInWorld("score:", new Vector3(board1.width + 1f, _graphics.PreferredBackBufferHeight/40 - 14, board1.depth / 2f), 660, 0.05f, new Vector3(0, 0, 0), Vector3.One);
                    _fontInterpreter.drawStringInWorld(board1.score.ToString(), new Vector3(board1.width + 1f, _graphics.PreferredBackBufferHeight/40 - 16, board1.depth / 2f), 10, 0.05f, new Vector3(0, 0, 0), Vector3.One);
                    break;



            }
            
            //Cube center = new Cube(Vector3.Zero, Color.White, 0.4f);                                                  //draw white sphere at origin
            //center.draw(GraphicsDevice);
            //_spriteBatch.Draw(_font.Texture, Vector2.Zero, Color.White);                                              //draw font texture
            //_fontInterpreter.drawString("hello_world", Vector2.Zero, 660, 1000, 1, Vector3.Zero, Vector3.One);        //test _fontinterpreter

            //_spriteBatch.DrawString(_font, "FPS: " + 1 / (float)gameTime.ElapsedGameTime.TotalSeconds, new Vector2(100, 100), Color.White);            //draw fps

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}