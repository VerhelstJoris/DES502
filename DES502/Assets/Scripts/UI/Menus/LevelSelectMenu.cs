using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour
{
    [System.Serializable]
    public class LevelSettings
    {
        public Sprite LevelImage;
        public string LevelName;
    }

    [SerializeField]
    private LoadOnClick _levelLoader;

    [SerializeField]
    private LevelSettings[] _levels;

    private int _currentLevelSelected = 0;

    private Image _image; 

    void Awake()
    {
        _image = this.GetComponent<Image>();
        _image.sprite = _levels[_currentLevelSelected].LevelImage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectNextLevel()
    {
        _currentLevelSelected = (_currentLevelSelected + 1) % _levels.Length;
        _image.sprite = _levels[_currentLevelSelected].LevelImage;

    }

    public void SelectPreviousLevel()
    {
        if (_currentLevelSelected < 0)
        {
            _currentLevelSelected = _levels.Length - 1;
        }
        _image.sprite = _levels[_currentLevelSelected].LevelImage;
    }

    public void StartGame()
    {
        _levelLoader.LoadScene(_levels[_currentLevelSelected].LevelName);
    }

}
