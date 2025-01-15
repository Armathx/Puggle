using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Victory : MonoBehaviour
{
    public string tagToCheck = "Green"; // Tag des objets verts
    public GameObject victoryScreen;   // Écran ou message de victoire
    public float slowMotionFactor = 0.2f; // Facteur de ralentissement
    public GameObject mainBall;  // Référence à la balle principale
    public Camera mainCamera;  // Référence à la caméra
    public float maxDistance = 5f;  // Distance maximale entre la dernière boule verte et la balle principale
    public float cameraZoomFactor = 3f;  // Valeur de la taille orthographique lors du zoom (plus petit = plus zoomé)
    public float cameraZoomSpeed = 1f;  // Vitesse du zoom de la caméra

    private float originalSize;  // Taille orthographique initiale de la caméra

    public GameObject explosionEffect;  // Référence au système de particules d'explosion

    public LevelGeneration levelGeneration;

    public PuggleAgent puggleAgent;


    bool bYouWin = false;
    void Start()
    {
        // Assurez-vous que le victoryScreen est désactivé au départ
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(false);
        }

        // Sauvegarde de la taille orthographique initiale de la caméra
        if (mainCamera != null)
        {
            originalSize = mainCamera.orthographicSize;
            Debug.Log("Original Camera Size: " + originalSize);  // Vérification de la taille initiale
        }
    }

    void Update()
    {
        if (!bYouWin)
        {
            CheckVictoryCondition();
        }


    }

    public void UpdateMainBall(GameObject newMainBall)
    {
        mainBall = newMainBall;
    }

    void CheckVictoryCondition()
    {
        // Trouver tous les objets avec le tag spécifié(Version LINQ
        GameObject[] objectsWithTag = levelGeneration.marbles.Where(x => x != null).Where(x =>x.CompareTag(tagToCheck)).ToArray();


        // Si un seul objet reste et que la distance à la balle principale est inférieure à la valeur donnée
        if (objectsWithTag.Length == 1)
        {
            // Trouver la dernière boule verte
            GameObject lastGreenBall = objectsWithTag[0]; // Hypothèse que c'est le dernier objet dans le tableau

            // Calculer la distance entre la balle principale et la dernière boule verte
            float distance = Vector3.Distance(mainBall.transform.position, lastGreenBall.transform.position);

            // Si la distance est inférieure à la distance maximale
            if (distance < maxDistance)
            {
                //    Time.timeScale = slowMotionFactor; // Appliquer le ralentissement


                // Zoomer la caméra (réduire la taille orthographique)
                if (mainCamera != null)
                {
                    // Log pour vérifier si le zoom est bien appliqué
                    Debug.Log("Zooming In... Current Size: " + mainCamera.orthographicSize);

                    // Interpolation de la taille orthographique pour effectuer un zoom progressif
                    //mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, cameraZoomFactor, cameraZoomSpeed * Time.unscaledDeltaTime);
                    //mainCamera.transform.position = mainBall.transform.position;
                }
            }
            else
            {
             /*   Time.timeScale = 1.25f;*/ // Réinitialiser la vitesse normale

                // Dé-zoomer la caméra (retour à la taille orthographique originale)
                if (mainCamera != null)
                {
                    // Log pour vérifier si le dézoom est bien appliqué
                    Debug.Log("Zooming Out... Current Size: " + mainCamera.orthographicSize);

                    // Interpolation de la taille orthographique pour effectuer un dézoom progressif
                    mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, originalSize, cameraZoomSpeed * Time.unscaledDeltaTime);

                }
            }
        }
        else if (objectsWithTag.Length == 0)
        {
            OnVictory();
        }
        else
        {
            /* Time.timeScale = 1.25f; */// Réinitialiser la vitesse normale si plus d'un objet reste
        }
    }

    void OnVictory()
    {
        Debug.Log("Victoire !");
        bYouWin = true;
        // Activer l'écran de victoire s'il existe
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true);
        }

        puggleAgent.AddReward(400.0f);

        puggleAgent.EndEpisode();

        levelGeneration.Init();




        //// Lancer l'explosion (si elle existe)
        //if (explosionEffect != null)
        //{
        //    Instantiate(explosionEffect, mainBall.transform.position, Quaternion.identity);

        //    Destroy(explosionEffect);
        //}

        // Vous pouvez arrêter le jeu ou exécuter d'autres actions ici
        //Time.timeScale = 1.0f; // Diminuer le timescale après la victoire

        //mainCamera.orthographicSize = Mathf.Lerp(originalSize, mainCamera.orthographicSize, cameraZoomSpeed * Time.unscaledDeltaTime);
    }

}
