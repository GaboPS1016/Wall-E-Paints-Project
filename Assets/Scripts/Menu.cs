using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    public GameObject Rules;
    public GameObject GroupOfText;
    public GameObject SBar;
    public AudioSource BackSound;
    public GameObject MuteButton;
    public SpriteRenderer Muted;
    public SpriteRenderer NoMuted;
    public bool IsMuted;
    public void Start()
    {
        Rules.SetActive(false);
        IsMuted = false;
        MuteButton.GetComponent<Image>().sprite = NoMuted.sprite;
        GroupOfText.transform.GetChild(0).gameObject.SetActive(true);
        for (int i = 1; i < GroupOfText.transform.childCount; i++)
        {
            GroupOfText.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void MenuInicial()
    {
        SceneManager.LoadScene(0);
    }
    public void Comenzar()
    {
        SceneManager.LoadScene(1);
    }
    public void Informacion()
    {
        SceneManager.LoadScene(2);
    }
    public void Salir()
    {
        Application.Quit();
    }
    public void OpenRules()
    {
        Rules.SetActive(true);
    }
    public void CloseRules()
    {
        Rules.SetActive(false);
    }
    public void ChangeText(int index)
    {
        for (int i = 0; i < GroupOfText.transform.childCount; i++)
        {
            if (index == i)
            {
                GroupOfText.transform.GetChild(i).gameObject.SetActive(true);
                continue;
            }
            GroupOfText.transform.GetChild(i).gameObject.SetActive(false);
        }
        SBar.GetComponent<Scrollbar>().value = 1;
    }
    public void SoundButton()
    {
        if (IsMuted)
        {
            IsMuted = false;
            BackSound.mute = false;
            MuteButton.GetComponent<Image>().sprite = NoMuted.sprite;
        }
        else
        {
            IsMuted = true;
            BackSound.mute = true;
            MuteButton.GetComponent<Image>().sprite = Muted.sprite;
        }
    }
}
