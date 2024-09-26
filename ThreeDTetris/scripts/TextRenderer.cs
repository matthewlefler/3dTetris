using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using CameraClass;
using System.Security.Principal;

namespace TextRenderer
{
    class Letter
    {
        float scale = 1f;

        public Vector3[] vertices;
        VertexPositionColorNormalTexture[] quads;
        List<VertexPositionColorNormalTexture> vertList = new();

        private int[,] whereQuads;

        VertexBuffer vertexBuffer;

        private int primativeCount;

        public Letter(VertexPositionColorNormalTexture[] quads, int[,] whereQuads)
        {
            this.quads = quads;
            this.whereQuads = whereQuads;

            primativeCount = 6*(quads.Length/4);

            for (int i = 0; i < quads.Length; i += 4)
            {
                vertList.Add(quads[i + 2]);
                vertList.Add(quads[i + 1]);
                vertList.Add(quads[i + 0]);
                vertList.Add(quads[i + 1]);
                vertList.Add(quads[i + 2]);
                vertList.Add(quads[i + 3]);
            }
        }

        public void draw(GraphicsDevice graphicsDevice, BasicEffect effect)
        {
            if (primativeCount == 0)
            {
                return;
            }
            vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionColorNormalTexture.VertexDeclaration, primativeCount, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertList.ToArray());

            graphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, primativeCount);
            }
        }
    }

    public class FontInterpreter
    {
        private List<char> charList = new List<char>();

        private SpriteFont font;
        private Texture2D fontTexture;
        private Dictionary<char, SpriteFont.Glyph> glyphs;
        private Dictionary<char, Letter> letters;

        public BasicEffect effect;
        private OrbitCamera camera;

        GraphicsDevice _graphicsDevice;
        Rectangle windowSize;

        public FontInterpreter(SpriteFont font, GraphicsDevice _graphicsDevice, Rectangle window, BasicEffect effect, OrbitCamera camera)
        {
            this._graphicsDevice = _graphicsDevice;
            this.windowSize = window;
            this.effect = effect;
            this.camera = camera;
            this.font = font;
            this.fontTexture = font.Texture;

            this.glyphs = font.GetGlyphs();

            foreach (var item in font.GetGlyphs())
            {
                charList.Add(item.Value.Character);
            }

            calculateAllLetters();
        }

        private void calculateAllLetters()
        {
            letters = new Dictionary<char, Letter>();
            Color[] colors = new Color[fontTexture.Width * fontTexture.Height];

            fontTexture.GetData(colors);

            //used to get position in colors array
            int width = fontTexture.Width;
            int height = fontTexture.Height; // 128 by 108

            int[,] data = new int[width, height];

            foreach (char charactor in charList)
            {
                //position starts in the top left corner
                Rectangle bounds = glyphs[charactor].BoundsInTexture;

                List<VertexPositionColorNormalTexture> quads = new List<VertexPositionColorNormalTexture>();

                for (int x = bounds.X; x < bounds.X + bounds.Width; x++)
                {
                    for (int y = bounds.Y; y < bounds.Y + bounds.Height; y++)
                    {
                        if ((colors[y * width + x].R + colors[y * width + x].G + colors[y * width + x].B) / 3 > 0.3f)
                        {
                            foreach (VertexPositionColorNormalTexture quad in createQuad(new Vector3(x - bounds.X, -1 * (y - bounds.Y), 0)))
                            {
                                quads.Add(quad);
                            }

                            data[x, y] = x * width + y;
                        }
                        else
                        {
                            data[x, y] = 0;
                        }
                    }
                }
                letters.Add(charactor, new Letter(quads.ToArray(), data));
            }
        }

        private VertexPositionColorNormalTexture[] createQuad(Vector3 position)
        {
            VertexPositionColorNormalTexture[] points = new VertexPositionColorNormalTexture[4];

            points[0] = new VertexPositionColorNormalTexture(position + new Vector3(0, 1, 0), Color.White, Vector3.Down, new Vector2(0, 1));
            points[1] = new VertexPositionColorNormalTexture(position + new Vector3(0, 0, 0), Color.White, Vector3.Down, new Vector2(0, 0));
            points[2] = new VertexPositionColorNormalTexture(position + new Vector3(1, 1, 0), Color.White, Vector3.Down, new Vector2(1, 1));
            points[3] = new VertexPositionColorNormalTexture(position + new Vector3(1, 0, 0), Color.White, Vector3.Down, new Vector2(1, 0));

            return points;
        }

        public void drawStringRelativeToCamera(string text, Vector2 position, float distanceFromCamera, int charactersPerLine, float scale, Vector3 rotation, Vector3 color)
        {
            char[] chars = text.ToCharArray();
            int x = 0, y = 0;

            float xOffset = 0f;

            int textwidth = 26;
            int textheight = 24;

            if(chars.Length == 1)
            {
                foreach (char c in chars)
                {
                    xOffset += glyphs[c].BoundsInTexture.Width;
                }
                xOffset /= 2;
            }
            else 
            {
                if(chars.Length <= charactersPerLine)
                {
                    xOffset = chars.Length * textwidth / 2f;
                }
            }
            if(chars.Length > charactersPerLine)
            {
                xOffset = charactersPerLine * textwidth / 2f;
            }


            this.effect.EmissiveColor = color;

            this.effect.Projection = camera.projectionMatrix;
            this.effect.View = camera.viewMatrix;

            for (int i = 0; i < chars.Length; i++)
            {
                if(x >= charactersPerLine)
                {
                    x = 0; 
                    y -= 1;
                }
                
                float characterWidth = glyphs[chars[i]].BoundsInTexture.Width;
                float characterHeight = glyphs[chars[i]].BoundsInTexture.Height;

                this.effect.World =
                    Matrix.CreateTranslation(new Vector3(-characterWidth/2f, textheight/2f, 0))
                    * Matrix.CreateScale(scale)
                    * Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z)
                    * Matrix.CreateTranslation(new Vector3((x * textwidth * scale) - (xOffset * scale) + position.X, y * textheight * scale + position.Y, 0))
                    * Matrix.CreateTranslation(new Vector3(characterWidth / 2f * scale, -textheight/2f * scale, 0))
                    * Matrix.CreateFromYawPitchRoll(MathF.PI / 2f - camera.orbitRotations.X, 2f * MathF.PI - camera.orbitRotations.Y, 0f)
                    * Matrix.CreateTranslation(camera.position - (scale * text.Length * 80f * Vector3.Normalize(camera.position)));

                letters[chars[i]].draw(_graphicsDevice, this.effect);

                x++;
            }
        }
        public void drawStringInWorld(string text, Vector3 position, int charactersPerLine, float scale, Vector3 rotation, Vector3 color)
        {
            char[] chars = text.ToCharArray();
            int x = 0, y = 0;

            float xOffset = 0f;

            int textwidth = 26;
            int textheight = 24;

            if (chars.Length == 1)
            {
                foreach (char c in chars)
                {
                    xOffset += glyphs[c].BoundsInTexture.Width;
                }
                xOffset /= 2;
            }
            else
            {
                if (chars.Length <= charactersPerLine)
                {
                    xOffset = chars.Length * textwidth / 2f;
                }
            }
            if (chars.Length > charactersPerLine)
            {
                xOffset = charactersPerLine * textwidth / 2f;
            }


            this.effect.EmissiveColor = color;

            this.effect.Projection = camera.projectionMatrix;
            this.effect.View = camera.viewMatrix;

            for (int i = 0; i < chars.Length; i++)
            {
                if (x >= charactersPerLine)
                {
                    x = 0;
                    y -= 1;
                }

                float characterWidth = glyphs[chars[i]].BoundsInTexture.Width;
                float characterHeight = glyphs[chars[i]].BoundsInTexture.Height;

                this.effect.World =
                    Matrix.CreateTranslation(new Vector3(-characterWidth / 2f, -characterHeight / 2f, 0))
                    * Matrix.CreateScale(scale)
                    * Matrix.CreateTranslation(new Vector3((x * textwidth * scale) - (xOffset * scale), y * textheight * scale, 0))
                    * Matrix.CreateTranslation(new Vector3(characterWidth / 2f * scale, characterHeight / 2f * scale, 0))
                    * Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z)
                    * Matrix.CreateFromYawPitchRoll(MathF.PI / 2f - camera.orbitRotations.X, 2f * MathF.PI - camera.orbitRotations.Y, 0f)
                    * Matrix.CreateTranslation(position);

                letters[chars[i]].draw(_graphicsDevice, this.effect);

                x++;
            }
        }

        public void menuDrawStringInWorld(string text, Vector3 position, int charactersPerLine, float scale, Vector3 rotation, Vector3 color, int longestText)
        {
            char[] chars = text.ToCharArray();
            int x = 0, y = 0;

            float xOffset = 0f;

            int textWidthSpacing = 26;
            int textHeightSpacing = 24;

            if (chars.Length == 1)
            {
                foreach (char c in chars)
                {
                    xOffset += glyphs[c].BoundsInTexture.Width;
                }
                xOffset /= 2;
            }
            else
            {
                if (chars.Length <= charactersPerLine)
                {
                    xOffset = chars.Length * textWidthSpacing / 2f;
                }
            }
            if (chars.Length > charactersPerLine)
            {
                xOffset = charactersPerLine * textWidthSpacing / 2f;
            }


            this.effect.EmissiveColor = color;

            this.effect.Projection = camera.projectionMatrix;
            this.effect.View = camera.viewMatrix;

            for (int i = 0; i < chars.Length; i++)
            {
                if (x >= charactersPerLine)
                {
                    x = 0;
                    y -= 1;
                }

                float characterWidth = glyphs[chars[i]].BoundsInTexture.Width;
                float characterHeight = glyphs[chars[i]].BoundsInTexture.Height;

                this.effect.World = 
                    Matrix.CreateTranslation(new Vector3(-characterWidth / 2f, -characterHeight / 2f, 0))
                    * Matrix.CreateScale(scale)
                    * Matrix.CreateTranslation(new Vector3((x * textWidthSpacing * scale) - (xOffset * scale), y * textHeightSpacing * scale, 0))
                    * Matrix.CreateTranslation(new Vector3(characterWidth / 2f * scale, characterHeight / 2f * scale, 0))
                    * Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z)
                    * Matrix.CreateFromYawPitchRoll(MathF.PI / 2f - camera.orbitRotations.X, 2f * MathF.PI - camera.orbitRotations.Y, 0f)
                    * Matrix.CreateTranslation(scale * position - (scale * longestText * 80f * Vector3.Normalize(camera.position)));

                letters[chars[i]].draw(_graphicsDevice, this.effect);

                x++;
            }
        }
    }


}