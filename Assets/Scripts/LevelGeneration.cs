using UnityEngine;
using System.Collections.Generic;

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
    bool bCanShoot = true;

    // Section: Spawn Points
    [Header("Points de Spawn")]
    public Transform[] spawnPoints;

    // Section: Rigidbody et physique
    [Header("Rigidbody et physique")]
    public Rigidbody2D rb;
    public int clickForce = 500;

    // Liste globale des positions utilis�es
    private HashSet<Vector3> usedPositions = new HashSet<Vector3>();

    void Start()
    {
        // G�n�rer la MainBall et assigner son Rigidbody2D � 'rb'
        SpawnMainBall(MainBall);

        // G�n�rer les objets
        SpawnObjects(objectToSpawn, Random.Range(minObjects, maxObjects));
        SpawnObjects(additionalObjectToSpawn, additionalObjectsCount);
        SpawnObjects(killableObjectToSpawn, killableObjectsCount);
    }

    void FixedUpdate()
    {
        if (bCanShoot && rb != null)
        {
            // R�cup�rer la position de la souris en coordonn�es mondiales
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f; // Assurer une position Z correcte pour un jeu 2D

            // Calculer la direction vers la souris
            Vector3 mouseDir = (mousePos - rb.transform.position).normalized;

            // Mettre � jour le LineRenderer
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
        // V�rifier si le tableau spawnPoints contient des �l�ments
        if (spawnPoints.Length > 0)
        {
            // S�lectionner un transform al�atoire dans le tableau spawnPoints
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instancier le prefab MainBall � la position de spawn al�atoire
            GameObject spawnedBall = Instantiate(prefab, randomSpawnPoint.position, Quaternion.identity);

            // R�cup�rer le Rigidbody2D de la MainBall instanci�e
            rb = spawnedBall.GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                Debug.LogError("Le prefab MainBall ne poss�de pas de Rigidbody2D !");
            }
            else
            {
                // D�finir le Rigidbody2D comme Kinematic
                rb.bodyType = RigidbodyType2D.Kinematic;

                // D�finir la d�tection des collisions sur Continuous
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
        // V�rifier si le tableau spawnPoints contient des �l�ments
        if (spawnPoints.Length > 0)
        {
            // S�lectionner un transform al�atoire dans le tableau spawnPoints
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instancier la MainBall � la position de spawn al�atoire
            GameObject spawnedBall = Instantiate(MainBall, randomSpawnPoint.position, Quaternion.identity);

            // R�cup�rer le Rigidbody2D de la MainBall instanci�e
            rb = spawnedBall.GetComponent<Rigidbody2D>();

            if (rb == null)
            {
                Debug.LogError("Le prefab MainBall ne poss�de pas de Rigidbody2D !");
            }
            else
            {
                // D�finir le Rigidbody2D comme Kinematic
                rb.bodyType = RigidbodyType2D.Kinematic;

                // D�finir la d�tection des collisions sur Continuous
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }
        }
        else
        {
            Debug.LogError("Aucun point de spawn disponible !");
        }

        // R�initialiser le flag pour pouvoir tirer � nouveau apr�s le respawn
        bCanShoot = true;
    }

    void SpawnObjects(GameObject prefab, int count)
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
                    Debug.LogWarning("Impossible de trouver une position valide apr�s 100 tentatives.");
                    break;
                }
            } while (attempts <= 100);

            // Instancier l'objet si une position valide a �t� trouv�e
            if (attempts <= 100)
            {
                Instantiate(prefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}
