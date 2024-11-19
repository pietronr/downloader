using NAudio.Lame;
using NAudio.Wave;

namespace Downloader.Helpers;

public static class Mp3Helper
{
    /// <summary>
    /// Convert um arquivo para mp3 ou wav.
    /// </summary>
    /// <param name="inputPath">Caminho de entrada.</param>
    /// <param name="outputPath">Caminho de saída.</param>
    public static void ConvertToMp3OrWav(string inputPath, string outputPath, bool is320kbps = false, bool isWav = false)
    {
        try
        {
            using var reader = new MediaFoundationReader(inputPath);
            using var writer = GetFileWriter(outputPath, reader, is320kbps);
            reader.CopyTo(writer);

            File.Delete(inputPath);
        }
        catch
        {
            File.Delete(inputPath);
        }
    }

    private static Stream GetFileWriter(string outputPath, MediaFoundationReader reader, bool is320kbps = false, bool isWav = false)
    {
        if (isWav)
        {
            if (!is320kbps)
            {
                return new LameMP3FileWriter(outputPath, reader.WaveFormat, LAMEPreset.STANDARD);
            }
            else
            {
                return new LameMP3FileWriter(outputPath, reader.WaveFormat, 320);
            }
        }
        else
        {
            return new WaveFileWriter(outputPath, reader.WaveFormat);
        }
    }
}
