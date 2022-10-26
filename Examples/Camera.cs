using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

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

        private float frustumNear;
        private float frustumFar;
        private float frustumBottom;
        private float frustumTop;
        private float frustumLeft;
        private float frustumRight;

        private bool leftHandSystem;
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

        private bool updateViewMatrix;
        private bool updateProjectionMatrix;
        private bool updateViewProjectionMatrix;

        private Matrix4 viewMatrix;
        private Matrix4 projectionMatrix;
        public bool is2D;

        public Camera(int w, int h, Vector3 position,Vector3 target)
        {
            iniCoordinateSystem(position, target);
            this.w = w;
            this.h = h;
            AspectRatio = this.w/this.h;
            this.fovy = 1.0471975511965976D;
            this.far = 1000.0D;
            this.near = 1000.0D;
            this.frustumNear = 1.0f;
            this.frustumFar = 2.0f;
            this.frustumLeft = -0.5f;
            this.frustumRight = 0.5f;
            this.frustumTop = 0.5f;
            this.frustumBottom = -0.5f;
            this.perspective = true;
            this.updateViewMatrix = true;
            this.updateProjectionMatrix = true;
            this.updateViewProjectionMatrix = true;
            this.leftHandSystem = false;
            this.is2D = false;
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
            this.UpdateViewMatrix();
        }

        public void Set2DProperties()
        {
            this.is2D = true;
            this.perspective = false;
        }
        public void Set3DProperties(bool perspective)
        {
            this.perspective = perspective;
            this.is2D = false;
        }
        public void Set2D(bool is2D)
        {
            this.is2D = is2D;
        }
        public void SetPerspective(bool perspective)
        {
            if (!this.is2D)
            {
                this.perspective = perspective;
            }
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
                this.UpdateViewMatrix();
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
                this.UpdateViewMatrix();
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
        public void GetViewMatrix()
        {

            //this.viewMatrix= Matrix4.LookAt(Position, target, _up);
            this.viewMatrix = Matrix4.LookAt(Position, Position+_front, _up);

        }

        // Get the projection matrix using the same method we have used up until this point
        public void GetProjectionMatrix()
        {
            this.getFrustumPerspective();
            this.projectionMatrix= Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100000f);
            //this.projectionMatrix=Matrix4.CreatePerspectiveOffCenter(this.frustumLeft, this.frustumRight, this.frustumBottom, this.frustumTop, this.frustumNear, this.frustumFar);
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
            //target = Position + _front;
            // Calculate both the right and the up vector using cross product.
            // Note that we are calculating the right from the global up; this behaviour might
            // not be what you need for all cameras so keep this in mind if you do not want a FPS camera.
            _right = Vector3.Normalize(Vector3.Cross(_front, _up));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }
        public void MoveIn(double dist)
        {
            Vector3 lookDirection = (this.target - this.Position).Normalized();

            this.Position+=(float)dist*lookDirection;
            //Console.WriteLine("position: "+this.Position);
            //Console.WriteLine("target: "+this.target);
            //Console.WriteLine("lookDirection: "+lookDirection);
            //this.target+= (float)dist * lookDirection;
            this.UpdateViewMatrix();
            if (!this.perspective)
            {
                this.UpdateProjectionMatrix();
            }

        }

        public void Pan(double dx, double dy, bool onXZ,double panDeltaFactor)
        {
            double DistToOrign = (this.target-this.Position).Length;
            if (onXZ)
            {

                dx *= -panDeltaFactor * DistToOrign;
                dy *= panDeltaFactor * DistToOrign;
                this.Position = new Vector3(this.Position.X + (float)dx, this.Position.Y + (float)dy, this.Position.Z);

            }
            else
            {

                dx *= -panDeltaFactor * DistToOrign;
                dy *= panDeltaFactor * DistToOrign;

                Vector3 movex = (float)dx * this.Right;
                Vector3 movey = (float)dy * this.Up;

                Vector3 move = movex + movey;
                //move.Z = 0.0F;
                this.Position += move;
                this.target += move;
            }
            this.UpdateViewMatrix();
        }
        public void RotateAroundLookAt(float deltaX, float deltaY, double rotateDeltaFactor)
        {
            bool noRotation = true;
            int dir;
            if (!is2D)
            {
                noRotation = false;
            }

            if (!noRotation && this.target != null)
            {
                dir = this.leftHandSystem ? 1 : -1;
                double ang1 = (double)(dir * deltaX) * rotateDeltaFactor;
                double ang2 = (double)-deltaY * rotateDeltaFactor;
                RotateLookAtHorizontal(ang1);
                RotateLookAtVertical(ang2);
                //Console.WriteLine("deltaX: " + deltaX);
                //Console.WriteLine("deltaY: " + deltaY);
                //Console.WriteLine("angle1: " + ang1);
                //Console.WriteLine("angle2: " + ang2);
            }
            
            //Console.WriteLine("target: " + this.target);


            //Console.WriteLine("*******************************");
           
        }
        public void RotateLookAtHorizontal(double angle)
        {
            if (this.target != null)
            {
                this.Rotate(this.target, new Vector3(0.0F,0.0F,1.0F), angle);
            }
        }
        public void RotateLookAtVertical(double angle)
        {
            if (this.target != null)
            {
                //this.Rotate(this.target,new Vector3(1.0f,1.0f,0.0f), angle);
                this.Rotate(this.target, this.Right, angle);
            }
        }

        public void Rotate(Vector3 center, Vector3 axis, double angle)
        {
            this.Position = Rot(this.Position, center, axis, angle);
            this.target = Rot(this.target, center, axis, angle);
            this._up = Rot(this._up, axis, angle);
            this._front = Rot(this._front, axis, angle);
            this._right = Rot(this._right, axis, angle);
            
            this.UpdateViewMatrix();
        }
        public Vector3 Rot(Vector3 based,double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            double xt = based.X;
            based.X = (float)(cos * xt - sin * based.Y);
            based.Y = (float)(sin * xt + cos * based.Y);
            return based;
        }
        public Vector3 Rot(Vector3 based,Vector3 center, Vector3 axis, double angle)
        {
            return center == based ? based : (Rot(based-center,axis,angle)+center);
        }

        public Vector3 Rot(Vector3 based,Vector3 axis, double angle)
        {
            if (axis == null)
            {
                return this.Rot(based,angle);
            }
            else
            {
                double[,] mat = new double[3,3];
                Vector3 ax = axis.Normalized();
                double sin = Math.Sin(angle);
                double cos = Math.Cos(angle);
                double icos = 1.0D - cos;
                mat[0,0] = ax.X * ax.X * icos + cos;
                mat[0,1] = ax.X * ax.Y * icos - ax.Z * sin;
                mat[0,2] = ax.X * ax.Z * icos + ax.Y * sin;
                mat[1,0] = ax.Y * ax.X * icos + ax.Z * sin;
                mat[1,1] = ax.Y * ax.Y * icos + cos;
                mat[1,2] = ax.Y * ax.Z * icos - ax.X * sin;
                mat[2,0] = ax.Z * ax.X * icos - ax.Y * sin;
                mat[2,1] = ax.Z * ax.Y * icos + ax.X* sin;
                mat[2,2] = ax.Z * ax.Z* icos + cos;
                double xt = based.X;
                double yt = based.Y;
                based.X = (float)(mat[0,0] * xt + mat[0,1] * yt + mat[0,2] * based.Z);
                based.Y = (float)(mat[1,0] * xt + mat[1,1] * yt + mat[1,2] * based.Z);
                based.Z = (float)(mat[2,0] * xt + mat[2,1] * yt + mat[2,2] * based.Z);
                return based;
            }
        }

        public void VerticalTransform(bool upDown, double angleDeltaFactor)
        {
            Vector3D position = new Vector3D(this.Position.X, this.Position.Y, this.Position.Z);
            Vector3D upDirection = new Vector3D(this._up.X, this._up.Y, this._up.Z);
            Vector3D lookDirection = new Vector3D(this._front.X, this._front.Y, this._front.Z);
            Vector3D rotateAxis = Vector3D.CrossProduct(position, upDirection);

            AxisAngleRotation3D rotate = new AxisAngleRotation3D(rotateAxis, angleDeltaFactor * (upDown ? -1 : 1));

            RotateTransform3D rt3d = new RotateTransform3D(rotate);
            Matrix3D matrix = rt3d.Value;
            Vector3D newPostition = matrix.Transform(position);
            //Console.WriteLine(newPostition);
            this.Position = new Vector3((float)newPostition.X, (float)newPostition.Y, (float)newPostition.Z);
            Vector3D lookDir = matrix.Transform(lookDirection);
            this._front = new Vector3((float)lookDir.X, (float)lookDir.Y, (float)lookDir.Z);


            //update the up direction
            Vector3 newUpDirection = Vector3.Cross(this.Front, new Vector3((float)rotateAxis.X, (float)rotateAxis.Y, (float)rotateAxis.Z));
            newUpDirection.Normalize();
            this.Up = newUpDirection;

            this.UpdateViewMatrix();
        }

        public void HorizontalTransform(bool leftRight, double angleDeltaFactor)
        {
            Vector3D position = new Vector3D(this.Position.X, this.Position.Y, this.Position.Z);
            Vector3D upDirection = new Vector3D(this._up.X, this._up.Y, this._up.Z);
            Vector3D lookDirection = new Vector3D(this._front.X, this._front.Y, this._front.Z);
            Vector3D rotateAxis = upDirection;

            AxisAngleRotation3D rotate = new AxisAngleRotation3D(rotateAxis, angleDeltaFactor * (leftRight ? -1 : 1));

            RotateTransform3D rt3d = new RotateTransform3D(rotate);
            Matrix3D matrix = rt3d.Value;
            Vector3D newPostition = matrix.Transform(position);
            this.Position = new Vector3((float)newPostition.X, (float)newPostition.Y, (float)newPostition.Z);
            Vector3D lookDir = matrix.Transform(lookDirection);
            this._front = new Vector3((float)lookDir.X, (float)lookDir.Y, (float)lookDir.Z);
            this.UpdateViewMatrix();
        }

        private void checkMatrixUpdate()
        {
            if (this.updateViewMatrix)
            {
                this.GetViewMatrix();
                this.updateViewMatrix = false;
            }

            if (this.updateProjectionMatrix)
            {
                this.GetProjectionMatrix();
                this.updateProjectionMatrix = false;
            }

        }
        public void IniUpdate(IApp app)
        {
            this.w = app.window.Width;
            this.h = app.window.Height;
            this.AspectRatio = app.window.Width / app.window.Height;
            this.UpdateProjectionMatrix();
            this.UpdateViewMatrix();
            this.Update(app);
        }
        bool refresh = false;
        public void Update(IApp app)
        {

            
            iniCoordinateSystem(this.Position, this.target);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref this.viewMatrix);
            this.lastApp = app;
            checkMatrixUpdate();



            if (this.perspective)
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.LoadMatrix(ref this.projectionMatrix);

                //GL.Frustum((float)this.frustumLeft, (float)this.frustumRight, (float)this.frustumBottom, (float)this.frustumTop, (float)this.frustumNear, (float)this.frustumFar);

                //GL.MatrixMode(MatrixMode.Modelview);
                //GL.LoadIdentity();
                refresh = true;
            }
            else
            {
                
                float widthHalf = (float)this.lastApp.window.Width / 2.0F;
                float heightHalf = (float)this.lastApp.window.Height / 2.0F;
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                double dx = this.frustumRight - this.frustumLeft;
                double dy = this.frustumTop - this.frustumBottom;


                if (Math.Abs(dx) > 0.001 || Math.Abs(dy) > 0.001)
                {
                    if (dx >= dy)
                    {
                        double dY = dx * this.h / this.w;
                        frustumTop = (float)(frustumBottom + dY);

                    }
                    else
                    {
                        double dX = dy * this.w / this.h;
                        frustumRight = (float)(frustumLeft + dX);

                    }
                    GL.Ortho(frustumLeft, frustumRight, frustumBottom, frustumTop, -frustumFar, frustumFar);
                    if (refresh)
                    {
                        MoveIn(-1);
                        refresh = false;
                    }
                    //GL.MatrixMode(MatrixMode.Modelview);
                    //GL.LoadIdentity();
                }


                //GL.Translate(Position.X, Position.Y, Position.Z);
                //GL.Ortho(0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 100.0f);
                //GL.MatrixMode(MatrixMode.Projection);

                ////Console.WriteLine(this.frustumLeft);
                ////GL.Ortho((float)this.frustumLeft + widthHalf, (float)this.frustumRight + widthHalf, (float)this.frustumBottom + heightHalf, (float)this.frustumTop + heightHalf, -(float)this.frustumFar, (float)this.frustumFar);
                ////Matrix4 ortho = Matrix4.CreateOrthographicOffCenter((float)this.frustumLeft + widthHalf, (float)this.frustumRight + widthHalf, (float)this.frustumBottom + heightHalf, (float)this.frustumTop + heightHalf, -(float)this.frustumFar, (float)this.frustumFar);
                //Matrix4 ortho = Matrix4.CreateOrthographicOffCenter((float)this.frustumLeft, (float)this.frustumRight , (float)this.frustumBottom, (float)this.frustumTop, -(float)this.frustumFar, (float)this.frustumFar);
                ////Matrix4 ortho = Matrix4.CreateOrthographic(this.w, this.h, (float)this.frustumNear, (float)this.frustumFar);
                //GL.LoadMatrix(ref ortho);
                //GL.Ortho((float)this.frustumLeft + widthHalf, (float)this.frustumRight + widthHalf, (float)this.frustumBottom + heightHalf, (float)this.frustumTop + heightHalf, (float)(-this.frustumFar), (float)this.frustumFar);
                //GL.MatrixMode(MatrixMode.Modelview);
            }


        }

        public void UpdateViewMatrix()
        {
            this.updateViewMatrix = true;
            this.updateViewProjectionMatrix = true;
        }

        public void UpdateProjectionMatrix()
        {
            this.updateProjectionMatrix = true;
            this.updateViewProjectionMatrix = true;
        }

        private void getFrustumPerspective()
        {
            double aspect = this.w / this.h;
            double halfHeight = this.h ;
            double tan = Math.Tan(this.fovy );
            double cameraZ = halfHeight / tan;
            double zNear = cameraZ / this.near;
            double zFar = cameraZ * this.far;
            double h = halfHeight / this.near;
            double w = h * aspect;
            this.frustumLeft = (float)-w;
            this.frustumRight = (float)w;
            this.frustumBottom = (float)-h;
            this.frustumTop = (float)h;
            this.frustumNear = (float)zNear;
            this.frustumFar = (float)zFar;

            if (!this.perspective)
            {
                float dist = (this.target - this.Position).Length;
                this.frustumLeft *= dist;
                this.frustumRight *= dist;
                this.frustumBottom *= dist;
                this.frustumTop *= dist;
            }

        }
    }
}
