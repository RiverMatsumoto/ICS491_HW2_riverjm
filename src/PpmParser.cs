using System.Text;

namespace ICS491_HW2_riverjm
{
    public class PpmParser
    {
        bool validPpmFile_;
        int width_;
        int height_;
        int maximumColorValue_;
        string path_;

        public PpmParser(string path)
        {
            path_ = path;
        }

        public PpmParser()
        {
            path_ = "";
        }

        public Image ParseImage(string path)
        {
            string[] allLines = File.ReadAllLines(path);
            List<string> lines = new List<string>(allLines);
            if (lines[0].Trim().ToUpper() != "P3")
            {
                Console.WriteLine("Input file is not p3 version, currently only p3 is supported.");
                validPpmFile_ = false;
            }
            else
            {
                validPpmFile_ = true;
            }

            // remove all comments and empty lines because we don't want them
            lines.RemoveAll((s) => s.StartsWith("#") || s == "");

            // get width, height
            string[] dimensions = stripExtraWhiteSpaces(lines[1]).Split(' ');
            if (dimensions.Length < 2)
            {
                Console.WriteLine($"Cannot parse the dimensions of the image, tried to parse: {String.Join(" ", dimensions)}");
            }
            width_ = int.Parse(dimensions[0]);
            height_ = int.Parse(dimensions[1]);
            maximumColorValue_ = int.Parse(stripExtraWhiteSpaces(lines[2]));

            // we are done with the header, discard that, we only have pixel values left. File better be formatted correctly
            lines.RemoveRange(0, 2);

            Console.WriteLine($"Width: {width_}, Height: {height_}");
            Pixel[][] pixels = new Pixel[height_][];
            for (int i = 0; i < height_; i++)
                pixels[i] = new Pixel[width_];
            // parse backwards since removing at end of list is O(1)
            for (int i = height_ - 1; i >= 0; i--)
            {
                for (int j = width_ - 1; j >= 0; j--)
                {
                    int[] rgb = stripExtraWhiteSpaces(lines.Last())
                        .Split(' ')
                        .Select(int.Parse)
                        .ToArray();
                    int r = stretchRgb(rgb[0], maximumColorValue_);
                    int g = stretchRgb(rgb[1], maximumColorValue_);
                    int b = stretchRgb(rgb[2], maximumColorValue_);
                    // Console.WriteLine($"{r} {g} {b}");
                    pixels[i][j] = new Pixel(r, g, b);
                    lines.RemoveAt(lines.Count - 1);
                }
            }
            Image image = new(pixels);
            return image;
        }
        
        public Image ParseImage()
        {
            return ParseImage(path_);
        }

        public static void SaveImageToPpmFile(string path, Image img, string ppmType = "P3")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ppmType);
            sb.AppendLine($"{img.Width} {img.Height}");
            sb.AppendLine($"{255}");
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    Pixel p = img.GetPixel(i, j);
                    sb.Append($"{p.R} {p.G} {p.B}");
                    sb.AppendLine();
                }
            }
            File.WriteAllText(path, sb.ToString());
        }

        private string stripExtraWhiteSpaces(string s)
        {
            StringBuilder sb = new StringBuilder();
            s = s.Trim();
            foreach (char c in s)
            {
                if (c != ' ')
                    sb.Append(c);
                else if (c == ' ' && sb.ToString().Last() == ' ')
                    continue;
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private int clamp(int val, int min, int max)
        {
            if (val < min)
                return min;
            else if (val > max)
                return max;
            else
                return val;
        }
        
        private int stretchRgb(float val, int max)
        {
            return clamp((int)(val / max * 255.0f), 0, 255);
        }
    }
}