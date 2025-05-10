using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

public class LevelGeneration : MonoBehaviour
{
    [Header("GameObjects à instancier")]
    public GameObject MainBall;
    public GameObject objectToSpawn;
    public GameObject additionalObjectToSpawn;
    public GameObject killableObjectToSpawn;

    [Header("Paramètres de génération")]
    public int minObjects = 5;
    public int maxObjects = 20;
    public int additionalObjectsCount = 15;
    public int killableObjectsCount = 5;
    public float sphereDiameter = 0.8f;

    [Header("Paramètres supplémentaires")]
    public bool Once = true;
    public LineRenderer lineRenderer;
    private bool bCanShoot = true;

    [Header("Statistics")]
    public int lifesMainBall = 5;

    private int ballsCount;
    private int TotalScore;

    [Header("Points de Spawn")]
    public Transform[] spawnPoints;
    private int currentSpawnIndex = 0;

    [Header("Rigidbody et physique")]
    public Rigidbody2D rb;
    public int clickForce = 500;

    private HashSet<Vector3> usedPositions = new HashSet<Vector3>();

    [Header("Containers")]
    private Transform mainBallContainer;
    private Transform objectContainer;
    private Transform additionalObjectContainer;
    private Transform killableObjectContainer;
    private Transform spawnPointContainer;

    public TextMeshProUGUI textMeshProUGUILife;
    public TextMeshProUGUI textMeshProUGUIScore;

    public int shootCount = 0;
    public Victory victoryScript;
    public List<GameObject> marbles;

    private bool isAimingWithTouch = false;
    private Vector3 currentAimDirection = Vector3.up;
    private bool hasAimed = false;

    private void OnEnable() => GetScore.OnScoreChanged += UpdateScoreDisplay;
    private void OnDisable() => GetScore.OnScoreChanged -= UpdateScoreDisplay;

    private void Start()
    {
        victoryScript.levelGeneration = this;
        CreateHierarchyContainers();
        GenerateSpawnPoints();
        UpdateScoreDisplay(TotalScore);
        Init();
    }

    private void Update()
    {
#if UNITY_EDITOR
        isAimingWithTouch = Input.GetMouseButton(0);
#else
        isAimingWithTouch = Input.touchCount > 0 && Input.GetTouch(0).phase != TouchPhase.Ended;
#endif
        if (isAimingWithTouch && bCanShoot && rb != null && !EventSystem.current.IsPointerOverGameObject())
        {
            UpdateAimDirection();
            hasAimed = true;
        }
    }

    private void UpdateAimDirection()
    {
        Vector3 screenPos = Input.mousePosition;
        screenPos.z = 0;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;

        currentAimDirection = (worldPos - rb.transform.position).normalized;

        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        if (rb == null || currentAimDirection == Vector3.zero) return;

        lineRenderer.SetPosition(0, rb.transform.position);
        lineRenderer.SetPosition(1, rb.transform.position + currentAimDirection * 2f);
    }

    public void Shoot()
    {
        if (!bCanShoot || currentAimDirection == Vector3.zero || rb == null) return;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(currentAimDirection * clickForce);

        bCanShoot = false;
        hasAimed = false;
        shootCount++;
        ballsCount--;

        textMeshProUGUILife.text = "Balls remaining : " + ballsCount.ToString();
    }

    public void TryShoot()
    {
        if (!bCanShoot || rb == null) return;

        if (!hasAimed)
        {
            hasAimed = true;
        }

        Shoot();
    }

    public void ShootFromUI() => TryShoot();

    private void UpdateScoreDisplay(int newScore)
    {
        if (textMeshProUGUIScore != null)
            textMeshProUGUIScore.text = "Score: " + newScore.ToString();
    }

    public void Init()
    {
        shootCount = 0;
        usedPositions.Clear();
        marbles.Clear();

        foreach (Transform t in mainBallContainer) Destroy(t.gameObject);
        foreach (Transform t in objectContainer) Destroy(t.gameObject);
        foreach (Transform t in additionalObjectContainer) Destroy(t.gameObject);
        foreach (Transform t in killableObjectContainer) Destroy(t.gameObject);

        SpawnMainBall();
        GetScore.totalScore = 0;

        SpawnObjects(additionalObjectToSpawn, additionalObjectsCount);
        SpawnObjects(objectToSpawn, Random.Range(minObjects, maxObjects));
        SpawnObjects(killableObjectToSpawn, killableObjectsCount);

        ballsCount = lifesMainBall;
        textMeshProUGUILife.text = "Balls remaining : " + ballsCount.ToString();
    }

    public void SpawnLeft()
    {
        if (!bCanShoot) return;
        currentSpawnIndex = (currentSpawnIndex - 1 + spawnPoints.Length) % spawnPoints.Length;
        RespawnMainBallAtCurrentPoint();
    }

    public void SpawnRight()
    {
        if (!bCanShoot) return;
        currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Length;
        RespawnMainBallAtCurrentPoint();
    }

    private void SpawnMainBall()
    {
        Transform spawnPoint = spawnPoints[currentSpawnIndex];
        GameObject spawnedBall = Instantiate(MainBall, spawnPoint.position, Quaternion.identity);
        spawnedBall.transform.SetParent(mainBallContainer);

        rb = spawnedBall.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Static;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        if (victoryScript != null)
            victoryScript.UpdateMainBall(spawnedBall);

        bCanShoot = true;
        UpdateLineRenderer();
        victoryScript.puggleAgent.RequestDecision();
    }

    public void RespawnMainBallAtCurrentPoint()
    {
        if (rb != null)
            Destroy(rb.gameObject);

        SpawnMainBall();

        if (ballsCount <= 0)
        {
            Debug.Log("PERDU");
            Init();
        }
    }

    private void CreateHierarchyContainers()
    {
        mainBallContainer = new GameObject("MainBall").transform;
        objectContainer = new GameObject("BlueDots").transform;
        additionalObjectContainer = new GameObject("GreenDots").transform;
        killableObjectContainer = new GameObject("OrangeDots").transform;
        spawnPointContainer = new GameObject("SpawnPoints").transform;
    }

    private void GenerateSpawnPoints()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Aucune caméra principale trouvée !");
            return;
        }

        float screenWidth = 2f * mainCamera.orthographicSize * mainCamera.aspect;
        float yPosition = 4f;
        float spacing = screenWidth / 6f;

        List<Transform> points = new List<Transform>();
        for (int i = 0; i < 5; i++)
        {
            float xPosition = -screenWidth / 2f + spacing * (i + 1);
            GameObject spawnPoint = new GameObject($"SpawnPoint_{i + 1}");
            spawnPoint.transform.position = new Vector3(xPosition, yPosition, -0.01f) + transform.position;

            GameObject visual = CreateSpawnPointVisual(spawnPoint.transform);
            visual.transform.SetParent(spawnPoint.transform);

            spawnPoint.transform.SetParent(spawnPointContainer.transform);
            points.Add(spawnPoint.transform);
        }

        spawnPoints = points.ToArray();
    }

    private GameObject CreateSpawnPointVisual(Transform parent)
    {
        GameObject circle = new GameObject("Visual");
        SpriteRenderer renderer = circle.AddComponent<SpriteRenderer>();
        renderer.sprite = GenerateCircleSprite();
        renderer.color = Color.red;
        circle.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
        circle.transform.position = parent.position;
        return circle;
    }

    private Sprite GenerateCircleSprite()
    {
        Texture2D texture = new Texture2D(128, 128);
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(64, 64));
                texture.SetPixel(x, y, distance <= 64 ? Color.white : Color.clear);
            }
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
    }

    private void SpawnObjects(GameObject prefab, int count)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Aucune caméra principale trouvée !");
            return;
        }

        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;

        float minX = -screenWidth / 2f;
        float maxX = screenWidth / 2f;
        float minY = Mathf.Max(-4f, -screenHeight / 2f);
        float maxY = Mathf.Min(3f, screenHeight / 2f);

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition;
            int attempts = 0;

            do
            {
                float randomX = Random.Range(minX, maxX);
                float randomY = Random.Range(minY, maxY);
                spawnPosition = new Vector3(randomX, randomY, 0) + transform.position;

                bool positionValid = true;
                foreach (Vector3 usedPosition in usedPositions)
                {
                    if (Vector3.Distance(spawnPosition, usedPosition) < sphereDiameter)
                    {
                        positionValid = false;
                        break;
                    }
                }

                if (positionValid)
                {
                    usedPositions.Add(spawnPosition);
                    break;
                }

                attempts++;
                if (attempts > 100)
                {
                    Debug.LogWarning("Impossible de trouver une position valide après 100 tentatives.");
                    break;
                }
            } while (true);

            if (attempts <= 100)
            {
                GameObject spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
                marbles.Add(spawnedObject);

                if (prefab == objectToSpawn)
                    spawnedObject.transform.SetParent(objectContainer);
                else if (prefab == additionalObjectToSpawn)
                    spawnedObject.transform.SetParent(additionalObjectContainer);
                else if (prefab == killableObjectToSpawn)
                    spawnedObject.transform.SetParent(killableObjectContainer);
            }
        }
    }
}
