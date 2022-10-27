using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace Flowing
{ 
    public class IGraphics
    {
        public GameWindow window;
        public int pixelCount;
        public bool smooth;
        public int quality;
        protected bool settingsInited=false;
        protected bool reapplySettings=false;
        protected IGraphics raw;
        protected String path;
        protected bool primarySurface;
        protected bool[] hints = new bool[11];

        static  int STYLE_STACK_DEPTH = 64;
        IStyle[] styleStack = new IStyle[64];
        int styleStackDepth;
        protected int vertexCount=0;

        private float[][] vertices = new float[512][];
        private static    int R = 3;
        private static    int G = 4;
        private static    int B = 5;
        private static    int A = 6;
        private static    int U = 7;
        private static    int V = 8;
        private static    int NX = 9;
        private static    int NY = 10;
        private static    int NZ = 11;
        private static    int EDGE = 12;
        private static    int SR = 13;
        private static    int SG = 14;
        private static    int SB = 15;
        private static    int SA = 16;
        private static    int SW = 17;
        private static    int TX = 18;
        private static    int TY = 19;
        private static    int TZ = 20;
        private static    int VX = 21;
        private static    int VY = 22;
        private static    int VZ = 23;
        private static    int VW = 24;
        private static    int AR = 25;
        private static int AG = 26;
        private static  int AB = 27;
        private static  int DR = 3;
        private static  int DG = 4;
        private static  int DB = 5;
        private static  int DA = 6;
        private static int SPR = 28;
        private static  int SPG = 29;
        private static  int SPB = 30;
        private static  int SHINE = 31;
        private static  int ER = 32;
        private static  int EG = 33;
        private static  int EB = 34;
        private static  int BEEN_LIT = 35;
        private static int HAS_NORMAL = 36;
        private static int VERTEX_FIELD_COUNT = 37;
        public int colorMode;
        private float colorModeX;
        private float colorModeY;
        private float colorModeZ;
        private float colorModeA;
        bool colorModeScale;
        bool colorModeDefault;
        public bool tint;
        public int tintColor;
        private bool tintAlpha;
        private float tintR;
        private float tintG;
        private float tintB;
        private float tintA;
        private int tintRi;
        private int tintGi;
        private int tintBi;
        private int tintAi;
        public bool fill;
        public int fillColor = -1;
        private bool fillAlpha;
        private float fillR;
        private float fillG;
        private float fillB;
        private float fillA;
        private int fillRi;
        private int fillGi;
        private int fillBi;
        private int fillAi;
        public bool stroke;
        public int strokeColor = -16777216;
        private bool strokeAlpha;
        private float strokeR;
        private float strokeG;
        private float strokeB;
        private float strokeA;
        private int strokeRi;
        private int strokeGi;
        private int strokeBi;
        private int strokeAi;
        public int backgroundColor = -3355444;
        private bool backgroundAlpha;
        private float backgroundR;
        private float backgroundG;
        private float backgroundB;
        private float backgroundA;
        private int backgroundRi;
        private int backgroundGi;
        private int backgroundBi;
        private int backgroundAi;
        private int shape;
        private bool autoNormal;
        public int samples;
        //Define styles
        protected static    float DEFAULT_STROKE_WEIGHT = 1.0F;
        protected static    int DEFAULT_STROKE_JOIN = 8;
        protected static    int DEFAULT_STROKE_CAP = 2;
        public float strokeWeight = 1.0F;
        public int strokeJoin = 8;
        public int strokeCap = 2;
        public int rectMode;
        public int ellipseMode;
        public int shapeMode;
        public int imageMode = 0;
        //public PFont textFont;
        public int textAlign = 37;
        public int textAlignY = 0;
        public int textMode = 4;
        public float textSize;
        public float textLeading;
        public int ambientColor;
        public float ambientR;
        public float ambientG;
        public float ambientB;
        public bool setAmbient;
        public int specularColor;
        public float specularR;
        public float specularG;
        public float specularB;
        public int emissiveColor;
        public float emissiveR;
        public float emissiveG;
        public float emissiveB;
        public float shininess;
        public int blendMode;

        //Calculate the color of every element
        protected float calcR;
        protected float calcG;
        protected float calcB;
        protected float calcA;
        protected int calcRi;
        protected int calcGi;
        protected int calcBi;
        protected int calcAi;
        protected int calcColor;
        protected bool calcAlpha;
        private int format;
        public int width;
        public int height;
        public void InitialStyleSettings()
        {
            this.allocate();
            this.defaultSettings();
        }
        public void ReapplyStyleSettings()
        {
            this.allocate();
            this.ReapplySettings();
        }
        protected void allocate()
        {
        }

        protected void ReapplySettings()
        {
            if (this.settingsInited)
            {
                this.ColorMode(this.colorMode, this.colorModeX, this.colorModeY, this.colorModeZ);
                if (this.fill)
                {
                    this.Fill(this.fillColor);
                }
                else
                {
                    this.NoFill();
                }

                if (this.stroke)
                {
                    this.Stroke(this.strokeColor);
                    this.StrokeWeight(this.strokeWeight);
                    this.StrokeCap(this.strokeCap);
                    this.StrokeJoin(this.strokeJoin);
                }
                else
                {
                    this.NoStroke();
                }

                if (this.tint)
                {
                    this.Tint(this.tintColor);
                }
                else
                {
                    this.NoTint();
                }

                if (this.smooth)
                {
                    this.Smooth();
                }
                else
                {
                    this.NoSmooth();
                }

                //if (this.textFont != null)
                //{
                //    float saveLeading = this.textLeading;
                //    this.textFont(this.textFont, this.textSize);
                //    this.textLeading(saveLeading);
                //}

                this.TextMode(this.textMode);
                this.TextAlign(this.textAlign, this.textAlignY);
                this.Background(this.backgroundColor);
                this.BlendMode(this.blendMode);
                this.reapplySettings = false;
            }
        }

        protected void checkSettings()
        {
            if (!this.settingsInited)
            {
                this.defaultSettings();
            }

            if (this.reapplySettings)
            {
                this.ReapplySettings();
            }
        }
        protected void defaultSettings()
        {
            quality = 1;
            if (this.quality > 0)
            {
                this.Smooth();
            }
            else
            {
                this.NoSmooth();
            }

            this.ColorMode(1, 255.0F);
            this.Fill(255);
            this.Stroke(0);
            this.StrokeWeight(1.0F);
            this.StrokeJoin(8);
            this.StrokeCap(2);
            this.shape = 0;
            this.RectMode(0);
            this.EllipseMode(3);
            this.autoNormal = true;
            //this.textFont = null;
            this.textSize = 12.0F;
            this.textLeading = 14.0F;
            this.textAlign = 37;
            this.textMode = 4;
            if (this.primarySurface)
            {
                this.Background(this.backgroundColor);
            }

            this.BlendMode(1);
            this.settingsInited = true;
        }
        public static Object expand(Object array)
        {
            Object[]ob=array as Object[];
            return expand(array, ob.Length << 1);
        }

        public static Object expand(Object list, int newSize)
        {
            Object[] ob = list as Object[];
            Type type= ob[0].GetType();
            Object[] temp = (object[])Array.CreateInstance(type, newSize);
            Array.Copy(ob, 0, temp, 0, Math.Min(ob.Length, newSize));
            return temp;
        }

        public static int[] expand(int[] list)
        {
            return expand(list, list.Length << 1);
        }

        public static int[] expand(int[] list, int newSize)
        {
            int[] temp = new int[newSize];
            System.Array.Copy(list, 0, temp, 0, Math.Min(newSize, list.Length));
            return temp;
        }

        public void PushStyle(bool Continue = true)
        {
            if (this.styleStackDepth == this.styleStack.Length)
            {
                this.styleStack = (IStyle[])expand(this.styleStack);
            }

            if (this.styleStack[this.styleStackDepth] == null)
            {
                this.styleStack[this.styleStackDepth] = new IStyle();
            }

            IStyle s = this.styleStack[this.styleStackDepth++];
            this.getStyle(s);
            if (!Continue)
            {
                this.NoFill();
                this.NoStroke();
            }
        }

        public void PopStyle()
        {
            if (this.styleStackDepth == 0)
            {
                throw new Exception("Too many poIStyle() without enough pushStyle()");
            }
            else
            {
                --this.styleStackDepth;
                this.style(this.styleStack[this.styleStackDepth]);
            }
        }

        public void style(IStyle s)
        {
            this.ImageMode(s.imageMode);
            this.RectMode(s.rectMode);
            this.EllipseMode(s.ellipseMode);
            this.ShapeMode(s.shapeMode);
            this.BlendMode(s.blendMode);
            if (s.tint)
            {
                this.Tint(s.tintColor);
            }
            else
            {
                this.NoTint();
            }

            if (s.fill)
            {
                this.Fill(s.fillColor);
            }
            else
            {
                this.NoFill();
            }

            if (s.stroke)
            {
                this.Stroke(s.strokeColor);
            }
            else
            {
                this.NoStroke();
            }

            this.StrokeWeight(s.strokeWeight);
            this.StrokeCap(s.strokeCap);
            this.StrokeJoin(s.strokeJoin);
            this.ColorMode(1, 1.0F);
            this.ambient(s.ambientR, s.ambientG, s.ambientB);
            this.emissive(s.emissiveR, s.emissiveG, s.emissiveB);
            this.specular(s.specularR, s.specularG, s.specularB);
            this.Shininess(s.shininess);
            this.ColorMode(s.colorMode, s.colorModeX, s.colorModeY, s.colorModeZ, s.colorModeA);
            //if (s.textFont != null)
            //{
            //    this.textFont(s.textFont, s.textSize);
            //    this.textLeading(s.textLeading);
            //}

            this.TextAlign(s.textAlign, s.textAlignY);
            this.TextMode(s.textMode);
        }

        public IStyle getStyle()
        {
            return this.getStyle((IStyle)null);
        }

        public IStyle getStyle(IStyle s)
        {
            if (s == null)
            {
                s = new IStyle();
            }

            s.imageMode = this.imageMode;
            s.rectMode = this.rectMode;
            s.ellipseMode = this.ellipseMode;
            s.shapeMode = this.shapeMode;
            s.blendMode = this.blendMode;
            s.colorMode = this.colorMode;
            s.colorModeX = this.colorModeX;
            s.colorModeY = this.colorModeY;
            s.colorModeZ = this.colorModeZ;
            s.colorModeA = this.colorModeA;
            s.tint = this.tint;
            s.tintColor = this.tintColor;
            s.fill = this.fill;
            s.fillColor = this.fillColor;
            s.stroke = this.stroke;
            s.strokeColor = this.strokeColor;
            s.strokeWeight = this.strokeWeight;
            s.strokeCap = this.strokeCap;
            s.strokeJoin = this.strokeJoin;
            s.ambientR = this.ambientR;
            s.ambientG = this.ambientG;
            s.ambientB = this.ambientB;
            s.specularR = this.specularR;
            s.specularG = this.specularG;
            s.specularB = this.specularB;
            s.emissiveR = this.emissiveR;
            s.emissiveG = this.emissiveG;
            s.emissiveB = this.emissiveB;
            s.shininess = this.shininess;
            //s.textFont = this.textFont;
            s.textAlign = this.textAlign;
            s.textAlignY = this.textAlignY;
            s.textMode = this.textMode;
            s.textSize = this.textSize;
            s.textLeading = this.textLeading;
            return s;
        }

        public void ImageMode(int mode)
        {
            if (mode != 0 && mode != 1 && mode != 3)
            {
                String msg = "imageMode() only works with CORNER, CORNERS, or CENTER";
                throw new Exception(msg);
            }
            else
            {
                this.imageMode = mode;
            }
        }
        public void RectMode(int mode)
        {
            this.rectMode = mode;
        }
        public void ShapeMode(int mode)
        {
            this.shapeMode = mode;
        }
        public void EllipseMode(int mode)
        {
            this.ellipseMode = mode;
        }
        public void BlendMode(int mode)
        {
            this.blendMode = mode;
            this.blendModeImpl();
        }
        public void TextMode(int mode)
        {
            if (mode != 37 && mode != 39)
            {
                if (mode == 256)
                {
                    Print("textMode(SCREEN) has been removed from Processing 2.0.");
                }
                else
                {
                    if (this.textModeCheck(mode))
                    {
                        this.textMode = mode;
                    }
                    else
                    {
                        String modeStr =mode.ToString();
                        switch (mode)
                        {
                            case 4:
                                modeStr = "MODEL";
                                break;
                            case 5:
                                modeStr = "SHAPE";
                                break;
                        }

                        Print("textMode(" + modeStr + ") is not supported by this renderer.");
                    }

                }
            }
            else
            {
                Print("Since Processing 1.0 beta, textMode() is now textAlign().");
                
            }
        }
        protected bool textModeCheck(int mode)
        {
            return true;
        }

        protected void blendModeImpl()
        {
            if (this.blendMode != 1)
            {
                Print("blendMode (), or this particular variation of it, is not available with this renderer.");
            }

        }
        public void setPrimary(bool primary)
        {
            this.primarySurface = primary;
            if (this.primarySurface)
            {
                this.format = 1;
            }

        }
        public void Background(int rgb)
        {
            
            this.colorCalc(rgb);
            this.backgroundFromCalc();
        }
        public static void Print(Object o)
        {
            Console.WriteLine(o.ToString());
        }


        public void Background(int rgb, float alpha)
        {
            this.colorCalc(rgb, alpha);
            this.backgroundFromCalc();
        }

        public void Background(float gray)
        {
            this.colorCalc(gray);
            this.backgroundFromCalc();
        }

        public void Background(float gray, float alpha)
        {
            if (this.format == 1)
            {
                this.Background(gray);
            }
            else
            {
                this.colorCalc(gray, alpha);
                this.backgroundFromCalc();
            }

        }

        public void Background(float v1, float v2, float v3)
        {
            this.colorCalc(v1, v2, v3);
            this.backgroundFromCalc();
        }

        public void Background(float v1, float v2, float v3, float alpha)
        {
            this.colorCalc(v1, v2, v3, alpha);
            this.backgroundFromCalc();
        }
        protected void backgroundFromCalc()
        {
            this.backgroundR = this.calcR;
            this.backgroundG = this.calcG;
            this.backgroundB = this.calcB;
            this.backgroundA = this.format == 1 ? this.colorModeA : this.calcA;
            this.backgroundRi = this.calcRi;
            this.backgroundGi = this.calcGi;
            this.backgroundBi = this.calcBi;
            this.backgroundAi = this.format == 1 ? 255 : this.calcAi;
            this.backgroundAlpha = this.format == 1 ? false : this.calcAlpha;
            this.backgroundColor = this.calcColor;
            this.backgroundImpl();
        }

        protected void backgroundImpl()
        {
            this.PushStyle();
            this.PushMatrix();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.FromArgb(backgroundColor));
            
            this.PopMatrix();
            this.PopStyle();
        }
        protected void PushMatrix()
        {
            GL.PushMatrix();
        }

        protected void PopMatrix()
        {
            GL.PopMatrix();
        }

        public void Smooth()
        {
            this.smooth = true;
        }

        public void Smooth(int level)
        {
            this.smooth = true;
            samples = 2 * (level / 2);
        }

        public void NoSmooth()
        {
            this.smooth = false;
        }
        public void StrokeWeight(float weight)
        {
            this.strokeWeight = weight;
        }
        public void StrokeJoin(int join)
        {
            this.strokeJoin = join;
        }

        public void StrokeCap(int cap)
        {
            this.strokeCap = cap;
        }


        public void NoStroke()
        {
            this.stroke = false;
        }

        public void Stroke(int rgb)
        {
            this.colorCalc(rgb);
            this.strokeFromCalc();
        }

        public void Stroke(int rgb, float alpha)
        {
            this.colorCalc(rgb, alpha);
            this.strokeFromCalc();
        }

        public void Stroke(float gray)
        {
            this.colorCalc(gray);
            this.strokeFromCalc();
        }

        public void Stroke(float gray, float alpha)
        {
            this.colorCalc(gray, alpha);
            this.strokeFromCalc();
        }

        public void Stroke(float v1, float v2, float v3)
        {
            this.colorCalc(v1, v2, v3);
            this.strokeFromCalc();
        }

        public void Stroke(float v1, float v2, float v3, float alpha)
        {
            this.colorCalc(v1, v2, v3, alpha);
            this.strokeFromCalc();
        }

        public void NoFill()
        {
            this.fill = false;
        }

        public void Fill(int rgb)
        {
            this.colorCalc(rgb);
            this.fillFromCalc();
        }

        public void Fill(int rgb, float alpha)
        {
            this.colorCalc(rgb, alpha);
            this.fillFromCalc();
        }

        public void Fill(float gray)
        {
            this.colorCalc(gray);
            this.fillFromCalc();
        }

        public void Fill(float gray, float alpha)
        {
            this.colorCalc(gray, alpha);
            this.fillFromCalc();
        }

        public void Fill(float v1, float v2, float v3)
        {
            this.colorCalc(v1, v2, v3);
            this.fillFromCalc();
        }

        public void Fill(float v1, float v2, float v3, float alpha)
        {
            this.colorCalc(v1, v2, v3, alpha);
            this.fillFromCalc();
        }
        protected void fillFromCalc()
        {
            this.fill = true;
            this.fillR = this.calcR;
            this.fillG = this.calcG;
            this.fillB = this.calcB;
            this.fillA = this.calcA;
            this.fillRi = this.calcRi;
            this.fillGi = this.calcGi;
            this.fillBi = this.calcBi;
            this.fillAi = this.calcAi;
            this.fillColor = this.calcColor;
            this.fillAlpha = this.calcAlpha;
        }
        protected void strokeFromCalc()
        {
            this.stroke = true;
            this.strokeR = this.calcR;
            this.strokeG = this.calcG;
            this.strokeB = this.calcB;
            this.strokeA = this.calcA;
            this.strokeRi = this.calcRi;
            this.strokeGi = this.calcGi;
            this.strokeBi = this.calcBi;
            this.strokeAi = this.calcAi;
            this.strokeColor = this.calcColor;
            this.strokeAlpha = this.calcAlpha;
        }
        public void NoTint()
        {
            this.tint = false;
        }
        public void Tint(int rgb)
        {
            this.colorCalc(rgb);
            this.tintFromCalc();
        }

        public void Tint(int rgb, float alpha)
        {
            this.colorCalc(rgb, alpha);
            this.tintFromCalc();
        }

        public void Tint(float gray)
        {
            this.colorCalc(gray);
            this.tintFromCalc();
        }

        public void Tint(float gray, float alpha)
        {
            this.colorCalc(gray, alpha);
            this.tintFromCalc();
        }

        public void Tint(float v1, float v2, float v3)
        {
            this.colorCalc(v1, v2, v3);
            this.tintFromCalc();
        }

        public void Tint(float v1, float v2, float v3, float alpha)
        {
            this.colorCalc(v1, v2, v3, alpha);
            this.tintFromCalc();
        }

        protected void tintFromCalc()
        {
            this.tint = true;
            this.tintR = this.calcR;
            this.tintG = this.calcG;
            this.tintB = this.calcB;
            this.tintA = this.calcA;
            this.tintRi = this.calcRi;
            this.tintGi = this.calcGi;
            this.tintBi = this.calcBi;
            this.tintAi = this.calcAi;
            this.tintColor = this.calcColor;
            this.tintAlpha = this.calcAlpha;
        }
        public void ambient(int rgb)
        {
            this.colorCalc(rgb);
            this.ambientFromCalc();
        }

        public void ambient(float gray)
        {
            this.colorCalc(gray);
            this.ambientFromCalc();
        }

        public void ambient(float v1, float v2, float v3)
        {
            this.colorCalc(v1, v2, v3);
            this.ambientFromCalc();
        }

        protected void ambientFromCalc()
        {
            this.ambientColor = this.calcColor;
            this.ambientR = this.calcR;
            this.ambientG = this.calcG;
            this.ambientB = this.calcB;
            this.setAmbient = true;
        }

        public void specular(int rgb)
        {
            this.colorCalc(rgb);
            this.specularFromCalc();
        }

        public void specular(float gray)
        {
            this.colorCalc(gray);
            this.specularFromCalc();
        }

        public void specular(float v1, float v2, float v3)
        {
            this.colorCalc(v1, v2, v3);
            this.specularFromCalc();
        }

        protected void specularFromCalc()
        {
            this.specularColor = this.calcColor;
            this.specularR = this.calcR;
            this.specularG = this.calcG;
            this.specularB = this.calcB;
        }

        public void Shininess(float shine)
        {
            this.shininess = shine;
        }

        public void emissive(int rgb)
        {
            this.colorCalc(rgb);
            this.emissiveFromCalc();
        }

        public void emissive(float gray)
        {
            this.colorCalc(gray);
            this.emissiveFromCalc();
        }

        public void emissive(float v1, float v2, float v3)
        {
            this.colorCalc(v1, v2, v3);
            this.emissiveFromCalc();
        }

        protected void emissiveFromCalc()
        {
            this.emissiveColor = this.calcColor;
            this.emissiveR = this.calcR;
            this.emissiveG = this.calcG;
            this.emissiveB = this.calcB;
        }

        public void TextAlign(int alignX)
        {
            this.TextAlign(alignX, 0);
        }

        public void TextAlign(int alignX, int alignY)
        {
            this.textAlign = alignX;
            this.textAlignY = alignY;
        }

        public void ColorMode(int mode)
        {
            this.ColorMode(mode, this.colorModeX, this.colorModeY, this.colorModeZ, this.colorModeA);
        }

        public void ColorMode(int mode, float max)
        {
            this.ColorMode(mode, max, max, max, max);
        }

        public void ColorMode(int mode, float max1, float max2, float max3)
        {
            this.ColorMode(mode, max1, max2, max3, this.colorModeA);
        }

        public void ColorMode(int mode, float max1, float max2, float max3, float maxA)
        {
            this.colorMode = mode;
            this.colorModeX = max1;
            this.colorModeY = max2;
            this.colorModeZ = max3;
            this.colorModeA = maxA;
            this.colorModeScale = maxA != 1.0F || max1 != max2 || max2 != max3 || max3 != maxA;
            this.colorModeDefault = this.colorMode == 1 && this.colorModeA == 255.0F && this.colorModeX == 255.0F && this.colorModeY == 255.0F && this.colorModeZ == 255.0F;
        }

        public void Translate(params float[] p)
        {
            if (p.Length < 2 || p.Length > 3)
            {
                throw new Exception("the nums of parameters should larger than 1 and smaller than 4");
            }
            else
            {

                if (p.Length == 2)
                {
                    GL.Translate(p[0], p[1], 0);
                }
                else
                {
                    GL.Translate(p[0], p[1],p[2]);
                }
            }
        }
        protected void vertexCheck()
        {
            if (this.vertexCount == this.vertices.Length)
            {
                float[][] temp = new float[this.vertexCount << 1][];
                Array.Copy(this.vertices, 0, temp, 0, this.vertexCount);
                Print(this.vertices.Length);
                this.vertices = temp;
            }

        }

        public void Vertex(params float[]p)
        {
            this.vertexCheck();
            if (p.Length < 2 || p.Length > 3)
            {
                throw new Exception("the nums of parameters should larger than 1 and smaller than 4");
            }else
            {
                float[] vertex = vertices[this.vertexCount] = new float[37];
                if (p.Length == 2)
                {
                   
                    vertex[0] = p[0];
                    vertex[1] = p[1];
                    vertex[2] = 0.0f;
                }
                else
                { 
                    vertex[0] = p[0];
                    vertex[1] = p[1];
                    vertex[2] = p[2];
                }
                this.vertexCount++;
            }
        }

        //private void GatherSettings()
        //{
        //    if (fill)
        //    {
        //        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        //    }
        //    if (stroke)
        //    {
        //        GL.
        //    }
        //}

        public void BeginShape()
        {
            BeginShape(PrimitiveType.Polygon);
        }
        private int LastIndex = 0;

        public void BeginShape(PrimitiveType p)
        {
            this.shape = ((int)p);
            
        }


        int getMultiSampleTexture(int samples)
        {
            int texture;
            GL.GenTextures(1, out texture);
            GL.BindTexture(TextureTarget.Texture2DMultisample, texture);
            GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, samples, PixelInternalFormat.Rgb, width, height, true);
            GL.BindTexture(TextureTarget.Texture2DMultisample, 0); ;
            return texture;
        }
        public void EndShape()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            if (smooth)
            {

                
                //GL.Enable(EnableCap.PointSmooth);
                //GL.Enable(EnableCap.LineSmooth);
                //GL.Enable(EnableCap.PolygonSmooth);

                //GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
                //GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);

            }
            if (fill)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                
                GL.Begin((PrimitiveType)this.shape);
                GL.Color4(Color.FromArgb(fillColor));

                for (int i =0; i < this.vertexCount; i++)
                { 
                    GL.Vertex3(vertices[i][0], vertices[i][1], vertices[i][2]);
                }
                GL.End();
            }


            if (stroke)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.Color4(Color.FromArgb(strokeColor));
                GL.LineWidth(strokeWeight);
                GL.Begin((PrimitiveType)this.shape);
                for (int i = 0; i < this.vertexCount; i++)
                {
                    GL.Vertex3(vertices[i][0], vertices[i][1], vertices[i][2]);
                }
                GL.End();
            }

            GL.Disable(EnableCap.Blend);
            //GL.Viewport(0, 0, window.Width, window.Height);

            //LastIndex = this.vertexCount;

            this.vertices = new float[512][];
            this.vertexCount = 0;
        }

        protected void CreateCube()
        {
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(1.0, 1.0, 0.0);
            GL.Vertex3(-10.0, 10.0, 10.0);
            GL.Vertex3(-10.0, 10.0, -10.0);
            GL.Vertex3(-10.0, -10.0, -10.0);
            GL.Vertex3(-10.0, -10.0, 10.0);

            GL.Color3(1.0, 0.0, 1.0);
            GL.Vertex3(10.0, 10.0, 10.0);
            GL.Vertex3(10.0, 10.0, -10.0);
            GL.Vertex3(10.0, -10.0, -10.0);
            GL.Vertex3(10.0, -10.0, 10.0);

            GL.Color3(0.0, 1.0, 1.0);
            GL.Vertex3(10.0, -10.0, 10.0);
            GL.Vertex3(10.0, -10.0, -10.0);
            GL.Vertex3(-10.0, -10.0, -10.0);
            GL.Vertex3(-10.0, -10.0, 10.0);

            GL.Color3(1.0, 0.0, 0.0);
            GL.Vertex3(10.0, 10.0, 10.0);
            GL.Vertex3(10.0, 10.0, -10.0);
            GL.Vertex3(-10.0, 10.0, -10.0);
            GL.Vertex3(-10.0, 10.0, 10.0);

            GL.Color3(0.0, 1.0, 0.0);
            GL.Vertex3(10.0, 10.0, -10.0);
            GL.Vertex3(10.0, -10.0, -10.0);
            GL.Vertex3(-10.0, -10.0, -10.0);
            GL.Vertex3(-10.0, 10.0, -10.0);

            GL.Color3(0.0, 0.0, 1.0);
            GL.Vertex3(10.0, 10.0, 10.0);
            GL.Vertex3(10.0, -10.0, 10.0);
            GL.Vertex3(-10.0, -10.0, 10.0);
            GL.Vertex3(-10.0, 10.0, 10.0);
            GL.End();
        }
        protected void Cube(float length, float width, float height)
        {
            BeginShape(PrimitiveType.Quads);

            Vertex(-length / 2, width / 2, height / 2);
            Vertex(-length / 2, width / 2, -height / 2);
            Vertex(-length / 2, -width / 2, -height / 2);
            Vertex(-length / 2, -width / 2, height / 2);


            Vertex(length / 2, width / 2, height / 2);
            Vertex(length / 2, width / 2, -height / 2);
            Vertex(length / 2, -width / 2, -height / 2);
            Vertex(length / 2, -width / 2, height / 2);

            Vertex(length / 2, -width / 2, height / 2);
            Vertex(length / 2, -width / 2, -height / 2);
            Vertex(-length / 2, -width / 2, -height / 2);
            Vertex(-length / 2, -width / 2, height / 2);

            Vertex(length / 2, width / 2, height / 2);
            Vertex(length / 2, width / 2, -height / 2);
            Vertex(-length / 2, width / 2, -height / 2);
            Vertex(-length / 2, width / 2, height / 2);


            Vertex(length / 2, width / 2, -height / 2);
            Vertex(length / 2, -width / 2, -height / 2);
            Vertex(-length / 2, -width / 2, -height / 2);
            Vertex(-length / 2, width / 2, -height / 2);

            Vertex(length / 2, width / 2, height / 2);
            Vertex(length / 2, -width / 2, height / 2);
            Vertex(-length / 2, -width / 2, height / 2);
            Vertex(-length / 2, width / 2, height / 2);
            EndShape();
        }
        public void Line(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            this.NoFill();
            this.BeginShape(PrimitiveType.Lines);
            this.Vertex(x1, y1, z1);
            this.Vertex(x2, y2, z2);
            this.EndShape();
        }



        protected void colorCalc(int rgb)
        {
            if ((rgb & -16777216) == 0 && (float)rgb <= this.colorModeX)
            {
                this.colorCalc((float)rgb);
            }
            else
            {
                this.colorCalcARGB(rgb, this.colorModeA);
            }

        }

        protected void colorCalc(int rgb, float alpha)
        {
            if ((rgb & -16777216) == 0 && (float)rgb <= this.colorModeX)
            {
                this.colorCalc((float)rgb, alpha);
            }
            else
            {
                this.colorCalcARGB(rgb, alpha);
            }

        }

        protected void colorCalc(float gray)
        {
            this.colorCalc(gray, this.colorModeA);
        }
        protected void colorCalc(float gray, float alpha)
        {
            if (gray > this.colorModeX)
            {
                gray = this.colorModeX;
            }

            if (alpha > this.colorModeA)
            {
                alpha = this.colorModeA;
            }

            if (gray < 0.0F)
            {
                gray = 0.0F;
            }

            if (alpha < 0.0F)
            {
                alpha = 0.0F;
            }

            this.calcR = this.colorModeScale ? gray / this.colorModeX : gray;
            this.calcG = this.calcR;
            this.calcB = this.calcR;
            this.calcA = this.colorModeScale ? alpha / this.colorModeA : alpha;
            this.calcRi = (int)(this.calcR * 255.0F);
            this.calcGi = (int)(this.calcG * 255.0F);
            this.calcBi = (int)(this.calcB * 255.0F);
            this.calcAi = (int)(this.calcA * 255.0F);
            this.calcColor = this.calcAi << 24 | this.calcRi << 16 | this.calcGi << 8 | this.calcBi;
            this.calcAlpha = this.calcAi != 255;
        }
        protected void colorCalc(float x, float y, float z)
        {
            this.colorCalc(x, y, z, this.colorModeA);
        }

        protected void colorCalc(float x, float y, float z, float a)
        {
            if (x > this.colorModeX)
            {
                x = this.colorModeX;
            }

            if (y > this.colorModeY)
            {
                y = this.colorModeY;
            }

            if (z > this.colorModeZ)
            {
                z = this.colorModeZ;
            }

            if (a > this.colorModeA)
            {
                a = this.colorModeA;
            }

            if (x < 0.0F)
            {
                x = 0.0F;
            }

            if (y < 0.0F)
            {
                y = 0.0F;
            }

            if (z < 0.0F)
            {
                z = 0.0F;
            }

            if (a < 0.0F)
            {
                a = 0.0F;
            }

            switch (this.colorMode)
            {
                case 1:
                    if (this.colorModeScale)
                    {
                        this.calcR = x / this.colorModeX;
                        this.calcG = y / this.colorModeY;
                        this.calcB = z / this.colorModeZ;
                        this.calcA = a / this.colorModeA;
                    }
                    else
                    {
                        this.calcR = x;
                        this.calcG = y;
                        this.calcB = z;
                        this.calcA = a;
                    }
                    break;
                case 2:
                default:
                    break;
                case 3:
                    x /= this.colorModeX;
                    y /= this.colorModeY;
                    z /= this.colorModeZ;
                    this.calcA = this.colorModeScale ? a / this.colorModeA : a;
                    if (y == 0.0F)
                    {
                        this.calcR = this.calcG = this.calcB = z;
                    }
                    else
                    {
                        float which = (x - (float)((int)x)) * 6.0F;
                        float f = which - (float)((int)which);
                        float p = z * (1.0F - y);
                        float q = z * (1.0F - y * f);
                        float t = z * (1.0F - y * (1.0F - f));
                        switch ((int)which)
                        {
                            case 0:
                                this.calcR = z;
                                this.calcG = t;
                                this.calcB = p;
                                break;
                            case 1:
                                this.calcR = q;
                                this.calcG = z;
                                this.calcB = p;
                                break;
                            case 2:
                                this.calcR = p;
                                this.calcG = z;
                                this.calcB = t;
                                break;
                            case 3:
                                this.calcR = p;
                                this.calcG = q;
                                this.calcB = z;
                                break;
                            case 4:
                                this.calcR = t;
                                this.calcG = p;
                                this.calcB = z;
                                break;
                            case 5:
                                this.calcR = z;
                                this.calcG = p;
                                this.calcB = q;
                                break;
                        }
                    }
                    break;
            }

            this.calcRi = (int)(255.0F * this.calcR);
            this.calcGi = (int)(255.0F * this.calcG);
            this.calcBi = (int)(255.0F * this.calcB);
            this.calcAi = (int)(255.0F * this.calcA);
            this.calcColor = this.calcAi << 24 | this.calcRi << 16 | this.calcGi << 8 | this.calcBi;
            this.calcAlpha = this.calcAi != 255;
        }
        protected void colorCalcARGB(int argb, float alpha)
        {
            if (alpha == this.colorModeA)
            {
                this.calcAi = argb >> 24 & 255;
                this.calcColor = argb;
            }
            else
            {
                this.calcAi = (int)((float)(argb >> 24 & 255) * IApp.constrain(alpha / this.colorModeA, 0.0F, 1.0F));
                this.calcColor = this.calcAi << 24 | argb & 16777215;
            }

            this.calcRi = argb >> 16 & 255;
            this.calcGi = argb >> 8 & 255;
            this.calcBi = argb & 255;
            this.calcA = (float)this.calcAi / 255.0F;
            this.calcR = (float)this.calcRi / 255.0F;
            this.calcG = (float)this.calcGi / 255.0F;
            this.calcB = (float)this.calcBi / 255.0F;
            this.calcAlpha = this.calcAi != 255;
        }

        public void Rotate(float angle)
        {
            GL.Rotate(angle, new Vector3d(0, 0, 1));
        }

        public void RotateX(float angle)
        {
            GL.Rotate(angle, new Vector3d(1, 0, 0));

        }
        public void RotateY(float angle)
        {
            GL.Rotate(angle, new Vector3d(0, 1, 0));

        }
        public void RotateZ(float angle)
        {
            GL.Rotate(angle, new Vector3d(0, 0, 1));

        }

        public void Rotate(double angle, float x, float y, float z)
        {
            GL.Rotate(angle, new Vector3d(x, y, z));

        }

        public void scale(float s)
        {
            GL.Scale(new Vector3d(s, s, s));

        }
        public void scale(float x, float y)
        {
            GL.Scale(new Vector3d(x, y, 1));
        }

        public void scale(float x, float y, float z)
        {
            GL.Scale(new Vector3d(x, y, z));
        }


        //public void ResetMatrix()
        //{
        //    GL.LoadMatrix();
        //}

        public void GetMatrix()
        {
            float[] matrix = new float[6];
            GL.GetFloat(GetPName.ProjectionMatrix,matrix);

        }
       public void Perspective()
        {
            
        }
        public void Ortho(float left, float right, float bottom, float top, float near, float far)
        {
            GL.Ortho(left, right, bottom, top, near, far);
        }

        public void Frustum(float left, float right, float bottom, float top, float near, float far)
        {
            GL.Frustum(left, right, bottom, top, near, far);
        }
    }



}
