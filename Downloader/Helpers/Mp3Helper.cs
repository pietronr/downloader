using NAudio.Lame;
using NAudio.Wave;

namespace Downloader.Helpers;

public static class Mp3Helper
{
    /// <summary>
    /// Convert um arquivo para mp3.
    /// </summary>
    /// <param name="inputPath">Caminho de entrada.</param>
    /// <param name="outputPath">Caminho de saída.</param>
    public static void ConvertToMp3(string inputPath, string outputPath)
    {
        try
        {
            using var reader = new MediaFoundationReader(inputPath);
            using var writer = new LameMP3FileWriter(outputPath, reader.WaveFormat, LAMEPreset.STANDARD);
            reader.CopyTo(writer);
            
            File.Delete(inputPath);
        }
        catch
        {
            File.Delete(inputPath);
        }
    }
}
