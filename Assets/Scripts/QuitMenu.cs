using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitMenu : MonoBehaviour
{
    public GameObject levelSelectionPanel; // Référence au panel de sélection de niveaux

  
    public void QuitGame(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}

