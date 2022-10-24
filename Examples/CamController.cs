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
        private Camera camPerspective;
        private Camera camTop;
        public Camera CurrentView;

        public double RotateDeltaFactor = 0.004d;
        public const double panDeltaFactor = 0.001D;
        public double zoomDeltaFactor = 0.2D;
        private Vector2 mouseLastPosition;
        public CamController(IApp app,float dis)
        {

            this.app = app;
            this.window = app.window;
            camPerspective = new Camera(this.window.Width, this.window.Height, new Vector3(-dis, -dis,dis),new Vector3(0,0,0));
            camTop = new Camera(this.window.Width, this.window.Height, new Vector3(0, 0, dis),new Vector3(0,0,0));
            camTop.perspective = false;
            CurrentView = camPerspective;
            this.window.Title = "Grid";
            this.window.Load += Window_Load;
            this.window.RenderFrame += Window_RenderFrame;
            this.window.UpdateFrame += Window_UpdateFrame;
            this.window.MouseMove += new EventHandler<OpenTK.Input.MouseMoveEventArgs>(Mouse_Move);
            this.window.MouseWheel += new EventHandler<OpenTK.Input.MouseWheelEventArgs>(Mouse_Wheel);
            this.window.Resize += new EventHandler<EventArgs>(Window_Resize);
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
            bool noRotation = true;
            if (this.CurrentView == camPerspective)
            {
                noRotation = false;
            }

            var dX = e.XDelta;
            var dY = e.YDelta;
            if (e.Mouse.LeftButton == OpenTK.Input.ButtonState.Pressed)
            {
                if (!noRotation && this.CurrentView.target != null)
                {
                    Vector2 newMousePosition = new Vector2(e.Mouse.X, e.Mouse.Y);

                    if (mouseLastPosition.X != newMousePosition.X)
                    {
                        HorizontalTransform(mouseLastPosition.X < newMousePosition.X, this.RotateDeltaFactor);
                    }

                    if (mouseLastPosition.Y != newMousePosition.Y)// change position in the horizontal direction
                    {

                        VerticalTransform(mouseLastPosition.Y > newMousePosition.Y, this.RotateDeltaFactor);
                    }
                    mouseLastPosition = newMousePosition;
                    //Rotate(dX, dY);
                }

            }
            else if (e.Mouse.RightButton == OpenTK.Input.ButtonState.Pressed)
            {
                Pan(dX,dY, false);
            }

        }
        private void Mouse_Down(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            mouseLastPosition = new Vector2(e.Position.X, e.Position.Y);

        }
        private void Mouse_Wheel(object sender, OpenTK.Input.MouseWheelEventArgs e)
        {
            //camera.position += camera.objZ * e.DeltaPrecise*5;
            Console.WriteLine(e.ValuePrecise);
            //CurrentView.Position += CurrentView.Front * e.DeltaPrecise * 5;
            Zoom(-e.Value);
        }

        public void Top()
        {
            CurrentView = camTop;
        }
        public void Perspective()
        {
            CurrentView = camPerspective;
        }
        protected void Window_Resize(object sender, EventArgs e)
        {
            
            GL.Viewport(0, 0, this.window.Width, this.window.Height);
            CurrentView.AspectRatio = this.window.Width / (float)this.window.Height;
        }
        private void VerticalTransform(bool upDown, double angleDeltaFactor)
        {
            Vector3 position = CurrentView.Position;
            Vector3 rotateAxis =Vector3.Cross(position,CurrentView.Up);


            Matrix3 ro = Matrix3.CreateFromAxisAngle(rotateAxis, (float)angleDeltaFactor * (upDown ? -1 : 1));
            Vector3 newPosition = Vector3.Transform(ro, position);

            CurrentView.Position = newPosition;

            CurrentView.Front = Vector3.Transform(ro, CurrentView.Front);

            //update the up direction
            Vector3 newUpDirection = Vector3.Cross(CurrentView.Front, rotateAxis);
            newUpDirection.Normalize();
            CurrentView.Up = newUpDirection;
        }

        private void HorizontalTransform(bool leftRight, double angleDeltaFactor)
        {
            Vector3 postion = CurrentView.Position;
            Vector3 rotateAxis = CurrentView.Up;


            Matrix3 ro=Matrix3.CreateFromAxisAngle(rotateAxis, (float)angleDeltaFactor * (leftRight ? -1 : 1));
            Vector3 newPosition=Vector3.Transform(ro, postion);
        
            CurrentView.Position = newPosition;

            CurrentView.Front = Vector3.Transform(ro, CurrentView.Front);

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
        private void Pan(double dx, double dy, bool onXZ)
        {
            double DistToOrign = CurrentView.Position.Length;
            if (onXZ)
            {

                dx *= -panDeltaFactor * DistToOrign;
                dy *= panDeltaFactor * DistToOrign;
                CurrentView.Position = new Vector3(CurrentView.Position.X + (float)dx, CurrentView.Position.Y + (float)dy, CurrentView.Position.Z);

            }
            else
            {

                dx *= -panDeltaFactor * DistToOrign;
                dy *= panDeltaFactor * DistToOrign;

                Vector3 movex = (float)dx * CurrentView.Right;
                Vector3 movey = (float)dy * CurrentView.Up;

                Vector3 move = movex + movey;
                //move.Z = 0.0F;
                CurrentView.Position += move;
                CurrentView.target += move;
            }

        }

        protected void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
            //CurrentView.ApplyCam(this.app);
        }


        protected void Window_RenderFrame(object sender, FrameEventArgs e)
        {


            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

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
