using UnityEngine;
public class Colliding : MonoBehaviour
{
    public Collider2D Collider;
    public GameObject go;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

}
