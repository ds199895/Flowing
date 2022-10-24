//using OpenTK;
//using OpenTK.Graphics;
//using OpenTK.Graphics.OpenGL;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Flowing
//{
//    class CameraController
//    {
//        Camera CamPerspective;
//        Camera CamTop;
//        Camera CurrentView;
//        IApp app;
//        public CameraController(IApp app,float dis)
//        {
//            this.app = app;
//            CamPerspective = new Camera(this.app.window.Width, this.app.window.Height, new Vector3(0, 0, dis));
//            CurrentView = CamPerspective;
//            this.app.window.MouseMove += new EventHandler<OpenTK.Input.MouseMoveEventArgs>(Mouse_Move);
//            this.app.window.MouseWheel += new EventHandler<OpenTK.Input.MouseWheelEventArgs>(Mouse_Wheel);
//            this.app.window.Resize += new EventHandler<EventArgs>(Resize);
//            this.app.window.RenderFrame += new EventHandler<FrameEventArgs>(Render);
//        }

//        public double RotateDeltaFactor = 0.1d;
//        public const double panDeltaFactor = 0.003D;
//        private void Mouse_Move(object sender, OpenTK.Input.MouseMoveEventArgs e)
//        {
//            var dX = e.XDelta;
//            var dY = e.YDelta;
//            if (e.Mouse.LeftButton == OpenTK.Input.ButtonState.Pressed)
//            {

//                Rotate(dX, dY);
//            }
//            else if (e.Mouse.RightButton == OpenTK.Input.ButtonState.Pressed)
//            {
//                Pan(dX, dY, false);
//            }

//        }

//        private void Mouse_Wheel(object sender, OpenTK.Input.MouseWheelEventArgs e)
//        {
//            //camera.position += camera.objZ * e.DeltaPrecise*5;
//            CurrentView.Position += CurrentView.Front * e.DeltaPrecise * 10;
//        }

//        protected void Resize(object sender, EventArgs e)
//        {

//            GL.Viewport(0, 0, this.app.window.Width, this.app.window.Height);
//            CurrentView.w = this.app.window.Width;
//            CurrentView.h = this.app.window.Height;
//            CurrentView.AspectRatio = this.app.window.Width / this.app.window.Height;
//        }
//        protected void Render(object sender, FrameEventArgs e)
//        {
//            CurrentView.Update();
            
//        }
//        public void Rotate(float dX, float dY)
//        {
//            CurrentView.Yaw += (float)(dX * RotateDeltaFactor);
//            CurrentView.Pitch -= (float)(dY * RotateDeltaFactor);
//        }
//        private void Pan(double dx, double dy, bool onXZ)
//        {
//            double DistToOrign = CurrentView.Position.Length;
//            if (onXZ)
//            {

//                dx *= -panDeltaFactor * DistToOrign;
//                dy *= panDeltaFactor * DistToOrign;
//                CurrentView.Position = new Vector3(CurrentView.Position.X + (float)dx, CurrentView.Position.Y + (float)dy, CurrentView.Position.Z);
//            }
//            else
//            {

//                dx *= -panDeltaFactor * DistToOrign;
//                dy *= panDeltaFactor * DistToOrign;

//                Vector3 movex = (float)dx * CurrentView.Right;
//                Vector3 movey = (float)dy * CurrentView.Up;

//                Vector3 move = movex + movey;
//                CurrentView.Position += move;
//            }

//        }

//        public void DrawSystem()
//        {
//            DrawGrid(new Color4(0.8f, 0.8f, 0.8f, 1.0f));
//            DrawAxies();
//        }
//        public void DrawAxies()
//        {
//            this.app.PushStyle();
//            this.app.StrokeWeight(2);
//            this.app.Stroke(255, 0, 0);
//            this.app.Line(0, 0, 0, 2000, 0, 0);
//            this.app.Stroke(0, 255, 0);
//            this.app.Line(0, 0, 0, 0, 2000, 0);
//            this.app.Stroke(0, 0, 255);
//            this.app.Line(0, 0, 0, 0, 0, 2000);

//            this.app.PopStyle();


//            //GL.Color4(Color.Red);
//            //GL.Begin(PrimitiveType.Lines);

//            //GL.Vertex3(0, 0, 0);
//            //GL.Vertex3(2000, 0, 0);

//            //GL.End();
//            //GL.Color4(Color.Green);
//            //GL.Begin(PrimitiveType.Lines);


//            //GL.Vertex3(0, 0, 0);
//            //GL.Vertex3(0, 2000, 0);

//            //GL.End();
//            //GL.Color4(Color.Blue);
//            //GL.Begin(PrimitiveType.Lines);


//            //GL.Vertex3(0, 0, 0);
//            //GL.Vertex3(0, 0, 2000);

//            //GL.End();
//        }
//        public void DrawGrid(Color4 color, int cell_size = 10)
//        {
//            this.app.PushStyle();
//            for (int i = 0; i < 20 + 1; i++)
//            {
                
//                GL.Color4(color);
//                this.app.Stroke(color.ToArgb());
//                this.app.StrokeWeight(1);
//                int current = i * cell_size;
//                this.app.Line(current, 0, 0, current, 200, 0);
//                this.app.Line(0, current, 0, 200, current, 0);

//            }
//            this.app.PopStyle();
//        }
//    }
//}
