﻿using Lucene.Net.Support;
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
        public bool is2D = true;
        public int FrameRate=30;
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
            
            app.InitialStyleSettings();
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
                this.SetUp();
                GL.Enable(EnableCap.DepthTest);
                GL.DepthMask(true);
                GL.DepthFunc(DepthFunction.Less);

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
            this.textFont.Dispose();
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
            GL.ClearColor(Color.FromArgb(this.backgroundColor));

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

        public float degrees(float radians)
        {
            return radians * RAD_TO_DEG;
        }
        public float radians(float degrees)
        {
            return degrees * DEG_TO_RAD;
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
