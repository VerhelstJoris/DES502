using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{

    private float _timeLeft;

    private GameManager _manager;
    [SerializeField]
    private Text _text;

    private void Awake()
    {
    }

    private void Update()
    {
        if (_manager != null)
        {
            _text.text = _manager._GameTimerLeft.ToString("F0");
        }
        else
        {
            _manager = FindObjectOfType<GameManager>();
        }
    }

    public void Initialize(GameManager manager, float amountOfTime)
    {
        _manager = manager;
        _timeLeft = amountOfTime;
        _text.text = _timeLeft.ToString("F0");
    }
}
