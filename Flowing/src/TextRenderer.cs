using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace Flowing
{
    public class TextRenderer : IDisposable
    {
        Bitmap bmp;
        Graphics gfx;
        int texture;
        Rectangle dirty_region;
        bool disposed;
        Bitmap bmpFinal;
        String text;
        Font font;
        public int width;
        public int height;
        #region Constructors

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="width">The width of the backing store in pixels.</param>
        /// <param name="height">The height of the backing store in pixels.</param>
        public TextRenderer(String s, Font font)
        {
            this.text = s;
            this.font = font;

            bmp = new Bitmap(1000, 1000, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            gfx = Graphics.FromImage(bmp);

            SizeF textBound = gfx.MeasureString(s, font);
            this.width = (int)textBound.Width;
            this.height = (int)textBound.Height;
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height ");
            if (GraphicsContext.CurrentContext == null)
                throw new InvalidOperationException("No GraphicsContext is current on the calling thread.");
            this.bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            gfx = Graphics.FromImage(this.bmp);

            gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;


            texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

        }

        #endregion

        #region Public Members

        /// <summary>
        /// Clears the backing store to the specified color.
        /// </summary>
        /// <param name="color">A <see cref="System.Drawing.Color"/>.</param>
        public void Clear(Color color)
        {
            gfx.Clear(color);
            dirty_region = new Rectangle(0, 0, bmp.Width, bmp.Height);
        }

        /// <summary>
        /// Draws the specified string to the backing store.
        /// </summary>
        /// <param name="text">The <see cref="System.String"/> to draw.</param>
        /// <param name="font">The <see cref="System.Drawing.Font"/> that will be used.</param>
        /// <param name="brush">The <see cref="System.Drawing.Brush"/> that will be used.</param>
        /// <param name="point">The location of the text on the backing store, in 2d pixel coordinates.
        /// The origin (0, 0) lies at the top-left corner of the backing store.</param>
        public void DrawString(Brush brush, PointF point)
        {


            SizeF size = gfx.MeasureString(text, font);
            gfx.DrawString(text, font, brush, point);
            //Console.WriteLine(size.Width + " , " + size.Height);
            dirty_region = Rectangle.Round(RectangleF.Union(dirty_region, new RectangleF(point, size)));
            dirty_region = Rectangle.Intersect(dirty_region, new Rectangle(0, 0, bmp.Width, bmp.Height));
        }


        /// <summary>
        /// Gets a <see cref="System.Int32"/> that represents an OpenGL 2d texture handle.
        /// The texture contains a copy of the backing store. Bind this texture to TextureTarget.Texture2d
        /// in order to render the drawn text on screen.
        /// </summary>
        public int Texture
        {
            get
            {
                UploadBitmap();
                return texture;
            }
        }

        #endregion

        #region Private Members

        // Uploads the dirty regions of the backing store to the OpenGL texture.
        void UploadBitmap()
        {

            if (dirty_region != RectangleF.Empty)
            {
                System.Drawing.Imaging.BitmapData data = bmp.LockBits(dirty_region,
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexSubImage2D(TextureTarget.Texture2D, 0,
                    dirty_region.X, dirty_region.Y, dirty_region.Width, dirty_region.Height,
                    PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                bmp.UnlockBits(data);

                dirty_region = Rectangle.Empty;
            }
            //RemoveWhiteEdge();
            //BitmapData bitmapData = bmp.LockBits(
            //                   new Rectangle(0, 0, bmp.Width, bmp.Height),
            //                   ImageLockMode.ReadWrite,
            //                   System.Drawing.Imaging.PixelFormat.Format32bppArgb
            //                   );

            //IntPtr ptr = bitmapData.Scan0;
            //int bytesLength = bitmapData.Stride * bitmapData.Height;
            //byte[] rgbValues = new byte[bytesLength];


            //GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);
            //GL.GenTextures(1, out texture);
            //GL.BindTexture(TextureTarget.Texture2D, texture);

            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            //GL.TexImage2D(
            //        TextureTarget.Texture2D,
            //        0,
            //        PixelInternalFormat.Rgba,
            //        bitmapData.Width,
            //        bitmapData.Height,
            //        0,
            //        OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
            //        PixelType.UnsignedByte,
            //        bitmapData.Scan0
            //    );

            //bmp.UnlockBits(bitmapData);


        }

        #endregion
        /// <summary>
        /// 裁剪图片（去掉白边）
        /// </summary>
        private void RemoveWhiteEdge()
        {
            //上左右下
            int top = 0, left = 0, right = bmp.Width, bottom = bmp.Height;

            //寻找最上面的标线,从左(0)到右，从上(0)到下
            for (int i = 0; i < bmp.Height; i++)//行
            {
                bool find = false;
                for (int j = 0; j < bmp.Width; j++)//列
                {
                    Color c = bmp.GetPixel(j, i);
                    if (!IsWhite(c))
                    {
                        top = i;
                        find = true;
                        break;
                    }
                }
                if (find)
                    break;
            }
            //寻找最左边的标线，从上（top位）到下，从左到右
            for (int i = 0; i < bmp.Width; i++)//列
            {
                bool find = false;
                for (int j = top; j < bmp.Height; j++)//行
                {
                    Color c = bmp.GetPixel(i, j);
                    if (!IsWhite(c))
                    {
                        left = i;
                        find = true;
                        break;
                    }
                }
                if (find)
                    break;
            }
            //寻找最下边标线，从下到上，从左到右
            for (int i = bmp.Height - 1; i >= 0; i--)//行
            {
                bool find = false;
                for (int j = left; j < bmp.Width; j++)//列
                {
                    Color c = bmp.GetPixel(j, i);
                    if (!IsWhite(c))
                    {
                        bottom = i;
                        find = true;
                        break;
                    }
                }
                if (find)
                    break;
            }
            //寻找最右边的标线，从上到下，从右往左
            for (int i = bmp.Width - 1; i >= 0; i--)//列
            {
                bool find = false;
                for (int j = 0; j <= bottom; j++)//行
                {
                    Color c = bmp.GetPixel(i, j);
                    if (!IsWhite(c))
                    {
                        right = i;
                        find = true;
                        break;
                    }
                }
                if (find)
                    break;
            }

            //克隆位图对象的一部分。
            Rectangle cloneRect = new Rectangle(left, top, right - left, bottom - top);
            Bitmap cloneBitmap = bmp.Clone(cloneRect, bmp.PixelFormat);
            cloneBitmap.Save(@"D:/123.png", System.Drawing.Imaging.ImageFormat.Png);
            bmp.Dispose();
            bmp = cloneBitmap;
        }

        /// <summary>
        /// 判断是否白色和纯透明色（10点的容差）
        /// </summary>
        private static bool IsWhite(Color c)
        {
            //纯透明也是白色,RGB都为255为纯白
            if (c.A < 10 || (c.R > 245 && c.G > 245 && c.B > 245))
                return true;

            return false;
        }
        #region IDisposable Members

        void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                    bmp.Dispose();
                    gfx.Dispose();
                    if (GraphicsContext.CurrentContext != null)
                        GL.DeleteTexture(texture);
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //~TextRenderer()
        //{
        //    Console.WriteLine("[Warning] Resource leaked: {0}.", typeof(TextRenderer));
        //}

        #endregion
    }
}
