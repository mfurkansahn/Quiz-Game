using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu: MonoBehaviour
{
    // Endless Marble sahnesine ge�
    public void LoadEndlessMarble()
    {
        SceneManager.LoadScene(1); // Endless Marble sahnesi
    }

    // Breakout sahnesine ge�
    public void LoadBreakout()
    {
        SceneManager.LoadScene(2); // Breakout sahnesi
    }

    // Quiz Game sahnesine ge�
    public void LoadQuizGame()
    {
        SceneManager.LoadScene(3); // Quiz Game sahnesi
    }

    // Uygulamay� kapat
    public void ExitGame()
    {
        // Unity Edit�rde ise Play Mode'u durdur
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // Uygulamay� kapat
#endif
    }
}
