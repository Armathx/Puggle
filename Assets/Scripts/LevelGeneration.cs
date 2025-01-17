using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Diagnostics.Contracts;
using TMPro;
using System.Linq.Expressions;

public class LevelGeneration : MonoBehaviour
{
    // Section: GameObjects à instancier
    [Header("GameObjects à instancier")]
    public GameObject MainBall;                   // Le premier prefab à instancier
    public GameObject objectToSpawn;              // Le premier prefab à instancier
    public GameObject additionalObjectToSpawn;   // Le second prefab à instancier
    public GameObject killableObjectToSpawn;     // Le troisième prefab à instancier

    // Section: Paramètres de génération
    [Header("Paramètres de génération")]
    public int minObjects = 5;                   // Nombre minimum d'objets à générer
    public int maxObjects = 20;                  // Nombre maximum d'objets à générer
    public int additionalObjectsCount = 15;      // Nombre d'objets supplémentaires à générer
    public int killableObjectsCount = 5;         // Nombre d'objets "éliminables" à générer
    public float sphereDiameter = 0.8f;          // Diamètre des sphères

    // Section: Autres paramètres
    [Header("Paramètres supplémentaires")]
    public bool Once = true;
    public LineRenderer lineRenderer;
    bool bCanShoot = true;

    // Section: Autres paramètres
    [Header("Statistics")]
    public int lifesMainBall = 5;
    int ballsCount;
    int TotalScore;
    int TempScore;


    // Section: Spawn Points
    [Header("Points de Spawn")]
    public Transform[] spawnPoints;
    private int currentSpawnIndex = 0; // Index du point de spawn actif

    // Section: Rigidbody et physique
    [Header("Rigidbody et physique")]
    public Rigidbody2D rb;
    public int clickForce = 500;

    // Liste globale des positions utilisées
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

    public Victory victoryScript; // Référence au script Victory

    public List<GameObject> marbles;

    private void OnEnable()
    {
        // S'abonner à l'événement
        GetScore.OnScoreChanged += UpdateScoreDisplay;
    }

    private void OnDisable()
    {
        // Se désabonner de l'événement pour éviter les erreurs
        GetScore.OnScoreChanged -= UpdateScoreDisplay;
    }


    void CreateHierarchyContainers()
    {
        // Créer des conteneurs pour organiser les objets
        mainBallContainer = new GameObject("MainBall").transform;
        objectContainer = new GameObject("BlueDots").transform;
        additionalObjectContainer = new GameObject("GreenDots").transform;
        killableObjectContainer = new GameObject("OrangeDots").transform;
        spawnPointContainer = new GameObject("SpawnPoints").transform;
    }


    private void Start()
    {

        victoryScript.levelGeneration = this;

        // Créer des conteneurs pour organiser les objets
        CreateHierarchyContainers();

        // Générer dynamiquement les points de spawn
        GenerateSpawnPoints();
        //// Mettre à jour l'affichage du score au démarrage
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

        // Générer la MainBall et assigner son Rigidbody2D à 'rb'
        SpawnMainBall();

        GetScore.totalScore = 0;

        // Générer les objets
        SpawnObjects(objectToSpawn, Random.Range(minObjects, maxObjects));
        SpawnObjects(additionalObjectToSpawn, additionalObjectsCount);
        SpawnObjects(killableObjectToSpawn, killableObjectsCount);

        ballsCount = lifesMainBall;

        textMeshProUGUILife.text = "Balls remaining : " + ballsCount.ToString();

    }


    void Update()
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
            Debug.LogWarning("TextMeshProUGUI non assigné !");
        }


    }

    #endregion

    #region ---------------MainBall---------------

    public void Shoot(Vector3 dir)
    {
        Debug.Log("Shoot");
        rb.bodyType = RigidbodyType2D.Dynamic; // Passer le Rigidbody en mode dynamique
        rb.AddForce(dir * clickForce); // Appliquer une force

        bCanShoot = false;

        shootCount++;
        ballsCount--;

        textMeshProUGUILife.text = "Balls remaining : " + ballsCount.ToString();
    }


    void Aim()
    {

        if (bCanShoot)
        {

            // Gestion des touches pour changer de point de spawn
            if (Input.GetKeyDown(KeyCode.A))
            {
                // Passer au point de spawn précédent
                currentSpawnIndex = (currentSpawnIndex - 1 + spawnPoints.Length) % spawnPoints.Length;
                Debug.Log("Point de spawn actuel : " + currentSpawnIndex);

                RespawnMainBallAtCurrentPoint();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                // Passer au point de spawn suivant
                currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Length;
                Debug.Log("Point de spawn actuel : " + currentSpawnIndex);

                RespawnMainBallAtCurrentPoint();
            }

            // Récupérer la position de la souris en coordonnées mondiales
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f; // Assurer une position Z correcte pour un jeu 2D

            // Calculer la direction vers la souris
            Vector3 mouseDir = (mousePos - rb.transform.position).normalized;

            // Mettre à jour le LineRenderer
            lineRenderer.SetPosition(0, mousePos);
            lineRenderer.SetPosition(1, rb.transform.position);


            // Appliquer une force lors du clic gauche / Espace

            if (Input.GetMouseButtonDown(0) || (Input.GetKeyDown(KeyCode.Space)))
            {
                Shoot(mouseDir);
            }

        }

    }


    void SpawnMainBall()
    {
        // Vérifier si le tableau spawnPoints contient des éléments
        if (spawnPoints.Length > 0)
        {
            // Sélectionner un transform aléatoire dans le tableau spawnPoints
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instancier la MainBall au point de spawn actif
            Transform spawnPoint = spawnPoints[currentSpawnIndex];
            GameObject spawnedBall = Instantiate(MainBall, spawnPoint.position, Quaternion.identity);

            // Ajouter la MainBall dans le conteneur
            spawnedBall.transform.SetParent(mainBallContainer);

            // Récupérer le Rigidbody2D de la MainBall instanciée
            rb = spawnedBall.GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                Debug.LogError("Le prefab MainBall ne possède pas de Rigidbody2D !");
            }
            else
            {
                // Définir le Rigidbody2D comme Static
                rb.bodyType = RigidbodyType2D.Static;

                // Définir la détection des collisions sur Continuous
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }

            // Mettre à jour la mainBall dans le script Victory
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

        // Récupérer et configurer le Rigidbody2D de la balle instanciée
        rb = spawnedBall.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Le prefab MainBall ne possède pas de Rigidbody2D !");
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Static;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        // Mettre à jour la mainBall dans le script Victory
        if (victoryScript != null)
        {
            victoryScript.UpdateMainBall(spawnedBall); // Assigner la nouvelle MainBall
        }

        // Réinitialiser le flag pour permettre de tirer à nouveau
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

    #endregion

    #region ---------------Spawn---------------

    void GenerateSpawnPoints()
    {
        // Obtenir les dimensions de l'écran
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Aucune caméra principale trouvée !");
            return;
        }

        float screenWidth = 2f * mainCamera.orthographicSize * mainCamera.aspect;

        // Position verticale (hauteur constante)
        float yPosition = 4f;

        // Calcul de l'espacement entre les points
        float spacing = screenWidth / 6f; // Diviser par 6 pour laisser de l'espace sur les bords

        // Générer les points de spawn
        List<Transform> points = new List<Transform>();
        for (int i = 0; i < 5; i++)
        {
            // Calculer la position du point
            float xPosition = -screenWidth / 2f + spacing * (i + 1);

            // Créer un nouveau GameObject pour représenter le point de spawn
            GameObject spawnPoint = new GameObject($"SpawnPoint_{i + 1}");
            spawnPoint.transform.position = new Vector3(xPosition, yPosition, -0.01f) + transform.position;

            // Ajouter un visuel pour représenter le point de spawn
            GameObject visual = CreateSpawnPointVisual(spawnPoint.transform);
            visual.transform.SetParent(spawnPoint.transform); // Faire du cercle un enfant du point

            // Ajouter le point comme enfant du conteneur
            spawnPoint.transform.SetParent(spawnPointContainer.transform);

            // Ajouter le Transform à la liste
            points.Add(spawnPoint.transform);
        }

        // Assigner les points générés au tableau spawnPoints
        spawnPoints = points.ToArray();
    }
    // Crée un cercle visuel pour représenter un point de spawn
    GameObject CreateSpawnPointVisual(Transform parent)
    {
        // Créer un GameObject pour le visuel
        GameObject circle = new GameObject("Visual");

        // Ajouter un composant SpriteRenderer pour afficher un cercle
        SpriteRenderer renderer = circle.AddComponent<SpriteRenderer>();
        renderer.sprite = GenerateCircleSprite();
        renderer.color = Color.red; // Couleur du cercle

        // Réduire la taille pour qu'il soit petit
        circle.transform.localScale = new Vector3(0.1f, 0.1f, 1f);

        // Positionner au centre du point de spawn
        circle.transform.position = parent.position;

        return circle;
    }
    // Génère un Sprite circulaire à utiliser pour les visuels
    Sprite GenerateCircleSprite()
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

        // Créer le sprite à partir de la texture
        return Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
    }



    void SpawnObjects(GameObject prefab, int count)//Spawn All Objects hitable
    {
        // Obtenir les limites de l'écran
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
                    Debug.LogWarning("Impossible de trouver une position valide après 100 tentatives.");
                    break;
                }
            } while (true);

            // Instancier l'objet si une position valide a été trouvée
            if (attempts <= 100)
            {
                GameObject spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity);
                marbles.Add(spawnedObject);

                // Ajouter l'objet dans le conteneur approprié
                if (prefab == objectToSpawn)
                    spawnedObject.transform.SetParent(objectContainer);
                else if (prefab == additionalObjectToSpawn)
                    spawnedObject.transform.SetParent(additionalObjectContainer);
                else if (prefab == killableObjectToSpawn)
                    spawnedObject.transform.SetParent(killableObjectContainer);
            }
        }
    }

    #endregion



}
