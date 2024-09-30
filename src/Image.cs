namespace ICS491_HW2_riverjm
{
    public class Image
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        Pixel[][] pixels_;

        public Image(Pixel[][] pixels)
        {
            this.pixels_ = pixels;
            Height = pixels.Length;
            Width = pixels[0].Length;
        }

        public Pixel GetPixel(int row, int column)
        {
            if (column >= pixels_[0].Length || column < 0 || row < 0 || row >= pixels_.Length)
                return Pixel.WHITE;
            return pixels_[row][column];
        }
        
        public void SetPixel(int row, int column, Pixel color)
        {
            if (column < pixels_.GetLength(1) || row < pixels_.GetLength(0))
                return;
            pixels_[row][column] = color;
        }
    }

    public class Pixel
    {
        int r_;
        int g_;
        int b_;
        public int R
        {
            get 
            {
                return r_;
            }
            set 
            {
                if (r_ < 0)
                    r_ = 0;
                else if (r_ > 255)
                    r_ = 255;
                else
                    r_ = value;
            }
        }
        public int G
        {
            get 
            {
                return g_;
            }
            set 
            {
                if (g_ < 0)
                    g_ = 0;
                else if (g_ > 255)
                    g_ = 255;
                else
                    g_ = value;
            }
        }
        public int B
        {
            get 
            {
                return b_;
            }
            set 
            {
                if (b_ < 0)
                    b_ = 0;
                else if (b_ > 255)
                    b_ = 255;
                else
                    b_ = value;
            }
        }
        public static Pixel WHITE = new Pixel(255, 255, 255);
        public static Pixel BLACK = new Pixel(0, 0, 0);

        public Pixel(int r = 0, int g = 0, int b = 0)
        {
            r_ = r;
            g_ = g;
            b_ = b;
        }

        public override string ToString()
        {
            return $"R={r_:000},G={g_:000},B={b_:000}";
        }
    }
}