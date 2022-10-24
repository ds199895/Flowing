using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowing
{
    public class Camera
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

        private readonly Dictionary<string, int> _uniformLocations;


        // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, target, upVector);
        }

        // Get the projection matrix using the same method we have used up until this point
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView((float)fovy, (float)AspectRatio, 0.01f, 100f);
        }


        public Camera(double w,double h,Vector3 position, Vector3 target,Vector3 upVector)
        {
            this.fovy = 1.0471975511965976D;
            this.width = w;
            this.height = h;
            this.AspectRatio = this.height / this.width;
            this.position = position;
            this.target = target;
            //this.upVector = upVector;
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
        private int Handle;
        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
            
        }
        public void applyCamera(IGraphics app)
        {
            Matrix4 ViewMatrix = this.GetViewMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref ViewMatrix);
            
            Matrix4 ProjMatrix = this.GetProjectionMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref ProjMatrix);

            this.lastApp = app;
            getFrustumPerspective();
            if (this.perspective)
            {
                this.lastApp.Frustum((float)this.frustumLeft, (float)this.frustumRight, (float)this.frustumBottom, (float)this.frustumTop, (float)this.frustumNear, (float)this.frustumFar);
            }
            else
            {
                float widthHalf = (float)this.lastApp.width / 2.0F;
                float heightHalf = (float)this.lastApp.height / 2.0F;
                this.lastApp.Ortho((float)this.frustumLeft + widthHalf, (float)this.frustumRight + widthHalf, (float)this.frustumBottom + heightHalf, (float)this.frustumTop + heightHalf, (float)(-this.frustumFar), (float)this.frustumFar);
            }

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
