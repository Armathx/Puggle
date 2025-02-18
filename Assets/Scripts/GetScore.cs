using UnityEngine;

public class GetScore : MonoBehaviour
{
    // D�finir un d�l�gu� pour les changements de score
    public delegate void ScoreChanged(int newScore);
    public static event ScoreChanged OnScoreChanged;

    public static int totalScore;

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.CompareTag("Blue"))
        {
            //Debug.Log("BLUE");

            totalScore += 1000; // Ajoute des points
        }
        else if (other.CompareTag("Green"))
        {
            totalScore += 2222; // Ajoute plus de points
        }
        else if (other.CompareTag("Orange"))
        {
            totalScore -= 525; // Retire des points


        }

        // D�clenche l'�v�nement via le d�l�gu�
        OnScoreChanged?.Invoke(totalScore);
    }
}
