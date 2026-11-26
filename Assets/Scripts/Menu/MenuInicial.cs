using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    [SerializeField] private string cenaDaCorrida = "Corrida";
    [SerializeField] private string cenaDoTutorial = "Tutorial";
    [SerializeField] private GameObject painelInicial;
    [SerializeField] private GameObject painelMenu;

    public void AbrirMenu()
    {
        painelInicial.SetActive(false);
        painelMenu.SetActive(true);
    }

    public void Jogar()
    {
        StartCoroutine(CarregarCena(cenaDaCorrida));
    }

    public void Tutorial()
    {
        StartCoroutine(CarregarCena(cenaDoTutorial));
    }

    public void Sair()
    {
        Application.Quit();
    }

    private IEnumerator CarregarCena(string nome)
    {
        AsyncOperation operacao = SceneManager.LoadSceneAsync(nome);

        while (!operacao.isDone)
            yield return null;
    }
}