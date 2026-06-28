using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class MipurinEnemySpriteTransparencyTools
{
    private static readonly string[] TargetFolders =
    {
        "Assets/_Project/Sprites/Enemies/StingerBee",
        "Assets/_Project/Sprites/Enemies/FlowerTurret",
        "Assets/_Project/Sprites/Enemies/HeavyBeetle"
    };

    private const byte EdgeBackgroundMinimum = 215;
    private const byte EdgeBackgroundChannelRange = 28;
    private const byte FullTransparentMinimum = 238;
    private const byte FeatherMinimum = 220;
    private const float EdgeFeatherStrength = 0.55f;

    [MenuItem("Mipurin/Tools/Remove White Background From Phase 4 Enemy Sprites")]
    public static void RemoveWhiteBackgroundFromPhase4EnemySprites()
    {
        int processedCount = 0;

        foreach (string folder in TargetFolders)
        {
            processedCount += ProcessFolder(folder);
        }

        AssetDatabase.Refresh();
        Debug.Log($"Enemy sprite transparency complete. Processed PNG files: {processedCount}");
    }

    private static int ProcessFolder(string folder)
    {
        if (!AssetDatabase.IsValidFolder(folder))
        {
            Debug.LogWarning("Folder not found: " + folder);
            return 0;
        }

        string absoluteFolder = ToAbsolutePath(folder);
        if (!Directory.Exists(absoluteFolder))
        {
            Debug.LogWarning("Directory not found: " + absoluteFolder);
            return 0;
        }

        string[] pngFiles = Directory.GetFiles(absoluteFolder, "*.png", SearchOption.TopDirectoryOnly);
        int count = 0;

        foreach (string pngFile in pngFiles)
        {
            if (ProcessPng(pngFile))
            {
                count++;
            }
        }

        return count;
    }

    private static bool ProcessPng(string absolutePath)
    {
        byte[] originalBytes = File.ReadAllBytes(absolutePath);
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);

        if (!texture.LoadImage(originalBytes))
        {
            Object.DestroyImmediate(texture);
            Debug.LogWarning("Failed to load PNG: " + absolutePath);
            return false;
        }

        int width = texture.width;
        int height = texture.height;
        Color32[] pixels = texture.GetPixels32();
        bool[] backgroundMask = CreateEdgeConnectedBackgroundMask(pixels, width, height);
        bool changed = false;

        for (int i = 0; i < pixels.Length; i++)
        {
            if (!backgroundMask[i])
            {
                continue;
            }

            Color32 pixel = pixels[i];
            byte alpha = CalculateBackgroundAlpha(pixel);

            if (alpha < pixel.a)
            {
                pixels[i] = new Color32(pixel.r, pixel.g, pixel.b, alpha);
                changed = true;
            }
        }

        if (changed)
        {
            texture.SetPixels32(pixels);
            texture.Apply(false, false);
            File.WriteAllBytes(absolutePath, texture.EncodeToPNG());
            Debug.Log("Removed baked checkerboard/white background: " + ToAssetPath(absolutePath));
        }
        else
        {
            Debug.Log("No background change needed: " + ToAssetPath(absolutePath));
        }

        Object.DestroyImmediate(texture);
        return true;
    }

    private static bool[] CreateEdgeConnectedBackgroundMask(Color32[] pixels, int width, int height)
    {
        bool[] visited = new bool[pixels.Length];
        Queue<int> queue = new Queue<int>();

        for (int x = 0; x < width; x++)
        {
            TryEnqueue(x, 0, pixels, width, height, visited, queue);
            TryEnqueue(x, height - 1, pixels, width, height, visited, queue);
        }

        for (int y = 0; y < height; y++)
        {
            TryEnqueue(0, y, pixels, width, height, visited, queue);
            TryEnqueue(width - 1, y, pixels, width, height, visited, queue);
        }

        while (queue.Count > 0)
        {
            int index = queue.Dequeue();
            int x = index % width;
            int y = index / width;

            TryEnqueue(x + 1, y, pixels, width, height, visited, queue);
            TryEnqueue(x - 1, y, pixels, width, height, visited, queue);
            TryEnqueue(x, y + 1, pixels, width, height, visited, queue);
            TryEnqueue(x, y - 1, pixels, width, height, visited, queue);
        }

        return visited;
    }

    private static void TryEnqueue(int x, int y, Color32[] pixels, int width, int height, bool[] visited, Queue<int> queue)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return;
        }

        int index = y * width + x;
        if (visited[index])
        {
            return;
        }

        if (!IsBackgroundCandidate(pixels[index]))
        {
            return;
        }

        visited[index] = true;
        queue.Enqueue(index);
    }

    private static bool IsBackgroundCandidate(Color32 pixel)
    {
        if (pixel.a == 0)
        {
            return true;
        }

        byte min = MinChannel(pixel);
        byte max = MaxChannel(pixel);

        return min >= EdgeBackgroundMinimum && max - min <= EdgeBackgroundChannelRange;
    }

    private static byte CalculateBackgroundAlpha(Color32 pixel)
    {
        byte min = MinChannel(pixel);

        if (min >= FullTransparentMinimum)
        {
            return 0;
        }

        if (min >= FeatherMinimum)
        {
            float amount = Mathf.InverseLerp(FeatherMinimum, FullTransparentMinimum, min);
            return (byte)Mathf.Clamp(Mathf.RoundToInt((1f - amount) * 255f * EdgeFeatherStrength), 0, 255);
        }

        return pixel.a;
    }

    private static byte MinChannel(Color32 pixel)
    {
        return (byte)Mathf.Min(pixel.r, Mathf.Min(pixel.g, pixel.b));
    }

    private static byte MaxChannel(Color32 pixel)
    {
        return (byte)Mathf.Max(pixel.r, Mathf.Max(pixel.g, pixel.b));
    }

    private static string ToAbsolutePath(string assetPath)
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName.Replace("\\", "/");
        return Path.Combine(projectRoot, assetPath).Replace("\\", "/");
    }

    private static string ToAssetPath(string absolutePath)
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName.Replace("\\", "/");
        return absolutePath.Replace("\\", "/").Replace(projectRoot + "/", "");
    }
}
