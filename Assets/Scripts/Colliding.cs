using UnityEngine;

public class Colliding : MonoBehaviour
{
    public Collider2D Collider;         // R�f�rence au collider de l'objet (optionnel si d�j� attach� au GameObject)
    public GameObject explosionEffect;  // R�f�rence � l'effet de particules de l'explosion
   /* public AudioClip explosionSound; */   // R�f�rence au son d'explosion (facultatif)

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si un effet de particules est assign�, instancier l'explosion
        if (explosionEffect != null)
        {
            // Cr�er l'effet de particules � la position de l'objet
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Si un son d'explosion est assign�, jouer le son
        //if (explosionSound != null)
        //{
        //    AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        //}

        // D�truire l'objet (l'objet explose)
        Destroy(gameObject);
    }
}
