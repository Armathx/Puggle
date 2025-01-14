using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class ScreenCollider : MonoBehaviour
{
    private EdgeCollider2D edgeCollider;
    private LineRenderer lineRenderer;

    void Awake()
    {
        // R�cup�rer ou ajouter le EdgeCollider2D
        edgeCollider = GetComponent<EdgeCollider2D>();

        if (edgeCollider == null)
        {
            edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        }

        // Ajouter un LineRenderer pour l'affichage visuel
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 5;  // 4 points pour les coins, 1 pour fermer la boucle
        lineRenderer.widthMultiplier = 0.1f;  // �paisseur de la ligne
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;  // Couleur de la ligne
        lineRenderer.endColor = Color.red;    // Couleur de la ligne
        lineRenderer.useWorldSpace = true;    // Utiliser l'espace monde pour la ligne

        CreateEdgeCollider();
        UpdateLineRenderer();
    }

    void Update()
    {
        // Optionnel : v�rifier les changements de r�solution � chaque frame (si n�cessaire)
        if (ScreenResolutionChanged())
        {
            CreateEdgeCollider();
            UpdateLineRenderer();
        }
    }

    // Cr�e ou met � jour le EdgeCollider2D pour correspondre aux bords de l'�cran
    void CreateEdgeCollider()
    {
        List<Vector2> edges = new List<Vector2>
        {
            Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)), // Bas-gauche
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, Camera.main.nearClipPlane)), // Bas-droit
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane)), // Haut-droit
            Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane)), // Haut-gauche
            Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)) // Retour au bas-gauche
        };

        // D�finir les points pour l'EdgeCollider2D
        edgeCollider.SetPoints(edges);
    }

    // Met � jour le LineRenderer pour dessiner les bords de l'�cran
    void UpdateLineRenderer()
    {
        List<Vector3> positions = new List<Vector3>
        {
            Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)), // Bas-gauche
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, Camera.main.nearClipPlane)), // Bas-droit
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane)), // Haut-droit
            Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane)), // Haut-gauche
            Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)) // Retour au bas-gauche
        };

        // Appliquer les positions au LineRenderer
        lineRenderer.SetPositions(positions.ToArray());
    }

    // D�tecte si la r�solution a chang�
    private Vector2 lastScreenSize;
    private bool ScreenResolutionChanged()
    {
        if (lastScreenSize != new Vector2(Screen.width, Screen.height))
        {
            lastScreenSize = new Vector2(Screen.width, Screen.height);
            return true;
        }
        return false;
    }
}
