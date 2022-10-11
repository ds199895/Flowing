using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Flowing
{
    public class IApp:IGraphics
    {
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]   //找子窗体   
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]   //用于发送信息给窗体   
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("User32.dll", EntryPoint = "ShowWindow")]   //
        private static extern bool ShowWindow(IntPtr hWnd, int type);
        
        public int FrameRate=60;
        public Key key;
        public int keyCode;
        public int MouseButton;
        private bool consoleShow = true;

        private static IntPtr ParenthWnd = new IntPtr(0);
        private static IntPtr et = new IntPtr(0);
        public IApp()
        {
            
        }

        private static Type InitClassName()
        {
            StackTrace trace = new StackTrace();
            Type type = trace.GetFrame(2).GetMethod().ReflectedType;

            return type;
        }
        public static void main()
        {
            Console.Title = "Console";
            Print("Press Escape to close Console window");
            Type type = InitClassName();
            GameWindow _window = new GameWindow(500, 500,new GraphicsMode(32, 24, 8, 8));
            IApp app;
            try
            {
                // 创建实例
                app = (IApp)System.Activator.CreateInstance(type);
            }
            catch (Exception var26)
            {
                throw new Exception("err: ", var26);
            }
            
            app.InitialStyleSettings();
            app.window = _window;
            app.window.Title = type.ToString();
            app.HandleWindowEvents();
            app.HandleConsole();

            app.window.Run(1/app.FrameRate);

        }
        private void HandleWindowEvents()
        {
            this.window.Load += this.Window_Load;
            this.window.UpdateFrame += this.Window_UpdateFrame;
            this.window.RenderFrame += this.Window_RenderFrame;
            this.window.Resize += this.Window_Resized;
            this.window.MouseWheel += this.Window_MouseWheel;
            this.window.MouseDown += this.Window_MouseDown;
            this.window.MouseUp += this.Window_MouseUp;
            this.window.MouseMove += this.Window_MouseMove;
            this.window.KeyDown += this.Window_KeyDown;
            this.window.KeyUp += this.Window_KeyUp;
        }
        private void HandleConsole()
        {
            if (consoleShow)
            { 
                ParenthWnd = FindWindow(null, "Console");
                ShowWindow(ParenthWnd, 1);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化
                //Console.ReadLine();
            }
            else
            {
                ParenthWnd = FindWindow(null, "Console");
                ShowWindow(ParenthWnd, 0);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化
                //Console.ReadLine();
            }
        }
        private void Window_Load(Object sender, EventArgs e)
        {
            

            this.SetUp();
        }
        private void Window_UpdateFrame(Object sender, EventArgs e)
        {
            //GL.Clear(ClearBufferMask.ColorBufferBit);
            //Background(255,0,0);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            //GL.LineWidth(5.0f);
            //GL.Begin(PrimitiveType.Triangles);

            //GL.Color3(Color.FromArgb(255, 0, 0, 0));
            //GL.Vertex3(1.0f, 1.0f, 0.0f);
            //GL.Vertex3(49.0f, 1.0f, 0.0f);
            //GL.Vertex3(25.0f, 49.0f, 0.0f);

            //GL.End();

            //this.window.SwapBuffers();
        }


        private void Window_RenderFrame(Object sender, EventArgs e)
        {
            this.Draw();
            this.window.SwapBuffers();
        }

        private void Window_Resized(Object sender, EventArgs e)
        {
            GL.Viewport(0, 0, this.window.Width, this.window.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0.0f, this.window.Width, 0.0f, this.window.Height, -1.0f, 1.0f);
            GL.MatrixMode(MatrixMode.Modelview);

            PushStyle();
            Background(backgroundColor);
            PopStyle();
            this.window.SwapBuffers();
        }

        private void Window_MouseWheel(Object sender, MouseWheelEventArgs e)
        {
            this.MouseWheel();
        }

        private void Window_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            this.MouseButton = ((int)e.Button);
            this.MousePressed();
        }
        private void Window_MouseUp(Object sender, MouseButtonEventArgs e)
        {
            this.MouseButton = ((int)e.Button);
            this.MouseReleased();
        }
        private void Window_KeyDown(Object sender, KeyboardKeyEventArgs e)
        {
            this.key =e.Key;
            this.keyCode = e.Key.GetHashCode();
            this.KeyPressed();
        }
        private void Window_KeyUp(Object sender, KeyboardKeyEventArgs e)
        {
            this.key = e.Key;
            this.keyCode = e.Key.GetHashCode();
            if (key == Key.Escape)
            {
                consoleShow = !consoleShow;
            }
            this.KeyReleased();
            this.HandleConsole();
        }

        private void Window_MouseMove(Object sender, EventArgs e)
        {

        }

        public virtual void MousePressed()
        {

        }

        public virtual void MouseReleased()
        {

        }
        public virtual void KeyPressed()
        {

        }
        public virtual void KeyReleased()
        {

        }

        public virtual void MouseDragged()
        {

        }
        public virtual void MouseWheel()
        {

        }
        public virtual void SetUp()
        {

        }

        public void Size(int width,int height)
        {
            this.width = width;
            this.height = height;
            this.window.Width = this.width;
            this.window.Height = this.height;
            Background(200.0f);
            this.window.SwapBuffers();
        }


        public virtual void Draw()
        {

        }


        public static int constrain(int amt, int low, int high)
        {
            return amt < low ? low : (amt > high ? high : amt);
        }

        public static float constrain(float amt, float low, float high)
        {
            return amt < low ? low : (amt > high ? high : amt);
        }


    }
}
