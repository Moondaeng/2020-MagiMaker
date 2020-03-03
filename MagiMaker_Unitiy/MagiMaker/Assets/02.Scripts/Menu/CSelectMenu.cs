using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CSelectMenu : MonoBehaviour
{
    #region menuButton
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject loadGameButton;
    [SerializeField] private GameObject optionButton;
    [SerializeField] private GameObject quitButton;
    #endregion

    #region Canvas
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject singleOrMulti;
    [SerializeField] private GameObject option;
    #endregion

    public void OnClickMenu(string button)
    {
        switch(button)
        {
            case "newGame":
                mainMenu.SetActive(false);
                singleOrMulti.SetActive(true);
                break;

            case "loadGame":
                //임시로 해둠 구현 필요
                CGlobal.isPlay = true;
                SceneManager.LoadScene("Play");
                break;

            case "option":
                mainMenu.SetActive(false);
                option.SetActive(true);
                break;

            case "quit":
#if UNITY_EDITOR  //에디터 디버깅 용
                UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
                break;

            case "back":
                if (CGlobal.isPlay == true)
                    SceneManager.LoadScene("Play");
                else
                {
                    option.SetActive(false);
                    SceneManager.LoadScene("Menu");
                }
                break;

            case "single":
                CGlobal.isPlay = true;
                SceneManager.LoadScene("Play");
                break;

            case "multi":
                CGlobal.isPlay = true;
                SceneManager.LoadScene("Play");
                break;
        }
    }

    private void Start()
    {
        option.SetActive(false);
        singleOrMulti.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))  //esc 입력 시 옵션 메뉴 열기
        {
            OnClickMenu("back");
        }
    }
}
