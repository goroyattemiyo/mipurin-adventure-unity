using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MipurinNectarSpriteSetupTools
{
    private const string PickupFolderPath = "Assets/_Project/Sprites/Items/Pickups";
    private const string NectarSpritePath = PickupFolderPath + "/honey_nectar_01.png";
    private const string PickupPrefabPath = "Assets/_Project/Prefabs/Items/Pickup_HoneyNectar.prefab";
    private const int SpriteSize = 128;
    private const int PixelsPerUnit = 300;

    [MenuItem("Mipurin/Setup/Setup Honey Nectar Sprite")]
    public static void SetupHoneyNectarSprite()
    {
        EnsureFolders();
        GenerateNectarPng();
        ApplySpriteImportSettings();
        ApplyPickupPrefabSprite();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("Honey nectar sprite setup complete: " + NectarSpritePath);
    }

    private static void EnsureFolders()
    {
        Directory.CreateDirectory(PickupFolderPath);
        AssetDatabase.Refresh();
    }

    private static void GenerateNectarPng()
    {
        Texture2D texture = new Texture2D(SpriteSize, SpriteSize, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;

        Color clear = new Color(0f, 0f, 0f, 0f);
        Color outline = new Color(0.45f, 0.22f, 0.04f, 1f);
        Color honeyDark = new Color(0.72f, 0.36f, 0.04f, 1f);
        Color honey = new Color(1f, 0.62f, 0.08f, 1f);
        Color honeyLight = new Color(1f, 0.88f, 0.26f, 1f);
        Color cork = new Color(0.56f, 0.28f, 0.09f, 1f);
        Color shine = new Color(1f, 1f, 1f, 0.82f);

        Fill(texture, clear);

        DrawEllipse(texture, 64, 70, 34, 26, outline);
        DrawEllipse(texture, 64, 72, 30, 22, honeyDark);
        DrawEllipse(texture, 64, 75, 27, 19, honey);

        DrawRect(texture, 47, 87, 34, 11, outline);
        DrawRect(texture, 51, 89, 26, 8, honeyDark);
        DrawRect(texture, 54, 91, 20, 4, honeyLight);

        DrawRect(texture, 51, 46, 26, 19, outline);
        DrawRect(texture, 55, 49, 18, 14, honey);
        DrawRect(texture, 53, 38, 22, 12, outline);
        DrawRect(texture, 57, 40, 14, 8, cork);

        DrawEllipse(texture, 39, 71, 8, 13, outline);
        DrawEllipse(texture, 89, 71, 8, 13, outline);
        DrawEllipse(texture, 40, 72, 5, 9, honeyDark);
        DrawEllipse(texture, 88, 72, 5, 9, honeyDark);

        DrawEllipse(texture, 54, 80, 8, 5, honeyLight);
        DrawCircle(texture, 52, 84, 3, shine);
        DrawStar(texture, 92, 95, 7, shine);
        DrawStar(texture, 36, 94, 5, honeyLight);

        texture.Apply();
        File.WriteAllBytes(NectarSpritePath, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(NectarSpritePath, ImportAssetOptions.ForceUpdate);
    }

    private static void ApplySpriteImportSettings()
    {
        TextureImporter importer = AssetImporter.GetAtPath(NectarSpritePath) as TextureImporter;
        if (importer == null)
        {
            Debug.LogWarning("TextureImporter not found: " + NectarSpritePath);
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = PixelsPerUnit;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();
    }

    private static void ApplyPickupPrefabSprite()
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(NectarSpritePath);
        if (sprite == null)
        {
            Debug.LogWarning("Nectar sprite not found: " + NectarSpritePath);
            return;
        }

        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(PickupPrefabPath);
        if (prefabRoot == null)
        {
            Debug.LogWarning("Pickup prefab not found: " + PickupPrefabPath);
            return;
        }

        prefabRoot.transform.localScale = new Vector3(0.58f, 0.58f, 1f);

        SpriteRenderer renderer = prefabRoot.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            renderer = prefabRoot.AddComponent<SpriteRenderer>();
        }

        renderer.sprite = sprite;
        renderer.sortingOrder = 25;

        EditorUtility.SetDirty(prefabRoot);
        EditorUtility.SetDirty(renderer);
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, PickupPrefabPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
    }

    private static void Fill(Texture2D texture, Color color)
    {
        for (int y = 0; y < SpriteSize; y++)
        {
            for (int x = 0; x < SpriteSize; x++)
            {
                texture.SetPixel(x, y, color);
            }
        }
    }

    private static void DrawRect(Texture2D texture, int xMin, int yMin, int width, int height, Color color)
    {
        for (int y = yMin; y < yMin + height; y++)
        {
            for (int x = xMin; x < xMin + width; x++)
            {
                SetPixelSafe(texture, x, y, color);
            }
        }
    }

    private static void DrawCircle(Texture2D texture, int centerX, int centerY, int radius, Color color)
    {
        DrawEllipse(texture, centerX, centerY, radius, radius, color);
    }

    private static void DrawEllipse(Texture2D texture, int centerX, int centerY, int radiusX, int radiusY, Color color)
    {
        for (int y = centerY - radiusY; y <= centerY + radiusY; y++)
        {
            for (int x = centerX - radiusX; x <= centerX + radiusX; x++)
            {
                float dx = (x - centerX) / (float)Mathf.Max(1, radiusX);
                float dy = (y - centerY) / (float)Mathf.Max(1, radiusY);
                if (dx * dx + dy * dy <= 1f)
                {
                    SetPixelSafe(texture, x, y, color);
                }
            }
        }
    }

    private static void DrawStar(Texture2D texture, int centerX, int centerY, int radius, Color color)
    {
        for (int i = -radius; i <= radius; i++)
        {
            SetPixelSafe(texture, centerX + i, centerY, color);
            SetPixelSafe(texture, centerX, centerY + i, color);
        }

        int small = Mathf.Max(1, radius / 2);
        for (int i = -small; i <= small; i++)
        {
            SetPixelSafe(texture, centerX + i, centerY + i, color);
            SetPixelSafe(texture, centerX + i, centerY - i, color);
        }
    }

    private static void SetPixelSafe(Texture2D texture, int x, int y, Color color)
    {
        if (x < 0 || x >= SpriteSize || y < 0 || y >= SpriteSize)
        {
            return;
        }

        Color current = texture.GetPixel(x, y);
        Color blended = Color.Lerp(current, color, color.a);
        blended.a = Mathf.Max(current.a, color.a);
        texture.SetPixel(x, y, blended);
    }
}
