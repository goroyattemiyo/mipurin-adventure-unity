using System.Collections.Generic;
using UnityEngine;

public class CharacterTestEnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject honeySlimePrefab;
    [SerializeField] private GameObject poisonMushroomPrefab;
    [SerializeField] private GameObject stingerBeePrefab;
    [SerializeField] private GameObject flowerTurretPrefab;
    [SerializeField] private GameObject heavyBeetlePrefab;

    [Header("Spawn")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float nextWaveDelay = 1.6f;
    [SerializeField] private int nectarGoal = 18;
    [SerializeField] private int maxWave = 5;

    [Header("Wave Reward")]
    [SerializeField] private int waveClearHealAmount = 1;

    [Header("Spawn Effect")]
    [SerializeField] private bool showSpawnEffect = true;
    [SerializeField] private Color spawnEffectColor = new Color(1f, 0.82f, 0.16f, 0.75f);

    [Header("Runtime")]
    [SerializeField] private int currentWave;
    [SerializeField] private bool stageCleared;
    [SerializeField] private int spawnedEnemyCount;
    [SerializeField] private int waveBonusCount;

    private readonly List<MipurinEnemy> aliveEnemies = new List<MipurinEnemy>();
    private NectarWallet nectarWallet;
    private MipurinHealth playerHealth;
    private float nextWaveTimer;
    private bool waitingForNextWave;

    public int CurrentWave => currentWave;
    public int AliveEnemyCount => CountAliveEnemies();
    public int DefeatedEnemyCount => Mathf.Max(0, spawnedEnemyCount - AliveEnemyCount);
    public int WaveBonusCount => waveBonusCount;
    public float NextWaveTimer => nextWaveTimer;
    public bool WaitingForNextWave => waitingForNextWave;
    public bool StageCleared => stageCleared;
    public int NectarGoal => nectarGoal;
    public int MaxWave => maxWave;

    private void Start()
    {
        ApplyPhase3Balance();
        nectarWallet = FindObjectOfType<NectarWallet>();
        playerHealth = FindObjectOfType<MipurinHealth>();
        StartWave(1);
    }

    private void Update()
    {
        if (CheckRestartInput())
        {
            return;
        }

        if (nectarWallet == null)
        {
            nectarWallet = FindObjectOfType<NectarWallet>();
        }

        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<MipurinHealth>();
        }

        if (stageCleared)
        {
            return;
        }

        if (nectarWallet != null && nectarWallet.Nectar >= nectarGoal)
        {
            ClearStage();
            return;
        }

        if (waitingForNextWave)
        {
            nextWaveTimer -= Time.deltaTime;
            if (nextWaveTimer <= 0f)
            {
                StartWave(currentWave + 1);
            }

            return;
        }

        if (CountAliveEnemies() <= 0)
        {
            if (currentWave >= maxWave)
            {
                ClearStage();
                return;
            }

            ApplyWaveClearBonus();
            waitingForNextWave = true;
            nextWaveTimer = nextWaveDelay;
        }
    }

    public void Configure(GameObject honeyPrefab, GameObject mushroomPrefab, Transform[] points, int targetNectarGoal, int targetMaxWave, float delay)
    {
        honeySlimePrefab = honeyPrefab;
        poisonMushroomPrefab = mushroomPrefab;
        spawnPoints = points;
        nectarGoal = Mathf.Max(1, targetNectarGoal);
        maxWave = Mathf.Max(1, targetMaxWave);
        nextWaveDelay = Mathf.Max(0.1f, delay);
        ApplyPhase3Balance();
    }

    public void ConfigurePhase4EnemyPrefabs(GameObject honeyPrefab, GameObject mushroomPrefab, GameObject stingerPrefab, GameObject turretPrefab, GameObject beetlePrefab)
    {
        honeySlimePrefab = honeyPrefab;
        poisonMushroomPrefab = mushroomPrefab;
        stingerBeePrefab = stingerPrefab;
        flowerTurretPrefab = turretPrefab;
        heavyBeetlePrefab = beetlePrefab;
    }

    public void RestartTestRun()
    {
        ClearAliveEnemies();
        ClearLoosePickups();

        if (nectarWallet == null)
        {
            nectarWallet = FindObjectOfType<NectarWallet>();
        }

        if (nectarWallet != null)
        {
            nectarWallet.ResetNectar();
        }

        ResetPlayerState();
        ApplyPhase3Balance();
        currentWave = 0;
        spawnedEnemyCount = 0;
        waveBonusCount = 0;
        stageCleared = false;
        waitingForNextWave = false;
        nextWaveTimer = 0f;
        StartWave(1);
    }

    private void ApplyPhase3Balance()
    {
        nectarGoal = Mathf.Max(nectarGoal, 18);
        maxWave = Mathf.Max(maxWave, 5);
        nextWaveDelay = Mathf.Clamp(nextWaveDelay, 1.4f, 2f);
        waveClearHealAmount = Mathf.Max(0, waveClearHealAmount);
    }

    private void StartWave(int wave)
    {
        ClearDeadEnemyReferences();

        currentWave = Mathf.Max(1, wave);
        waitingForNextWave = false;
        nextWaveTimer = 0f;

        int spawnOffset = 0;
        int honeyCount = GetHoneyCount(currentWave);
        int mushroomCount = GetMushroomCount(currentWave);
        int stingerCount = GetStingerCount(currentWave);
        int turretCount = GetTurretCount(currentWave);
        int beetleCount = GetBeetleCount(currentWave);

        SpawnEnemies(honeySlimePrefab, "HoneySlime", honeyCount, spawnOffset, 1);
        spawnOffset += honeyCount;

        SpawnEnemies(poisonMushroomPrefab, "PoisonMushroom", mushroomCount, spawnOffset, 2);
        spawnOffset += mushroomCount;

        SpawnEnemies(GetPrefabOrFallback(stingerBeePrefab, honeySlimePrefab), "StingerBee", stingerCount, spawnOffset, 1);
        spawnOffset += stingerCount;

        SpawnEnemies(GetPrefabOrFallback(flowerTurretPrefab, poisonMushroomPrefab), "FlowerTurret", turretCount, spawnOffset, 2);
        spawnOffset += turretCount;

        SpawnEnemies(GetPrefabOrFallback(heavyBeetlePrefab, honeySlimePrefab), "HeavyBeetle", beetleCount, spawnOffset, 3);

        Debug.Log($"Wave {currentWave} started. HoneySlime: {honeyCount}, PoisonMushroom: {mushroomCount}, StingerBee: {stingerCount}, FlowerTurret: {turretCount}, HeavyBeetle: {beetleCount}");
    }

    private GameObject GetPrefabOrFallback(GameObject prefab, GameObject fallback)
    {
        return prefab != null ? prefab : fallback;
    }

    private int GetHoneyCount(int wave)
    {
        switch (wave)
        {
            case 1: return 1;
            case 2: return 2;
            case 3: return 2;
            case 4: return 2;
            default: return 2;
        }
    }

    private int GetMushroomCount(int wave)
    {
        switch (wave)
        {
            case 1: return 0;
            case 2: return 1;
            case 3: return 1;
            case 4: return 1;
            default: return 1;
        }
    }

    private int GetStingerCount(int wave)
    {
        if (wave < 3)
        {
            return 0;
        }

        return wave >= 5 ? 2 : 1;
    }

    private int GetTurretCount(int wave)
    {
        return wave >= 4 ? 1 : 0;
    }

    private int GetBeetleCount(int wave)
    {
        return wave >= 5 ? 1 : 0;
    }

    private void SpawnEnemies(GameObject prefab, string prototypeName, int count, int spawnOffset, int nectarAmount)
    {
        if (prefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Transform point = spawnPoints[(i + spawnOffset) % spawnPoints.Length];
            Vector3 randomOffset = new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.18f, 0.18f), 0f);
            Vector3 spawnPosition = point.position + randomOffset;
            GameObject enemyObject = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
            enemyObject.name = "Enemy_" + prototypeName + "_Wave" + currentWave + "_" + (i + 1);
            spawnedEnemyCount++;
            PlaySpawnEffect(spawnPosition);

            EnemyDropper dropper = enemyObject.GetComponent<EnemyDropper>();
            if (dropper != null)
            {
                dropper.ConfigureAmount(nectarAmount);
            }

            MipurinEnemy enemy = enemyObject.GetComponent<MipurinEnemy>();
            if (enemy != null)
            {
                enemy.ApplyRuntimePrototype(prototypeName);
                enemy.SetTarget(FindPlayerTransform());
                aliveEnemies.Add(enemy);
            }
        }
    }

    private void ApplyWaveClearBonus()
    {
        if (currentWave <= 0)
        {
            return;
        }

        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<MipurinHealth>();
        }

        if (playerHealth != null && waveClearHealAmount > 0 && playerHealth.CurrentHp < playerHealth.MaxHp)
        {
            playerHealth.Heal(waveClearHealAmount);
            PickupFloatingText.Show(playerHealth.transform.position + new Vector3(0f, 0.7f, 0f), "WAVE BONUS HP +" + waveClearHealAmount);
        }
        else if (playerHealth != null)
        {
            PickupFloatingText.Show(playerHealth.transform.position + new Vector3(0f, 0.7f, 0f), "WAVE BONUS");
        }

        waveBonusCount++;
    }

    private void PlaySpawnEffect(Vector3 position)
    {
        if (!showSpawnEffect)
        {
            return;
        }

        GameObject effectObject = new GameObject("SpawnEffect_Burst");
        effectObject.transform.SetParent(transform);
        effectObject.transform.position = position;

        ParticleSystem particles = effectObject.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particles.main;
        main.duration = 0.25f;
        main.loop = false;
        main.startLifetime = 0.28f;
        main.startSpeed = 1.1f;
        main.startSize = 0.09f;
        main.startColor = spawnEffectColor;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 12) });

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.18f;

        ParticleSystemRenderer renderer = effectObject.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = 12;
        }

        particles.Play();
        Destroy(effectObject, 0.6f);
    }

    private Transform FindPlayerTransform()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<MipurinHealth>();
        }

        return playerHealth != null ? playerHealth.transform : null;
    }

    private int CountAliveEnemies()
    {
        ClearDeadEnemyReferences();
        int count = 0;

        foreach (MipurinEnemy enemy in aliveEnemies)
        {
            if (enemy != null && enemy.IsAlive)
            {
                count++;
            }
        }

        return count;
    }

    private void ClearDeadEnemyReferences()
    {
        for (int i = aliveEnemies.Count - 1; i >= 0; i--)
        {
            if (aliveEnemies[i] == null)
            {
                aliveEnemies.RemoveAt(i);
            }
        }
    }

    private void ClearAliveEnemies()
    {
        for (int i = aliveEnemies.Count - 1; i >= 0; i--)
        {
            if (aliveEnemies[i] != null)
            {
                Destroy(aliveEnemies[i].gameObject);
            }
        }

        aliveEnemies.Clear();

        MipurinEnemy[] sceneEnemies = FindObjectsOfType<MipurinEnemy>();
        foreach (MipurinEnemy enemy in sceneEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
    }

    private void ClearLoosePickups()
    {
        PickupItem[] pickups = FindObjectsOfType<PickupItem>();
        foreach (PickupItem pickup in pickups)
        {
            if (pickup != null)
            {
                Destroy(pickup.gameObject);
            }
        }

        EnemyProjectile[] projectiles = FindObjectsOfType<EnemyProjectile>();
        foreach (EnemyProjectile projectile in projectiles)
        {
            if (projectile != null)
            {
                Destroy(projectile.gameObject);
            }
        }
    }

    private void ResetPlayerState()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<MipurinHealth>();
        }

        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
            playerHealth.transform.position = Vector3.zero;
        }
    }

    private void ClearStage()
    {
        stageCleared = true;
        waitingForNextWave = false;
        nextWaveTimer = 0f;
        StoryProgress.Instance.ObtainGoldenHoneyShardA();
        Debug.Log("CharacterTest Stage Clear! GoldenHoneyShardA obtained.");
    }

    private bool CheckRestartInput()
    {
#if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartTestRun();
            return true;
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartTestRun();
            return true;
        }
#endif
        return false;
    }
}
