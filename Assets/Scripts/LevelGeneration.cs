using UnityEngine;
using System.Collections.Generic;

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

    // Section: Spawn Points
    [Header("Points de Spawn")]
    public Transform[] spawnPoints;

    // Section: Rigidbody et physique
    [Header("Rigidbody et physique")]
    public Rigidbody2D rb;
    public int clickForce = 500;

    // Liste globale des positions utilisées
    private HashSet<Vector3> usedPositions = new HashSet<Vector3>();

    void Start()
    {
        // Générer la MainBall et assigner son Rigidbody2D à 'rb'
        SpawnMainBall(MainBall);

        // Générer les objets
        SpawnObjects(objectToSpawn, Random.Range(minObjects, maxObjects));
        SpawnObjects(additionalObjectToSpawn, additionalObjectsCount);
        SpawnObjects(killableObjectToSpawn, killableObjectsCount);
    }

    void FixedUpdate()
    {
        if (bCanShoot && rb != null)
        {
            // Récupérer la position de la souris en coordonnées mondiales
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f; // Assurer une position Z correcte pour un jeu 2D

            // Calculer la direction vers la souris
            Vector3 mouseDir = (mousePos - rb.transform.position).normalized;

            // Mettre à jour le LineRenderer
            lineRenderer.SetPosition(0, mousePos);
            lineRenderer.SetPosition(1, rb.transform.position);

            // Appliquer une force lors du clic gauche
            if (Input.GetMouseButtonDown(0))
            {
                rb.bodyType = RigidbodyType2D.Dynamic; // Passer le Rigidbody en mode dynamique
                rb.AddForce(mouseDir * clickForce); // Appliquer une force

                bCanShoot = false;
            }
        }
    }

    void SpawnMainBall(GameObject prefab)
    {
        // Vérifier si le tableau spawnPoints contient des éléments
        if (spawnPoints.Length > 0)
        {
            // Sélectionner un transform aléatoire dans le tableau spawnPoints
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instancier le prefab MainBall à la position de spawn aléatoire
            GameObject spawnedBall = Instantiate(prefab, randomSpawnPoint.position, Quaternion.identity);

            // Récupérer le Rigidbody2D de la MainBall instanciée
            rb = spawnedBall.GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                Debug.LogError("Le prefab MainBall ne possède pas de Rigidbody2D !");
            }
            else
            {
                // Définir le Rigidbody2D comme Kinematic
                rb.bodyType = RigidbodyType2D.Kinematic;

                // Définir la détection des collisions sur Continuous
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }
        }
        else
        {
            Debug.LogError("Aucun point de spawn disponible !");
        }
    }

    public void RespawnMainBall()
    {
        // Vérifier si le tableau spawnPoints contient des éléments
        if (spawnPoints.Length > 0)
        {
            // Sélectionner un transform aléatoire dans le tableau spawnPoints
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instancier la MainBall à la position de spawn aléatoire
            GameObject spawnedBall = Instantiate(MainBall, randomSpawnPoint.position, Quaternion.identity);

            // Récupérer le Rigidbody2D de la MainBall instanciée
            rb = spawnedBall.GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                Debug.LogError("Le prefab MainBall ne possède pas de Rigidbody2D !");
            }
            else
            {
                // Définir le Rigidbody2D comme Kinematic
                rb.bodyType = RigidbodyType2D.Kinematic;

                // Définir la détection des collisions sur Continuous
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }
        }
        else
        {
            Debug.LogError("Aucun point de spawn disponible !");
        }

        // Réinitialiser le flag pour pouvoir tirer à nouveau après le respawn
        bCanShoot = true;
    }

    void SpawnObjects(GameObject prefab, int count)
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
                spawnPosition = new Vector3(randomX, randomY, 0);

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
            } while (attempts <= 100);

            // Instancier l'objet si une position valide a été trouvée
            if (attempts <= 100)
            {
                Instantiate(prefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}
