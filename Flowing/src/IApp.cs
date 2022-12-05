using Lucene.Net.Support;
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
    public class IApp:IConstants
    {
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]   //找子窗体   
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]   //用于发送信息给窗体   
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("User32.dll", EntryPoint = "ShowWindow")]   //
        private static extern bool ShowWindow(IntPtr hWnd, int type);
        public IGraphics g;
        public GameWindow window { get { return this.g.window; }set { this.g.window = value; } }
        public int samples { get { return this.g.samples; } set { this.g.samples = value; } }
        public int width { get { return this.g.width; } set { this.g.width = value; } }
        public int height { get { return this.g.height; } set { this.g.height = value; } }
        public bool wireFrame { get { return this.g.wireFrame; } set { this.g.wireFrame = value; } }
        public bool is2D = true;
        public int FrameRate=60;
        public string key;
        private bool smoothReseted = false;
        public int keyCode;
        public MouseButton MouseButton;
        private bool consoleShow = true;

        private static IntPtr ParenthWnd = new IntPtr(0);
        private static IntPtr et = new IntPtr(0);
        public static GraphicsMode mode=new GraphicsMode(32, 24, 8,2);
        public HashMap<int, GameWindow> wins = new HashMap<int, GameWindow>();
        int bakeWidth;
        int bakeHeight;

        Random internalRandom;
        public IApp()
        {
            
        }

        private static Type InitClassName()
        {
            StackTrace trace = new StackTrace();
            Type type = trace.GetFrame(2).GetMethod().ReflectedType;

            return type;
        }
        public void init()
        {
            this.g = new IGraphics();
        }

        public static void main()
        {
            Console.Title = "Console";
            Print("Press Escape to close Console window");
            Type type = InitClassName();
            GameWindow _window = new GameWindow(800, 600,mode);
           
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
            app.init();
            app.g.InitialStyleSettings();
            app.window = _window;
            app.window.Title = type.ToString();
            app.HandleWindowEvents();
            app.HandleConsole();
            app.samples = mode.Samples;
            app.window.Run(1/app.FrameRate);
        }
        public void HandleWindowEvents()
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
            //if (!setup)
            //{
            Print("Load-----------------------");
            this.SetUp();
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.DepthFunc(DepthFunction.Lequal);
                
                this.Draw();
                //setup = true;
        //}
    }
        private void Window_UpdateFrame(Object sender, EventArgs e)
        {


     
        }

        public void ResetSmooth()
        {
            if (mode.Samples != samples)
            {
                smoothReseted = true;
            }
                if (smoothReseted)
            {
                Print("Reset Smooth!");

                Point p = window.Location;
                GameWindow old = this.window;
                mode = new GraphicsMode(32, 24, 8, samples);
                Print("w : " + this.window.Width);
                bakeWidth = old.Width;
                bakeHeight = old.Height;

                this.window = new GameWindow(this.window.Width, this.window.Height, mode);

                this.window.Location = p;
                this.window.Title =old.Title;


                this.window.MakeCurrent();
                HandleWindowEvents();

                Print("mode: " + mode.Samples);
                old.Close();
                smoothReseted =false;
                this.window.Run(1 / FrameRate);

                
            }
        }
        private void Window_RenderFrame(Object sender, EventArgs e)
        {
            ResetSmooth();
            if (smoothReseted)
            {
                this.window.Width = bakeWidth;
                this.window.Height = bakeHeight;
            }

            this.Draw();
            this.g.textFont.Dispose();
            if (this.window.Exists)
            {
                this.window.SwapBuffers();
            }
            
        }

        private void Window_Resized(Object sender, EventArgs e)
        {
            if (is2D)
            {
                GL.Viewport(0, 0, this.window.Width, this.window.Height);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0.0f, this.window.Width, 0.0f, this.window.Height, -1.0f, 1.0f);
                GL.MatrixMode(MatrixMode.Modelview);
            }
            this.PushStyle();
            this.PushMatrix();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.FromArgb(this.g.backgroundColor));
            
            this.PopMatrix();
            this.PopStyle();
        }

        private void Window_MouseWheel(Object sender, MouseWheelEventArgs e)
        {
            this.MouseWheel();
        }

        private void Window_MouseDown(Object sender, MouseButtonEventArgs e)
        {

            this.MouseButton = e.Button;
            this.MousePressed();
        }
        private void Window_MouseUp(Object sender, MouseButtonEventArgs e)
        {
            this.MouseButton = e.Button;
           
            this.MouseReleased();
        }
        private void Window_KeyDown(Object sender, KeyboardKeyEventArgs e)
        {
            this.key =e.Key.ToString();
            this.keyCode = e.Key.GetHashCode();
            this.KeyPressed();
        }
        private void Window_KeyUp(Object sender, KeyboardKeyEventArgs e)
        {
            
            this.key =e.Key.ToString();
            this.keyCode = e.Key.GetHashCode();
            if (key ==Key.Escape.ToString())
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
        public void PushStyle(bool Continue=false)
        {
            this.g.PushStyle(Continue);
        }

        public void PopStyle()
        {
            this.g.PopStyle();
        }
        public void PushMatrix()
        {
            this.g.PushMatrix();
        }

        public void PopMatrix()
        {
            this.g.PopMatrix();
        }

        public void Background(int rgb, float alpha)
        {
            this.g.Background(rgb,alpha);
        }

        public void Background(float gray)
        {
            this.g.Background(gray);
      
        }

        public void Background(float gray, float alpha)
        {
            this.g.Background(gray,alpha);

        }

        public void Background(float v1, float v2, float v3)
        {
            this.g.Background(v1,v2,v3);
        }

        public void Background(float v1, float v2, float v3, float alpha)
        {
            this.g.Background(v1,v2,v3, alpha);
        }

        public void Smooth()
        {
            this.g.Smooth();
        }

        public void Smooth(int level)
        {
            this.g.Smooth(level);
        }

        public void NoSmooth()
        {
            this.g.NoSmooth();
        }
        public void StrokeWeight(float weight)
        {
            this.g.StrokeWeight(weight);
        }
        public void StrokeJoin(int join)
        {
            this.g.StrokeJoin(join);
        }

        public void StrokeCap(int cap)
        {
            this.g.StrokeCap(cap);
        }


        public void NoStroke()
        {
            this.g.NoStroke();
        }

        public void Stroke(int rgb)
        {
            this.g.Stroke(rgb);
        }

        public void Stroke(int rgb, float alpha)
        {
            this.g.Stroke(rgb,alpha);
        }

        public void Stroke(float gray)
        {
            this.g.Stroke(gray);
        }

        public void Stroke(float gray, float alpha)
        {
            this.g.Stroke(gray, alpha);
        }

        public void Stroke(float v1, float v2, float v3)
        {
            this.g.Stroke(v1,v2,v3);
        }

        public void Stroke(float v1, float v2, float v3, float alpha)
        {
            this.g.Stroke(v1, v2, v3,alpha);
        }

        public void NoFill()
        {
            this.g.NoFill();
        }

        public void Fill(int rgb)
        {
            this.g.Fill(rgb);
        }

        public void Fill(int rgb, float alpha)
        {
            this.g.Fill(rgb,alpha);
        }

        public void Fill(float gray)
        {
            this.g.Fill(gray);
        }

        public void Fill(float gray, float alpha)
        {
            this.g.Fill(gray,alpha);
        }

        public void Fill(float v1, float v2, float v3)
        {
            this.g.Fill(v1,v2,v3);
        }

        public void Fill(float v1, float v2, float v3, float alpha)
        {
            this.g.Fill(v1, v2, v3,alpha);
        }

        public void NoTint()
        {
            this.g.NoTint();
        }
        public void Tint(int rgb)
        {
            this.g.Tint(rgb);
        }

        public void Tint(int rgb, float alpha)
        {
            this.g.Tint(rgb,alpha); 
        }

        public void Tint(float gray)
        {

            this.g.Tint(gray);
        }

        public void Tint(float gray, float alpha)
        {
            this.g.Tint(gray, alpha);
        }

        public void Tint(float v1, float v2, float v3)
        {
            this.g.Tint(v1,v2,v3);
        }

        public void Tint(float v1, float v2, float v3, float alpha)
        {
            this.g.Tint(v1,v2,v3,alpha);
        }

        public void Ambient(int rgb)
        {
            this.g.Ambient(rgb);
        }

        public void Ambient(float gray)
        {
            this.g.Ambient(gray);
        }

        public void Ambient(float v1, float v2, float v3)
        {
            this.g.Ambient(v1,v2,v3);
        }

        public void Specular(int rgb)
        {
            this.g.Specular(rgb);
        }

        public void Specular(float gray)
        {
            this.g.Specular(gray);
        }

        public void Specular(float v1, float v2, float v3)
        {
            this.g.Specular(v1,v2,v3);
        }

        public void Shininess(float shine)
        {
            this.g.Shininess(shine);
        }

        public void Emissive(int rgb)
       {
            this.g.Emissive(rgb);
        }

        public void Emissive(float gray)
        {
            this.g.Emissive(gray);
        }

        public void Emissive(float v1, float v2, float v3)
        {
            this.g.Emissive(v1,v2,v3);
        }

        public void TextAlign(int alignX)
        {
            this.g.TextAlign(alignX);
        }

        public void TextAlign(int alignX, int alignY)
        {
            this.g.TextAlign(alignX,alignY);
        }


        public void ColorMode(int mode)
        {
            this.g.ColorMode(mode);
        }

        public void ColorMode(int mode, float max)
        {
            this.g.ColorMode(mode,max);
        }

        public void ColorMode(int mode, float max1, float max2, float max3)
        {
            this.g.ColorMode(mode, max1, max2, max3); 
        }

        public void ColorMode(int mode, float max1, float max2, float max3, float maxA)
        {
            this.g.ColorMode(mode, max1, max2, max3, maxA);
        }

        public void Translate(params float[] p)
        {
            this.g.Translate(p);
        }
        public void Rotate(float angle)
        {
            this.g.Rotate(angle);
        }

        public void RotateX(float angle)
        {
            this.g.RotateX(angle);

        }
        public void RotateY(float angle)
        {
            this.g.RotateY(angle);
        }
        public void RotateZ(float angle)
        {
            this.g.RotateZ(angle);

        }

        public void Rotate(double angle, float x, float y, float z)
        {
            this.g.Rotate(angle,x,y,z);
        }

        public void Scale(float s)
        {
            this.g.Scale(s);

        }
        public void scale(float x, float y)
        {
            this.g.Scale(x,y);
        }

        public void scale(float x, float y, float z)
        {
            this.g.Scale(x,y,z);
        }
        public void TextFont(Font which)
        {
            this.g.TextFont(which);
        }

        public void TextFont(Font which, float size)
        {

            this.g.TextFont(which, size);
        }
        public void TextSize(float size)
        {
            this.g.TextSize(size);
        }
        public Font CreateFont(String name, float size)
        {
            return this.g.CreateFont(name, size);
        }


        public void Vertex(params float[] p)
        {
            this.g.Vertex(p);
        }
        public void BeginShape(bool hole = false)
        {
            this.g.BeginShape(hole);
        }
        private int LastIndex = 0;

        public void BeginShape(PrimitiveType p)
        {
            this.g.BeginShape(p);
        }


        public void EndShape(IGraphics.EndMode endMode = IGraphics.EndMode.Close)
        {
            this.g.EndShape(endMode);
        }

        public void Line(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            this.g.Line(x1, y1, z1, x2, y2, z2) ;
        }
        public void Cube(float l)
        {
            this.g.Cube(l, l, l);
        }
        public void Cube(float length, float width, float height)
        {
            this.g.Cube(length, width, height);
        }
        public void Sphere(float r)
        {
            this.g.Sphere(r);
        }
        public void Sphere(float r,int detail)
        {
            this.g.Sphere(r,detail);
        }
        public void Text(string str, float x, float y, float z)
        {
            this.g.Text(str, x, y, z);
        }
            public float Sin(float angle)
        {
            return (float)Math.Sin((double)angle);
        }
        public float Cos(float angle)
        {
            return (float)Math.Cos((double)angle);
        }
        public float Tan(float angle)
        {
            return (float)Math.Tan((double)angle);
        }
        public float Asin(float angle)
        {
            return (float)Math.Asin((double)angle);
        }

        public float Acos(float angle)
        {
            return (float)Math.Acos((double)angle);
        }

        public float Atan(float angle)
        {
            return (float)Math.Atan((double)angle);
        }
        public float Atan2(float y,float x)
        {
            return (float)Math.Atan2((double)y,(double)x);
        }
        public double max(double a,double b)
        {
            return a > b ? a : b;
        }
        public double max(params double[] list)
        {
            if (list.Length == 0)
            {
                throw new IndexOutOfRangeException("Cannot use min() or max() on an empty array.");
            }
            else
            {
                double max = list[0];
                for(int i = 0; i < list.Length; i++)
                {
                    if (list[i] > max)
                    {
                        max = list[i];
                    }
                }
                return max;
            }
        }
        public int max(params int[] list)
        {
            if (list.Length == 0)
            {
                throw new IndexOutOfRangeException("Cannot use min() or max() on an empty array.");
            }
            else
            {
                int max = list[0];
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i] > max)
                    {
                        max = list[i];
                    }
                }
                return max;
            }
        }
        public double min(double a, double b)
        {
            return a < b ? a : b;
        }
        public double min(params double[] list)
        {
            if (list.Length == 0)
            {
                throw new IndexOutOfRangeException("Cannot use min() or max() on an empty array.");
            }
            else
            {
                double min = list[0];
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i] > min)
                    {
                        min = list[i];
                    }
                }
                return min;
            }
        }
        public int min(params int[] list)
        {
            if (list.Length == 0)
            {
                throw new IndexOutOfRangeException("Cannot use min() or max() on an empty array.");
            }
            else
            {
                int min = list[0];
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i] < min)
                    {
                        min = list[i];
                    }
                }
                return min;
            }
        }

        public float degrees(float radians)
        {
            return radians * RAD_TO_DEG;
        }
        public float radians(float degrees)
        {
            return degrees * DEG_TO_RAD;
        }

        public static void Print(Object o)
        {
           IGraphics.Print(o);
        }

        public float random(float low,float high)
        {
            if (low >= high)
            {
                return low;
            }
            else
            {
                float diff = high - low;
                return this.random(diff) + low;
            }
        }

        public float random(float high)
        {
            if (high != 0.0f && high ==high)
            {
                if (this.internalRandom == null)
                {
                    this.internalRandom = new Random();
                }
                float value = 0.0F;
                do
                {
                    value = (float)(this.internalRandom.NextDouble() * high);
                } while (value == high);
                return value;
            }
            else
            {
                return 0.0F;
            }
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
