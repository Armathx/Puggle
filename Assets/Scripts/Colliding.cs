using UnityEngine;

public class Colliding : MonoBehaviour
{
    public Collider2D Collider;         // Référence au collider de l'objet (optionnel si déjà attaché au GameObject)
    public GameObject explosionEffect;  // Référence à l'effet de particules de l'explosion
    /* public AudioClip explosionSound; */   // Référence au son d'explosion (facultatif)

    private static Transform ExplodeParticleContainer; // Conteneur global pour les particules

    private void Awake()
    {
        // Vérifier si le conteneur existe déjà
        if (ExplodeParticleContainer == null)
        {
            // Créer un conteneur global pour organiser les particules d'explosion
            ExplodeParticleContainer = new GameObject("ExplodeParticles").transform;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si un effet de particules est assigné, instancier l'explosion
        if (explosionEffect != null)
        {
            // Créer l'effet de particules à la position de l'objet
            GameObject ExplodeP = Instantiate(explosionEffect, transform.position, Quaternion.identity);

            // Ajouter l'effet au conteneur
            ExplodeP.transform.SetParent(ExplodeParticleContainer);
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
