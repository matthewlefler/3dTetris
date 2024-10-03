using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Linq;

using CubeClass;
using BoardClass;

namespace PieceClass
{
    public struct piecePrefab
    {
        public Vector3 rotationPoint;
        public List<Vector3> cubesInPiece;
        public Color color;
        public (int x, int y)[] KickOffsets;

        public piecePrefab(Vector3 rotationPoint, List<Vector3> cubesInPiece, Color color, (int x, int y)[] KickOffsets)
        {
            this.rotationPoint = rotationPoint;
            this.cubesInPiece = cubesInPiece;
            this.color = color;
            this.KickOffsets = KickOffsets;
        }
    }

    public class Piece
    {
        //base arguments
        public bool allowRotation;
        Color color;

        private float _blockSize;

        public float blockSize
        {
            get { return _blockSize; } //the size of the blocks within the piece
            set
            {
                foreach (Cube cube in cubes)
                {
                    cube.scale = value;
                }
                _blockSize = value;
            }
        }

        //list of the cubes that comprise the piece
        public List<Cube> cubes = new List<Cube>();
        private Texture2D cubeTexture;

        //piece position
        public Vector3 _position;
        public Vector3 position { get { return _position; } private set { _position = value; } }
        //rotation point of the blocks
        private Vector3 rotationPoint;
        private Vector3 rotation;

        //kicking offsets
        Dictionary<(int, int), int> RotationTokickOffset = new Dictionary<(int, int), int>
        {
            {(0,1), 0},
            {(1,0), 5},

            {(1,2), 10},
            {(2,1), 15},

            {(2,3), 20},
            {(3,2), 25},

            {(3,0), 30},
            {(0,3), 35},
        };
        private (int x, int y)[] kickOffsets;

        private GraphicsDevice _graphicsDevice;
        private Board board;

        //grounded elements to control when peicces can no longer be controled and when to spawn the next piece
        public bool grounded = false;
        private float timeGrounded = 0f; //count the amount of time a piece has to be unable to move down
        private float maxTimeGrounded = 1.5f; //amount of time a piece has to be unable to move down to be then grounded/fixed in place

        //constructor for the class Piece
        public Piece(piecePrefab piecePrefab, Vector3 position, Vector3 bottomLeftPosition, GraphicsDevice graphicsDevice, Board board)
        {
            color = piecePrefab.color;
            _position = position;
            rotationPoint = piecePrefab.rotationPoint;
            rotation = Vector3.Zero;

            kickOffsets = piecePrefab.KickOffsets;

            foreach (Vector3 pos in piecePrefab.cubesInPiece)
            {
                Cube cube = new CubeClass.Cube(pos, color, 1f, graphicsDevice, bottomLeftPosition);
                cube.position += position;
                cubes.Add(cube);
            }

            _graphicsDevice = graphicsDevice;
            this.board = board;

            cubeTexture = new Texture2D(graphicsDevice, 50, 50);

            //cube texture
            Color[] cubeColorData = new Color[50 * 50];
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    int outlineWidth = 3;
                    int insideWidth = 7;

                    if (j < outlineWidth || j > 50 - outlineWidth || i < outlineWidth || i > 50 - outlineWidth)
                    {
                        cubeColorData[i * 50 + j] = new Color(0.1f, 0.1f, 0.1f, 1f);
                        continue;
                    }

                    if (j > insideWidth && j < 50 - insideWidth && i > insideWidth && i < 50 - insideWidth)
                    {
                        cubeColorData[i * 50 + j] = new Color(0f, 0f, 0f, 1f);
                        continue;
                    }

                    cubeColorData[i * 50 + j] = piecePrefab.color;
                }
            }
            cubeTexture.SetData(cubeColorData);

        }

        public void Draw(BasicEffect effect)
        {
            foreach (Cube cube in cubes)
            {
                effect.World = cube.worldMatrix;
                effect.Texture = cubeTexture;
                
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    VertexPositionColor point = new VertexPositionColor(rotationPoint, Color.White);
                    VertexPositionColor[] points = new VertexPositionColor[] { point };

                    _graphicsDevice.DrawUserPrimitives(PrimitiveType.PointList, points, 0, 1);
                }

                cube.draw(effect);
            }
        }

        public void DrawMatrixAfter(BasicEffect effect, Matrix matrix)
        {
            foreach (Cube cube in cubes)
            {
                effect.World = cube.worldMatrix * matrix;
                effect.Texture = cubeTexture;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    VertexPositionColor point = new VertexPositionColor(rotationPoint, Color.White);
                    VertexPositionColor[] points = new VertexPositionColor[] { point };

                    _graphicsDevice.DrawUserPrimitives(PrimitiveType.PointList, points, 0, 1);
                }

                cube.draw(effect);
            }
        }

        public void DrawMatrixBefore(BasicEffect effect, Matrix matrix)
        {
            foreach (Cube cube in cubes)
            {
                effect.World = matrix * cube.worldMatrix;
                effect.Texture = cubeTexture;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    VertexPositionColor point = new VertexPositionColor(rotationPoint, Color.White);
                    VertexPositionColor[] points = new VertexPositionColor[] { point };

                    _graphicsDevice.DrawUserPrimitives(PrimitiveType.PointList, points, 0, 1);
                }

                cube.draw(effect);
            }
        }

        public void moveWithoutChecks(Vector3 offsets)
        {
            _position += offsets;

            foreach (Cube cube in cubes)
            {
                cube.move(offsets);
            }
        }

        public void move(Vector3 offsets, float timePast)
        {
            List<Vector3> originalPositions = new List<Vector3>();
            List<Vector3> newPositions = new List<Vector3>();

            for (int i = 0; i < cubes.Count; i++)
            {
                newPositions.Add(cubes[i].position + offsets);
                originalPositions.Add(new Vector3(cubes[i].position.X, cubes[i].position.Y, cubes[i].position.Z));
            }

            int isTheMoveLegal = isMoveLegal(originalPositions, newPositions);

            if (isTheMoveLegal == 1)
            {
                _position += offsets;

                foreach (Cube cube in cubes)
                {
                    cube.move(offsets);
                }
            }
            else
            {
                if (offsets.Y != 0) { isTheMoveLegal = -1; }

                if (isTheMoveLegal == 0)
                {
                    _position.X = MathF.Round(_position.X, 1);
                    _position.Y = MathF.Round(_position.Y, 1);
                    _position.Z = MathF.Round(_position.Z, 1);
                    foreach (Cube cube in cubes)
                    {
                        cube.position.X = MathF.Round(cube.position.X, 1);
                        cube.position.Y = MathF.Round(cube.position.Y, 1);
                        cube.position.Z = MathF.Round(cube.position.Z, 1);
                    }
                }

                if (isTheMoveLegal == -1)
                {
                    if(timeGrounded > maxTimeGrounded)
                    {
                        this.grounded = true;
                    }
                    else
                    {
                        this.timeGrounded += timePast;
                    }
                    
                    _position = Vector3.Floor(_position);
                    foreach (Cube cube in cubes)
                    {
                        cube.position.Y = MathF.Floor(cube.position.Y);
                    }
                }
            }
        }

        //todo: add repeated kick moves + checks
        //rotationAmounts.X refers to the XY plane, rotationAmounts.Y the XZ plane, rotationAmounts.Z the the ZY plane
        public void rotate(Vector3 rotationAmounts)
        {
            if (allowRotation)
            {
                Vector3[] positions = new Vector3[cubes.Count];
                Vector3[] tempPositions = new Vector3[cubes.Count];

                List<Vector3> originalPositions = new List<Vector3>();
                List<Vector3> newPositions = new List<Vector3>();

                int currentRotation = (int)(rotationAmounts * rotation).Length();
                int kickRotation = currentRotation;

                //add rotation to kickRotation
                if (rotationAmounts.X == -1 || rotationAmounts.Y == -1 || rotationAmounts.Z == -1)
                {
                    kickRotation -= 1;
                }
                else
                {
                    kickRotation += 1;
                }

                if(kickRotation < 0)
                {
                    kickRotation += 4;
                }
                if (kickRotation > 3)
                {
                    kickRotation -= 4;
                }
                //set rickRotation to the offset index in the kickOffsets array so that the loop loops over the correct offsets for the rotation
                kickRotation = RotationTokickOffset[(currentRotation, kickRotation)];


                for (int kickOffsetIndex = 0; kickOffsetIndex < 5; kickOffsetIndex++)
                {
                    (int x, int y) offset = kickOffsets[kickRotation + kickOffsetIndex];

                    originalPositions.Clear();
                    for (int i = 0; i < positions.Length; i++)
                    {
                        tempPositions[i] = cubes[i].position - position - rotationPoint + new Vector3(offset.x, offset.y, 0);
                        tempPositions[i].X = MathF.Round(tempPositions[i].X, 1);
                        tempPositions[i].Y = MathF.Round(tempPositions[i].Y, 1);
                        tempPositions[i].Z = MathF.Round(tempPositions[i].Z, 1);

                        originalPositions.Add(cubes[i].position);
                    }

                    (Vector3[] positions, List<Vector3> newPositions) value = rotatePositions(rotationAmounts, tempPositions);

                    newPositions = value.newPositions;
                    positions = value.positions;

                    if (isMoveLegal(originalPositions, newPositions) == 1)
                    {
                        for (int i = 0; i < positions.Length; i++)
                        {
                            cubes[i].position = positions[i] + position + rotationPoint;
                        }
                        return;
                    }
                }
                foreach (Cube cube in cubes)
                {
                    cube.position.X = MathF.Round(cube.position.X, 1);
                    cube.position.Y = MathF.Round(cube.position.Y, 1);
                    cube.position.Z = MathF.Round(cube.position.Z, 1);
                }

                rotation += rotationAmounts;

                if(rotation.X > 3)
                {
                    rotation.X -= 4;
                }
                if (rotation.Y > 3)
                {
                    rotation.Y -= 4;
                }
                if (rotation.Z > 3)
                {
                    rotation.Z -= 4;
                }
                if (rotation.X < 0)
                {
                    rotation.X += 4;
                }
                if (rotation.Y < 0)
                {
                    rotation.Y += 4;
                }
                if (rotation.Z < 0)
                {
                    rotation.Z += 4;
                }

            }
        }

        public (Vector3[] positions, List<Vector3> newPositions) rotatePositions(Vector3 rotationAmounts, Vector3[] tempPositions)
        {
            List<Vector3> newPositions = new List<Vector3>();
            Vector3[] positions = new Vector3[tempPositions.Length];

            for (int i = 0; i < tempPositions.Length; i++)
            {
                if (rotationAmounts.X > 0)
                {
                    positions[i].X = -tempPositions[i].Y;
                    positions[i].Y = tempPositions[i].X;
                    positions[i].Z = tempPositions[i].Z;

                    newPositions.Add(positions[i] + position + rotationPoint);
                }
                if (rotationAmounts.X < 0)
                {
                    positions[i].X = tempPositions[i].Y;
                    positions[i].Y = -tempPositions[i].X;
                    positions[i].Z = tempPositions[i].Z;

                    newPositions.Add(positions[i] + position + rotationPoint);
                }

                if (rotationAmounts.Y > 0)
                {
                    positions[i].X = -tempPositions[i].Z;
                    positions[i].Y = tempPositions[i].Y;
                    positions[i].Z = tempPositions[i].X;

                    newPositions.Add(positions[i] + position + rotationPoint);
                }
                if (rotationAmounts.Y < 0)
                {
                    positions[i].X = tempPositions[i].Z;
                    positions[i].Y = tempPositions[i].Y;
                    positions[i].Z = -tempPositions[i].X;

                    newPositions.Add(positions[i] + position + rotationPoint);
                }

                if (rotationAmounts.Z > 0)
                {
                    positions[i].X = tempPositions[i].X;
                    positions[i].Y = tempPositions[i].Z;
                    positions[i].Z = -tempPositions[i].Y;

                    newPositions.Add(positions[i] + position + rotationPoint);
                }
                if (rotationAmounts.Z < 0)
                {
                    positions[i].X = tempPositions[i].X;
                    positions[i].Y = -tempPositions[i].Z;
                    positions[i].Z = tempPositions[i].Y;

                    newPositions.Add(positions[i] + position + rotationPoint);
                }
            }

            return (positions, newPositions);
        }


        /// <summary>
        /// checks if a translational move is legal
        /// </summary>
        /// <param name="offsets"></param>
        /// <returns></returns>
        public int isMoveLegal(List<Vector3> originalPositions, List<Vector3> newPositions)
        {

            List<Vector3> newPositionsTemp = new List<Vector3>();
            foreach (Vector3 pos in newPositions)
            {
                newPositionsTemp.Add(new Vector3(pos.X, pos.Y, pos.Z));
            }

            foreach (Vector3 position in newPositionsTemp)
            {
                if (isVector3InList(position, originalPositions.ToArray()))
                {
                    newPositions.Remove(position);
                }
            }

            List<Vector3> allCubePositions = new List<Vector3>();
            foreach (Piece piece in board.pieceList)
            {
                //remove the currently being check piece's blocks
                if (ReferenceEquals(this, piece))
                {
                    continue;
                }

                //add the other cubs to the list for collision checks
                foreach (Cube cube in piece.cubes)
                {
                    allCubePositions.Add(cube.position);
                }
            }

            foreach (Vector3 newPosition in newPositions)
            {
                //outside of the gameboard
                if (newPosition.Y < 0)
                {
                    return -1;
                }
                if (newPosition.X < 0)
                {
                    return 0;
                }
                if (newPosition.X > board.width - 1)
                {
                    return 0;
                }
                if (newPosition.Z < 0)
                {
                    return 0;
                }
                if (newPosition.Z > board.depth - 1)
                {
                    return 0;
                }

                //hitting any other blocks
                if (isVector3Colliding(newPosition, allCubePositions.ToArray()))
                {
                    return 0;
                }
            }

            return 1;
        }

        private bool isVector3InList(Vector3 position, Vector3[] positions)
        {
            foreach (Vector3 pos in positions)
            {
                if (pos == position)
                {
                    return true;
                }
            }
            return false;
        }

        private bool isVector3Colliding(Vector3 position, Vector3[] testPositions)
        {
            foreach (Vector3 testPosition in testPositions)
            {
                if (testPosition.X - 1 < position.X && position.X < testPosition.X + 1)
                {
                    if (testPosition.Y - 1 < position.Y && position.Y < testPosition.Y + 1)
                    {
                        if (testPosition.Z - 1 < position.Z && position.Z < testPosition.Z + 1)
                        {
                            return true;
                        }
                    }
                }

            }
            return false;
        }

    }

}
