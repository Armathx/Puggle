using UnityEngine;

public class VictoryScreenScript : MonoBehaviour
{

    //Référence à l'objet Triangle
    public GameObject triangle;

    //Temps d'animation
    public float scaleUpTIme = 2f;
    public float scaleDownTIme = 1f;

    //facteur de mise a lechelle
    private Vector3 targetScale = new Vector3(20f, 20f, 20f);
    private Vector3 finalScale = new Vector3(10f, 10f, 10f);



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        StartCoroutine(AnimateTriangle());


    }


    //Coroutine 
    private System.Collections.IEnumerator AnimateTriangle()
    {

        triangle.transform.localScale = Vector3.zero;

        //animation
        float elapsedTime = 0f;
        while (elapsedTime <  scaleUpTIme)
        {

            triangle.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsedTime /scaleUpTIme);
            elapsedTime += Time.deltaTime;

            yield return null;


        }

        triangle.transform.localScale = targetScale;
        elapsedTime = 0f;
        while (elapsedTime < scaleDownTIme)
        {

            triangle.transform.localScale = Vector3.Lerp(targetScale, finalScale, elapsedTime / scaleDownTIme);
            elapsedTime += Time.deltaTime;

            yield return null;


        }

        triangle.transform.localScale = finalScale; ///assurer lechelle a 10

    }

}
