using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Diagnostics.Contracts;
using TMPro;
using System.Linq.Expressions;
using System.Linq;

public class LevelGeneration : MonoBehaviour
{
    // Section: GameObjects � instancier
    [Header("GameObjects � instancier")]
    public GameObject MainBall;                   // Le premier prefab � instancier

    public GameObject objectToSpawn;              // Le premier prefab � instancier
    public GameObject additionalObjectToSpawn;   // Le second prefab � instancier
    public GameObject killableObjectToSpawn;     // Le troisi�me prefab � instancier

    // Section: Param�tres de g�n�ration
    [Header("Param�tres de g�n�ration")]
    public int minObjects = 5;                   // Nombre minimum d'objets � g�n�rer

    public int maxObjects = 20;                  // Nombre maximum d'objets � g�n�rer
    public int additionalObjectsCount = 15;      // Nombre d'objets suppl�mentaires � g�n�rer
    public int killableObjectsCount = 5;         // Nombre d'objets "�liminables" � g�n�rer
    public float sphereDiameter = 0.8f;          // Diam�tre des sph�res

    // Section: Autres param�tres
    [Header("Param�tres suppl�mentaires")]
    public bool Once = true;

    public LineRenderer lineRenderer;
    private bool bCanShoot = true;

    // Section: Autres param�tres
    [Header("Statistics")]
    public int lifesMainBall = 5;

    private int ballsCount;
    private int TotalScore;
    private int TempScore;

    // Section: Spawn Points
    [Header("Points de Spawn")]
    public Transform[] spawnPoints;

    private int currentSpawnIndex = 0; // Index du point de spawn actif

    // Section: Rigidbody et physique
    [Header("Rigidbody et physique")]
    public Rigidbody2D rb;

    public int clickForce = 500;

    // Liste globale des positions utilis�es
    private HashSet<Vector3> usedPositions = new HashSet<Vector3>();

    // Section: Container d'objets
    [Header("Container d'objets")]
    private Transform mainBallContainer;

    private Transform objectContainer;
    private Transform additionalObjectContainer;
    private Transform killableObjectContainer;
    private Transform spawnPointContainer;

    public TextMeshProUGUI textMeshProUGUILife;
    public TextMeshProUGUI textMeshProUGUIScore;

    public int shootCount = 0;

    public Victory victoryScript; // R�f�rence au script Victory

    public List<GameObject> marbles;
    public Vector3 dir;

    private void OnEnable()
    {
        // S'abonner � l'�v�nement
        GetScore.OnScoreChanged += UpdateScoreDisplay;
    }

    private void OnDisable()
    {
        // Se d�sabonner de l'�v�nement pour �viter les erreurs
        GetScore.OnScoreChanged -= UpdateScoreDisplay;
    }

    private void CreateHierarchyContainers()
    {
        // Cr�er des conteneurs pour organiser les objets
        mainBallContainer = new GameObject("MainBall").transform;
        objectContainer = new GameObject("BlueDots").transform;
        additionalObjectContainer = new GameObject("GreenDots").transform;
        killableObjectContainer = new GameObject("OrangeDots").transform;
        spawnPointContainer = new GameObject("SpawnPoints").transform;
    }

    private void Start()
    {
        victoryScript.levelGeneration = this;

        // Cr�er des conteneurs pour organiser les objets
        CreateHierarchyContainers();

        // G�n�rer dynamiquement les points de spawn
        GenerateSpawnPoints();
        //// Mettre � jour l'affichage du score au d�marrage
        UpdateScoreDisplay(TotalScore);

        Init();
    }

    public void Init()
    {
        shootCount = 0;
        usedPositions.Clear();
        marbles.Clear();

        foreach (Transform t in mainBallContainer)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in objectContainer)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in additionalObjectContainer)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in killableObjectContainer)
        {
            Destroy(t.gameObject);
        }

        // G�n�rer la MainBall et assigner son Rigidbody2D � 'rb'
        SpawnMainBall();

        GetScore.totalScore = 0;

        // G�n�rer les objets
        SpawnObjects(additionalObjectToSpawn, additionalObjectsCount);
        SpawnObjects(objectToSpawn, Random.Range(minObjects, maxObjects));

        SpawnObjects(killableObjectToSpawn, killableObjectsCount);

        ballsCount = lifesMainBall;

        textMeshProUGUILife.text = "Balls remaining : " + ballsCount.ToString();
    }

    private void Update()
    {
        Aim();
    }

    #region ---------------ScoreDisplay---------------

    private void UpdateScoreDisplay(int newScore)
    {
        if (textMeshProUGUIScore != null)
        {
            textMeshProUGUIScore.text = "Score: " + (newScore).ToString();
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI non assign� !");
        }
    }

    #endregion ---------------ScoreDisplay---------------

    #region ---------------MainBall---------------

    public void Shoot()
    {
        if (!bCanShoot) return; // ⛔ Empêche de tirer si ce n'est pas autorisé

        if (dir == Vector3.zero)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            dir = (mousePos - rb.transform.position).normalized;
        }

        Debug.Log("Shoot");
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(dir * clickForce);

        bCanShoot = false;

        shootCount++;
        ballsCount--;

        textMeshProUGUILife.text = "Balls remaining : " + ballsCount.ToString();
    }



    private void Aim()
    {
        if (bCanShoot)
        {
            // Gestion des touches pour changer de point de spawn
            if (Input.GetKeyDown(KeyCode.A) || (Input.GetMouseButtonDown(0)))
            {
                // Passer au point de spawn pr�c�dent
                currentSpawnIndex = (currentSpawnIndex - 1 + spawnPoints.Length) % spawnPoints.Length;
                //Debug.Log("Point de spawn actuel : " + currentSpawnIndex);

                RespawnMainBallAtCurrentPoint();
            }

            if (Input.GetKeyDown(KeyCode.D) || (Input.GetMouseButtonDown(1)))
            {
                // Passer au point de spawn suivant
                currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Length;
                //Debug.Log("Point de spawn actuel : " + currentSpawnIndex);

                RespawnMainBallAtCurrentPoint();
            }

            // R�cup�rer la position de la souris en coordonn�es mondiales
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f; // Assurer une position Z correcte pour un jeu 2D

            // Calculer la direction vers la souris
            Vector3 mouseDir = (mousePos - rb.transform.position).normalized;

            // Mettre � jour le LineRenderer
            lineRenderer.SetPosition(0, mousePos);
            lineRenderer.SetPosition(1, rb.transform.position);

            // Appliquer une force lors du clic gauche / Espace

            if (((Input.GetKeyDown(KeyCode.Space)) || (Input.GetMouseButtonDown(2))))
            {
                dir = mouseDir;

                Shoot();
            }
        }
    }

    private void SpawnMainBall()
    {
        // V�rifier si le tableau spawnPoints contient des �l�ments
        if (spawnPoints.Length > 0)
        {
            // S�lectionner un transform al�atoire dans le tableau spawnPoints
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instancier la MainBall au point de spawn actif
            Transform spawnPoint = spawnPoints[currentSpawnIndex];
            GameObject spawnedBall = Instantiate(MainBall, spawnPoint.position, Quaternion.identity);

            // Ajouter la MainBall dans le conteneur
            spawnedBall.transform.SetParent(mainBallContainer);

            // R�cup�rer le Rigidbody2D de la MainBall instanci�e
            rb = spawnedBall.GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                Debug.LogError("Le prefab MainBall ne poss�de pas de Rigidbody2D !");
            }
            else
            {
                // D�finir le Rigidbody2D comme Static
                rb.bodyType = RigidbodyType2D.Static;

                // D�finir la d�tection des collisions sur Continuous
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }

            // Mettre � jour la mainBall dans le script Victory
            if (victoryScript != null)
            {
                victoryScript.UpdateMainBall(spawnedBall); // Assigner la nouvelle MainBall
            }
        }
        else
        {
            Debug.LogError("Aucun point de spawn disponible !");
        }

        bCanShoot = true;

        victoryScript.puggleAgent.RequestDecision();
    }

    public void RespawnMainBallAtCurrentPoint() //Respawn ball after Death
    {
        // Supprimer la balle actuelle si elle existe
        if (rb != null)
        {
            Destroy(rb.gameObject);
        }

        // Instancier la MainBall au point de spawn actif
        Transform spawnPoint = spawnPoints[currentSpawnIndex];
        GameObject spawnedBall = Instantiate(MainBall, spawnPoint.position, Quaternion.identity);

        // Ajouter la MainBall dans le conteneur
        spawnedBall.transform.SetParent(mainBallContainer);

        // R�cup�rer et configurer le Rigidbody2D de la balle instanci�e
        rb = spawnedBall.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Le prefab MainBall ne poss�de pas de Rigidbody2D !");
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Static;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        // Mettre � jour la mainBall dans le script Victory
        if (victoryScript != null)
        {
            victoryScript.UpdateMainBall(spawnedBall); // Assigner la nouvelle MainBall
        }

        // R�initialiser le flag pour permettre de tirer � nouveau
        bCanShoot = true;

        if (ballsCount <= 0)
        {
            Debug.Log("PERDU");
            //victoryScript.puggleAgent.AddReward(-40.0f);
            //victoryScript.puggleAgent.EndEpisode();

            Init();
        }
        else
        {
            //victoryScript.puggleAgent.AddReward(-0.1f);
            //victoryScript.puggleAgent.RequestDecision();
        }
    }

    #endregion ---------------MainBall---------------

    #region ---------------Spawn---------------

    private void GenerateSpawnPoints()
    {
        // Obtenir les dimensions de l'�cran
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Aucune cam�ra principale trouv�e !");
            return;
        }

        float screenWidth = 2f * mainCamera.orthographicSize * mainCamera.aspect;

        // Position verticale (hauteur constante)
        float yPosition = 4f;

        // Calcul de l'espacement entre les points
        float spacing = screenWidth / 6f; // Diviser par 6 pour laisser de l'espace sur les bords

        // G�n�rer les points de spawn
        List<Transform> points = new List<Transform>();
        for (int i = 0; i < 5; i++)
        {
            // Calculer la position du point
            float xPosition = -screenWidth / 2f + spacing * (i + 1);

            // Cr�er un nouveau GameObject pour repr�senter le point de spawn
            GameObject spawnPoint = new GameObject($"SpawnPoint_{i + 1}");
            spawnPoint.transform.position = new Vector3(xPosition, yPosition, -0.01f) + transform.position;

            // Ajouter un visuel pour repr�senter le point de spawn
            GameObject visual = CreateSpawnPointVisual(spawnPoint.transform);
            visual.transform.SetParent(spawnPoint.transform); // Faire du cercle un enfant du point

            // Ajouter le point comme enfant du conteneur
            spawnPoint.transform.SetParent(spawnPointContainer.transform);

            // Ajouter le Transform � la liste
            points.Add(spawnPoint.transform);
        }

        // Assigner les points g�n�r�s au tableau spawnPoints
        spawnPoints = points.ToArray();
    }

    // Cr�e un cercle visuel pour repr�senter un point de spawn
    private GameObject CreateSpawnPointVisual(Transform parent)
    {
        // Cr�er un GameObject pour le visuel
        GameObject circle = new GameObject("Visual");

        // Ajouter un composant SpriteRenderer pour afficher un cercle
        SpriteRenderer renderer = circle.AddComponent<SpriteRenderer>();
        renderer.sprite = GenerateCircleSprite();
        renderer.color = Color.red; // Couleur du cercle

        // R�duire la taille pour qu'il soit petit
        circle.transform.localScale = new Vector3(0.1f, 0.1f, 1f);

        // Positionner au centre du point de spawn
        circle.transform.position = parent.position;

        return circle;
    }

    // G�n�re un Sprite circulaire � utiliser pour les visuels
    private Sprite GenerateCircleSprite()
    {
        Texture2D texture = new Texture2D(128, 128);
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                // Calculer la distance au centre
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(64, 64));
                texture.SetPixel(x, y, distance <= 64 ? Color.white : Color.clear);
            }
        }
        texture.Apply();

        // Cr�er le sprite � partir de la texture
        return Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
    }

    private void SpawnObjects(GameObject prefab, int count)//Spawn All Objects hitable
    {
        // Obtenir les limites de l'�cran
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Aucune cam�ra principale trouv�e !");
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

            // Rechercher une position valide
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
                    Debug.LogWarning("Impossible de trouver une position valide apr�s 100 tentatives.");
                    break;
                }
            } while (true);

            // Instancier l'objet si une position valide a �t� trouv�e
            if (attempts <= 100)
            {
                GameObject spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
                marbles.Add(spawnedObject);

                // Ajouter l'objet dans le conteneur appropri�
                if (prefab == objectToSpawn)
                    spawnedObject.transform.SetParent(objectContainer);
                else if (prefab == additionalObjectToSpawn)
                    spawnedObject.transform.SetParent(additionalObjectContainer);
                else if (prefab == killableObjectToSpawn)
                    spawnedObject.transform.SetParent(killableObjectContainer);
            }
        }
        Debug.Log("BilleCount : " + count);
    }

    #endregion ---------------Spawn---------------
}