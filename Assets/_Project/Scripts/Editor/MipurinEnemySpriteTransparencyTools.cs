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

    private const byte WhiteThreshold = 245;
    private const byte NearWhiteThreshold = 232;
    private const float EdgeFeatherStrength = 0.65f;

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

        Color32[] pixels = texture.GetPixels32();
        bool changed = false;

        for (int i = 0; i < pixels.Length; i++)
        {
            Color32 pixel = pixels[i];

            if (IsWhite(pixel, WhiteThreshold))
            {
                pixels[i] = new Color32(pixel.r, pixel.g, pixel.b, 0);
                changed = true;
                continue;
            }

            if (IsWhite(pixel, NearWhiteThreshold))
            {
                float whiteness = Mathf.Min(pixel.r, Mathf.Min(pixel.g, pixel.b)) / 255f;
                byte alpha = (byte)Mathf.Clamp(Mathf.RoundToInt((1f - whiteness) * 255f / EdgeFeatherStrength), 0, 255);

                if (alpha < pixel.a)
                {
                    pixels[i] = new Color32(pixel.r, pixel.g, pixel.b, alpha);
                    changed = true;
                }
            }
        }

        if (changed)
        {
            texture.SetPixels32(pixels);
            texture.Apply(false, false);
            File.WriteAllBytes(absolutePath, texture.EncodeToPNG());
            Debug.Log("Removed white background: " + ToAssetPath(absolutePath));
        }
        else
        {
            Debug.Log("No white background change needed: " + ToAssetPath(absolutePath));
        }

        Object.DestroyImmediate(texture);
        return true;
    }

    private static bool IsWhite(Color32 pixel, byte threshold)
    {
        return pixel.a > 0 && pixel.r >= threshold && pixel.g >= threshold && pixel.b >= threshold;
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
