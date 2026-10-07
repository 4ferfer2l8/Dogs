using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuInicial : MonoBehaviour
{
    [SerializeField] private string cenaDaCorrida = "Corrida";
    [SerializeField] private GameObject painelInicial;
    [SerializeField] private GameObject painelMenu;
    public void AbrirMenu()
    {
        painelInicial.SetActive(false);
        painelMenu.SetActive(true);
    }
    public void Jogar()
    {
        StartCoroutine(CarregarCorrida());
    }

    public void Sair()
    {
        Application.Quit();
    }

    private IEnumerator CarregarCorrida()
    {

        AsyncOperation operacao = SceneManager.LoadSceneAsync(cenaDaCorrida); //Carregamento assíncrono da cena da corrida

        while (!operacao.isDone)
        {
            yield return null;
        }
    }
}