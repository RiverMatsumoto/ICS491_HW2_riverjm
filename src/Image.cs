using ComputeSharp;

namespace ICS491_HW2_riverjm
{
    public class Image
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        Pixel[] pixels_;

        public Image(Pixel[] pixels, int width, int height)
        {
            pixels_ = pixels;
            Width = width;
            Height = height;
        }
        public Image(int width, int height)
        {
            pixels_ = new Pixel[width * height];
            Height = height;
            Width = width;
        }

        public Pixel GetPixel(int row, int column)
        {
            if (column >= Width || column < 0 || row < 0 || row >= Height)
                return Pixel.WHITE;
            return pixels_[row * Width + column];
        }
        
        public void SetPixel(int row, int column, Pixel color)
        {
            if (column >= Width || column < 0 || row < 0 || row >= Height)
                return;
            pixels_[row * Width + column] = color;
        }

        public static Image CreateGrayscaleCopy(Image img)
        {
            Image newImg = new Image(img.Width, img.Height);
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    Pixel c = img.GetPixel(i, j);
                    // luminance formula
                    int grayvalue = (int)(c.R * 0.299 + c.G * 0.587 + c.B * 0.114);
                    Pixel graypixel = new Pixel(grayvalue, grayvalue, grayvalue);
                    newImg.SetPixel(i, j, graypixel);
                }
            }
            return newImg;
        }

        public Image CreateGrayscaleCopy()
        {
            Image img = new Image(Width, Height);
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Pixel c = GetPixel(i, j);
                    // luminance formula
                    int grayvalue = (int)(c.R * 0.299 + c.G * 0.587 + c.B * 0.114);
                    Pixel graypixel = new Pixel(grayvalue, grayvalue, grayvalue);
                    img.SetPixel(i, j, graypixel);
                }
            }
            return img;
        }

        public static Image CreateThresholdCopy(Image img, double threshold = 0.5f)
        {
            Image newImg = new Image(img.Width, img.Height);
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    Pixel c = img.GetPixel(i, j);
                    if (c.R > 255 / threshold)
                        newImg.SetPixel(i, j, Pixel.WHITE);
                    else
                        newImg.SetPixel(i, j, Pixel.BLACK);
                }
            }
            return newImg;
        }

        public Image CreateThresholdCopy(double threshold = 0.5f)
        {
            Image img = new Image(Width, Height);
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Pixel c = GetPixel(i, j);
                    if (c.R > (int)(255 * threshold))
                        img.SetPixel(i, j, Pixel.WHITE);
                    else
                        img.SetPixel(i, j, Pixel.BLACK);
                }
            }
            return img;
        }

        public Image ApplyThreshold(double threshold = 0.5f)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Pixel c = GetPixel(i, j);
                    if (c.R > (int)(255 * threshold))
                        SetPixel(i, j, Pixel.WHITE);
                    else
                        SetPixel(i, j, Pixel.BLACK);
                }
            }
            return this;
        }

        public Image AdditiveBlend(Image img)
        {
            Image newImg = new Image(Width, Height);
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Pixel c1 = GetPixel(i, j);
                    Pixel c2 = img.GetPixel(i, j);
                    int r = Clamp(c1.R + c2.R);
                    int g = Clamp(c1.G + c2.G);
                    int b = Clamp(c1.B + c2.B);
                    newImg.SetPixel(i, j, new Pixel(r, g, b));
                }
            }
            return newImg;
        }

        private int Clamp(int value)
        {
            return value < 0 ? 0 : value > 255 ? 255 : value;
        }

        public Image ScaledAdditiveBlend(Image img, double scale)
        {
            Clamp(scale, 0, 1);
            Image newImg = new Image(Width, Height);
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Pixel c1 = GetPixel(i, j);
                    Pixel c2 = img.GetPixel(i, j);
                    int r = c1.R + (int)(c2.R * scale);
                    int g = c1.G + (int)(c2.G * scale);
                    int b = c1.B + (int)(c2.B * scale);
                    newImg.SetPixel(i, j, new Pixel(r, g, b));
                }
            }
            return newImg;
        }

        public Image ScreenBlend(Image img)
        {
            Image newImg = new Image(Width, Height);
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Pixel c1 = GetPixel(i, j);
                    Pixel c2 = img.GetPixel(i, j);
                    int r = 255 - (255 - c1.R) * (255 - c2.R) / 255;
                    int g = 255 - (255 - c1.G) * (255 - c2.G) / 255;
                    int b = 255 - (255 - c1.B) * (255 - c2.B) / 255;
                    newImg.SetPixel(i, j, new Pixel(r, g, b));
                }
            }
            return newImg;
        }

        double Clamp(double value, double min, double max)
        {
            return value < min ? min : value > max ? max : value;
        }
}
    public struct Pixel
    {
        public int R;
        public int G;
        public int B;

        public static readonly Pixel WHITE = new Pixel(255, 255, 255);
        public static readonly Pixel BLACK = new Pixel(0, 0, 0);

        public Pixel(int r, int g, int b)
        {
            R = Clamp(r, 0, 255);
            G = Clamp(g, 0, 255);
            B = Clamp(b, 0, 255);
        }

        private static int Clamp(int value, int min, int max) =>
            value < min ? min : value > max ? max : value;

        // public override string ToString() => $"R={r_:000},G={g_:000},B={b_:000}";
    }
}