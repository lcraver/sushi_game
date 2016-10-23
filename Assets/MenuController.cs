using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    public Vector3 mainPos;
    public Vector3 aboutPos;

    public GameObject startButton;
    public GameObject aboutButton;
    public GameObject backButton;

    public bool isMoving = false; 

    public void Start()
    {
        EventSystem.current.SetSelectedGameObject(startButton);
        PlayerController.inst.CreateExampleTentacles();
    }

    public void Play()
    {
        SceneManager.LoadScene("test");
    }

    public void About()
    {
        StartCoroutine(AboutLoop());
    }

    public IEnumerator AboutLoop()
    {
        isMoving = true;
        EventSystem.current.sendNavigationEvents = false;
        this.transform.DOMove(aboutPos + new Vector3(0, 0, this.transform.position.z), 0.25f);
        yield return new WaitForSeconds(0.2f);
        isMoving = false;
        EventSystem.current.sendNavigationEvents = true;
        EventSystem.current.SetSelectedGameObject(backButton);
    }

    public void ReturnAbout()
    {
        StartCoroutine(ReturnAboutLoop());
    }

    public IEnumerator ReturnAboutLoop()
    {
        isMoving = true;
        EventSystem.current.sendNavigationEvents = false;
        this.transform.DOMove(mainPos + new Vector3(0,0,this.transform.position.z), 0.25f);
        yield return new WaitForSeconds(0.2f);
        isMoving = false;
        EventSystem.current.sendNavigationEvents = true;
        EventSystem.current.SetSelectedGameObject(aboutButton);
    }
}
