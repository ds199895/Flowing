using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowing;
using Hsy.Geo;
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
        double x = 10;
        bool stop = false;
        HS_Vector[] vertices = new HS_Vector[9];
        Font font;
        override
        public void SetUp()
        {
            Size(800, 600);

            //Smooth(4);
            //Background(255, 0, 0);
            Print("Hello, this is the first example of Flowing! Nice to meet you~");
            cam = new CamController(this);
            cam.FixZaxisRotation = true;
            font = createFont("微软雅黑", 24);
            TextFont(font);
            
            this.vertices[0] = new HS_Vector(-400, 600);
            this.vertices[1] = new HS_Vector(0, 200);
            this.vertices[2] = new HS_Vector(200, 500);
            this.vertices[3] = new HS_Vector(700, 0);
            this.vertices[4] = new HS_Vector(500, -600);
            this.vertices[5] = new HS_Vector(300, 300);
            this.vertices[6] = new HS_Vector(0, -500);
            this.vertices[7] = new HS_Vector(-600, 0);
            this.vertices[8] = new HS_Vector(-200, 100);
        }
        override
        public void Draw()
        {
            //Rhino:157, 163, 170

            Background(255);

            //Smooth(4);
            cam.DrawSystem(this, 200);
            PushStyle();
            Fill(255, 0, 0);

            Cube((float)(20 + x), (float)(20 + x), (float)(20 + x));

            PopStyle();
            Fill(255, 0, 255);
            //NoFill();
            StrokeWeight(2);
            Stroke(0, 255, 0);
            //NoStroke();

            BeginShape();
            for (int i = 0; i < vertices.Length; i++)
            {
                Vertex(vertices[i].xf, vertices[i].yf);
            }
            Vertex(vertices[0].xf, vertices[0].yf);

            //Vertex(200, 100, 0);
            //Vertex(100, 200, 0);
            //Vertex(220, 400, 0);
            //Vertex(200, 100, 0);

            //Vertex(50, 0, 0);
            //Vertex(-50, 50, 0);
            //Vertex(100, 250, 0);
            //Vertex(50, 0, 0);

            Vertex(220, 400, 0);
            Vertex(100, 200, 0);
            Vertex(200, 100, 0);
            Vertex(220, 400, 0);

            Vertex(100, 250, 0);
            Vertex(-50, 50, 0);
            Vertex(50, 0, 0);
            Vertex(100, 250, 0);

            EndShape();

            PushStyle();
            Fill(255, 0, 0);

            Text("这是第一行文字！", 200, 200, 0);
            Fill(0, 0, 255);
            Text("This is The First Line of Text!", 200, 150, 0);
            PopStyle();
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
            else if (key == OpenTK.Input.Key.S)
            {
                stop = !stop;
            }
        }

    }
}
