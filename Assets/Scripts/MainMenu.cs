using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject levelSelectionPanel; // R�f�rence au panel de s�lection de niveaux

    void Start()
    {
        levelSelectionPanel.SetActive(true); // Cache le menu des niveaux au d�part
    }

    public void OpenLevelSelection()
    {
        levelSelectionPanel.SetActive(true);  // Affiche les niveaux
    }

    public void CloseLevelSelection()
    {
        levelSelectionPanel.SetActive(false); // Cache les niveaux
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Le jeu est quitt�"); // Utile pour voir si �a marche dans l'�diteur
    }
}
