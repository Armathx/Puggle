using TMPro;
using UnityEngine;

public class GetScore : MonoBehaviour
{

    public TextMeshProUGUI textMeshProUGUI;
    public int TotalScore = 0;

    public void Update()
    {

        textMeshProUGUI.text = "Balls remaining : " + TotalScore.ToString();

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.collider.CompareTag("Blue")))
        {
            TotalScore = TotalScore + 100;
        }
        else if ((collision.collider.CompareTag("Green")))
        {

        }
        else if ((collision.collider.CompareTag("Orange")))
        {

        }

    }
}
