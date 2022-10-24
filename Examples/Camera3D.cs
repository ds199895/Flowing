using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Flowing
{
    class Camera3D
    {
        public enum Direction
        {
            Forward,
            Backward,
            Left,
            Right,
            Up,
            Down
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }

        public Vector3 LookDirection;
        public Vector3 RightDirection;
        public Vector3 UpDirection;
        public Camera3D(float x = 0.0f, float y = 0.0f, float z = 0.0f)
        {
            X = x;
            Y = y;
            Z = z;
            iniCoordinateSystem();
        }

        public void Update()
        {
            GL.Rotate(RotationX, 1, 0, 0);
            GL.Rotate(RotationY, 0, 1, 0);
            //GL.Rotate(RotationZ, 0, 0, 1);
            GL.Translate(-X, -Y, -Z);
            iniCoordinateSystem();

        }
        private void iniCoordinateSystem()
        {
            float dX = RotationX * (float)(Math.PI / 180);
            float dY = RotationY * (float)(Math.PI / 180);
            this.LookDirection = new Vector3((float)Math.Sin(dY), 0, (float)Math.Cos(dY));
            Vector3 zz = this.LookDirection;
            Vector3 planxz;
            if (zz.X == 0.0D && zz.Y == 0.0D)
            {
                planxz = Vector3.Cross(Vector3.UnitY, zz);
            }
            else
            {
                planxz = Vector3.Cross(Vector3.UnitZ, zz);
            }
            this.iniCoordinateSystem(zz, planxz);
        }

        private void iniCoordinateSystem(Vector3 z,Vector3 planxz)
        {
            this.RightDirection = planxz;
            this.UpDirection = Vector3.Cross(z, this.RightDirection).Normalized();
            this.RightDirection = Vector3.Cross(this.LookDirection, this.UpDirection).Normalized();


        }

        public void Move(Direction direction, float speed = 1.0f, bool flying = false)
        {
            float dX = RotationX * (float)(Math.PI / 180);
            float dY = RotationY * (float)(Math.PI / 180);

            switch (direction)
            {
                case Direction.Forward:
                    X += (float)Math.Sin(dY) * speed;
                    Z += -(float)Math.Cos(dY) * speed;
                    if (flying)
                        Y += -(float)Math.Sin(dX) * speed;
                    break;
                case Direction.Backward:
                    X += -(float)Math.Sin(dY) * speed;
                    Z += (float)Math.Cos(dY) * speed;
                    if (flying)
                        Y += (float)Math.Sin(dX) * speed;
                    break;
                case Direction.Left:
                    X += -(float)Math.Cos(dY) * speed;
                    Z += -(float)Math.Sin(dY) * speed;
                    break;
                case Direction.Right:
                    X += (float)Math.Cos(dY) * speed;
                    Z += (float)Math.Sin(dY) * speed;
                    break;
                case Direction.Up:
                    Y += speed;
                    break;
                case Direction.Down:
                    Y -= speed;
                    break;
            }
        }


        public void DrawGrid(System.Drawing.Color color, float X, float Y, int cell_size = 16, int grid_size = 256)
        {
            int dX = (int)Math.Round(X / cell_size) * cell_size;
            int dZ = (int)Math.Round(Z / cell_size) * cell_size;

            int ratio = grid_size / cell_size;

            GL.PushMatrix();

            GL.Translate(dX - grid_size / 2, dZ - grid_size / 2, 0);

            int i;

            GL.Color3(color);
            GL.Begin(PrimitiveType.Lines);

            for (i = 0; i < ratio + 1; i++)
            {
                int current = i * cell_size;

                GL.Vertex3(current, 0, 0);
                GL.Vertex3(current, grid_size,0);

                GL.Vertex3(0, current, 0);
                GL.Vertex3(grid_size, current, 0);
            }

            GL.End();

            GL.PopMatrix();
        }
    }
}
