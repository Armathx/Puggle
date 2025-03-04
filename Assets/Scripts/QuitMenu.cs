using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitMenu : MonoBehaviour
{
  
    public void QuitGame(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}

