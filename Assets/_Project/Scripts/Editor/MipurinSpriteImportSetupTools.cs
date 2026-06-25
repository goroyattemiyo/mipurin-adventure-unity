using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class MipurinSpriteImportSetupTools
{
    private static readonly string[] TargetFolders =
    {
        "Assets/_Project/Sprites/Player",
        "Assets/_Project/Sprites/Enemies",
        "Assets/_Project/Sprites/Effects",
        "Assets/_Project/Sprites/Stage",
        "Assets/_Project/Sprites/Items",
        "Assets/_Project/Sprites/UI",
        "Assets/_Project/Sprites/Weapons"
    };

    [MenuItem("Mipurin/Setup/Apply Sprite Import Settings")]
    public static void ApplySpriteImportSettings()
    {
        int updatedCount = 0;

        foreach (string folder in TargetFolders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                continue;
            }

            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                if (!assetPath.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

                if (importer == null)
                {
                    continue;
                }

                bool changed = ApplySettings(importer);

                if (!changed)
                {
                    continue;
                }

                importer.SaveAndReimport();
                updatedCount++;
                Debug.Log("Updated sprite import: " + assetPath);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Mipurin sprite import setup complete. Updated: " + updatedCount + " files.");
    }

    private static bool ApplySettings(TextureImporter importer)
    {
        bool changed = false;

        changed |= SetIfDifferent(() => importer.textureType, value => importer.textureType = value, TextureImporterType.Sprite);
        changed |= SetIfDifferent(() => importer.spriteImportMode, value => importer.spriteImportMode = value, SpriteImportMode.Single);
        changed |= SetIfDifferent(() => importer.alphaIsTransparency, value => importer.alphaIsTransparency = value, true);
        changed |= SetIfDifferent(() => importer.mipmapEnabled, value => importer.mipmapEnabled = value, false);
        changed |= SetIfDifferent(() => importer.filterMode, value => importer.filterMode = value, FilterMode.Bilinear);
        changed |= SetIfDifferent(() => importer.wrapMode, value => importer.wrapMode = value, TextureWrapMode.Clamp);
        changed |= SetIfDifferent(() => importer.maxTextureSize, value => importer.maxTextureSize = value, 2048);
        changed |= SetIfDifferent(() => importer.textureCompression, value => importer.textureCompression = value, TextureImporterCompression.Uncompressed);
        changed |= SetIfDifferent(() => importer.spriteMeshType, value => importer.spriteMeshType = value, SpriteMeshType.FullRect);
        changed |= SetIfDifferent(() => importer.isReadable, value => importer.isReadable = value, false);

        changed |= ApplyPlatformSettings(importer, "Standalone", 2048);
        changed |= ApplyPlatformSettings(importer, "Android", 2048);
        changed |= ApplyPlatformSettings(importer, "iPhone", 2048);

        return changed;
    }

    private static bool ApplyPlatformSettings(TextureImporter importer, string platformName, int maxSize)
    {
        TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings(platformName);
        bool changed = false;

        if (!settings.overridden)
        {
            settings.overridden = true;
            changed = true;
        }

        if (settings.maxTextureSize != maxSize)
        {
            settings.maxTextureSize = maxSize;
            changed = true;
        }

        if (settings.format != TextureImporterFormat.RGBA32)
        {
            settings.format = TextureImporterFormat.RGBA32;
            changed = true;
        }

        if (settings.textureCompression != TextureImporterCompression.Uncompressed)
        {
            settings.textureCompression = TextureImporterCompression.Uncompressed;
            changed = true;
        }

        if (changed)
        {
            importer.SetPlatformTextureSettings(settings);
        }

        return changed;
    }

    private static bool SetIfDifferent<T>(System.Func<T> getter, System.Action<T> setter, T newValue)
    {
        if (EqualityComparer<T>.Default.Equals(getter(), newValue))
        {
            return false;
        }

        setter(newValue);
        return true;
    }
}
