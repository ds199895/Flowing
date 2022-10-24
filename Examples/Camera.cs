using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowing
{
    // This is the camera class as it could be set up after the tutorials on the website.
    // It is important to note there are a few ways you could have set up this camera.
    // For example, you could have also managed the player input inside the camera class,
    // and a lot of the properties could have been made into functions.

    // TL;DR: This is just one of many ways in which we could have set up the camera.
    // Check out the web version if you don't know why we are doing a specific thing or want to know more about the code.
    public class Camera
    {
        private IApp lastApp;
        public bool perspective;
        private double fovy;
        private double far;
        private double near;

        private double frustumNear;
        private double frustumFar;
        private double frustumBottom;
        private double frustumTop;
        private double frustumLeft;
        private double frustumRight;
        // Those vectors are directions pointing outwards from the camera to define how it rotated.
        private Vector3 _front = -Vector3.UnitZ;

        private Vector3 _up = Vector3.UnitY;

        private Vector3 _right = Vector3.UnitX;

        // Rotation around the X axis (radians)
        private float _pitch;

        // Rotation around the Y axis (radians)
        private float _yaw = -MathHelper.PiOver2; // Without this, you would be started rotated 90 degrees right.

        // The field of view of the camera (radians)
        private float _fov = MathHelper.PiOver2;
        public int w;
        public int h;
        public Vector3 target;
        public Camera(int w, int h, Vector3 position,Vector3 target)
        {
            iniCoordinateSystem(position, target);
            this.w = w;
            this.h = h;
            AspectRatio = this.w/this.h;
            this.fovy = 1.0471975511965976D;
            this.far = 1000.0D;
            this.near = 1000.0D;
            this.frustumNear = 1.0D;
            this.frustumFar = 2.0D;
            this.frustumLeft = -0.5D;
            this.frustumRight = 0.5D;
            this.frustumTop = 0.5D;
            this.frustumBottom = -0.5D;
            this.perspective = true;
        }
        private void iniCoordinateSystem(Vector3 position,Vector3 target)
        {
            Vector3 zz = target - position;
            Vector3 planxz;
            if (zz.X == 0.0D && zz.Y == 0.0D)
            {
                planxz = Vector3.Cross(Vector3.UnitY, zz);
            }
            else
            {
                planxz = Vector3.Cross(Vector3.UnitZ, zz);
            }
            this.iniCoordinateSystem(position,zz, planxz);
        }

        private void iniCoordinateSystem(Vector3 position,Vector3 z, Vector3 planxz)
        {
            this.Position = position;
            this._front = z.Normalized();
            this._right = planxz;
            this._up = Vector3.Cross(z, this._right).Normalized();
            this._right = Vector3.Cross(this._front, this._up).Normalized();
            if (this.target == null)
            {
                this.target = position + z;
            }

        }

        public void SetPerspective(bool perspective)
        {
            this.perspective = perspective;
        }
        // The position of the camera
        public Vector3 Position { get; set; }

        // This is simply the aspect ratio of the viewport, used for the projection matrix.
        public float AspectRatio { private get; set; }

        public Vector3 Front {
            get => _front;
            set
            {
                _front = value;
            }
        }

        public Vector3 Up { 
            get => _up;
            set
            {
                _up = value;
            }
        }

        public Vector3 Right
        { 
            get => _right;
            set
            {
                _right=value;
            }
        }

        // We convert from degrees to radians as soon as the property is set to improve performance.
        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                // We clamp the pitch value between -89 and 89 to prevent the camera from going upside down, and a bunch
                // of weird "bugs" when you are using euler angles for rotation.
                // If you want to read more about this you can try researching a topic called gimbal lock
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        // We convert from degrees to radians as soon as the property is set to improve performance.
        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        // The field of view (FOV) is the vertical angle of the camera view.
        // This has been discussed more in depth in a previous tutorial,
        // but in this tutorial, you have also learned how we can use this to simulate a zoom feature.
        // We convert from degrees to radians as soon as the property is set to improve performance.
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }

        // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
        public Matrix4 GetViewMatrix()
        {
            //return Matrix4.LookAt(Position, Position+_front, _up);
            return Matrix4.LookAt(Position, target, _up);
        }

        // Get the projection matrix using the same method we have used up until this point
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 1000f);
        }

        // This function is going to update the direction vertices using some of the math learned in the web tutorials.
        private void UpdateVectors()
        {
            // First, the front matrix is calculated using some basic trigonometry.
            _front.X = (float)(Math.Cos(_pitch) * Math.Cos(_yaw));
            _front.Y = (float)Math.Sin(_pitch);
            _front.Z = (float)(Math.Cos(_pitch) * Math.Sin(_yaw));

            // We need to make sure the vectors are all normalized, as otherwise we would get some funky results.
            _front = Vector3.Normalize(_front);

            // Calculate both the right and the up vector using cross product.
            // Note that we are calculating the right from the global up; this behaviour might
            // not be what you need for all cameras so keep this in mind if you do not want a FPS camera.
            _right = Vector3.Normalize(Vector3.Cross(_front, _up));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }
        public void MoveIn(double dist)
        {
            this.Position+=(float)dist*(this.target-this.Position).Normalized();
            //this.updateViewMatrix();
            //if (!this.perspective)
            //{
            //    this.updateProjectionMatrix();
            //}

        }
        
        public void ApplyCam(IApp app)
        {
            this.lastApp=app;
            getFrustumPerspective();
            if (this.perspective)
            {
                //GL.Frustum((float)this.frustumLeft, (float)this.frustumRight, (float)this.frustumBottom, (float)this.frustumTop, (float)this.frustumNear, (float)this.frustumFar);
            }
            else
            {
                //GL.Translate(Position.X, Position.Y, Position.Z);
                //GL.Ortho(0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 100.0f);
                GL.MatrixMode(MatrixMode.Projection);
                float widthHalf = (float)this.lastApp.window.Width / 2.0F;
                float heightHalf = (float)this.lastApp.window.Height / 2.0F;
                //Console.WriteLine(this.frustumLeft);
                //GL.Ortho((float)this.frustumLeft + widthHalf, (float)this.frustumRight + widthHalf, (float)this.frustumBottom + heightHalf, (float)this.frustumTop + heightHalf, -(float)this.frustumFar, (float)this.frustumFar);
                //GL.Ortho(-w + widthHalf, (float)this.frustumRight + widthHalf, -h + heightHalf, (float)this.frustumTop + heightHalf, -(float)this.frustumFar, (float)this.frustumFar);
                //Matrix4 ortho = Matrix4.CreateOrthographicOffCenter((float)this.frustumLeft + widthHalf, (float)this.frustumRight + widthHalf, (float)this.frustumBottom + heightHalf, (float)this.frustumTop + heightHalf, -(float)this.frustumFar, (float)this.frustumFar);
                Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(-w + widthHalf, (float)this.frustumRight + widthHalf, -h + heightHalf, (float)this.frustumTop + heightHalf, -(float)this.frustumFar, (float)this.frustumFar);
                 GL.LoadMatrix(ref ortho);
                //GL.Ortho((float)this.frustumLeft + widthHalf, (float)this.frustumRight + widthHalf, (float)this.frustumBottom + heightHalf, (float)this.frustumTop + heightHalf, (float)(-this.frustumFar), (float)this.frustumFar);
                //GL.MatrixMode(MatrixMode.Modelview);
            }
        }

        public void Update(IApp app)
        {
            iniCoordinateSystem(this.Position, this.target);
            this.lastApp = app;
            getFrustumPerspective();
            if (this.perspective)
            {
                Matrix4 projection = this.GetProjectionMatrix();
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref projection);
                //GL.Frustum((float)this.frustumLeft, (float)this.frustumRight, (float)this.frustumBottom, (float)this.frustumTop, (float)this.frustumNear, (float)this.frustumFar);
            }
            else
            {
                //GL.Translate(Position.X, Position.Y, Position.Z);
                //GL.Ortho(0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 100.0f);
                GL.MatrixMode(MatrixMode.Projection);
                float widthHalf = (float)this.lastApp.window.Width / 2.0F;
                float heightHalf = (float)this.lastApp.window.Height / 2.0F;
                //Console.WriteLine(this.frustumLeft);
                //GL.Ortho((float)this.frustumLeft + widthHalf, (float)this.frustumRight + widthHalf, (float)this.frustumBottom + heightHalf, (float)this.frustumTop + heightHalf, -(float)this.frustumFar, (float)this.frustumFar);
                //Matrix4 ortho = Matrix4.CreateOrthographicOffCenter((float)this.frustumLeft + widthHalf, (float)this.frustumRight + widthHalf, (float)this.frustumBottom + heightHalf, (float)this.frustumTop + heightHalf, -(float)this.frustumFar, (float)this.frustumFar);
                Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(-w + widthHalf, (float)this.frustumRight + widthHalf, -h + heightHalf, (float)this.frustumTop + heightHalf, -(float)this.frustumFar, (float)this.frustumFar);

                GL.LoadMatrix(ref ortho);
                //GL.Ortho((float)this.frustumLeft + widthHalf, (float)this.frustumRight + widthHalf, (float)this.frustumBottom + heightHalf, (float)this.frustumTop + heightHalf, (float)(-this.frustumFar), (float)this.frustumFar);
                //GL.MatrixMode(MatrixMode.Modelview);
            }
            
            Matrix4 lookat = this.GetViewMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

        }

        private void getFrustumPerspective()
        {
            double aspect = this.w / this.h;
            double halfHeight = this.h / 2.0D;
            double tan = Math.Tan(this.fovy / 2.0D);
            double cameraZ = halfHeight / tan;
            double zNear = cameraZ / this.near;
            double zFar = cameraZ * this.far;
            double h = halfHeight / this.near;
            double w = h * aspect;
            this.frustumLeft = -w;
            this.frustumRight = w;
            this.frustumBottom = -h;
            this.frustumTop = h;
            this.frustumNear = zNear;
            this.frustumFar = zFar;
            
            if (!this.perspective)
            {
                double dist = (this.target - this.Position).Length;
                this.frustumLeft *= dist;
                this.frustumRight *= dist;
                this.frustumBottom *= dist;
                this.frustumTop *= dist;
            }

        }
    }
}
