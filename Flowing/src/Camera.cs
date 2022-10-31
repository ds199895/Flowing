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
        public void Resize(int w, int h)
        {
            if (w != 0)
            {
                this.w = w;
            }
            if (h != 0)
            {
                this.h = h;
            }
            this.AspectRatio = (float)this.w / (float)this.h;
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
            this.UpdateProjectionMatrix();
        }
        public void SetPerspective(bool perspective)
        {
            if (!this.is2D)
            {
                this.perspective = perspective;
            }
            this.UpdateProjectionMatrix();
        }
        // The position of the camera
        public Vector3 Position { get; set; }

        // This is simply the aspect ratio of the viewport, used for the projection matrix.
        public float AspectRatio {get; set; }

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
                //this.UpdateViewMatrix();
                //this.UpdateProjectionMatrix();
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
                //this.UpdateViewMatrix();
                //this.UpdateProjectionMatrix();
            }
        }

        // The field of view (FOV) is the vertical angle of the camera view.
        // This has been discussed more in depth in a previous tutorial,
        // but in this tutorial, you have also learned how we can use this to simulate a zoom feature.
        // We convert from degrees to radians as soon as the property is set to improve performance.
        public float Fov
        {
            get => (float)MathHelper.RadiansToDegrees(fovy);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                fovy = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
                //this.UpdateViewMatrix();
                //this.UpdateProjectionMatrix();
            }
        }

        // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
        public void GetViewMatrix()
        {

            this.viewMatrix= Matrix4.LookAt(Position, target, _up);

        }

        // Get the projection matrix using the same method we have used up until this point
        public void GetProjectionMatrix()
        {
            this.getFrustumPerspective();
            this.projectionMatrix= Matrix4.CreatePerspectiveFieldOfView((float)fovy, AspectRatio, 0.01f, 100000f);

            //this.projectionMatrix=Matrix4.CreatePerspectiveOffCenter(this.frustumLeft, this.frustumRight, this.frustumBottom, this.frustumTop, this.frustumNear, this.frustumFar);
            //this.projectionMatrix = Matrix4.CreatePerspectiveOffCenter(this.frustumLeft*2, this.frustumRight-this.frustumLeft, this.frustumBottom*2, this.frustumTop-this.frustumBottom, this.frustumNear, this.frustumFar);

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
        private void getFrustumPerspective()
        {
            double aspect = this.w / this.h;
            double halfHeight = this.h / 2;
            double tan = Math.Tan(this.fovy / 2);
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
            }


        }

        // This function is going to update the direction vertices using some of the math learned in the web tutorials.  待解决：基于欧拉角的旋转
        private void UpdateVectors()
        {
            float dis = (this.target - this.Position).Length;
            // First, the front matrix is calculated using some basic trigonometry.
            _front.X = (float)(Math.Cos(_pitch) * Math.Cos(_yaw));
            _front.Y = (float)Math.Sin(_pitch);
            _front.Z = (float)(Math.Cos(_pitch) * Math.Sin(_yaw));

            // We need to make sure the vectors are all normalized, as otherwise we would get some funky results.
            _front = Vector3.Normalize(_front);

            this.target = this.Position + _front * dis;
            //target = Position + _front;
            // Calculate both the right and the up vector using cross product.
            // Note that we are calculating the right from the global up; this behaviour might
            // not be what you need for all cameras so keep this in mind if you do not want a FPS camera.
            _right = Vector3.Normalize(Vector3.Cross(_front, _up));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }


        public void Zoom(double data,double zoomDeltaFactor)
        {


            if (data != 0.0D)
            {
                double dist = 500.0D;
                if (this.target != null)
                {
                    dist = (this.target - this.Position).Length;

                }

                this.MoveIn(data * dist * zoomDeltaFactor);

            }
        }

        public void MoveIn(double dist)
        {
            Vector3 lookDirection = (this.target - this.Position).Normalized();

            this.Position += (float)dist * lookDirection;
            this.UpdateViewMatrix();
            if (!this.perspective)
            {
                this.UpdateProjectionMatrix();
            }

        }

        public void Pan(double dx, double dy,double panDeltaFactor)
        {
            //iniCoordinateSystem(this.Position, this.target);
            bool onXZ = false;
            if (this._front.X == 0 && this._front.Y == 0)
            {
                onXZ = true;
            }
            double DistToOrign = (this.target-this.Position).Length;
            if (onXZ)
            {

                dx *= -panDeltaFactor * DistToOrign;
                dy *= panDeltaFactor * DistToOrign;
                this.Position+= new Vector3((float)dx, (float)dy, 0);
                this.target += new Vector3((float)dx, (float)dy, 0);
            }
            else
            {

                dx *= -panDeltaFactor * DistToOrign;
                dy *= panDeltaFactor * DistToOrign;

                Vector3 movex = (float)dx * this.Right;
                //movex.Z = 0.0F;
                Vector3 movey = (float)dy * this.Up;
                //movey.Z = 0.0F;
                Vector3 move = movex + movey;
                this.Position += move;
                this.target += move;
            }
            this.UpdateViewMatrix();
        }

        public void RotateAroundTarget(double dx, double dy, double rotateDeltaFactor, bool FixZaxisRotation = false)
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


                if (!FixZaxisRotation)
                {
                    double ang1 = (double)(dir * dx) * rotateDeltaFactor;
                    double ang2 = (double)dy * rotateDeltaFactor;
                    HorizontalTransform(ang1);
                    VerticalTransform(ang2);
                }
                else
                {
                    double ang1 = (double)(dir * dx) * rotateDeltaFactor;
                    double ang2 = (double)dy * rotateDeltaFactor;
                    RotateLookAtHorizontal(ang1);
                    RotateLookAtVertical(-ang2);
                }
            }
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
    
                this.Rotate(this.target, new Vector3(this._right.X,this._right.Y, this._right.Z), angle);
            }
        }
        
        public void UpdateSystem()
        {
            
        }
        public void VerticalTransform(double angle)
        {
            float length = (this.target - this.Position).Length;
            Vector3 rotateAxis = Vector3.Cross(this.Position, this._up);

            this.Position = RotateVector(this.Position, rotateAxis, angle);
            this._front = RotateVector(this._front, rotateAxis, angle);

            //this.target = this.Position + this._front*length;
            ////update the up direction
            //Vector3 newUpDirection = Vector3.Cross(this.Front, rotateAxis);
            //newUpDirection.Normalize();
            //this.Up = newUpDirection;
            //this._right = Vector3.Cross(this._front, this._up).Normalized();
            this.UpdateViewMatrix();
        }

        public void HorizontalTransform( double angle)
        {
            Vector3 rotateAxis = this.Up;
            float length = (this.target - this.Position).Length;
            this.Position = RotateVector(this.Position, rotateAxis, angle);
            this._front = RotateVector(this._front, rotateAxis, angle);
            this.target = this.Position + this._front * length;
            this._right = Vector3.Cross(this._front, rotateAxis).Normalized();
            this._up = Vector3.Cross(this._right, this._front).Normalized();

            this.UpdateViewMatrix();
        }


        public void Rotate(Vector3 center, Vector3 axis, double angle)
        {
            this.Position = RotateVector(this.Position, center, axis, angle);
            this.target = RotateVector(this.target, center, axis, angle);
            this._up = RotateVector(this._up, axis, angle);
            this._front = RotateVector(this._front, axis, angle);
            this._right = RotateVector(this._right, axis, angle);

            this.UpdateViewMatrix();
        }

        public Vector3 RotateVector(Vector3 based, Vector3 center, Vector3 axis, double angle)
        {
            return center == based ? based : (RotateVector(based - center, axis, angle) + center);
        }
        private Vector3 RotateVector(Vector3 based, Vector3 axis, double angle)
        {

            Matrix4 ro = Matrix4.CreateFromAxisAngle(axis, (float)angle);
            Vector3 newV = Vector4.Transform(new Vector4(based, 0.0f), ro).Xyz;
            return newV;
        }

       

        public void Update(IApp app)
        {

            this.UpdateProjectionMatrix();
            this.UpdateViewMatrix();

            this.lastApp = app;
            checkMatrixUpdate();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref this.viewMatrix);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();


            if (this.perspective)
            {

                GL.LoadMatrix(ref this.projectionMatrix);
                //GL.Frustum(this.frustumLeft, this.frustumRight,this.frustumBottom, this.frustumTop, (float)this.frustumNear, (float)this.frustumFar);
            }
            else
            {
                //GL.Ortho(frustumLeft, frustumRight, frustumBottom, frustumTop, -frustumFar, frustumFar);
                Matrix4 ortho = Matrix4.CreateOrthographic((frustumRight-frustumLeft)*2.0f, (frustumTop-frustumBottom)*2.0f, (float)-this.frustumFar, (float)this.frustumFar);
                GL.LoadMatrix(ref ortho);
            }

            GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadIdentity();
        }

        //private Matrix4 FromFrustum(double near,double far,double left,double right,double bottom,double top,bool parallel)
        //{
        //    Matrix4 newMatrix=new Matrix4();
        //    if (parallel)
        //    {
        //        newMatrix.M11 = (float)(2.0 / (right - left));
        //        newMatrix.M12 = (float)(2.0 / (top - bottom));
        //        newMatrix.M13
        //    }
        //}

    }
}
