using UnityEngine;

public class Colliding : MonoBehaviour
{
    public GameObject explosionEffect;  // Référence au prefab de l'explosion
    private static Transform ExplodeParticleContainer; // Conteneur global pour les particules

    private void Awake()
    {
        // Créer un conteneur global si nécessaire
        if (ExplodeParticleContainer == null)
        {
            ExplodeParticleContainer = new GameObject("ExplodeParticles").transform;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (explosionEffect != null)
        {
            // Instancier l'effet d'explosion
            GameObject explosionInstance = Instantiate(explosionEffect, transform.position, Quaternion.identity);

            // Ajouter l'instance au conteneur global
            explosionInstance.transform.SetParent(ExplodeParticleContainer);

            // Détruire l'instance après une courte durée
            Destroy(explosionInstance, 1f); // 1 seconde de délai avant destruction
        }

        // Détruire l'objet courant (celui qui déclenche la collision)
        Destroy(gameObject);
    }
}
