using ComputeSharp;
using ComputeSharp.Shaders;
using System.Numerics;

[ThreadGroupSize(16, 16, 1)]
public readonly struct GaussianBlurShader : IComputeShader
{
    public readonly ReadWriteTexture2D<float4> Output;
    public readonly ReadOnlyTexture2D<float4> Input;
    public readonly ReadOnlyBuffer<double> Kernel;
    public readonly int KernelSize;

    public GaussianBlurShader(ReadOnlyTexture2D<float4> input, ReadWriteTexture2D<float4> output, ReadOnlyBuffer<double> kernel, int kernelSize)
    {
        Input = input;
        Output = output;
        Kernel = kernel;
        KernelSize = kernelSize;
    }

    public void Execute()
    {
        // Get the current thread's pixel coordinates
        Int2 coords = ThreadIds.XY;
        int width = Output.Width;
        int height = Output.Height;
        int halfKernel = KernelSize / 2;

        Vector4 sum = Vector4.Zero;

        for (int y = -halfKernel; y <= halfKernel; y++)
        {
            for (int x = -halfKernel; x <= halfKernel; x++)
            {
                int sampleX = Math.Clamp(coords.X + x, 0, width - 1);
                int sampleY = Math.Clamp(coords.Y + y, 0, height - 1);

                Vector4 sample = Input[sampleX, sampleY];
                double weight = Kernel[(y + halfKernel) * KernelSize + (x + halfKernel)];
                sum += (float)weight * sample;
            }
        }

        Output[coords.X, coords.Y] = sum;
    }
}
