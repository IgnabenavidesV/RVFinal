using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuVR : MonoBehaviour
{
    public void IniciarJuego()
    {
        SceneManager.LoadScene("01_MainScene");
        // Pon el nombre real de tu escena del mapa
    }

    public void AbrirControles()
    {
        Debug.Log("Abrir menú de Controles");
        // Aquí luego abrimos otro panel
    }

    public void AbrirSonido()
    {
        Debug.Log("Abrir menú de Sonido");
        // Aquí luego abrimos sliders etc.
    }

    public void SalirJuego()
    {
        Application.Quit();
        Debug.Log("Salir del juego");
    }
}
