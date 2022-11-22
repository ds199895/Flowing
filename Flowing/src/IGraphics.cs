using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Flowing.Triangulation;
using OpenTK.Graphics;

namespace Flowing
{
    public class IGraphics:IConstants
    {
        public enum EndMode
        {
            Close = 1,
            Open = 2
        }
        

        public GameWindow window;
        TextRenderer TextRenderer;
        public int pixelCount;
        public bool smooth;
        public int quality;
        public bool settingsInited = false;
        public bool reapplySettings = false;
        public IGraphics raw;
        public String path;
        public bool primarySurface;
        public bool[] hints = new bool[11];

        static int STYLE_STACK_DEPTH = 64;
        IStyle[] styleStack = new IStyle[64];
        int styleStackDepth;
        public int vertexCount = 0;

        private float[][] vertices = new float[512][];
        private static int R = 3;
        private static int G = 4;
        private static int B = 5;
        private static int A = 6;
        private static int U = 7;
        private static int V = 8;
        private static int NX = 9;
        private static int NY = 10;
        private static int NZ = 11;
        private static int EDGE = 12;
        private static int SR = 13;
        private static int SG = 14;
        private static int SB = 15;
        private static int SA = 16;
        private static int SW = 17;
        private static int TX = 18;
        private static int TY = 19;
        private static int TZ = 20;
        private static int VX = 21;
        private static int VY = 22;
        private static int VZ = 23;
        private static int VW = 24;
        private static int AR = 25;
        private static int AG = 26;
        private static int AB = 27;
        private static int DR = 3;
        private static int DG = 4;
        private static int DB = 5;
        private static int DA = 6;
        private static int SPR = 28;
        private static int SPG = 29;
        private static int SPB = 30;
        private static int SHINE = 31;
        private static int ER = 32;
        private static int EG = 33;
        private static int EB = 34;
        private static int BEEN_LIT = 35;
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
        public static float DEFAULT_STROKE_WEIGHT = 1.0F;
        public static int DEFAULT_STROKE_JOIN = 8;
        public static int DEFAULT_STROKE_CAP = 2;
        public float strokeWeight = 1.0F;
        public int strokeJoin = 8;
        public int strokeCap = 2;
        public int rectMode;
        public int ellipseMode;
        public int shapeMode;
        public int imageMode = 0;
        public Font textFont;
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
        public float calcR;
        public float calcG;
        public float calcB;
        public float calcA;
        public int calcRi;
        public int calcGi;
        public int calcBi;
        public int calcAi;
        public int calcColor;
        public bool calcAlpha;
        private int format;
        public int width;
        public int height;

        protected float[] sphereX;
        protected float[] sphereY;
        protected float[] sphereZ;

        public int sphereDetailU = 0;
        public int sphereDetailV = 0;

        public bool wireFrame = false;

        protected static float[] sinLUT = new float[720];
        protected static float[] cosLUT = new float[720];

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
        public void allocate()
        {
        }

        public void ReapplySettings()
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

                if (this.textFont != null)
                {
                    float saveLeading = this.textLeading;
                    this.TextFont(this.textFont, this.textSize);
                    this.TextLeading(saveLeading);
                }

                this.TextMode(this.textMode);
                this.TextAlign(this.textAlign, this.textAlignY);
                this.Background(this.backgroundColor);
                this.BlendMode(this.blendMode);
                this.reapplySettings = false;
            }
        }

        public void checkSettings()
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
        public void defaultSettings()
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


            this.textSize = 12.0F;
            this.textFont = createDefaultFont(textSize);
            this.textLeading = 14.0F;
            this.textAlign = 37;
            this.textMode = 4;
            if (this.primarySurface)
            {
                this.Background(this.backgroundColor);
            }

            this.BlendMode(1);
            this.settingsInited = true;
            for (int i = 0; i < 720; ++i)
            {
                sinLUT[i] = (float)Math.Sin((double)((float)i * 0.017453292F * 0.5F));
                cosLUT[i] = (float)Math.Cos((double)((float)i * 0.017453292F * 0.5F));
            }
        }
        public static Object expand(Object array)
        {
            Object[] ob = array as Object[];
            return expand(array, ob.Length << 1);
        }

        public static Object expand(Object list, int newSize)
        {
            Object[] ob = list as Object[];
            Type type = ob[0].GetType();
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
        public void TextFont(Font which)
        {
            if (which != null)
            {
                this.textFont = which;
                this.TextSize((float)which.Size);
            }
            else
            {
                throw new Exception("A null Font was passed to textFont()");
            }
        }

        public void TextFont(Font which, float size)
        {
            this.TextFont(which);
            this.TextSize(size);
        }
        public void TextSize(float size)
        {
            if (this.textFont == null)
            {
                this.defaultFontOrDeath("textSize", size);
            }

            this.textSize = size;

            this.textFont = new Font(this.textFont.Name, this.textSize);
            
            
            //this.textLeading = (this.textAscent() + this.textDescent()) * 1.275F;
        }
        public void defaultFontOrDeath(String method, float size)
        {
            this.textFont = this.createDefaultFont(size);
        }
        public Font createDefaultFont(float size)
        {
            return this.createFont("Lucida Sans", size);
        }

        public Font createFont(String name, float size)
        {
            return new Font(name, size);
        }
        public void TextLeading(float leading)
        {
            this.textLeading = leading;
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
            if (s.textFont != null)
            {
                this.TextFont(s.textFont, s.textSize);
                this.TextLeading(s.textLeading);
            }

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
            s.textFont = this.textFont;
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
                        String modeStr = mode.ToString();
                        switch (mode)
                        {
                            case 4:
                                modeStr = "MODEL";
                                break;
                            case 5:
                                modeStr = "SHAPE";
                                break;
                        }

                        Print("textMode(" + modeStr + ") is not supported by this TextRenderer.");
                    }

                }
            }
            else
            {
                Print("Since Processing 1.0 beta, textMode() is now textAlign().");

            }
        }
        public bool textModeCheck(int mode)
        {
            return true;
        }

        public void blendModeImpl()
        {
            if (this.blendMode != 1)
            {
                Print("blendMode (), or this particular variation of it, is not available with this TextRenderer.");
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
        public void backgroundFromCalc()
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

        public void backgroundImpl()
        {
            this.PushStyle();
            this.PushMatrix();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color.FromArgb(backgroundColor));

            this.PopMatrix();
            this.PopStyle();
        }
        public void PushMatrix()
        {
            GL.PushMatrix();
        }

        public void PopMatrix()
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
        public void fillFromCalc()
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
        public void strokeFromCalc()
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

        public void tintFromCalc()
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

        public void ambientFromCalc()
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

        public void specularFromCalc()
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

        public void emissiveFromCalc()
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
                    GL.Translate(p[0], p[1], p[2]);
                }
            }
        }
        public void vertexCheck()
        {
            if (this.vertexCount == this.vertices.Length)
            {
                float[][] temp = new float[this.vertexCount << 1][];
                Array.Copy(this.vertices, 0, temp, 0, this.vertexCount);
                Print(this.vertices.Length);
                this.vertices = temp;
            }

        }

        public void Vertex(params float[] p)
        {
            this.vertexCheck();
            if (p.Length < 2 || p.Length > 3)
            {
                throw new Exception("the nums of parameters should larger than 1 and smaller than 4");
            }
            else
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

        //public void BeginShape()
        //{
        //    BeginShape(PrimitiveType.Polygon);
        //}
        public bool hole = false;
        private int cutIndex = 0;
        private List<int> cutIndices = new List<int>();
        public void BeginShape(bool hole = false)
        {

            this.hole = hole;
            if (!hole)
            {

                BeginShape(PrimitiveType.Polygon);
                //cutIndices.Add(0);
            }
            else
            {
                //Vertex(vertices[0][0],vertices[0][1],vertices[0][2]);
                //Vertex(vertices[cutIndex][0], vertices[cutIndex][1], vertices[cutIndex][2]);
                cutIndex = this.vertexCount;
                cutIndices.Add(cutIndex);
            }
        }
        private int LastIndex = 0;

        public void BeginShape(PrimitiveType p)
        {
            this.shape = ((int)p);

        }


        public void EndShape(EndMode endMode = EndMode.Close)
        {
            if (this.vertexCount > 0)
            {
                if (hole)
                {
                    Vertex(vertices[cutIndex][0], vertices[cutIndex][1], vertices[cutIndex][2]);

                    hole = false;
                }
                else
                {


                    cutIndices.Add(this.vertexCount);
                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

                    if (smooth)
                    {
                        GL.Enable(EnableCap.PointSmooth);
                        GL.Enable(EnableCap.LineSmooth);
                        GL.Enable(EnableCap.PolygonSmooth);

                        GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
                        GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);

                    }

                    if (fill)
                    {
                        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                        GL.Color4(Color.FromArgb(fillColor));
                        if (this.shape != (int)PrimitiveType.Polygon)
                        {
                            GL.Begin((PrimitiveType)this.shape);
                            for(int i = 0; i < this.vertexCount; i++)
                            {
                                GL.Vertex3(vertices[i][0], vertices[i][1], vertices[i][2]);
                            }
                            

                            GL.End();
                        }
                        else
                        {
                            DrawPolygonTrianglesFill(vertices, cutIndices);
                        }
                        
                    }


                    if (stroke)
                    {
                        GL.Enable(EnableCap.PolygonOffsetLine);
                        GL.PolygonOffset(-1.0f, -1.0f);

                        GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                        GL.Color4(Color.FromArgb(strokeColor));
                        GL.LineWidth(strokeWeight);

                        if (this.shape != (int)PrimitiveType.Polygon)
                        {
                            GL.Begin((PrimitiveType)this.shape);
                            for (int i = 0; i <this.vertexCount; i++)
                            {
                                GL.Vertex3(vertices[i][0], vertices[i][1], vertices[i][2]);
                            }


                            GL.End();
                        }
                        else
                        {
                            DrawPolygonTrianglesStroke(vertices, cutIndices);
                        }


                        
                    }
                    GL.Disable(EnableCap.PolygonOffsetLine);
                    GL.Disable(EnableCap.Blend);

                    this.vertices = new float[512][];
                    this.vertexCount = 0;
                    this.cutIndex = 0;
                    this.cutIndices.Clear();
                }
            }
            //GL.Disable(EnableCap.DepthTest);
        }
        private void DrawPolygonTrianglesFill(float[][] vecs, List<int> indices)
        {
            Vector3[][] triangles;
            Triangulate(vecs, this.vertexCount, out triangles);

            if (!wireFrame)
            {
                GL.Enable(EnableCap.PolygonOffsetFill);
                GL.PolygonOffset(1.0f, 1.0f);
                for (int i = 0; i < triangles.Length; i++)
                {
                    GL.Begin(PrimitiveType.Triangles);
                    GL.Vertex3(triangles[i][0].X, triangles[i][0].Y, triangles[i][0].Z);
                    GL.Vertex3(triangles[i][1].X, triangles[i][1].Y, triangles[i][1].Z);
                    GL.Vertex3(triangles[i][2].X, triangles[i][2].Y, triangles[i][2].Z);
                    GL.End();
                }
                GL.Disable(EnableCap.PolygonOffsetFill);
            }
            else{
                //绘制内部网格线
                //GL.Enable(EnableCap.PolygonOffsetLine);
                //GL.PolygonOffset(-2.0f, -2.0f);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.Color4(Color.FromArgb(strokeColor));
                GL.LineWidth(strokeWeight);
                for (int i = 0; i < triangles.Length; i++)
                {

                    GL.Begin(PrimitiveType.Triangles);
                    GL.Vertex3(triangles[i][0].X, triangles[i][0].Y, triangles[i][0].Z);
                    GL.Vertex3(triangles[i][1].X, triangles[i][1].Y, triangles[i][1].Z);
                    GL.Vertex3(triangles[i][2].X, triangles[i][2].Y, triangles[i][2].Z);
                    GL.End();
                }
                //GL.Disable(EnableCap.PolygonOffsetLine);
            }
            

            //depreciated 耳切法
            //List<Vector3> triangles;
            //Triangulation(vecs, indices, this.vertexCount, out triangles);

            //for (int i = 0; i < triangles.Count; i += 3)
            //{
            //    //Print(va);
            //    //Print(vb);
            //    GL.Begin(PrimitiveType.Triangles);
            //    GL.Vertex3(triangles[i].X, triangles[i].Y, triangles[i].Z);
            //    GL.Vertex3(triangles[i + 1].X, triangles[i + 1].Y, triangles[i + 1].Z);
            //    GL.Vertex3(triangles[i + 2].X, triangles[i + 2].Y, triangles[i + 2].Z);
            //    //Line(va.xf, va.yf, va.zf, vb.xf, vb.yf, vb.zf);
            //    //Line(vb.xf, vb.yf, vb.zf, vc.xf, vc.yf, vc.zf);
            //    //Line(vc.xf, vc.yf, vc.zf, va.xf, va.yf, va.zf);
            //    GL.End();
            //}
        }
        private void DrawPolygonTrianglesStroke(float[][] vecs, List<int> indices)
        {
            if (indices.Count > 0)
            {
                int prevIndex = 0;
                for (int i = 0; i < indices.Count; i++)
                {
                    GL.Begin((PrimitiveType)this.shape);

                    for (int j = prevIndex; j < indices[i]; j++)
                    {

                        GL.Vertex3(vecs[j]);
                    }

                    GL.End();
                    prevIndex = indices[i] + 1;
                }
            }
        }

        private bool cutHoles(float[][] vecs, out List<int> cutIndices)
        {
            int index = 0;
            bool cut = false;
            cutIndices = new List<int>();
            //cutIndices.Add(index);

            //Print("num: " + vecs[index][0]+" "+ vecs[index][1]+" "+ vecs[index][2]);
            for (int i = 0; i < this.vertexCount; i++)
            {
                if (i != index)
                {
                    int nxt = i + 1;
                    //Print("set: "+ vecs[index][0] + " " + vecs[index][1] + " " + vecs[index][2]);
                    if (vecs[i][0] == vecs[index][0] && vecs[i][1] == vecs[index][1] && vecs[i][2] == vecs[index][2])
                    {

                        index = nxt;
                        cutIndices.Add(index);
                        cut = true;
                        //Print("nxt: " + index);
                    }
                }

            }

            //cutIndices.Add(this.vertexCount);
            return cut;
        }

        private static void Triangulation(float[][] vecs, List<int> indices, int vertexCount, out List<Vector3> vertices)
        {

            vertices = new List<Vector3>();
            if (indices.Count > 0)
            {

                List<List<Vector3m>> holes = new List<List<Vector3m>>();
                List<Vector3m> shell = new List<Vector3m>();


                //Print("test index: " + indices[0]);
                //for (int i = 0; i < indices[0]; i++)
                //{

                //    Vector3m p = new Vector3m(vecs[i][0], vecs[i][1], vecs[i][2]);
                //    shell.Add(p);
                //}
                //Print(shell.Count);
                int prevIndex = 0;
                for (int i = 0; i < indices.Count; i++)
                {
                    List<Vector3m> points = new List<Vector3m>();
                    for (int j = prevIndex; j < indices[i]; j++)
                    {
                        Vector3m p;

                        p = new Vector3m(vecs[j][0], vecs[j][1], vecs[j][2]);

                        points.Add(p);
                    }
                    if (prevIndex != 0)
                    {
                        //Print("holes points num :" + points.Count);
                        holes.Add(points);
                    }
                    else
                    {
                        shell = points;
                    }

                    prevIndex = indices[i] + 1;
                }
                //Print("holes num :" + holes.Count);

                EarClipping earClipping = new EarClipping();
                earClipping.SetPoints(shell, holes);
                earClipping.Triangulate();
                //var res = earClipping.ResultIndex;

                //foreach (int v in res)
                //{
                //    vertices.Add(new Vector3((float)vecs[v][0], (float)vecs[v][1], (float)vecs[v][2]));
                //}
                var res = earClipping.Result;

                foreach (Vector3m v in res)
                {
                    vertices.Add(new Vector3((float)v.X.ToDouble(), (float)v.Y.ToDouble(), (float)v.Z.ToDouble()));
                }
            }


        }
        private static object VertexCombine(LibTessDotNet.Vec3 position, object[] data, float[] weights)
        {
            // Fetch the vertex data.
            var colors = new Color[] { (Color)data[0], (Color)data[1], (Color)data[2], (Color)data[3] };
            // Interpolate with the 4 weights.
            var rgba = new float[] {
                (float)colors[0].R * weights[0] + (float)colors[1].R * weights[1] + (float)colors[2].R * weights[2] + (float)colors[3].R * weights[3],
                (float)colors[0].G * weights[0] + (float)colors[1].G * weights[1] + (float)colors[2].G * weights[2] + (float)colors[3].G * weights[3],
                (float)colors[0].B * weights[0] + (float)colors[1].B * weights[1] + (float)colors[2].B * weights[2] + (float)colors[3].B * weights[3],
                (float)colors[0].A * weights[0] + (float)colors[1].A * weights[1] + (float)colors[2].A * weights[2] + (float)colors[3].A * weights[3]
            };
            // Return interpolated data for the new vertex.
            return Color.FromArgb((int)rgba[3], (int)rgba[0], (int)rgba[1], (int)rgba[2]);
        }
        private void Triangulate(float[][] vecs, int vertexCount, out Vector3[][] vertices)
        {

            List<List<Vector3>> holes = new List<List<Vector3>>();
            List<Vector3> shell = new List<Vector3>();
            List<Vector3> sortedVertices = new List<Vector3>();

            if (cutIndices.Count > 0)
            {
                int prevIndex = 0;
                for (int i = 0; i < cutIndices.Count; i++)
                {
                    List<Vector3> points = new List<Vector3>();
                    for (int j = prevIndex; j < cutIndices[i]; j++)
                    {
                        Vector3 p;

                        p = new Vector3(vecs[j][0], vecs[j][1], vecs[j][2]);

                        points.Add(p);
                    }
                    points.Add(points[0]);
                    if (prevIndex != 0)
                    {
                        holes.Add(points);

                    }
                    else
                    {
                        shell = points;

                        sortedVertices.AddRange(shell);
                    }

                    prevIndex = cutIndices[i];
                }
            }
            if (holes.Count > 0)
            {
                Vector3 start = holes[0][0];
                int Id = -1;

                double MinDis = double.MaxValue;
                List<Vector3> candidates = new List<Vector3>();


                List<Vector3> holePts = new List<Vector3>();
                for (int i = 0; i < holes.Count; i++)
                {
                    for (int j = 0; j < holes[i].Count; j++)
                    {
                        holePts.Add(holes[i][j]);
                    }

                }
                //find the first shell points which has no intersection with others
                for (int i = 0; i < shell.Count; i++)
                {
                    int count = 0;
                    for (int j = 0; j < shell.Count; j++)
                    {
                        int nxt = (j + 1) % holePts.Count;
                        bool intersect = getIntersection2D(start, shell[i], shell[j], shell[nxt], out Vector3 inter);
                        if (intersect)
                        {
                            //Print("shell "+i+ " intersect "+j+" "+nxt);
                            count++;
                        }
                    }

                    for (int j = 0; j < holePts.Count; j++)
                    {
                        int nxt = (j + 1) % holePts.Count;
                        bool intersect = getIntersection2D(start, shell[i], holePts[j], holePts[nxt], out Vector3 inter);
                        if (intersect)
                        {
                            count++;
                        }
                    }
                    if (count == 0)
                    {
                        Id = i;
                        break;
                    }
                }
                //Print("Id: "+Id);
                //sort the shell points according to the Id founded;
                for (int i = 0; i < shell.Count; i++)
                {
                    int index = (i - Id + shell.Count) % shell.Count;
                    sortedVertices.RemoveAt(index);
                    sortedVertices.Insert(index, shell[i]);
                }
                //circulate the shell points
                sortedVertices.Add(sortedVertices[0]);

                for (int i = 0; i < holes.Count; i++)
                {
                    sortedVertices.AddRange(holes[i]);
                }

                //circulate the hole points
                sortedVertices.Add(holes[0][0]);

            }



            // Create an instance of the tessellator. Can be reused.
            var tess = new LibTessDotNet.Tess();

            // Construct the contour from inputData.
            // A polygon can be composed of multiple contours which are all tessellated at the same time.
            //int numPoints = inputData.Length/2;

            int numPoints = sortedVertices.Count;
            var contour = new LibTessDotNet.ContourVertex[numPoints];
            for (int i = 0; i < numPoints; i++)
            {
                // NOTE : Z is here for convenience if you want to keep a 3D vertex position throughout the tessellation process but only X and Y are important.
                //contour[i].Position = new LibTessDotNet.Vec3(inputData[i * 2], inputData[i * 2 + 1], 0);
                contour[i].Position = new LibTessDotNet.Vec3(sortedVertices[i].X, sortedVertices[i].Y, sortedVertices[i].Z);
                // Data can contain any per-vertex data, here a constant color.
                contour[i].Data = Color.Azure;
            }
            // Add the contour with a specific orientation, use "Original" if you want to keep the input orientation.
            tess.AddContour(contour, LibTessDotNet.ContourOrientation.Clockwise);

            // Tessellate!
            // The winding rule determines how the different contours are combined together.
            // See http://www.glprogramming.com/red/chapter11.html (section "Winding Numbers and Winding Rules") for more information.
            // If you want triangles as output, you need to use "Polygons" type as output and 3 vertices per polygon.
            tess.Tessellate(LibTessDotNet.WindingRule.EvenOdd, LibTessDotNet.ElementType.Polygons, 3, VertexCombine);

            // Same call but the last callback is optional. Data will be null because no interpolated data would have been generated.
            //tess.Tessellate(LibTessDotNet.WindingRule.EvenOdd, LibTessDotNet.ElementType.Polygons, 3); // Some vertices will have null Data in this case.

            //Console.WriteLine("Output triangles:");
            int numTriangles = tess.ElementCount;
            vertices = new Vector3[numTriangles][];
            for (int i = 0; i < numTriangles; i++)
            {
                var v0 = tess.Vertices[tess.Elements[i * 3]].Position;
                var v1 = tess.Vertices[tess.Elements[i * 3 + 1]].Position;
                var v2 = tess.Vertices[tess.Elements[i * 3 + 2]].Position;

                vertices[i] = new Vector3[3];
                vertices[i][0] = new Vector3(v0.X, v0.Y, v0.Z);
                vertices[i][1] = new Vector3(v1.X, v1.Y, v1.Z);
                vertices[i][2] = new Vector3(v2.X, v2.Y, v2.Z);

                //Console.WriteLine(tess.Elements[i * 3] + " " + tess.Elements[i * 3 + 1] + " " + tess.Elements[i * 3 + 2]);

                //Console.WriteLine("#{0} ({1:F1},{2:F1}) ({3:F1},{4:F1}) ({5:F1},{6:F1})", i, v0.X, v0.Y,  v0.Z,v1.X, v1.Y,v1.Z, v2.X, v2.Y, v2.Z);
            }


        }

        public static bool getIntersection2D(Vector3 s1, Vector3 e1, Vector3 s2, Vector3 e2, out Vector3 i)
        {
            i = new Vector3();
            double a1 = twiceSignedTriArea2D(s1, e1, e2);
            double a2 = twiceSignedTriArea2D(s1, e1, s2);
            if (a1 != 0 && a2 != 0 && a1 * a2 < 0.0D)
            {
                double a3 = twiceSignedTriArea2D(s2, e2, s1);
                double a4 = a3 + a2 - a1;
                if (a3 * a4 < 0.0D)
                {
                    double t1 = a3 / (a3 - a4);
                    double t2 = a1 / (a1 - a2);

                    Vector3 dir = (e1 - s1).Normalized();
                    Vector3 result = dir * (float)clamp(t1, 0.0d, 1.0d) * (e1 - s1).Length;
                    result += s1;
                    return true;
                }
            }

            return false;
        }
        public static double clamp(double v, double min, double max)
        {
            return v < min ? min : (v > max ? max : v);
        }
        public static double twiceSignedTriArea2D(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p1.Y - p3.Y) * (p2.X - p3.X);
        }

        public void Cube(float length, float width, float height)
        {
            BeginShape(PrimitiveType.Quads);

            Vertex(-length / 2, width / 2, height / 2);
            Vertex(-length / 2, width / 2, -height / 2);
            Vertex(-length / 2, -width / 2, -height / 2);
            Vertex(-length / 2, -width / 2, height / 2);
            EndShape();
            BeginShape(PrimitiveType.Quads);
            Vertex(length / 2, width / 2, height / 2);
            Vertex(length / 2, width / 2, -height / 2);
            Vertex(length / 2, -width / 2, -height / 2);
            Vertex(length / 2, -width / 2, height / 2);
            EndShape();
            BeginShape(PrimitiveType.Quads);
            Vertex(length / 2, -width / 2, height / 2);
            Vertex(length / 2, -width / 2, -height / 2);
            Vertex(-length / 2, -width / 2, -height / 2);
            Vertex(-length / 2, -width / 2, height / 2);
            EndShape();
            BeginShape(PrimitiveType.Quads);
            Vertex(length / 2, width / 2, height / 2);
            Vertex(length / 2, width / 2, -height / 2);
            Vertex(-length / 2, width / 2, -height / 2);
            Vertex(-length / 2, width / 2, height / 2);
            EndShape();
            BeginShape(PrimitiveType.Quads);

            Vertex(length / 2, width / 2, -height / 2);
            Vertex(length / 2, -width / 2, -height / 2);
            Vertex(-length / 2, -width / 2, -height / 2);
            Vertex(-length / 2, width / 2, -height / 2);
            EndShape();
            BeginShape(PrimitiveType.Quads);
            Vertex(length / 2, width / 2, height / 2);
            Vertex(length / 2, -width / 2, height / 2);
            Vertex(-length / 2, -width / 2, height / 2);
            Vertex(-length / 2, width / 2, height / 2);
            EndShape();
        }
        public void sphereDetail(int res)
        {
            this.sphereDetail(res, res);
        }
        public void sphereDetail(int ures, int vres)
        {
            if (ures < 3)
            {
                ures = 3;
            }

            if (vres < 2)
            {
                vres = 2;
            }

            if (ures != this.sphereDetailU || vres != this.sphereDetailV)
            {
                float delta = 720.0F / (float)ures;
                float[] cx = new float[ures];
                float[] cz = new float[ures];

                int vertCount;
                for (vertCount = 0; vertCount < ures; ++vertCount)
                {
                    cx[vertCount] = cosLUT[(int)((float)vertCount * delta) % 720];
                    cz[vertCount] = sinLUT[(int)((float)vertCount * delta) % 720];
                }

                vertCount = ures * (vres - 1) + 2;
                int currVert = 0;
                this.sphereX = new float[vertCount];
                this.sphereY = new float[vertCount];
                this.sphereZ = new float[vertCount];
                float angle_step = 360.0F / (float)vres;
                float angle = angle_step;

                for (int i = 1; i < vres; ++i)
                {
                    float curradius = sinLUT[(int)angle % 720];
                    float currY = cosLUT[(int)angle % 720];

                    for (int j = 0; j < ures; ++j)
                    {
                        this.sphereX[currVert] = cx[j] * curradius;
                        this.sphereY[currVert] = currY;
                        this.sphereZ[currVert++] = cz[j] * curradius;
                    }

                    angle += angle_step;
                }

                this.sphereDetailU = ures;
                this.sphereDetailV = vres;
            }
        }


        public void sphere(float r)
        {
            if (this.sphereDetailU < 3 || this.sphereDetailV < 2)
            {
                this.sphereDetail(30);
            }
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            //GL.Color4(Color.FromArgb(fillColor));
            this.BeginShape(PrimitiveType.TriangleStrip);
            //GL.Begin(PrimitiveType.TriangleStrip);
            int v1;
            for (v1 = 0; v1 < this.sphereDetailU; ++v1)
            {
                //GL.Normal3(0.0F, 1.0F, 0.0F);
                this.Vertex(0.0F, r, 0.0F);

                //GL.Normal3(this.sphereX[v1], this.sphereY[v1], this.sphereZ[v1]);
                this.Vertex(r * this.sphereX[v1], r * this.sphereY[v1], r * this.sphereZ[v1]);

            }


            //GL.Normal3(0.0F, r, 0.0F);
            this.Vertex(0.0F,r, 0.0F);

            //GL.Normal3(this.sphereX[0], this.sphereY[0], this.sphereZ[0]);
            this.Vertex(r * this.sphereX[0], r * this.sphereY[0], r * this.sphereZ[0]);
            this.EndShape();

            int voff = 0;

            int v2;
            int i;
            for (i = 2; i < this.sphereDetailV; ++i)
            {
                int v11 = voff;
                v1 = voff;
                voff += this.sphereDetailU;
                v2 = voff;
                //GL.Begin(PrimitiveType.TriangleStrip);
                this.BeginShape(PrimitiveType.TriangleStrip);
                for (int j = 0; j < this.sphereDetailU; ++j)
                {

                    //GL.Normal3(this.sphereX[v1], this.sphereY[v1], this.sphereZ[v1]);
                    this.Vertex(r * this.sphereX[v1], r * this.sphereY[v1], r * this.sphereZ[v1++]);
                    //GL.Normal3(this.sphereX[v2], this.sphereY[v2], this.sphereZ[v2]);
                    this.Vertex(r * this.sphereX[v2], r * this.sphereY[v2], r * this.sphereZ[v2++]);
                }

                //GL.Normal3(this.sphereX[v11], this.sphereY[v11], this.sphereZ[v11]);
                this.Vertex(r * this.sphereX[v11], r * this.sphereY[v11], r * this.sphereZ[v11]);
                //GL.Normal3(this.sphereX[voff], this.sphereY[voff], this.sphereZ[voff]);
                this.Vertex(r * this.sphereX[voff], r * this.sphereY[voff], r * this.sphereZ[voff]);
                //GL.End();
                this.EndShape();
            }

            this.BeginShape(PrimitiveType.TriangleStrip);
            //GL.Begin(PrimitiveType.TriangleStrip);
            for (i = 0; i < this.sphereDetailU; ++i)
            {
                v2 = voff + i;
                //GL.Normal3(this.sphereX[v2], this.sphereY[v2], this.sphereZ[v2]);
                this.Vertex(r * this.sphereX[v2], r * this.sphereY[v2], r * this.sphereZ[v2]);
                //GL.Normal3(0.0F, -1.0F, 0.0F);
                this.Vertex(0.0F, -r, 0.0F);
            }

            //GL.Normal3(this.sphereX[voff], this.sphereY[voff], this.sphereZ[voff]);
            this.Vertex(r * this.sphereX[voff], r * this.sphereY[voff], r * this.sphereZ[voff]);
            //GL.Normal3(0.0F, 1.0F, 0.0F);
            this.Vertex(0.0F, -r, 0.0F);
            this.EndShape();
            //this.edge(true);
            //Console.WriteLine("___________");


        }


        public void Line(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            this.NoFill();
            this.BeginShape(PrimitiveType.Lines);
            this.Vertex(x1, y1, z1);
            this.Vertex(x2, y2, z2);
            this.EndShape();
        }

        public void processText(String s, Font f, Color co)
        {
            TextRenderer = new TextRenderer(s, f);
            PointF position = PointF.Empty;
            Color4 c = new Color4(1.0f, 1.0f, 1.0f, 0.01f);

            TextRenderer.Clear(Color.FromArgb(c.ToArgb()));
            TextRenderer.DrawString(new SolidBrush(co), position);
        }

        public void Text(string str, Vector3 vec)
        {
            this.Text(str, vec.X, vec.Y, vec.Z);
        }

        public void Text(string str, float x, float y, float z)
        {
            processText(str, this.textFont, Color.FromArgb(this.fillColor));
            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(-1.0f, -1.0f);
            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Greater, 0.01f);

            GL.Enable(EnableCap.Texture2D);
            GL.PushMatrix();
            GL.BindTexture(TextureTarget.Texture2D, TextRenderer.Texture);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Begin(BeginMode.Quads);


            GL.Color4(1.0f, 1.0f, 1.0f, 0.1f);

            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(x, y, z);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(x + TextRenderer.width, y, z);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(x + TextRenderer.width, y + TextRenderer.height, z);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(x, y + TextRenderer.height, z);

            GL.End();
            GL.PopMatrix();
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.PolygonOffsetFill);
            //important 释放内存！
            this.TextRenderer.Dispose();
        }

        public void colorCalc(int rgb)
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

        public void colorCalc(int rgb, float alpha)
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

        public void colorCalc(float gray)
        {
            this.colorCalc(gray, this.colorModeA);
        }
        public void colorCalc(float gray, float alpha)
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
        public void colorCalc(float x, float y, float z)
        {
            this.colorCalc(x, y, z, this.colorModeA);
        }

        public void colorCalc(float x, float y, float z, float a)
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
        public void colorCalcARGB(int argb, float alpha)
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
            GL.GetFloat(GetPName.ProjectionMatrix, matrix);

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
