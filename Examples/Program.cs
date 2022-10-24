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
            Size(1200, 1000);
            //cam = new CameraController(this, 200);
            cam = new CamController(this,200);
            Console.WriteLine("Hello, this is the first example of Flowing! Nice to meet you~");
        }
        override
        public void Draw()
        {

            Background(255, 255, 255);
            //cam.DrawSystem();
            //CreateCube();
            //Smooth(2);
            cam.drawSystem(this, 200);

            Fill(0, 0, 255);
            StrokeWeight(2);
            Stroke(0, 255, 0);
            for(int i = 0; i < 500; i++)
            {
                BeginShape();
                Vertex(200, 100, 0);
                Vertex(100, 200, 0);
                Vertex(220, 400, 0);
                EndShape();
            }


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
            }
        }

    }
}
