using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuVR : MonoBehaviour
{
    [Header("Scene to load")]
    [SerializeField] private string sceneToLoad = "BasicScene";

    public void Play()
    {
        // Seguridad: si la escena no está en Build Settings, no cargará
        SceneManager.LoadScene(sceneToLoad);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
