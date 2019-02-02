using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MenuScript : MonoBehaviour
{

    public GameObject StartMenu;
    public GameObject QuitMenu;
    public GameObject TitleText;
    public Button StartText;
    public Button ExitText;

    // Use this for initialization
    void Start()
    {
        StartText = StartText.GetComponent<Button>();
        ExitText = ExitText.GetComponent<Button>();
        QuitMenu.SetActive(false);
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape) && !GameManager.Instance.IsPaused()){
            if(QuitMenu.activeSelf){
                NoPress();
            }else{
                ExitPress();
            }
        }
    }

    public void ExitPress()

    {
        QuitMenu.SetActive(true);
        TitleText.SetActive(false);
        StartText.gameObject.SetActive(false);
        ExitText.gameObject.SetActive(false);
    }

    public void NoPress()

    {
        QuitMenu.SetActive(false);
        TitleText.SetActive(true);
        StartText.gameObject.SetActive(true);
        ExitText.gameObject.SetActive(true);
    }

    public void StartLevel()
    {
        GameManager.Instance.DisplayVignette("Intro", WhenFinished);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void WhenFinished(){
        GameManager.Instance.ForceOpenLoadingHider();
        SceneManager.UnloadScene(0);
        GameManager.Instance.LoadLevel("Grand & Euclid");
    }

    public void ExitGame()
    {
        GameManager.Instance.QuitGame();
    }

}
