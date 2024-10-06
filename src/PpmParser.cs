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
            lines.RemoveRange(0, 3);
            string pixelData = string.Join('\n', lines).Replace('\n',  ' ');
            string[] pixelDataArr = pixelData.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Image image = new Image(width_, height_);

            for (int i = 0; i < pixelDataArr.Length; i += 3)
            {
                int r = int.Parse(pixelDataArr[i]);
                int g = int.Parse(pixelDataArr[i + 1]);
                int b = int.Parse(pixelDataArr[i + 2]);
                int currentRow = (i / 3) / width_;
                int currentCol = (i / 3) % width_;
                image.SetPixel(currentRow, currentCol, new Pixel(r, g, b));
            }
            return image;
        }

        string CompressPixelData(string[] pixelData)
        {
            return "";
        }
        
        public Image ParseImage()
        {
            return ParseImage(path_);
        }

        public static void SaveImageToPpmFile(string path, Image img, string ppmType = "P3")
        {
            StringBuilder sb = new();
            sb.AppendLine(ppmType);
            sb.AppendLine($"{img.Width} {img.Height}");
            sb.AppendLine($"{255}");
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    Pixel p = img.GetPixel(i, j);
                    sb.Append($"{p.R} {p.G} {p.B} ");
                }
                sb.AppendLine();
            }
            Console.WriteLine($"Writing image to file: {path}");
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