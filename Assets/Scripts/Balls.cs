
using UnityEngine;



public class Balls : MonoBehaviour
{

    public GameObject BallsPrefabs;
    private Vector2 mouseOffset;
    private bool isDragging = false;
    private BoxCollider2D boxCollider;


    // Start is called before the first frame update
    void Start()
    {
        // Taille de l'écran en unités du monde.
        float screenWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        float screenHeight = Camera.main.orthographicSize * 2;

        // Ajouter un Rigidbody2D pour bloquer la physique pendant le drag
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        if (isDragging)
        {
            // Suivre la souris ou le toucher
            Vector2 newPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) + mouseOffset;
            transform.position = newPosition;
            
            
        }
    }

    void OnMouseDown()
    {
        if (!isDragging)
        {
            // Sauvegarde l'offset pour le drag
            mouseOffset = (Vector2)transform.position - (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            transform.position = transform.position;
            isDragging = false;

        }
    }

    void OnSpaceDown()
    {

    }


}