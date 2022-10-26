using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowing
{
    class CamController
    {
        IApp app;
        public GameWindow window;
        public int w;
        public int h;

        private Camera camPerspective;
        private Camera camTop;
        private Camera camFront;
        private Camera camBack;
        private Camera camLeft;
        private Camera camRight;
        private Camera camOrtho;
        public List<Camera> cams = new List<Camera>();
        public Camera CurrentView;

        public double RotateDeltaFactor =0.005D;
        //public double RotateDeltaFactor = 1.8D;
        public const double panDeltaFactor = 0.003D;
        public double zoomDeltaFactor = 0.1D;
        private Vector2 mouseLastPosition;
        public CamController(IApp app,float dis=1000)
        {

            this.app = app;
            this.window = this.app.window;
            ResetCameras(dis);
            HandleWindow();
        }
        protected void HandleWindow()
        {
            this.app.window.Load += Window_Load;
            this.app.window.RenderFrame += Window_RenderFrame;
            this.app.window.UpdateFrame += Window_UpdateFrame;
            this.app.window.MouseMove += new EventHandler<OpenTK.Input.MouseMoveEventArgs>(Mouse_Move);
            this.app.window.MouseWheel += new EventHandler<OpenTK.Input.MouseWheelEventArgs>(Mouse_Wheel);
            this.app.window.Resize += new EventHandler<EventArgs>(Window_Resize);
            this.app.window.MouseDown += Mouse_Down;
        }
        public void ResetCameras(float dis=1000)
        {
            Console.WriteLine("current width: " + this.window.Width);
            int length = Math.Max(this.window.Width, this.window.Height);
            //this.w = length;
            //this.h = length;
            this.w = this.window.Width;
            this.h = this.window.Height;
            this.camPerspective = new Camera(w, h, new Vector3(-dis, -dis, dis), new Vector3(0.0F, 0.0F, 0.0F));
            this.camTop = new Camera(w, h, new Vector3(0.0F, 0.0F, dis), new Vector3(0.0F, 0.0F, 0.0F));
            this.camTop.Set2DProperties();
            this.camFront = new Camera(w, h, new Vector3(0.0F, -dis, 0.0F), new Vector3(0.0F, 0.0F, 0.0F));
            this.camTop.Set2DProperties();
            this.camBack = new Camera(w, h, new Vector3(0.0F, dis, 0.0F), new Vector3(0.0F, 0.0F, 0.0F));
            this.camBack.Set2DProperties();
            this.camLeft = new Camera(w, h, new Vector3(-dis, 0.0F, 0.0F), new Vector3(0.0F, 0.0F, 0.0F));
            this.camLeft.Set2DProperties();
            this.camRight = new Camera(w, h, new Vector3(dis, 0.0F, 0.0F), new Vector3(0.0F, 0.0F, 0.0F));
            this.camRight.Set2DProperties();
            this.camOrtho = new Camera(w, h, new Vector3(-dis, -dis, dis), new Vector3(0.0F, 0.0F, 0.0F));
            this.camOrtho.Set3DProperties(false);
            this.cams.Add(camPerspective);
            this.cams.Add(camTop);
            this.cams.Add(camFront);
            this.cams.Add(camBack);
            this.cams.Add(camLeft);
            this.cams.Add(camRight);
            this.cams.Add(camOrtho);

            //this.Perspective();
            //this.CurrentView.SetPerspective(false);
            this.Top();
            //this.Ortho();
        }

      
        protected void Window_Load(Object sender, EventArgs e)
        {

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            GL.Enable(EnableCap.Fog);
            GL.Fog(FogParameter.FogColor, new float[4] { 0.0f, 0.0f, 0.0f, 1.0f });//Same color as clear.
            GL.Fog(FogParameter.FogMode, (int)FogMode.Linear);
            GL.Fog(FogParameter.FogEnd, (float)(4096 / 3.0f));

  
        }

        private void Mouse_Move(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {


  
            if (e.Mouse.MiddleButton == OpenTK.Input.ButtonState.Pressed)
            {
                bool noRotation = true;
                if (this.CurrentView == this.camPerspective)
                {
                    noRotation = false;
                }

                float dX = e.X - mouseLastPosition.X;
                float dY = e.Y - mouseLastPosition.Y;

                //if (!noRotation && this.CurrentView.target != null)
                //{

                //    if (mouseLastPosition.X != e.X)
                //    {
                //        this.CurrentView.HorizontalTransform(mouseLastPosition.X < e.X, this.RotateDeltaFactor);
                //    }

                //    if (mouseLastPosition.Y != e.Y)// change position in the horizontal direction
                //    {

                //        this.CurrentView.VerticalTransform(mouseLastPosition.Y > e.Y, this.RotateDeltaFactor);
                //    }

                //    //Rotate(dX, dY);
                //}

                this.CurrentView.RotateAroundLookAt(dX, dY, RotateDeltaFactor);
                //this.CurrentView.Yaw += dX * (float)RotateDeltaFactor;
                //this.CurrentView.Pitch -= dY * (float)RotateDeltaFactor;
                mouseLastPosition = new Vector2(e.X, e.Y);


            }
            else if (e.Mouse.RightButton == OpenTK.Input.ButtonState.Pressed)
            {
                var dX = e.XDelta;
                var dY = e.YDelta;
                this.CurrentView.Pan(dX,dY, false,panDeltaFactor);
            }

        }
        private void Mouse_Down(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            mouseLastPosition = new Vector2(e.X, e.Y);

        }
        private void Mouse_Wheel(object sender, OpenTK.Input.MouseWheelEventArgs e)
        {
            //camera.position += camera.objZ * e.DeltaPrecise*5;

            //CurrentView.Position += CurrentView.Front * e.DeltaPrecise * 5;
            Zoom(-e.ValuePrecise);
        }
        public void Top()
        {
            this.CurrentView = this.cams[1];
            
        }
        public void Perspective()
        {
            CurrentView = this.cams[0];
            Resize();
   
        }
        public void Ortho()
        {
            CurrentView = this.cams[6];
        }
        
        protected void Window_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, this.app.window.Width, this.app.window.Height);
            foreach (Camera cam in cams)
            {
                cam.IniUpdate(this.app);
            }
            Resize();
  
            this.app.window.SwapBuffers();
        }

        private void Resize()
        {
            this.CurrentView.w = this.window.Width;
            this.CurrentView.h = this.window.Height;

            this.CurrentView.AspectRatio = this.window.Width / (float)this.window.Height;
            this.CurrentView.UpdateViewMatrix();
            this.CurrentView.UpdateProjectionMatrix();
            this.CurrentView.Update(this.app);
        }




        private float lastZoomData=0;
        protected void Zoom(float data)
        {
            double signum = 0.0D;
            if (Math.Abs(data) == 1)
            {
                signum = (double)(-data);
            }
            else
            {
                signum = (double)Math.Sign((float)(this.lastZoomData - data));
            }

            if (signum != 0.0D)
            {
                double dist = 500.0D;
                if (this.CurrentView.target!= null)
                {
                    dist = (this.CurrentView.target-this.CurrentView.Position).Length;

                }

                this.CurrentView.MoveIn(signum * dist * this.zoomDeltaFactor);
                
            }

            this.lastZoomData = data;
        }
        
        public void Rotate(float dX,float dY)
        {
            CurrentView.Yaw += (float)(dX * RotateDeltaFactor);
            CurrentView.Pitch -= (float)(dY * RotateDeltaFactor);
        }


        protected void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
            //CurrentView.ApplyCam(this.app);
        }


        protected void Window_RenderFrame(object sender, FrameEventArgs e)
        {


            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadIdentity();

            CurrentView.Update(this.app);
            
            
            //GL.ClearColor(Color4.White);
            //GL.Color4(Color4.Red);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            //GL.Begin(PrimitiveType.Polygon);
            //GL.Vertex3(200, 100, 0);
            //GL.Vertex3(100, 200, 0);
            //GL.Vertex3(220, 400, 0);
            //GL.End();
            //this.window.SwapBuffers();
        }
        public void DrawSystem()
        {
            DrawGrid(new Color4(0.8f, 0.8f, 0.8f, 1.0f));
            DrawAxies();
        }
        public void DrawAxies()
        {
            this.app.PushStyle();
            this.app.StrokeWeight(2);
            this.app.Stroke(255, 0, 0);
            this.app.Line(0, 0, 0, 2000, 0, 0);
            this.app.Stroke(0, 255, 0);
            this.app.Line(0, 0, 0, 0, 2000, 0);
            this.app.Stroke(0, 0, 255);
            this.app.Line(0, 0, 0, 0, 0, 2000);

            this.app.PopStyle();


            //GL.Color4(Color.Red);
            //GL.Begin(PrimitiveType.Lines);

            //GL.Vertex3(0, 0, 0);
            //GL.Vertex3(2000, 0, 0);

            //GL.End();
            //GL.Color4(Color.Green);
            //GL.Begin(PrimitiveType.Lines);


            //GL.Vertex3(0, 0, 0);
            //GL.Vertex3(0, 2000, 0);

            //GL.End();
            //GL.Color4(Color.Blue);
            //GL.Begin(PrimitiveType.Lines);


            //GL.Vertex3(0, 0, 0);
            //GL.Vertex3(0, 0, 2000);

            //GL.End();
        }
        private void drawGrid(IApp app, float len, float num)
        {
            app.PushStyle();
            float weight = 0.3F;
            int gray = 100;
            len *= 2.0F;
            float step = len / num;
            float start = -len / 2.0F;
            app.Stroke(gray);
            app.StrokeWeight(weight);

            for (int i = 0; (float)i <= num; ++i)
            {
                app.Line(start, (float)i * step + start, 0.0F, len + start, (float)i * step + start, 0.0F);
                app.Line((float)i * step + start, start, 0.0F, (float)i * step + start, len + start, 0.0F);
            }
            app.PopStyle();

        }

        public void drawSystem(IApp app, float len)
        {
            app.PushStyle();
            drawGrid(app, len, 20.0F);
            app.StrokeWeight(2.0F);
            app.Stroke(-65536);
            app.Line(0.0F, 0.0F, 0.0F, len, 0.0F, 0.0F);
            app.Stroke(-16711936);
            app.Line(0.0F, 0.0F, 0.0F, 0.0F, len, 0.0F);
            app.Stroke(-16776961);
            app.Line(0.0F, 0.0F, 0.0F, 0.0F, 0.0F, len);
            app.PopStyle();
        }
        public void DrawGrid(Color4 color, int cell_size = 10)
        {
            this.app.PushStyle();
            for (int i = 0; i < 20 + 1; i++)
            {

                GL.Color4(color);
                this.app.Stroke(color.ToArgb());
                this.app.StrokeWeight(1);
                int current = i * cell_size;
                this.app.Line(current, 0, 0, current, 200, 0);
                this.app.Line(0, current, 0, 200, current, 0);

            }
            this.app.PopStyle();
        }
    }
}
