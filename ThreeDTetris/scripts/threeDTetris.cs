using Microsoft.Xna.Framework;
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
        private MainMenu _mainMenu;
        private Menu selectedMenu;

        private static gameStates gameState = new gameStates();

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

        private void setUpCamera()
        {
            Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        }

        //the models:
        //all 3d models are to be imported in the .fbx format
        private static Model cubeModel;

        //piece cube lists
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

        //used in testing only
        private static void DrawModel(Model model, Matrix world, Matrix view, Matrix projection, Color color)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EmissiveColor = new Vector3(color.R, color.G, color.B);
                    effect.EnableDefaultLighting();
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }

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

            board1 = new BoardClass.Board(6, 20, 6, _graphics.GraphicsDevice, piecesPresets, player1Camera);
            board2 = new BoardClass.Board(6, 20, 6, _graphics.GraphicsDevice, piecesPresets, player2Camera);

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
            cubeModel = Content.Load<Model>("Cube");

            /// <summary>
            /// this is the menu structure; 
            /// has to be in LoadContent due to font requirement;
            /// </summary>
            _mainMenu = new MainMenu(_graphics.GraphicsDevice, _font, windowSize);

            MainMenu playMenu = new MainMenu(_graphics.GraphicsDevice, _font, windowSize);
            
            SettingsMenu settingsMenu = new SettingsMenu(_graphics.GraphicsDevice, _font, windowSize);

            BoardSizeMenu boardSizeMenu = new BoardSizeMenu(_graphics.GraphicsDevice, _font, windowSize);

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

            IntValueMenu boardWidth = new IntValueMenu(_graphics.GraphicsDevice, _font, windowSize, boardWidthValue, 2, settingsMenu);
            IntValueMenu boardHeight = new IntValueMenu(_graphics.GraphicsDevice, _font, windowSize, boardHeightValue, 1, settingsMenu);
            IntValueMenu boardDepth = new IntValueMenu(_graphics.GraphicsDevice, _font, windowSize, boardDepthValue, 2, settingsMenu);

            boardSizeMenu.add("Width", boardWidth, gameStates.Menu);
            boardSizeMenu.add("Depth", boardDepth, gameStates.Menu);
            boardSizeMenu.add("Height", boardHeight, gameStates.Menu);
            boardSizeMenu.add("Back", settingsMenu, gameStates.Menu);

            settingsMenu.add("Board Size", boardSizeMenu, gameStates.Menu);
            settingsMenu.add("Back", _mainMenu, gameStates.Menu);

            playMenu.add("Standard", playMenu, gameStates.SoloStandard);
            playMenu.add("Solo 3D", playMenu, gameStates.SoloThreeD);
            playMenu.add("Back", _mainMenu, gameStates.Menu);

            _mainMenu.add("Play", playMenu, gameStates.Menu);
            _mainMenu.add("Settings", settingsMenu, gameStates.Menu);
            _mainMenu.add("Quit", settingsMenu, gameStates.Quit);

            selectedMenu = _mainMenu;

            BasicEffect fontEffect = new BasicEffect(_graphics.GraphicsDevice);

            fontEffect.LightingEnabled = true;
            fontEffect.VertexColorEnabled = true;
            fontEffect.TextureEnabled = false;

            fontEffect.EmissiveColor = new Vector3(1f, 1f, 1f);
            
            _fontInterpreter = new FontInterpreter(_font, GraphicsDevice, windowSize, fontEffect, player1Camera);
        }

        //keylist for only activating keys once per press;
        List<Keys> keyList = new List<Keys>();
        //returns true only on the first time a key is down
        private bool runOnKeyDown(Keys key)
        {
            if (Keyboard.GetState().IsKeyDown(key))
            {
                if (!keyList.Contains(key))
                {

                    keyList.Add(key);
                    return true;
                }
                return false;
            }
            else
            {
                keyList.Remove(key);
                return false;
            }
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
                    if (runOnKeyDown(Keys.Up))
                    {
                        selectedMenu.ChangeSelectedOptionUp();
                    }
                    if (runOnKeyDown(Keys.Down))
                    {
                        selectedMenu.ChangeSelectedOptionDown();
                    }
                    if (runOnKeyDown(Keys.Enter))
                    {
                        selectedMenu = selectedMenu.selectOption(out gameStates state);
                        gameState = state;
                    }

                    player1Camera.distanceFromMiddle = 10000;

                    break;



                case gameStates.Pause:
                    break;



                case gameStates.Quit:
                    Exit();
                    break;



                case gameStates.Lost:
                    if (runOnKeyDown(Keys.Enter))
                    {
                        gameState = gameStates.Menu;
                        selectedMenu = _mainMenu;
                    }
                    break;



                case gameStates.SoloStandard:
                    if (board1.zoomInAnimation == true)
                    {
                        if (player1Camera.distanceAnimation == null)
                        {
                            board1 = new BoardClass.Board(10, 20, 1, _graphics.GraphicsDevice, piecesPresets, player1Camera);
                            float startDistance = 10000;
                            player1Camera.distanceFromMiddle = startDistance;
                            player1Camera.distanceAnimation = new AnimationFloat(startDistance, player1Camera._finalDistanceFromMiddle, startDistance);
                            mainMenuPosition = player1Camera.position - Vector3.Normalize(player1Camera.position) * 400;
                        }
                        else
                        {
                            if (player1Camera.distanceAnimation.done == true)
                            {
                                player1Camera.distanceFromMiddle = player1Camera._finalDistanceFromMiddle;
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

                            board1.startTextScale = 1;
                            board1.startTextColor = Vector3.One;
                        }
                        board1.startTextScale -= secondsAfterLastFrame * board1.startTimerSpeed;
                        board1.startTextColor -= new Vector3(0.3f * secondsAfterLastFrame, 0.3f * secondsAfterLastFrame, 0.3f * secondsAfterLastFrame) * board1.startTimerSpeed;
                    }

                    if (board1.enabled == true)
                    {
                        //key inputs to manipulate camera
                        if (Keyboard.GetState().IsKeyDown(Keys.W))
                        {
                            player1Camera.move(0, player1Camera.speed * secondsAfterLastFrame);
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.S))
                        {
                            player1Camera.move(0, -player1Camera.speed * secondsAfterLastFrame);
                        }
                        if (runOnKeyDown(Keys.D) && camera1Animation == null)
                        {
                            camera1Animation = new AnimationFloat(0, -MathHelper.PiOver2, 0);
                        }
                        if (runOnKeyDown(Keys.A) && camera1Animation == null)
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
                            if (runOnKeyDown(Keys.Up))
                            {
                                board1.moveSelectedPieceRelativeToCamera(Vector3.Forward);
                            }

                            if (runOnKeyDown(Keys.Down))
                            {
                                board1.moveSelectedPieceRelativeToCamera(Vector3.Backward);
                            }

                            if (runOnKeyDown(Keys.Right))
                            {
                                board1.moveSelectedPieceRelativeToCamera(Vector3.Right);
                            }

                            if (runOnKeyDown(Keys.Left))
                            {
                                board1.moveSelectedPieceRelativeToCamera(Vector3.Left);
                            }

                            //move piece down faster
                            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                            {
                                board1.moveSelectedPieceRelativeToCamera(Vector3.Down * secondsAfterLastFrame * 30f);
                            }

                            //rotate piece
                            if (runOnKeyDown(Keys.I))
                            {
                                board1.rotateSelectedPieceRelativeToCamera(new Vector3(1, 0, 0));
                            }
                            if (runOnKeyDown(Keys.J))
                            {
                                board1.rotateSelectedPieceRelativeToCamera(new Vector3(0, 0, 1));
                            }
                            if (runOnKeyDown(Keys.K))
                            {
                                board1.rotateSelectedPieceRelativeToCamera(new Vector3(-1, 0, 0));
                            }
                            if (runOnKeyDown(Keys.L))
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
                            piece.move(new Vector3(0f, board1.downSpeed * secondsAfterLastFrame, 0f));
                            if (piece.position.Y > board1.height && piece.grounded == true)
                            {
                                gameState = gameStates.Lost;
                                board1.zoomInAnimation = true;
                            }
                        }

                        board1.lineClear();
                    }

                    break;
                case gameStates.SoloThreeD:
                    if (board1.zoomInAnimation == true)
                    {
                        if (player1Camera.distanceAnimation == null)
                        {
                            board1 = new BoardClass.Board(6, 20, 6, _graphics.GraphicsDevice, piecesPresets, player1Camera);
                            float startDistance = 10000;
                            player1Camera.distanceFromMiddle = startDistance;
                            player1Camera.distanceAnimation = new AnimationFloat(startDistance, player1Camera._finalDistanceFromMiddle, startDistance);
                            mainMenuPosition = player1Camera.position - Vector3.Normalize(player1Camera.position) * 400;
                        }
                        else
                        {
                            if (player1Camera.distanceAnimation.done == true)
                            {
                                player1Camera.distanceFromMiddle = player1Camera._finalDistanceFromMiddle;
                                player1Camera.distanceAnimation = null;
                                board1.startAnimation = true;
                                board1.zoomInAnimation = false;
                            }
                            else
                            {
                                player1Camera.distanceFromMiddle = player1Camera.distanceAnimation.easeInEaseOutBump(secondsAfterLastFrame);
                            }
                        }
                    }
                    if(board1.startAnimation == true)
                    {

                        board1.startTimer += secondsAfterLastFrame * board1.startTimerSpeed;
                        if(board1.startTimer > 1)
                        {
                            if(board1.startText == 1)
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

                            board1.startTextScale = 10;
                            board1.startTextColor = Vector3.One;
                        }
                        board1.startTextScale -= secondsAfterLastFrame * 9 * board1.startTimerSpeed;
                        board1.startTextColor -= new Vector3(0.3f * secondsAfterLastFrame, 0.3f * secondsAfterLastFrame, 0.3f * secondsAfterLastFrame) * board1.startTimerSpeed;
                    }
                    if(board1.enabled == true)
                    {
                        //key inputs to manipulate camera
                        if (Keyboard.GetState().IsKeyDown(Keys.W))
                        {
                            player1Camera.move(0, player1Camera.speed * secondsAfterLastFrame);
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.S))
                        {
                            player1Camera.move(0, -player1Camera.speed * secondsAfterLastFrame);
                        }
                        if (runOnKeyDown(Keys.D) && camera1Animation == null)
                        {
                            camera1Animation = new AnimationFloat(0, -MathHelper.PiOver2, 0);
                        }
                        if (runOnKeyDown(Keys.A) && camera1Animation == null)
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
                            if (runOnKeyDown(Keys.Up))
                            {
                                board1.moveSelectedPieceRelativeToCamera(Vector3.Forward);
                            }

                            if (runOnKeyDown(Keys.Down))
                            {
                                board1.moveSelectedPieceRelativeToCamera(Vector3.Backward);
                            }

                            if (runOnKeyDown(Keys.Right))
                            {
                                board1.moveSelectedPieceRelativeToCamera(Vector3.Right);
                            }

                            if (runOnKeyDown(Keys.Left))
                            {
                                board1.moveSelectedPieceRelativeToCamera(Vector3.Left);
                            }

                            //move piece down faster
                            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                            {
                                board1.moveSelectedPieceRelativeToCamera(Vector3.Down * secondsAfterLastFrame * 30f);
                            }

                            //rotate piece
                            if (runOnKeyDown(Keys.I))
                            {
                                board1.rotateSelectedPieceRelativeToCamera(new Vector3(1, 0, 0));
                            }
                            if (runOnKeyDown(Keys.J))
                            {
                                board1.rotateSelectedPieceRelativeToCamera(new Vector3(0, 0, 1));
                            }
                            if (runOnKeyDown(Keys.K))
                            {
                                board1.rotateSelectedPieceRelativeToCamera(new Vector3(-1, 0, 0));
                            }
                            if (runOnKeyDown(Keys.L))
                            {
                                board1.rotateSelectedPieceRelativeToCamera(new Vector3(0, 0, -1));
                            }

                        }


                        //increment the time since last piece
                        newPieceTimeCounter += secondsAfterLastFrame;

                        //animate the size of each block in the selected piece when it is spawned in from 0 to 1f
                        if (board1.addPieceAnimation != null)
                        {
                            if(board1.selectedPiece.position.Y < board1.height - 2 || board1.selectedPiece.grounded == true)
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
                            piece.move(new Vector3(0f, board1.downSpeed * secondsAfterLastFrame, 0f));
                            if(piece.position.Y > board1.height && piece.grounded == true)
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
            _spriteBatch.Begin();
            // TODO: Add your drawing code here

            basicEffect.VertexColorEnabled = true;

            basicEffect.LightingEnabled = true;

            basicEffect.TextureEnabled = true;

            //matrices
            basicEffect.View = player1Camera.viewMatrix;
            basicEffect.Projection = player1Camera.projectionMatrix;

            switch (gameState)
            {
                case gameStates.Menu:
                    selectedMenu.draw(_fontInterpreter, player1Camera.position);
                    break;



                case gameStates.Pause:
                    board1.draw(basicEffect);
                    break;



                case gameStates.Lost:
                    board1.draw(basicEffect);
                    string continueText = "continue?";
                    _fontInterpreter.menuDrawStringInWorld("continue?", player1Camera.position, 1000, 1.4f, Vector3.Zero, Vector3.One, continueText.Length + 1);
                    break;



                case gameStates.SoloStandard:
                    if(board1.zoomInAnimation == true)
                    {
                        selectedMenu.draw(_fontInterpreter, mainMenuPosition);
                    }
                    if (board1.startAnimation)
                    {
                        _fontInterpreter.drawStringRelativeToCamera("start in:", new Vector2(0, 200), 660, 5, 1f, Vector3.Zero, Vector3.One);
                        _fontInterpreter.drawStringRelativeToCamera(((int)board1.startText).ToString(), new Vector2(0, 1.0f), 6, 100, 1f/2f, new Vector3(2 * MathF.PI - (2f * MathF.PI * board1.startTextScale),0,0), board1.startTextColor);
                    }
                    board1.draw(basicEffect);
                    _fontInterpreter.drawStringRelativeToCamera("score: " + board1.score.ToString(), new Vector2(300, 200), 660, 1000, 1, Vector3.Zero, Vector3.One);
                    break;



                case gameStates.SoloThreeD:
                    if (board1.zoomInAnimation == true)
                    {
                        selectedMenu.draw(_fontInterpreter, mainMenuPosition);
                    }
                    if (board1.startAnimation)
                    {
                        _fontInterpreter.drawStringRelativeToCamera("start in", new Vector2(0, 200), 660, 100, 3, Vector3.Zero, Vector3.One);
                        _fontInterpreter.drawStringRelativeToCamera(((int)board1.startText).ToString(), new Vector2(0, 1.0f), 6, 100, board1.startTextScale/100, Vector3.Zero, board1.startTextColor);
                    }
                    board1.draw(basicEffect);
                    _fontInterpreter.drawStringRelativeToCamera("score: " + board1.score.ToString(), new Vector2(300, 200), 660, 1000, 1, Vector3.Zero, Vector3.One);
                    break;
            }

            //DrawModel(cubeModel, world, viewMatrix, projectionMatrix);                                                //draw test 3d model

            //Cube center = new Cube(Vector3.Zero, Color.White, 0.4f);                                                  //draw white sphere at origin
            //center.draw(GraphicsDevice);
            //_spriteBatch.Draw(_font.Texture, Vector2.Zero, Color.White);                                              //draw font texture
            //_fontInterpreter.drawString("hello_world", Vector2.Zero, 660, 1000, 1, Vector3.Zero, Vector3.One);        //test _fontinterpreter

            _spriteBatch.DrawString(_font, "FPS: " + 1 / (float)gameTime.ElapsedGameTime.TotalSeconds, new Vector2(100, 100), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}