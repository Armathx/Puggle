using UnityEngine;
using System.Collections.Generic;

public class LevelGeneration : MonoBehaviour
{
    public GameObject objectToSpawn; // Le premier prefab à instancier
    public GameObject additionalObjectToSpawn; // Le second prefab à instancier
    public GameObject killableObjectToSpawn; // Le troisième prefab à instancier
    public int minObjects = 5; // Nombre minimum d'objets à générer
    public int maxObjects = 20; // Nombre maximum d'objets à générer
    public int additionalObjectsCount = 15; // Nombre d'objets supplémentaires à générer
    public int killableObjectsCount = 5; // Nombre d'objets "éliminables" à générer
    public float sphereDiameter = 0.8f; // Diamètre des sphères
    public bool Once = true;
    public LineRenderer lineRenderer;

    public Rigidbody2D rb;
    public int clickForce = 500;

    // Liste globale des positions utilisées
    private HashSet<Vector3> usedPositions = new HashSet<Vector3>();

    void Start()
    {
        // Générer les objets
        SpawnObjects(objectToSpawn, Random.Range(minObjects, maxObjects));
        SpawnObjects(additionalObjectToSpawn, additionalObjectsCount);
        SpawnObjects(killableObjectToSpawn, killableObjectsCount);
    }

    void FixedUpdate()
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
        }
    }

    private void OnDrawGizmos()
    {
        // Dessiner une ligne entre l'objet et la souris
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Gizmos.DrawLine(rb.transform.position, mousePos);
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
