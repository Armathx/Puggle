using UnityEngine;
using UnityEngine.EventSystems;

public class ShootButtonMobile : MonoBehaviour, IPointerUpHandler
{
    public LevelGeneration ballShooter; // Ton script principal (� renommer si besoin)

    public void OnPointerUp(PointerEventData eventData)
    {
        if (ballShooter != null)
        {
            ballShooter.TryShoot(); // Tir imm�diat d�s qu�on rel�che le bouton
        }
    }
}
