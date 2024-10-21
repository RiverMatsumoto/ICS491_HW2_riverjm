using ICS491_HW2_riverjm;
using CommandLine;
using ComputeSharp;

class Program
{
    // Define a class to receive parsed values
    public class Options
    {
        [Option('d', "input-directory", Required = false, HelpText = "Specifies the input directory.")]
        public string? InputDirectory { get; set; }

        [Option('o', "output-directory", Required = true, HelpText = "Specifies the output directory.")]
        public string? OutputDirectory { get; set; }

        [Option('f', "file", Required = false, HelpText = "Specifies the input file.")]
        public string? FileInput { get; set; }
    }
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o =>
            {
                Console.WriteLine($"Input directory: {o.InputDirectory}");
                Console.WriteLine($"Output directory: {o.OutputDirectory}");
                Console.WriteLine($"File input: {o.FileInput}");
                if (!Directory.Exists(o.InputDirectory) && !File.Exists(o.FileInput))
                {
                    Console.WriteLine($"Input directory {o.FileInput} does not exist, exiting");
                    return;
                }
                if (!Directory.Exists(o.OutputDirectory))
                {
                    Console.WriteLine($"Output directory {o.OutputDirectory} does not exist, exiting");
                    return;
                }
                string outputDirFullPath = Path.GetFullPath(o.OutputDirectory);
                if (o.FileInput != null && o.InputDirectory != null)
                {
                    string inputFileFullPath = Path.GetFullPath(o.FileInput);
                    Console.WriteLine("Both input file and input directory are specified, using input file");
                    ApplyBloomSingle(inputFileFullPath, outputDirFullPath);
                }
                else if (o.FileInput != null)
                {
                    string inputFileFullPath = Path.GetFullPath(o.FileInput);
                    ApplyBloomSingle(inputFileFullPath, outputDirFullPath);
                }
                else if (o.InputDirectory != null)
                {
                    string inputDirFullPath = Path.GetFullPath(o.InputDirectory);
                    ApplyBloomMultiple(inputDirFullPath, outputDirFullPath);
                }
            })
            .WithNotParsed<Options>((errors) =>
            {
                foreach (Error error in errors)
                {
                    Console.WriteLine(error.ToString());
                }
            });
    }

    public static void ApplyBloomSingle(string inputFile, string outputDirectory, int blurRadius = 15, double threshold = 0.5)
    {

        var ppm = new PpmParser();
        double[,] kernel = CreateGaussianKernel(blurRadius, 5);
        Image img = ppm.ParseImage(inputFile);
        Image grayscale = img.CreateGrayscaleCopy();
        Image thresholded = grayscale.CreateThresholdCopy(threshold);
        Image blurredThreshold = ApplyGaussianBlur(thresholded, kernel);
        Image bloom = img.AdditiveBlend(blurredThreshold);
        Console.WriteLine($"Writing grayscale image to file: {Path.Combine(outputDirectory, "grayscale.ppm")}");

        PpmParser.SaveImageToPpmFile(Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(inputFile) + "_grayscale.ppm"), grayscale, "P3");
        PpmParser.SaveImageToPpmFile(Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(inputFile) + "_threshold.ppm"), thresholded, "P3");
        PpmParser.SaveImageToPpmFile(Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(inputFile) + "_blurred.ppm"), blurredThreshold, "P3");
        PpmParser.SaveImageToPpmFile(Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(inputFile) + "_bloom.ppm"), bloom, "P3");
    }

    public static void ApplyBloomMultiple(string inputDirectory, string outputDirectory)
    {
        // string outputFile = args[1];
        Console.WriteLine($"Input ppm image file: {inputDirectory}");
        Console.WriteLine($"Output ppm ");

        // NOTE because the images take up so much memory with this implementation, I only created 2 arrays
        // to store the images.
        Console.WriteLine("Importing images...");
        var ppm = new PpmParser();
        string[] ppmFiles = Directory.GetFiles(inputDirectory, "*.ppm");
        int totalFiles = ppmFiles.Length;
        Image[] images = new Image[totalFiles];
        Image[] tempImages = new Image[totalFiles];
        for (int i = 0; i < totalFiles; i++)
        {
            images[i] = ppm.ParseImage(ppmFiles[i]);
        }
        Console.WriteLine("Done importing images.");


        // 1. Threshold animation
        // Threshold animation
        Console.WriteLine("Creating threshold animation");
        for (int i = 0; i < images.Length; i++)
        {
            tempImages[i] = images[i].CreateGrayscaleCopy();
        }
        for (int i = 0; i < tempImages.Length; i++)
        {
            tempImages[i] = tempImages[i].CreateThresholdCopy(
                Lerp(0.25, 0.75, 1.0 - ((double)i / tempImages.Length)));
            PpmParser.SaveImageToPpmFile(Path.Combine(outputDirectory, $"frame_{i.ToString().PadLeft(4, '0')}.ppm"), tempImages[i], "P3");
        }
        Console.WriteLine("Done creating threshold animation.");
        Console.WriteLine("Compiling video frames for threshold blur");

        // 2. Gaussian blur animation
        // cache gaussian kernel for animation
        double[,] kernel = CreateGaussianKernel(15, 5);
        double[][,] kernelCache = new double[120][,];
        for (int i = 0; i < 120; i++)
        {
            kernelCache[i] = new double[i, i];
        }
        // cache gaussian kernels frames 1 to 120
        for (int i = 0; i < 120; i += 2)
        {
            kernelCache[i] = CreateGaussianKernel(i + 1, 5);
            kernelCache[i + 1] = CreateGaussianKernel(i + 1, 5);
        }
        Console.WriteLine($"Finished caching gaussian kernels.");
        Console.WriteLine("Creating gaussian blur animation");
        for (int i = 0; i < images.Length; i++)
        {
            tempImages[i] = images[i].CreateGrayscaleCopy();
        }
        foreach (Image img in tempImages)
        {
            img.ApplyThreshold(0.5);
        }
        
        for (int i = 0; i < tempImages.Length; i++)
        {
            tempImages[i] = ApplyGaussianBlur(images[i], kernelCache[i]);
            Console.WriteLine($"Blurred image {i}");
            PpmParser.SaveImageToPpmFile(Path.Combine(outputDirectory, $"frame_{(i + 120).ToString().PadLeft(4, '0')}.ppm"), tempImages[i], "P3");
        }
        Console.WriteLine("Done creating gaussian blur animation.");
        Console.WriteLine("Compiling video frames for gaussian blur");


        Console.WriteLine("Applying bloom effect...");
        // animate bloom effect on each image
        for (int i = 0; i < images.Length; i++)
        {
            tempImages[i] = images[i].CreateGrayscaleCopy();
        }
        Console.WriteLine("Created grayscale images.");
        foreach (Image img in tempImages)
        {
            img.ApplyThreshold(0.5);
        }
        Console.WriteLine("Created threshold images.");
        for (int i = 0; i < tempImages.Length; i++)
        {
            tempImages[i] = ApplyGaussianBlur(tempImages[i], kernel);
            Console.WriteLine($"Blurred image {i}");
        }
        Console.WriteLine("Applied gaussian blur to threshold images.");
        for (int i = 0; i < images.Length; i++)
        {
            tempImages[i] = images[i].ScaledAdditiveBlend(tempImages[i], (double)i / (double)images.Length);
            PpmParser.SaveImageToPpmFile(Path.Combine(outputDirectory, $"frame_{(i + 240).ToString().PadLeft(4, '0')}.ppm"), tempImages[i], "P3");
        }
        Console.WriteLine("Done applying bloom effect.");
        Console.WriteLine("Compiling video frames for bloom effect");
        Console.WriteLine("Exiting...");
    }

    public static double Lerp(double a, double b, double t)
    {
        return a + (b - a) * t;
    }

    public static Image ApplyGaussianBlur(Image img, double[,] kernel)
    {
        int width = img.Width;
        int height = img.Height;
        int kernelSize = kernel.GetLength(0); // square kernel
        int radius = kernelSize / 2;

        // Create a new image to store the result
        Image result = new Image(width, height);

        // Parallelize the outer loop over image rows
        Parallel.For(0, height, i =>
        {
            for (int j = 0; j < width; j++)
            {
                // Apply the kernel weights to the pixel values
                double r = 0, g = 0, b = 0;
                for (int k = -radius; k <= radius; k++)
                {
                    for (int l = -radius; l <= radius; l++)
                    {
                        int row = Clamp(i + k, 0, height - 1);
                        int col = Clamp(j + l, 0, width - 1);
                        Pixel c = img.GetPixel(row, col);

                        // The kernel window into the image is used to weight the pixel values closer to the center
                        double kernelValue = kernel[k + radius, l + radius];
                        r += c.R * kernelValue;
                        g += c.G * kernelValue;
                        b += c.B * kernelValue;
                    }
                }

                // Set the computed pixel value in the result image
                result.SetPixel(i, j, new Pixel((int)r, (int)g, (int)b));
            }
        });

        return result;
    }

    public static int Clamp(int value, int min, int max)
    {
        return Math.Max(min, Math.Min(max, value));
    }

    /// <summary>
    /// Create a gaussian kernel to be used for blurring an image.
    /// </summary>
    /// <param name="size">Odd number for kernel size</param>
    /// <param name="sigma">Input for gaussian distribution formula</param>
    /// <returns></returns>
    public static double[,] CreateGaussianKernel(int size, double sigma)
    {
        if (size % 2 == 0)
            throw new ArgumentException("Kernel size must be odd.");

        double[,] kernel = new double[size, size];
        double mean = size / 2;
        double sum = 0.0; // for normalization

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                double exponent = -((Math.Pow(x - mean, 2) + Math.Pow(y - mean, 2)) / (2 * Math.Pow(sigma, 2)));
                kernel[x, y] = Math.Exp(exponent);
                sum += kernel[x, y];
            }
        }

        // Normalize the kernel so that the sum is 1
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                kernel[x, y] /= sum;

        return kernel;
    }
}
