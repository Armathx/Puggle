using UnityEngine;

public class BoundaryManager : MonoBehaviour
{
    // Ce script gère la destruction de la MainBall lorsqu'elle touche la limite inférieure
    public LevelGeneration levelGeneration;  // Référence au script LevelGeneration

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Vérifier si l'objet qui entre en collision est la MainBall
        if (other.CompareTag("MainBall"))
        {
            // Détruire la balle
            Destroy(other.gameObject);
            Debug.Log("MainBall Destroyed");

            // Appeler la fonction de respawn
            levelGeneration.RespawnMainBallAtCurrentPoint();


        }
    }
}
