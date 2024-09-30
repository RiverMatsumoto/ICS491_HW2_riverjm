using System.IO;
using ICS491_HW2_riverjm;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: ICS491_HW2_riverjm.exe <ppm_file> <output_file>");
            return;
        }
        string ppmFilePath = args[0];
        // string outputFile = args[1];
        string outputFile = Path.Combine(Path.GetFullPath(args[1]), Path.GetFileNameWithoutExtension(Path.GetFullPath(ppmFilePath)) + "_bloom.ppm");
        Console.WriteLine($"Input ppm image file: {ppmFilePath}");
        Console.WriteLine($"Output ppm ");

        // image grows down and to the right
        var ppm = new PpmParser();
        Image img = ppm.ParseImage(ppmFilePath);
        for (int i = 0; i < img.Height; i++)
        {
            for (int j = 0; j < img.Width; j++)
            {
                Pixel c = img.GetPixel(i, j);
            }
        }

        // process image, take ppm checkpoints along the way
        // 1. new copy with thresholded image
        
        // 2. gaussian blur on the black and white thresholded image

        // 3. blend original image with blurred image and that's bloom!


        PpmParser.SaveImageToPpmFile(outputFile, img, "P3");
    }
}
