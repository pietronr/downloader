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
    public static void ConvertToMp3(string inputPath, string outputPath, bool is320kbps = false)
    {
        try
        {
            LameMP3FileWriter GetMp3FileWriter(MediaFoundationReader reader)
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

            using var reader = new MediaFoundationReader(inputPath);
            using var writer = GetMp3FileWriter(reader);
            reader.CopyTo(writer);

            File.Delete(inputPath);
        }
        catch
        {
            File.Delete(inputPath);
        }
    }
}
