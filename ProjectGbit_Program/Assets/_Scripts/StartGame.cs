using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class StartGame : MonoBehaviour
{
    public Button btn;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            OnButtonClick();
        }
    }
    
    void OnButtonClick()
    {
        SceneLoader.Instance.LoadScene("Publish_Scene1");

        //UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Publish/Publish_Scene1");
        // 在这里添加你希望按钮按下时执行的逻辑
        Debug.Log("startload");
    }
}
