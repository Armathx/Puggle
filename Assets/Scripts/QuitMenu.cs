using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitMenu : MonoBehaviour
{
    public GameObject levelSelectionPanel; // R�f�rence au panel de s�lection de niveaux

  
    public void QuitGame(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}

