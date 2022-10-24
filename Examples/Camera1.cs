using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowing
{
    public class Camera1
    {
        public Vector3 position { get; set; }
        public Vector3 target { get; set; }
        public Vector3 upVector { get; set; }
        public Vector3 objX { get; set; }
        public Vector3 objY { get; set; }
        public Vector3 objZ { get; set; }

        private double fovy;
        private double far;
        private double near;

        private double frustumNear;
        private double frustumFar;
        private double frustumBottom;
        private double frustumTop;
        private double frustumLeft;
        private double frustumRight;

        private bool perspective;
        private bool is2D;

        private double width;
        private double height;

        public double AspectRatio;
        private bool updateViewMatrix;
        private bool updateProjectionMatrix;
        private bool updateViewProjectionMatrix;
        private Matrix4 projectionMatrix;
        private IGraphics lastApp;

        // Rotation around the X axis (radians)
        private float _pitch;

        // Rotation around the Y axis (radians)
        private float _yaw = -MathHelper.PiOver2; // Without this, you would be started rotated 90 degrees right.

        // The field of view of the camera (radians)
        private float _fov = MathHelper.PiOver2;

        // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, target, upVector);
        }

        // Get the projection matrix using the same method we have used up until this point
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView((float)fovy, (float)AspectRatio, 0.01f, 1000f);
        }


        public Camera1(double w,double h,Vector3 position, Vector3 target,Vector3 upVector)
        {
            this.fovy = 1.0471975511965976D;
            this.width = w;
            this.height = h;
            this.AspectRatio = this.height / this.width;
            this.position = position;
            this.target = target;
            this.upVector = upVector;
            this.far = 1000.0D;
            this.near = 1000.0D;
            this.frustumNear = 1.0D;
            this.frustumFar = 2.0D;
            this.frustumLeft = -0.5D;
            this.frustumRight = 0.5D;
            this.frustumTop = 0.5D;
            this.frustumBottom = -0.5D;
            this.perspective = true;
            iniObjectSystem(position, target);
        }

        public void iniObjectSystem(Vector3 pos, Vector3 target)
        {
            Vector3 lookDirection = target - pos;
            this.iniObjectSystem(pos, lookDirection, upVector);
        }

        public void iniObjectSystem(Vector3 pos,Vector3 lookDirection,Vector3 upVector)
        {
            this.objZ = lookDirection.Normalized();
            this.objY = upVector;
            this.objX = Vector3.Cross(this.objZ, this.objY);
            this.UpdateViewMatrix();
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

        //private void getProjectionMatrix()
        //{
        //    this.getFrustumPerspective();
        //    this.projectionMatrix = new Matrix4();
        //    this.projectionMatrix.fromFrustum(this.frustumNear, this.frustumFar, this.frustumLeft, this.frustumRight, this.frustumTop, this.frustumBottom, !this.perspective);
        //}
        //public  void resetAppMatrix()
        //{
        //    if (this.lastApp != null)
        //    {
        //        this.lastApp.camera();
        //        this.lastApp.perspective();
        //        this.lastApp = null;
        //    }

        //}
        //private void checkMatrixUpdate()
        //{
        //    if (this.updateViewMatrix)
        //    {
        //        this.getViewMatrix();
        //        this.updateViewMatrix = false;
        //    }

        //    if (this.updateProjectionMatrix)
        //    {
        //        this.getProjectionMatrix();
        //        this.updateProjectionMatrix = false;
        //    }

        //}

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

        private void UpdateVectors()
        {
            // First, the front matrix is calculated using some basic trigonometry.
            this.objZ = new Vector3((float)(Math.Cos(_pitch) * Math.Cos(_yaw)), (float)Math.Sin(_pitch), (float)(Math.Cos(_pitch) * Math.Sin(_yaw)));
            // We need to make sure the vectors are all normalized, as otherwise we would get some funky results.
            this.objZ = this.objZ.Normalized();

            // Calculate both the right and the up vector using cross product.
            // Note that we are calculating the right from the global up; this behaviour might
            // not be what you need for all cameras so keep this in mind if you do not want a FPS camera.
            this.objX = Vector3.Normalize(Vector3.Cross(this.objZ, this.objY));
            this.objY = Vector3.Normalize(Vector3.Cross(this.objX, this.objZ));
        }
        public void Update()
        {
            Matrix4 lookat = this.GetViewMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            Matrix4 projection = this.GetProjectionMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);

        }



        private void getFrustumPerspective()
        {
            double aspect = this.width / this.height;
            double halfHeight = this.height / 2.0D;
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
                double dist = (this.position-this.target).Length;
                this.frustumLeft *= dist;
                this.frustumRight *= dist;
                this.frustumBottom *= dist;
                this.frustumTop *= dist;
            }
            
        }
    }
}
