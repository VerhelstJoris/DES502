using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{

    private float _timeLeft;

    private GameManager _manager;
    private Text _text;

    private void Awake()
    {
        _text = GetComponentInChildren<Text>();
    }

    private void Update()
    {
        _text.text = _manager._GameTimerLeft.ToString("F0");

    }

    public void Initialize(GameManager manager, float amountOfTime)
    {
        _manager = manager;
        //this.transform.position = new Vector3(0, -240, 0);
        _timeLeft = amountOfTime;
        _text.text = _timeLeft.ToString("F0");
    }
}
