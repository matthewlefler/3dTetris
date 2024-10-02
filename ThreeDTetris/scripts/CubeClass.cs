using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeClass
{
    /// <summary>
    /// Cube object with draw and move functions
    /// </summary>
    public class Cube
    {
        public Vector3 position;
        public Vector3 rotation;
        public Color color;

        private Vector3 bottomLeftPosition;

        private GraphicsDevice _graphicsDevice;

        private float _scale;
        public float scale { get { return _scale; } set { _scale = value; recalcCubeVertices(); } }

        //controls for exact face sizes, goes in order of front:0, back:1, top:2, bottom:3, left:4, right:5
        public float[] faceSizes;

        //if facing the front side it goes: top to bottom left to right front to back
        public Vector3[] vertices = new Vector3[8];
        VertexPositionColorNormalTexture[,] quads = new VertexPositionColorNormalTexture[6, 6];
        VertexPositionColorNormalTexture[] colorVertList;

        readonly static int[,] cubeVertOrder = new int[6, 4] {
                { 0, 4, 1, 5 },
                { 2, 6, 3, 7 },
                { 0, 1, 3, 2 },
                { 4, 7, 5, 6 },
                { 3, 7, 0, 4 },
                { 1, 5, 2, 6 }
        };

        public Matrix worldMatrix
        {
            get
            {
                return
                    Matrix.CreateTranslation(position + bottomLeftPosition + new Vector3(scale / 2f, scale / 2f, scale / 2f))
                    * Matrix.CreateRotationX(rotation.X)
                    * Matrix.CreateRotationY(rotation.Y)
                    * Matrix.CreateRotationZ(rotation.Z);
            }
            private set { }
        }

        public Cube(Vector3 position, Color color, float scale, GraphicsDevice graphicsDevice, Vector3 bottomLeftPosition)
        {
            this.position = position;
            faceSizes = new float[6] { 1f, 1f, 1f, 1f, 1f, 1f };
            this.scale = scale;
            this.color = color;

            this.bottomLeftPosition = bottomLeftPosition;

            recalcCubeVertices();
            _graphicsDevice = graphicsDevice;
        }

        public void recalcCubeVertices()
        {
            vertices[0] = new Vector3((faceSizes[0] + faceSizes[2]) / 4f, (faceSizes[0] + faceSizes[4]) / 4f, (faceSizes[2] + faceSizes[4]) / 4f) * scale;
            vertices[1] = new Vector3((faceSizes[0] + faceSizes[2]) / -4f, (faceSizes[0] + faceSizes[5]) / 4f, (faceSizes[2] + faceSizes[5]) / 4f) * scale;

            vertices[2] = new Vector3((faceSizes[1] + faceSizes[2]) / -4f, (faceSizes[5] + faceSizes[1]) / 4f, (faceSizes[2] + faceSizes[5]) / -4f) * scale;
            vertices[3] = new Vector3((faceSizes[1] + faceSizes[2]) / 4f, (faceSizes[4] + faceSizes[1]) / 4f, (faceSizes[2] + faceSizes[4]) / -4f) * scale;

            vertices[4] = new Vector3((faceSizes[0] + faceSizes[3]) / 4f, (faceSizes[0] + faceSizes[4]) / -4f, (faceSizes[3] + faceSizes[4]) / 4f) * scale;
            vertices[5] = new Vector3((faceSizes[0] + faceSizes[3]) / -4f, (faceSizes[0] + faceSizes[5]) / -4f, (faceSizes[3] + faceSizes[5]) / 4f) * scale;

            vertices[6] = new Vector3((faceSizes[1] + faceSizes[3]) / -4f, (faceSizes[5] + faceSizes[1]) / -4f, (faceSizes[3] + faceSizes[5]) / -4f) * scale;
            vertices[7] = new Vector3((faceSizes[1] + faceSizes[3]) / 4f, (faceSizes[4] + faceSizes[1]) / -4f, (faceSizes[3] + faceSizes[4]) / -4f) * scale;



            for (int i = 0; i < 6; i++)
            {
                Vector3 normal = Vector3.Cross(vertices[cubeVertOrder[i, 1]] - vertices[cubeVertOrder[i, 2]], vertices[cubeVertOrder[i, 0]] - vertices[cubeVertOrder[i, 1]]);

                quads[i, 0] = new VertexPositionColorNormalTexture(vertices[cubeVertOrder[i, 0]], color, normal, new Vector2(0f, 1f));
                quads[i, 1] = new VertexPositionColorNormalTexture(vertices[cubeVertOrder[i, 1]], color, normal, new Vector2(1f, 1f));
                quads[i, 2] = new VertexPositionColorNormalTexture(vertices[cubeVertOrder[i, 2]], color, normal, new Vector2(0f, 0f));
                quads[i, 3] = new VertexPositionColorNormalTexture(vertices[cubeVertOrder[i, 1]], color, normal, new Vector2(1f, 1f));
                quads[i, 4] = new VertexPositionColorNormalTexture(vertices[cubeVertOrder[i, 3]], color, normal, new Vector2(1f, 0f));
                quads[i, 5] = new VertexPositionColorNormalTexture(vertices[cubeVertOrder[i, 2]], color, normal, new Vector2(0f, 0f));
            }
            colorVertList = new VertexPositionColorNormalTexture[quads.Length];
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    colorVertList[i * 6 + j] = quads[i, j];
                }
            }
        }

        public void draw(Effect effect)
        {
            VertexBuffer vertexBuffer = new VertexBuffer(_graphicsDevice, VertexPositionColorNormalTexture.VertexDeclaration, colorVertList.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(colorVertList);
            _graphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, colorVertList.Length / 2);
            }

        }

        /// <summary>
        /// Moves the cube translationaly by the 3 axes floats in the Vector3 offsets.
        /// </summary>
        /// <param name="offsets"></param>
        public void move(Vector3 offsets)
        {
            position += offsets;
        }
    }

}
