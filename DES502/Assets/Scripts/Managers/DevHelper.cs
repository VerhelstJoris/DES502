using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // scene switching
        if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SceneManager.LoadScene("Scenes/BasicScene");
            }
        if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SceneManager.LoadScene("Scenes/LvlTest");
            }
    }
}
