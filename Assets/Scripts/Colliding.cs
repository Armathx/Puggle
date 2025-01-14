using UnityEngine;

public class Colliding : MonoBehaviour
{
    public GameObject explosionEffect;  // R�f�rence au prefab de l'explosion
    private static Transform ExplodeParticleContainer; // Conteneur global pour les particules

    private void Awake()
    {
        // Cr�er un conteneur global si n�cessaire
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

            // D�truire l'instance apr�s une courte dur�e
            Destroy(explosionInstance, 1f); // 1 seconde de d�lai avant destruction
        }

        // D�truire l'objet courant (celui qui d�clenche la collision)
        Destroy(gameObject);
    }
}
