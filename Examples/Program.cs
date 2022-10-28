using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Flowing
{
    public class Program : IApp
    {

        public static void Main(string[] args)
        {
           IApp.main();
        }

        CamController cam;
        override
        public void SetUp()
        {
            Size(800, 600);
            //cam = new CameraController(this, 200);
            //Smooth(4);
            cam = new CamController(this);
            cam.FixZaxisRotation= true;
            Print("Hello, this is the first example of Flowing! Nice to meet you~");
        }
        override
        public void Draw()
        {
            //Rhino:157, 163, 170

            Background(255);
            //Smooth(4);
            cam.DrawSystem(this, 200);
            PushStyle();
            Fill(0, 0, 0);

            Cube(20, 20, 20);
            //PushMatrix();
            //Translate(0, 20, 0);

            //Cube(30, 30, 30);
            //PopMatrix();

                
            PopStyle();

            Fill(255, 0, 255);
            StrokeWeight(2);
            Stroke(0, 255, 0);
            BeginShape();
            Vertex(200, 100, 0);
            Vertex(100, 200, 0);
            Vertex(220, 400, 0);
            EndShape();



            //PushMatrix();
            //PushStyle();
            //Fill(100);
            //Stroke(255, 0, 255);
            //StrokeWeight(5);

            //BeginShape();
            //Translate(200, 0);
            //Vertex(200, 50, 0);
            //Vertex(400, 100, 0);
            //Vertex(320, 300, 0);
            //EndShape();
            //PopStyle();
            //PopMatrix();

            //PushMatrix();
            //PushStyle();
            //Fill(100);
            //Stroke(255, 0, 255);
            //StrokeWeight(5);

            //BeginShape();
            //NoStroke();
            //Translate(500, 500);
            //Vertex(200, 50, 0);
            //Vertex(400, 100, 0);
            //Vertex(320, 300, 0);
            //EndShape();
            //PopStyle();
            //PopMatrix();
            //Console.WriteLine("draw!");
        }

        public override void MouseWheel()
        {
        }
        public override void MousePressed()
        {
        }

        //public override void KeyPressed()
        //{
        //    Print(Key);
        //}
        public override void KeyReleased()
        {
            if (key == OpenTK.Input.Key.T)
            {
                cam.Top();
            }else if (key == OpenTK.Input.Key.P)
            {
                cam.Perspective();
            }
            else if (key == OpenTK.Input.Key.Z)
            {
                cam.CurrentView.SetPerspective(!cam.CurrentView.perspective);
            }else if (key == OpenTK.Input.Key.A)
            {
                cam.FixZaxisRotation = !cam.FixZaxisRotation;
            }
        }

    }
}
