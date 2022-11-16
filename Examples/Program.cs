using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowing;
using Flowing.Triangulation;
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
        Vector3[] vertices = new Vector3[9];
        Font font;
        List<Vector3m> points;
        EarClipping earClipping;
        List<Vector3m> res;
        List<List<Vector3m>> holes = new List<List<Vector3m>>();

        public override void SetUp()
        {
            Size(800, 600);
            //Smooth(4);
            Background(255, 0, 0);
            Print("Hello, this is the first example of Flowing! Nice to meet you~");
            cam = new CamController(this);
            cam.FixZaxisRotation = true;
            font = createFont("微软雅黑", 24);
            TextFont(font);


            this.vertices[0] = new Vector3(500, -600, 0);
            this.vertices[1] = new Vector3(300, 300, 0);
            this.vertices[2] = new Vector3(0, -500, 0);
            this.vertices[3] = new Vector3(-600, 0, 0);
            this.vertices[4] = new Vector3(-200, 100, 0);
            this.vertices[5] = new Vector3(-400, 600, 100);
            this.vertices[6] = new Vector3(0, 200, 0);
            this.vertices[7] = new Vector3(200, 500, 0);
            this.vertices[8] = new Vector3(700, 0, 0);

            //this.vertices[0] = new HS_Vector(-200, 100, -400);
            //this.vertices[1] = new HS_Vector(-400, 600, 100);
            //this.vertices[2] = new HS_Vector(0, 200);
            //this.vertices[3] = new HS_Vector(200, 500);
            //this.vertices[4] = new HS_Vector(700, 0, 0);
            //this.vertices[5] = new HS_Vector(500, -600, 200);
            //this.vertices[6] = new HS_Vector(300, 300, 0);
            //this.vertices[7] = new HS_Vector(0, -500);
            //this.vertices[8] = new HS_Vector(-600, 0, 0);

        }

        public override void Draw()
        {
            //Rhino:157, 163, 170
            Background(255);

            //Smooth(4);
            cam.DrawSystem(this, 200);
            PushStyle();
            Fill(0, 0, 0);

            Cube((float)(20 + x), (float)(20 + x), (float)(20 + x));

            PopStyle();
            Fill(200, 200, 200);
            //NoFill();
            StrokeWeight(1.5f);
            Stroke(255, 0, 0);

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            //GL.Begin(PrimitiveType.Polygon);
            //GL.Color3(1.0f, 0.0f, 0.0f);
            //GL.Vertex3(200, 350, 350);
            //GL.Color3(0.0f, 1.0f, 0.0f);
            //GL.Vertex3(150, 350, 350);
            //GL.Color3(0.0f, 0.0f, 1.0f);
            //GL.Vertex3(200, 500, 400);
            //GL.End();

            BeginShape();

            //测试任意图形
            //Vertex(200, 350, 350);
            //Vertex(150, 350, 350);
            //Vertex(200, 500, 400);
            //Vertex(200, 500, 500);
            //Vertex(400, 350, 350);

            //测试自相交图形
            //Vertex(200, 100, 0);
            //Vertex(50, 200, 0);
            //Vertex(0, 0, 0);
            //Vertex(220, 400, 0);

            //测试带洞多边形

            for (int i = 0; i < vertices.Length; i++)
            {
                Vertex(vertices[i].X, vertices[i].Y, vertices[i].Z);
                PushStyle();
                TextSize(15);
                Fill(255, 0, 0);
                Text(i.ToString(), vertices[i].X, vertices[i].Y, vertices[i].Z);
                PopStyle();
            }
            BeginShape(hole = true);
            Vertex(100, 250, 0);
            Vertex(50, 0, 0);
            Vertex(-50, 50, 10);
            EndShape();

            BeginShape(hole = true);
            Vertex(100, 200, 0);
            Vertex(220, 400, 0);
            Vertex(200, 100, 0);
            EndShape();

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

        public override void KeyReleased()
        {
            if (key == OpenTK.Input.Key.T)
            {
                cam.Top();
            }
            else if (key == OpenTK.Input.Key.P)
            {
                cam.Perspective();
            }
            else if (key == OpenTK.Input.Key.Z)
            {
                cam.CurrentView.SetPerspective(!cam.CurrentView.perspective);
            }
            else if (key == OpenTK.Input.Key.S)
            {
                stop = !stop;
            }
            else if (key == OpenTK.Input.Key.W)
            {
                wireFrame = !wireFrame;
            }
        }

    }
}
