using System.Collections.Generic;
using UnityEngine;

public class CharacterTestEnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject honeySlimePrefab;
    [SerializeField] private GameObject poisonMushroomPrefab;

    [Header("Spawn")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float nextWaveDelay = 2f;
    [SerializeField] private int nectarGoal = 10;
    [SerializeField] private int maxWave = 5;

    [Header("Spawn Effect")]
    [SerializeField] private bool showSpawnEffect = true;
    [SerializeField] private Color spawnEffectColor = new Color(1f, 0.82f, 0.16f, 0.75f);

    [Header("Runtime")]
    [SerializeField] private int currentWave;
    [SerializeField] private bool stageCleared;
    [SerializeField] private int spawnedEnemyCount;

    private readonly List<MipurinEnemy> aliveEnemies = new List<MipurinEnemy>();
    private NectarWallet nectarWallet;
    private float nextWaveTimer;
    private bool waitingForNextWave;

    public int CurrentWave => currentWave;
    public int AliveEnemyCount => CountAliveEnemies();
    public int DefeatedEnemyCount => Mathf.Max(0, spawnedEnemyCount - AliveEnemyCount);
    public float NextWaveTimer => nextWaveTimer;
    public bool WaitingForNextWave => waitingForNextWave;
    public bool StageCleared => stageCleared;
    public int NectarGoal => nectarGoal;
    public int MaxWave => maxWave;

    private void Start()
    {
        nectarWallet = FindObjectOfType<NectarWallet>();
        StartWave(1);
    }

    private void Update()
    {
        if (nectarWallet == null)
        {
            nectarWallet = FindObjectOfType<NectarWallet>();
        }

        if (stageCleared)
        {
            CheckRestartInput();
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

            waitingForNextWave = true;
            nextWaveTimer = nextWaveDelay;
        }

        CheckRestartInput();
    }

    public void Configure(GameObject honeyPrefab, GameObject mushroomPrefab, Transform[] points, int targetNectarGoal, int targetMaxWave, float delay)
    {
        honeySlimePrefab = honeyPrefab;
        poisonMushroomPrefab = mushroomPrefab;
        spawnPoints = points;
        nectarGoal = Mathf.Max(1, targetNectarGoal);
        maxWave = Mathf.Max(1, targetMaxWave);
        nextWaveDelay = Mathf.Max(0.1f, delay);
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

        currentWave = 0;
        spawnedEnemyCount = 0;
        stageCleared = false;
        waitingForNextWave = false;
        nextWaveTimer = 0f;
        StartWave(1);
    }

    private void StartWave(int wave)
    {
        ClearDeadEnemyReferences();

        currentWave = Mathf.Max(1, wave);
        waitingForNextWave = false;
        nextWaveTimer = 0f;

        int honeyCount = 1 + Mathf.FloorToInt((currentWave - 1) * 0.75f);
        int mushroomCount = currentWave >= 2 ? 1 + Mathf.FloorToInt((currentWave - 2) * 0.5f) : 0;

        SpawnEnemies(honeySlimePrefab, honeyCount, 0);
        SpawnEnemies(poisonMushroomPrefab, mushroomCount, honeyCount);

        Debug.Log($"Wave {currentWave} started. HoneySlime: {honeyCount}, PoisonMushroom: {mushroomCount}");
    }

    private void SpawnEnemies(GameObject prefab, int count, int spawnOffset)
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
            enemyObject.name = prefab.name + "_Wave" + currentWave + "_" + (i + 1);
            spawnedEnemyCount++;
            PlaySpawnEffect(spawnPosition);

            MipurinEnemy enemy = enemyObject.GetComponent<MipurinEnemy>();
            if (enemy != null)
            {
                enemy.SetTarget(FindPlayerTransform());
                aliveEnemies.Add(enemy);
            }
        }
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
        MipurinHealth playerHealth = FindObjectOfType<MipurinHealth>();
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
    }

    private void ClearStage()
    {
        stageCleared = true;
        waitingForNextWave = false;
        nextWaveTimer = 0f;
        Debug.Log("CharacterTest Stage Clear!");
    }

    private void CheckRestartInput()
    {
#if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartTestRun();
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartTestRun();
        }
#endif
    }
}
