using UnityEngine;
using UnityEngine.EventSystems;

public class ShootButtonMobile : MonoBehaviour, IPointerUpHandler
{
    public LevelGeneration ballShooter; // Ton script principal (à renommer si besoin)

    public void OnPointerUp(PointerEventData eventData)
    {
        if (ballShooter != null)
        {
            ballShooter.TryShoot(); // Tir immédiat dès qu’on relâche le bouton
        }
    }
}
