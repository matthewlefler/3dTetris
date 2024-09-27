using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SimpleAnimation;
using System.Diagnostics;

namespace CameraClass
{
    /// <summary>
    /// returns an orbit camera object
    /// </summary>
    public class OrbitCamera
    {
        public Vector3 position { get; private set; }

        private Vector2 _orbitRotations;
        public Vector2 orbitRotations { get { return _orbitRotations; } private set { } }

        public Vector3 rotation { get; set; }

        public float speed = 2f;

        public float _finalDistanceFromMiddle;
        private float _distanceFromMiddle = 35f;
        public float cameraHeight { get; private set; }
        public float distanceFromMiddle { set { absoluteMove(_orbitRotations.X, _orbitRotations.Y); _distanceFromMiddle = value; } get { return _distanceFromMiddle; } }
        public AnimationFloat distanceAnimation;

        public Matrix viewMatrix { get { return Matrix.CreateLookAt(position, new Vector3(0, 0, 0), Vector3.UnitY); } private set { } }
        public Matrix projectionMatrix;

        public OrbitCamera(Vector2 orbitRotations, GraphicsDeviceManager graphics, float cameraHeight)
        {
            this.cameraHeight = cameraHeight;
            move(orbitRotations.X, orbitRotations.Y);
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathF.PI / 4f, (float)graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight, 0.01f, 10000f);
            //projectionMatrix = Matrix.CreateOrthographic(30 * graphicsDevice.Viewport.AspectRatio, 30, 0.01f, 100f);

            int shortestWindowLength = graphics.PreferredBackBufferWidth;
            if(graphics.PreferredBackBufferWidth > graphics.PreferredBackBufferHeight)
            {
                shortestWindowLength = graphics.PreferredBackBufferHeight;
            }

            _finalDistanceFromMiddle = 1f;
        }

        public void move(float x, float y)
        {
            _orbitRotations += new Vector2(x, y);
            float maxY = 1.4f;
            float minY = -1.2f;
            if (_orbitRotations.Y > maxY)
            {
                _orbitRotations.Y = maxY;
            }
            if (_orbitRotations.Y < minY)
            {
                _orbitRotations.Y = minY;
            }

            if (_orbitRotations.X > MathF.PI * 2)
            {
                _orbitRotations.X -= MathF.PI * 2;
            }
            if (_orbitRotations.X < 0)
            {
                _orbitRotations.X += MathF.PI * 2;
            }

            float newYPos = MathF.Sin(_orbitRotations.Y) * _distanceFromMiddle;
            float newXPos = MathF.Cos(_orbitRotations.X) * MathF.Cos(_orbitRotations.Y) * _distanceFromMiddle;
            float newZPos = MathF.Sin(_orbitRotations.X) * MathF.Cos(_orbitRotations.Y) * _distanceFromMiddle;

            position = new Vector3(newXPos, newYPos, newZPos);
        }

        public void absoluteMove(float x, float y)
        {
            _orbitRotations = new Vector2(x, y);
            float maxY = 1.4f;
            float minY = -1.2f;
            if (_orbitRotations.Y > maxY)
            {
                _orbitRotations.Y = maxY;
            }
            if (_orbitRotations.Y < minY)
            {
                _orbitRotations.Y = minY;
            }

            if (_orbitRotations.X > MathF.PI * 2)
            {
                _orbitRotations.X -= MathF.PI * 2;
            }
            if (_orbitRotations.X < 0)
            {
                _orbitRotations.X += MathF.PI * 2;
            }

            float newYPos = MathF.Sin(_orbitRotations.Y) * _distanceFromMiddle;
            float newXPos = MathF.Cos(_orbitRotations.X) * MathF.Cos(_orbitRotations.Y) * _distanceFromMiddle;
            float newZPos = MathF.Sin(_orbitRotations.X) * MathF.Cos(_orbitRotations.Y) * _distanceFromMiddle;

            position = new Vector3(newXPos, newYPos, newZPos);
        }
    }


}
