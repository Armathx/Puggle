using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject levelSelectionPanel; // Référence au panel de sélection de niveaux

    void Start()
    {
        levelSelectionPanel.SetActive(true); // Cache le menu des niveaux au départ
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
        Debug.Log("Le jeu est quitté"); // Utile pour voir si ça marche dans l'éditeur
    }
}
