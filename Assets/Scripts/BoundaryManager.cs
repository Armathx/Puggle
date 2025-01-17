using UnityEngine;

public class BoundaryManager : MonoBehaviour
{
    // Ce script g�re la destruction de la MainBall lorsqu'elle touche la limite inf�rieure
    public LevelGeneration levelGeneration;  // R�f�rence au script LevelGeneration

    private void OnTriggerEnter2D(Collider2D other)
    {
        // V�rifier si l'objet qui entre en collision est la MainBall
        if (other.CompareTag("MainBall"))
        {
            // D�truire la balle
            Destroy(other.gameObject);
            Debug.Log("MainBall Destroyed");

            // Appeler la fonction de respawn
            levelGeneration.RespawnMainBallAtCurrentPoint();


        }
    }
}
