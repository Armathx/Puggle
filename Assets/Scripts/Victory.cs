using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Victory : MonoBehaviour
{
    public string tagToCheck = "Green"; // Tag des objets verts
    public GameObject victoryScreen;   // �cran ou message de victoire
    public float slowMotionFactor = 0.2f; // Facteur de ralentissement
    public GameObject mainBall;  // R�f�rence � la balle principale
    public Camera mainCamera;  // R�f�rence � la cam�ra
    public float maxDistance = 5f;  // Distance maximale entre la derni�re boule verte et la balle principale
    public float cameraZoomFactor = 3f;  // Valeur de la taille orthographique lors du zoom (plus petit = plus zoom�)
    public float cameraZoomSpeed = 1f;  // Vitesse du zoom de la cam�ra

    private float originalSize;  // Taille orthographique initiale de la cam�ra

    public GameObject explosionEffect;  // R�f�rence au syst�me de particules d'explosion

    public LevelGeneration levelGeneration;

    public PuggleAgent puggleAgent;


    bool bYouWin = false;
    void Start()
    {
        // Assurez-vous que le victoryScreen est d�sactiv� au d�part
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(false);
        }

        // Sauvegarde de la taille orthographique initiale de la cam�ra
        if (mainCamera != null)
        {
            originalSize = mainCamera.orthographicSize;
            Debug.Log("Original Camera Size: " + originalSize);  // V�rification de la taille initiale
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
        // Trouver tous les objets avec le tag sp�cifi�(Version LINQ
        GameObject[] objectsWithTag = levelGeneration.marbles.Where(x => x != null).Where(x =>x.CompareTag(tagToCheck)).ToArray();


        // Si un seul objet reste et que la distance � la balle principale est inf�rieure � la valeur donn�e
        if (objectsWithTag.Length == 1)
        {
            // Trouver la derni�re boule verte
            GameObject lastGreenBall = objectsWithTag[0]; // Hypoth�se que c'est le dernier objet dans le tableau

            // Calculer la distance entre la balle principale et la derni�re boule verte
            float distance = Vector3.Distance(mainBall.transform.position, lastGreenBall.transform.position);

            // Si la distance est inf�rieure � la distance maximale
            if (distance < maxDistance)
            {
                //    Time.timeScale = slowMotionFactor; // Appliquer le ralentissement


                // Zoomer la cam�ra (r�duire la taille orthographique)
                if (mainCamera != null)
                {
                    // Log pour v�rifier si le zoom est bien appliqu�
                    Debug.Log("Zooming In... Current Size: " + mainCamera.orthographicSize);

                    // Interpolation de la taille orthographique pour effectuer un zoom progressif
                    //mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, cameraZoomFactor, cameraZoomSpeed * Time.unscaledDeltaTime);
                    //mainCamera.transform.position = mainBall.transform.position;
                }
            }
            else
            {
             /*   Time.timeScale = 1.25f;*/ // R�initialiser la vitesse normale

                // D�-zoomer la cam�ra (retour � la taille orthographique originale)
                if (mainCamera != null)
                {
                    // Log pour v�rifier si le d�zoom est bien appliqu�
                    Debug.Log("Zooming Out... Current Size: " + mainCamera.orthographicSize);

                    // Interpolation de la taille orthographique pour effectuer un d�zoom progressif
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
            /* Time.timeScale = 1.25f; */// R�initialiser la vitesse normale si plus d'un objet reste
        }
    }

    void OnVictory()
    {
        Debug.Log("Victoire !");
        bYouWin = true;
        // Activer l'�cran de victoire s'il existe
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

        // Vous pouvez arr�ter le jeu ou ex�cuter d'autres actions ici
        //Time.timeScale = 1.0f; // Diminuer le timescale apr�s la victoire

        //mainCamera.orthographicSize = Mathf.Lerp(originalSize, mainCamera.orthographicSize, cameraZoomSpeed * Time.unscaledDeltaTime);
    }

}
