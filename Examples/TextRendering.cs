using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowing
{
    class TextRendering:GameWindow
    {

            TextRenderer renderer;
            Font serif = new Font(FontFamily.GenericSerif, 24);
            Font sans = new Font(FontFamily.GenericSansSerif, 24);
            Font mono = new Font("黑体", 24);
            
            /// <summary>
            /// Uses System.Drawing for 2d text rendering.
            /// </summary>


            #region Constructor

            public TextRendering()
                : base(800, 600)
            {
            }

            #endregion

            #region OnLoad

            protected override void OnLoad(EventArgs e)
            {
              //processText();


            }

            protected void processText(String s,Font f,Color co)
            {
                renderer = new TextRenderer(s, f);
                PointF position = PointF.Empty;
                Color4 c = new Color4(1.0f, 1.0f, 1.0f, 0.0f);

            renderer.Clear(Color.FromArgb(c.ToArgb()));
            renderer.DrawString(new SolidBrush(co), position);
            //renderer.DrawString(Brushes.Red, position);
        }
            #endregion

            #region OnUnload

            protected override void OnUnload(EventArgs e)
            {
                renderer.Dispose();
            }

            #endregion

            #region OnResize

            protected override void OnResize(EventArgs e)
            {
                GL.Viewport(ClientRectangle);

                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, Width, 0, Height, -1.0, 1.0);
            }

            #endregion

            #region OnUpdateFrame

            protected override void OnUpdateFrame(FrameEventArgs e)
            {
                //if (Keyboard[OpenTK.Input.Key.Escape])
                //{
                //    this.Exit();
                //}
            }

            #endregion

            #region OnRenderFrame

            protected override void OnRenderFrame(FrameEventArgs e)
            {

            GL.ClearColor(new Color4(1.0f, 1.0f, 1.0f, 1.0f));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusDstAlpha);
            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Greater, 0);


            GL.Begin(PrimitiveType.Polygon);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(220, 400, 0);
            GL.Vertex3(100, 200, 0);
            GL.Vertex3(200, 100, 0);
            GL.Vertex3(220, 400, 0);


            GL.End();
            Text("这是第一行文字！",100,200,0);
            Text("this is first line of text!", 100, 150, 0);
            //GL.Disable(EnableCap.Blend);
            SwapBuffers();
            }

        public void Text(string str, float x, float y, float z)
        {
            processText(str, serif,Color.FromArgb(255,255,0,255));
            GL.Enable(EnableCap.Texture2D);
            GL.PushMatrix();
            GL.BindTexture(TextureTarget.Texture2D, renderer.Texture);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Begin(BeginMode.Quads);


            GL.Color4(1.0f, 1.0f, 1.0f,0.02f);
            //GL.Translate(x, y, z);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(x, y, z);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(x + renderer.width, y, z);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(x + renderer.width, y + renderer.height, z);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(x, y + renderer.height, z);
            //GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(0, 0, 0);
            //GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(0, renderer.height, 0);
            //GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(renderer.width,renderer.height, 0);
            //GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(renderer.width, 0, 0);


            GL.End();
            GL.PopMatrix();
            GL.Disable(EnableCap.Texture2D);
            
        }
            #endregion

            #region Main

            public static void Main()
            {
                using (TextRendering example = new TextRendering())
                {
                    //Utilities.SetWindowTitle(example);
                    example.Run(30.0);
                }
            }

            #endregion
        }
    
}
