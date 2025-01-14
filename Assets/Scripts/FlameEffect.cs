using UnityEngine;

public class FlameEffect : MonoBehaviour
{
    [Header("Flame Effect")]
    public GameObject flameParticlePrefab; // Le prefab des particules de flamme
    public float flameWidth = 1.0f;        // Espace entre chaque particule
    public int flameCount = 20;            // Nombre de particules de flamme (ajustez selon la taille de l'écran)

    void Start()
    {
        // Obtenir les limites de l'écran
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Aucune caméra principale trouvée !");
            return;
        }

        // Calculer la largeur de l'écran
        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;

        // Limites de la largeur de l'écran
        float minX = -screenWidth / 2f;
        float maxX = screenWidth / 2f;

        // Positionner les particules le long de la limite inférieure de l'écran
        for (int i = 0; i < flameCount; i++)
        {
            // Calculer la position de chaque particule sur l'axe X
            float xPos = Mathf.Lerp(minX, maxX, (float)i / (flameCount - 1));

            // Position des particules sur la limite inférieure (y = -screenHeight / 2f)
            Vector3 spawnPosition = new Vector3(xPos, -screenHeight / 2f, 0);

            // Instancier la particule
            Instantiate(flameParticlePrefab, spawnPosition, Quaternion.identity);
        }
    }
}
