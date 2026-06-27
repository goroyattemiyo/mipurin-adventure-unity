using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class MipurinBackgroundRemovalTools
{
    private const byte WhiteThreshold = 245;
    private const byte SoftWhiteThreshold = 232;
    private const string OutputSuffix = "_transparent";

    [MenuItem("Mipurin/Tools/Remove White Background From Selected PNGs")]
    public static void RemoveWhiteBackgroundFromSelectedPngs()
    {
        Object[] selectedObjects = Selection.objects;
        int processedCount = 0;

        foreach (Object selectedObject in selectedObjects)
        {
            string assetPath = AssetDatabase.GetAssetPath(selectedObject);

            if (!IsValidPngAsset(assetPath))
            {
                continue;
            }

            if (CreateTransparentCopy(assetPath))
            {
                processedCount++;
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"White background removal complete. Processed PNG files: {processedCount}");
    }

    private static bool IsValidPngAsset(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath))
        {
            return false;
        }

        if (!assetPath.StartsWith("Assets/"))
        {
            return false;
        }

        return assetPath.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase);
    }

    private static bool CreateTransparentCopy(string assetPath)
    {
        string absolutePath = Path.GetFullPath(assetPath);

        if (!File.Exists(absolutePath))
        {
            Debug.LogWarning($"PNG file not found: {assetPath}");
            return false;
        }

        byte[] sourceBytes = File.ReadAllBytes(absolutePath);
        Texture2D sourceTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);

        if (!sourceTexture.LoadImage(sourceBytes))
        {
            Debug.LogWarning($"Failed to load PNG: {assetPath}");
            Object.DestroyImmediate(sourceTexture);
            return false;
        }

        Color32[] pixels = sourceTexture.GetPixels32();
        int width = sourceTexture.width;
        int height = sourceTexture.height;
        bool[] backgroundMask = BuildBackgroundMask(pixels, width, height);
        ApplyBackgroundRemoval(pixels, backgroundMask);

        Texture2D outputTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        outputTexture.SetPixels32(pixels);
        outputTexture.Apply();

        string outputPath = CreateOutputPath(assetPath);
        string outputAbsolutePath = Path.GetFullPath(outputPath);
        File.WriteAllBytes(outputAbsolutePath, outputTexture.EncodeToPNG());

        Object.DestroyImmediate(sourceTexture);
        Object.DestroyImmediate(outputTexture);

        Debug.Log($"Created transparent PNG: {outputPath}");
        return true;
    }

    private static bool[] BuildBackgroundMask(Color32[] pixels, int width, int height)
    {
        bool[] visited = new bool[pixels.Length];
        Queue<int> queue = new Queue<int>();

        for (int x = 0; x < width; x++)
        {
            TryEnqueueBackgroundPixel(x, 0, width, pixels, visited, queue);
            TryEnqueueBackgroundPixel(x, height - 1, width, pixels, visited, queue);
        }

        for (int y = 0; y < height; y++)
        {
            TryEnqueueBackgroundPixel(0, y, width, pixels, visited, queue);
            TryEnqueueBackgroundPixel(width - 1, y, width, pixels, visited, queue);
        }

        while (queue.Count > 0)
        {
            int index = queue.Dequeue();
            int x = index % width;
            int y = index / width;

            TryEnqueueBackgroundPixel(x + 1, y, width, pixels, visited, queue);
            TryEnqueueBackgroundPixel(x - 1, y, width, pixels, visited, queue);
            TryEnqueueBackgroundPixel(x, y + 1, width, pixels, visited, queue);
            TryEnqueueBackgroundPixel(x, y - 1, width, pixels, visited, queue);
        }

        return visited;
    }

    private static void TryEnqueueBackgroundPixel(int x, int y, int width, Color32[] pixels, bool[] visited, Queue<int> queue)
    {
        if (x < 0 || y < 0)
        {
            return;
        }

        int height = pixels.Length / width;
        if (x >= width || y >= height)
        {
            return;
        }

        int index = y * width + x;
        if (visited[index])
        {
            return;
        }

        if (!IsWhiteBackgroundPixel(pixels[index]))
        {
            return;
        }

        visited[index] = true;
        queue.Enqueue(index);
    }

    private static bool IsWhiteBackgroundPixel(Color32 color)
    {
        return color.r >= SoftWhiteThreshold && color.g >= SoftWhiteThreshold && color.b >= SoftWhiteThreshold;
    }

    private static void ApplyBackgroundRemoval(Color32[] pixels, bool[] backgroundMask)
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            if (!backgroundMask[i])
            {
                continue;
            }

            Color32 color = pixels[i];

            if (color.r >= WhiteThreshold && color.g >= WhiteThreshold && color.b >= WhiteThreshold)
            {
                color.a = 0;
            }
            else
            {
                byte minChannel = (byte)Mathf.Min(color.r, Mathf.Min(color.g, color.b));
                float fade = Mathf.InverseLerp(SoftWhiteThreshold, WhiteThreshold, minChannel);
                color.a = (byte)Mathf.RoundToInt(255f * (1f - fade));
            }

            pixels[i] = color;
        }
    }

    private static string CreateOutputPath(string assetPath)
    {
        string directory = Path.GetDirectoryName(assetPath)?.Replace("\\", "/") ?? "Assets";
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetPath);
        string outputPath = $"{directory}/{fileNameWithoutExtension}{OutputSuffix}.png";

        int index = 2;
        while (File.Exists(Path.GetFullPath(outputPath)))
        {
            outputPath = $"{directory}/{fileNameWithoutExtension}{OutputSuffix}_{index}.png";
            index++;
        }

        return outputPath;
    }
}
