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
    private Vector3 finalScale = new Vector3(0.01f, 0.01f, 0.01f);

    //rotationSpeed
    private float rotationSpeed = 150f;



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
            // Appliquer la rotation chaque frame autour de l'axe Y
            transform.Rotate(0,0, rotationSpeed * Time.deltaTime);
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
            transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);



        }

        triangle.transform.localScale = finalScale; ///assurer lechelle a 10

    }

}
