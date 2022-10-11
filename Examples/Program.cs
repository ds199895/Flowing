using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowing;

namespace Flowing
{
    public class Program : IApp
    {

        public static void Main(string[] args)
        {
           IApp.main();
        }

        override
        public void SetUp()
        {
             Size(1200, 1000);
             
             Console.WriteLine("Hello, this is the first example of Flowing! Nice to meet you~");
        }
        override
        public void Draw()
        {
            Background(255, 255, 255);
            Fill(0,0,255);
            StrokeWeight(3);
            Stroke(0, 255, 0);
            BeginShape();
            Vertex(200, 50, 0);
            Vertex(800, 600,0);
            Vertex(220, 800,0);
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
            Print("check wheel");
        }
        public override void MousePressed()
        {
            Print("check mouse");
        }

        //public override void KeyPressed()
        //{
        //    Print(Key);
        //}
        public override void KeyReleased()
        {
            Print(key);
        }

    }
}
