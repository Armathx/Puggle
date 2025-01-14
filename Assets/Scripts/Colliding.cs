using UnityEngine;

public class Colliding : MonoBehaviour
{
    public Collider2D Collider;         // Référence au collider de l'objet (optionnel si déjà attaché au GameObject)
    public GameObject explosionEffect;  // Référence à l'effet de particules de l'explosion
   /* public AudioClip explosionSound; */   // Référence au son d'explosion (facultatif)

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si un effet de particules est assigné, instancier l'explosion
        if (explosionEffect != null)
        {
            // Créer l'effet de particules à la position de l'objet
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Si un son d'explosion est assigné, jouer le son
        //if (explosionSound != null)
        //{
        //    AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        //}

        // Détruire l'objet (l'objet explose)
        Destroy(gameObject);
    }
}
