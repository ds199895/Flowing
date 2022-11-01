using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowing
{
    public class CamController
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

        //Init Some Operation Factors
        public double RotateDeltaFactor =0.003D;
        public const double panDeltaFactor = 0.002D;
        public double zoomDeltaFactor = 0.1D;
        private Vector2 mouseLastPosition;

        private float lastZoomData = 0;

        public bool FixZaxisRotation { get; set; } = true;

        public CamController(IApp app,float dis=1000)
        {
            this.app = app;
            this.window = this.app.window;
            this.app.is2D = false;
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
            this.app.window.KeyDown += Key_Down;
            this.app.window.KeyUp += Key_Up;
        }
        public void ResetCameras(float dis=1000)
        {
            this.w = this.window.Width;
            this.h = this.window.Height;
            this.camPerspective = new Camera(w, h, new Vector3(-dis, -dis, dis), new Vector3(0.0F, 0.0F, 0.0F));
            this.camTop = new Camera(w, h, new Vector3(0.0F, 0.0F, dis), new Vector3(0.0F, 0.0F, 0.0F));
            this.camTop.Set2DProperties();
            this.camFront = new Camera(w, h, new Vector3(0.0F, -dis, 0.0F), new Vector3(0.0F, 0.0F, 0.0F));
            this.camFront.Set2DProperties();
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

            this.Perspective();
            //this.CurrentView.SetPerspective(false);
            //this.Right();
            //this.Front();
            //this.Top();
            //this.Ortho();
        }

        //Override based window functions
        protected void Window_Load(Object sender, EventArgs e)
        {

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            GL.Enable(EnableCap.Fog);
            GL.Fog(FogParameter.FogColor, new float[4] { 0.0f, 0.0f, 0.0f, 1.0f });//Same color as clear.
            GL.Fog(FogParameter.FogMode, (int)FogMode.Linear);
            GL.Fog(FogParameter.FogEnd, (float)(4096 / 3.0f));
  
        }

        protected void Window_UpdateFrame(object sender, FrameEventArgs e)
        {

        }

        protected void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            CurrentView.Update(this.app);
        }

        protected void Window_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, this.window.Width, this.window.Height);
            //Refresh all cameras
            foreach (Camera cam in cams)
            {
                cam.Resize(this.window.Width, this.window.Height);
            }
        }
        private void Key_Down(Object sender, KeyboardKeyEventArgs e)
        {
            if (this.app.key == OpenTK.Input.Key.LShift || this.app.key == OpenTK.Input.Key.RShift)
            {
                this.FixZaxisRotation = false;
            }
        }
        private void Key_Up(Object sender, KeyboardKeyEventArgs e)
        {
            if (this.app.key == OpenTK.Input.Key.LShift || this.app.key == OpenTK.Input.Key.RShift)
            {
                this.FixZaxisRotation = true;
            }
        }

        private void Mouse_Move(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            if (e.Mouse.MiddleButton == OpenTK.Input.ButtonState.Pressed)
            {
                float dX = e.X - mouseLastPosition.X;
                float dY = e.Y - mouseLastPosition.Y;

                //this.CurrentView.RotateAroundLookAt(dX, dY, RotateDeltaFactor);
                this.CurrentView.RotateAroundTarget(dX, dY, RotateDeltaFactor,FixZaxisRotation);
                mouseLastPosition = new Vector2(e.X, e.Y);

            }
            else if (e.Mouse.RightButton == OpenTK.Input.ButtonState.Pressed)
            {
                var dX = e.XDelta;
                var dY = e.YDelta;
                this.CurrentView.Pan(dX,dY,panDeltaFactor);
                mouseLastPosition = new Vector2(e.X, e.Y);
            }

        }


        private void Mouse_Down(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {

            
            mouseLastPosition = new Vector2(e.X, e.Y);


        }

        private void Mouse_Up(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            Console.WriteLine("Up! " + this.app.key);
            if (this.app.key == OpenTK.Input.Key.LShift || this.app.key == OpenTK.Input.Key.RShift)
            {
                this.FixZaxisRotation = true;
            }

        }
        private void Mouse_Wheel(object sender, OpenTK.Input.MouseWheelEventArgs e)
        {
            double signum = 0.0D;
            if (Math.Abs(-e.ValuePrecise) == 1)
            {
                signum = (double)(e.ValuePrecise);
            }
            else
            {
                signum = (double)Math.Sign((float)(this.lastZoomData+e.ValuePrecise));
            }
            this.CurrentView.Zoom(signum, zoomDeltaFactor);
            this.lastZoomData = -e.ValuePrecise;
        }

        

        //depreciated 待处理：欧拉角
        public void Rotate(float dX,float dY)
        {
            this.CurrentView.Yaw += -(float)(dX * RotateDeltaFactor);
            this.CurrentView.Pitch -= -(float)(dY * RotateDeltaFactor);
        }


        //Shift cameras
        public void Top()
        {
            this.CurrentView = this.cams[1];

        }
        public void Right()
        {
            this.CurrentView = this.cams[5];

        }
        public void Front()
        {
            this.CurrentView = this.cams[2];

        }

        public void Perspective()
        {
            this.CurrentView = this.cams[0];
            this.CurrentView.Update(this.app);
        }
        public void Ortho()
        {
            CurrentView = this.cams[6];
        }

        //Draw Grid and System
        public void DrawSystem(IApp app, float len)
        {
            app.PushStyle();
            this.DrawGrid(app, len, 20.0F);
            app.StrokeWeight(2.5F);
            app.Stroke(180, 50, 50);
            app.Line(0.0F, 0.0F, 0.0F, len, 0.0F, 0.0F);
            app.Stroke(50, 180, 50);
            app.Line(0.0F, 0.0F, 0.0F, 0.0F, len, 0.0F);
            app.Stroke(50, 50, 180);
            app.Line(0.0F, 0.0F, 0.0F, 0.0F, 0.0F, len);
            app.PopStyle();
        }

        private void DrawGrid(IApp app, float len, float num)
        {
            
            float weight =1F;
            int gray = 100;
            len *= 2.0F;
            float step = len / num;
            float start = -len / 2.0F;


            for (int i = 0; (float)i <= num; ++i)
            {
                app.PushStyle(false);
                app.Stroke(129,134,140);
                app.StrokeWeight(weight);
                app.Line(start, (float)i * step + start, 0.0F, len + start, (float)i * step + start, 0.0F);
                app.Line((float)i * step + start, start, 0.0F, (float)i * step + start, len + start, 0.0F);

                app.PopStyle();

                app.PushStyle(false);
                app.Stroke(147,153,160);
                float step2 = step / 5.0F;
                app.StrokeWeight(weight/4);
                if (i != num)
                {
                    for (int j = 1; j < 5; j++)
                    {
                        app.Line(start, (float)i * step + j * step2 + start, 0.0F, len + start, (float)i * step + j * step2 + start, 0.0F);
                        app.Line((float)i * step + j * step2 + start, start, 0.0F, (float)i * step + j * step2 + start, len + start, 0.0F);
                    }
                }

                app.PopStyle();
            }
        }


    }
}
