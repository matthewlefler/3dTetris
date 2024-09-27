using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

using PieceClass;
using CameraClass;
using SimpleAnimation;
using CubeClass;
using System.Diagnostics;

namespace BoardClass
{

    public class Board
    {
        public int score { get; private set; } = 0;
        private Dictionary<int, int> linesToScores = new Dictionary<int, int>()
        {
            { 0, 0 },
            { 1, 100 },
            { 2, 300 },
            { 3, 500 },
            { 4, 800 },
        };
        
        private GraphicsDevice _graphicsDevice;
        private OrbitCamera camera;

        public bool enabled = false;
        public bool zoomInAnimation = true;
        public bool startAnimation = false;
        public float startTimer = 0f;
        public float startTimerSpeed = 0.8f;
        public float startText = 3;
        public float startTextScale = 0;
        public Vector3 startTextColor = Vector3.One;

        VertexPositionColor[] lines;

        int piecesPresetCounter = 0;

        public Piece selectedPiece;
        public AnimationVector3 selectedPieceAnimation;

        public Queue<Piece> nextPieces;
        public int numberOfQueuedPieces = 3;
        private Vector3 pieceQueueStartPostition;

        public List<Piece> pieceList = new List<Piece>();
        private List<piecePrefab> piecesPresets = new List<piecePrefab>();

        //board piece prams
        public float downSpeed = -2f;

        //size
        private int _width;
        private int _height;
        private int _depth;
        public int width { get {return _width; } set { _width = value; calcVaribleBoardValues(); } }
        public int height { get {return _height; } set { _height = value; calcVaribleBoardValues(); } }
        public int depth { get { return _depth; } set { _depth = value; calcVaribleBoardValues(); } }

        //randomly sort list
        private static Random rng = new Random();
        private static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        //refrence positions used in varying areas (botleft in translation matrix for peices, top middle as ref to spawn new pieces)
        public Vector3 bottomLeftPosition { get; private set; }
        public Vector3 topMiddle { get; private set; }
        public float boardDistanceFromMiddle { get; private set; }

        //animations relating to the board like spawning piece animations and new piece addition animations
        public AnimationFloat addPieceAnimation;
        public AnimationFloat nextPieceAnimation;

        public Board(int width, int height, int depth, GraphicsDevice graphicsDevice, List<piecePrefab> piecesPresets, OrbitCamera camera)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;

            const float c = 0.5f;
            float longestSide = depth;
            if(width > depth)
            {
                longestSide = width;
            }

            boardDistanceFromMiddle = (((-c + 1f) * longestSide) / 10f) + c;

            _graphicsDevice = graphicsDevice;
            this.camera = camera;

            float cameraHeight = camera.cameraHeight;

            calcVaribleBoardValues();

            //create queue
            nextPieces = new Queue<Piece>();
            pieceQueueStartPostition = new Vector3(width + 4, 0, depth / 2 - 1);

            Shuffle(piecesPresets);

            for (int i = numberOfQueuedPieces; i > 0; i--)
            {
                nextPieces.Enqueue(new Piece(piecesPresets[i], pieceQueueStartPostition + new Vector3(0, i * 4f - 4f, 0), bottomLeftPosition, _graphicsDevice, this));
                piecesPresetCounter++;
            }

            this.piecesPresets = piecesPresets;
        }

        void calcVaribleBoardValues()
        {
            float cameraHeight = 10f;

            bottomLeftPosition = new Vector3(-width / 2f, -cameraHeight, -depth / 2f);
            topMiddle = new Vector3(MathF.Floor(width / 2f) - 1, height + 2f, MathF.Floor(depth / 2f));

            lines = new VertexPositionColor[16];

            Color color = Color.White;

            //bottom horizontal lines
            lines[0] = new VertexPositionColor(new Vector3(width / 2f, -cameraHeight, depth / 2f), color);
            lines[1] = new VertexPositionColor(new Vector3(width / 2f, -cameraHeight, -depth / 2f), color);

            lines[2] = new VertexPositionColor(new Vector3(-width / 2f, -cameraHeight, depth / 2f), color);
            lines[3] = new VertexPositionColor(new Vector3(-width / 2f, -cameraHeight, -depth / 2f), color);

            lines[4] = new VertexPositionColor(new Vector3(width / 2f, -cameraHeight, -depth / 2f), color);
            lines[5] = new VertexPositionColor(new Vector3(-width / 2f, -cameraHeight, -depth / 2f), color);

            lines[6] = new VertexPositionColor(new Vector3(width / 2f, -cameraHeight, depth / 2f), color);
            lines[7] = new VertexPositionColor(new Vector3(-width / 2f, -cameraHeight, depth / 2f), color);

            //vertical lines
            lines[8] = new VertexPositionColor(new Vector3(width / 2f, -cameraHeight , -depth / 2f), color);
            lines[9] = new VertexPositionColor(new Vector3(width / 2f, height - cameraHeight, -depth / 2f), color);

            lines[10] = new VertexPositionColor(new Vector3(width / 2f, -cameraHeight, depth / 2f), color);
            lines[11] = new VertexPositionColor(new Vector3(width / 2f, height - cameraHeight, depth / 2f), color);

            lines[12] = new VertexPositionColor(new Vector3(-width / 2f, -cameraHeight, -depth / 2f), color);
            lines[13] = new VertexPositionColor(new Vector3(-width / 2f, height - cameraHeight, -depth / 2f), color);

            lines[14] = new VertexPositionColor(new Vector3(-width / 2f, -cameraHeight, depth / 2f), color);
            lines[15] = new VertexPositionColor(new Vector3(-width / 2f, height - cameraHeight, depth / 2f), color);
        }

        public void draw(BasicEffect effect)
        {

            foreach (Piece piece in pieceList)
            {
                piece.Draw(effect);
            }

            foreach (Piece piece in nextPieces)
            {
                piece.Draw(effect, Matrix.CreateFromAxisAngle(Vector3.Up, (MathF.PI - camera.orbitRotations.X) - MathHelper.PiOver2));
            }

            if (selectedPiece != null && selectedPiece.grounded == false)   
            {
                List<Vector3> orignialPositions = new List<Vector3>();
                List<Vector3> newPositions = new List<Vector3>();
                foreach (Cube cube in selectedPiece.cubes)
                {
                    orignialPositions.Add(cube.position);
                    newPositions.Add(Vector3.Floor(cube.position));
                }

                int count = 0;
                while (selectedPiece.isMoveLegal(orignialPositions, newPositions) == 1)
                {
                    for (int i = 0; i < newPositions.Count; i++)
                    {
                        newPositions[i] -= new Vector3(0, 1f, 0);
                    }

                    count++;
                    if(count > height*2)
                    {
                        break;
                    }
                }

                effect.AmbientLightColor = new Vector3(0.3f, 0.3f, 0.3f);
                effect.DirectionalLight0.DiffuseColor = Vector3.Zero;
                effect.EmissiveColor = Vector3.Zero;

                if(newPositions.Count > 0)
                {
                    selectedPiece.Draw(effect,  Matrix.CreateTranslation(new Vector3(0.1f, 0.1f, 0.1f)) * Matrix.CreateScale(0.9f) * Matrix.CreateTranslation(-1f * new Vector3(0.1f, 0.1f, 0.1f)) * Matrix.CreateTranslation(0, newPositions[0].Y - orignialPositions[0].Y + 1, 0));
                }
            }

            effect.VertexColorEnabled = true;

            effect.LightingEnabled = true;
            effect.EmissiveColor = new Vector3(1f, 1f, 1f);
            effect.AmbientLightColor = new Vector3(1f, 1f, 1f);

            effect.World = Matrix.CreateTranslation(Vector3.Zero);

            effect.TextureEnabled = false;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                _graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, lines, 0, 8);
            }
        }

        /// <summary>
        /// Checks then clears the tetris board lines if they are full
        /// </summary>
        public void lineClear()
        {
            List<List<Cube>> yLevelCubeSlices = new List<List<Cube>>();
            List<float> yValues = new List<float>();

            foreach (Piece piece in pieceList)
            {
                foreach (Cube cube in piece.cubes)
                {
                    if (!yValues.Contains(cube.position.Y))
                    {
                        yValues.Add(cube.position.Y);
                        yLevelCubeSlices.Add(new List<Cube>());
                    }

                    yLevelCubeSlices[yValues.IndexOf(cube.position.Y)].Add(cube);
                }
            }

            float?[] linesToClear = new float?[yValues.Count];
            int numberOfLinesToClear = 0;

            for (int y = 0; y < yValues.Count; y++)
            {
                if (yLevelCubeSlices[y].Count >= width * depth)
                {
                    linesToClear[y] = yValues[y];
                    numberOfLinesToClear++;
                }
                else
                {
                    linesToClear[y] = null;
                }
            }

            score += linesToScores[numberOfLinesToClear] * numberOfLinesToClear;

            foreach (float? y in linesToClear)
            {
                if (y == null)
                {
                    continue;
                }

                foreach (Piece piece in pieceList)
                {
                    for (int i = 0; i < piece.cubes.Count; i++)
                    {
                        if (piece.cubes[i].position.Y == y)
                        {
                            piece.cubes.Remove(piece.cubes[i]);
                            i--;
                        }
                    }
                }
            }
        }

        public void reset()
        {
            enabled = false;
            zoomInAnimation = true;
            startAnimation = false;

            startTimer = 0f;
            startTimerSpeed = 0.8f;
            startText = 3;
            startTextScale = 0;
            startTextColor = Vector3.One;

            this.pieceList.Clear();

            this.piecesPresetCounter = 0;
            
            const float c = 0.5f;
            float longestSide = depth;
            if(width > depth)
            {
                longestSide = width;
            }

            boardDistanceFromMiddle = (((-c + 1f) * longestSide) / 10f) + c;

            float cameraHeight = camera.cameraHeight;

            calcVaribleBoardValues();

            //create queue
            nextPieces = new Queue<Piece>();
            pieceQueueStartPostition = new Vector3(width + 4, 0, depth / 2 - 1);

            Shuffle(piecesPresets);

            for (int i = numberOfQueuedPieces; i > 0; i--)
            {
                nextPieces.Enqueue(new Piece(piecesPresets[i], pieceQueueStartPostition + new Vector3(0, i * 4f - 4f, 0), bottomLeftPosition, _graphicsDevice, this));
                piecesPresetCounter++;
            }
        }

        public void addNextPiece()
        {
            selectedPiece = nextPieces.Dequeue();

            foreach (Piece piece in nextPieces)
            {
                piece.moveWithoutChecks(new Vector3(0, 4, 0));
            }

            nextPieces.Enqueue(new Piece(piecesPresets[piecesPresetCounter], pieceQueueStartPostition, bottomLeftPosition, _graphicsDevice, this));

            piecesPresetCounter++;
            if (piecesPresetCounter >= piecesPresets.Count)
            {
                piecesPresetCounter = 0;
                Shuffle(piecesPresets);
            }

            selectedPiece.moveWithoutChecks(topMiddle - selectedPiece.position);
            selectedPiece.allowRotation = true;

            addPieceAnimation = new AnimationFloat(0, 1f, selectedPiece.blockSize);
            selectedPiece.blockSize = 0f;
            pieceList.Add(selectedPiece);
        }

        public void moveSelectedPieceRelativeToCamera(Vector3 offsets, float timePast)
        {
            if (selectedPiece != null)
            {
                float xRotation = camera.orbitRotations.X;
                if (xRotation >= 0 && xRotation <= MathHelper.PiOver4 || xRotation > MathHelper.PiOver4 * 7 && xRotation <= MathHelper.PiOver4 * 8)
                {
                    selectedPiece.move(new Vector3(offsets.Z, offsets.Y, -offsets.X), timePast);
                }
                if (xRotation > MathHelper.PiOver4 && xRotation <= MathHelper.PiOver4 * 3)
                {
                    selectedPiece.move(new Vector3(offsets.X, offsets.Y, offsets.Z), timePast);
                }
                if (xRotation > MathHelper.PiOver4 * 3 && xRotation <= MathHelper.PiOver4 * 5)
                {
                    selectedPiece.move(new Vector3(-offsets.Z, offsets.Y, offsets.X), timePast);
                }
                if (xRotation > MathHelper.PiOver4 * 5 && xRotation <= MathHelper.PiOver4 * 7)
                {
                    selectedPiece.move(new Vector3(-offsets.X, offsets.Y, -offsets.Z), timePast);
                }
            }
        }

        public void rotateSelectedPieceRelativeToCamera(Vector3 rotation)
        {
            if (selectedPiece != null)
            {
                float xRotation = camera.orbitRotations.X;
                if (xRotation >= 0 && xRotation <= MathHelper.PiOver4 || xRotation > MathHelper.PiOver4 * 7 && xRotation <= MathHelper.PiOver4 * 8)
                {
                    selectedPiece.rotate(new Vector3(rotation.X, rotation.Y, -rotation.Z));
                }
                if (xRotation > MathHelper.PiOver4 && xRotation <= MathHelper.PiOver4 * 3)
                {
                    selectedPiece.rotate(new Vector3(rotation.Z, rotation.Y, rotation.X));
                }
                if (xRotation > MathHelper.PiOver4 * 3 && xRotation <= MathHelper.PiOver4 * 5)
                {
                    selectedPiece.rotate(new Vector3(-rotation.X, rotation.Y, rotation.Z));
                }
                if (xRotation > MathHelper.PiOver4 * 5 && xRotation <= MathHelper.PiOver4 * 7)
                {
                    selectedPiece.rotate(new Vector3(-rotation.Z, rotation.Y, -rotation.X));
                }
            }
        }
    }

}
