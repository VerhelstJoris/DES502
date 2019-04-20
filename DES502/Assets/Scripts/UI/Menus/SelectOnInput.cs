using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectOnInput : MonoBehaviour
{
    [SerializeField]
    private Button[] _buttons;
    int _currentButtonSelected=0;
    bool _nextSelected = false;
    static bool _controllerDecided = false;

    public static ControllerID _CurrentController = ControllerID.Controller3;

    [SerializeField]
    bool _resetAtStart = false;

    [SerializeField]
    bool _active = false;

    // Start is called before the first frame update
    void Start()
    {
        //_buttons[0].Select();
        if(_resetAtStart)
        {
            ResetToFirst();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_active)
        {
            float inputVertical_P3 = Input.GetAxis("Vertical_P3");
            float inputVertical_P4 = Input.GetAxis("Vertical_P4");
            bool inputSubmit_P3 = Input.GetButtonDown("Submit_P3");
            bool inputSubmit_P4 = Input.GetButtonDown("Submit_P4");

            if (inputVertical_P3 != 0 && !_controllerDecided)
            {
                _CurrentController = ControllerID.Controller3;
                _controllerDecided = true;
            }
            else if ((inputVertical_P4 != 0) && !_controllerDecided)
            {
                _CurrentController = ControllerID.Controller4;
                _controllerDecided = true;
            }

            if ((inputVertical_P3 >= 0.5 || inputVertical_P4 >= 0.5) && !_nextSelected)
            {
                _nextSelected = true;

                _currentButtonSelected = (_currentButtonSelected + 1) % (_buttons.Length);
                _buttons[_currentButtonSelected].Select();
            }
            else if ((inputVertical_P3 <= -0.5 || inputVertical_P4 <= -0.5) && !_nextSelected)
            {
                _nextSelected = true;

                _currentButtonSelected = (_currentButtonSelected - 1) % (_buttons.Length);
                if (_currentButtonSelected < 0)
                {
                    _currentButtonSelected = _buttons.Length - 1;
                }
                _buttons[_currentButtonSelected].Select();
            }

            if (Input.GetAxisRaw("Vertical_P3") == 0 && _CurrentController == ControllerID.Controller3)
            {
                _nextSelected = false;
            }
            else if (Input.GetAxisRaw("Vertical_P4") == 0 && _CurrentController == ControllerID.Controller4)
            {
                _nextSelected = false;
            }

            //button
            if ((inputSubmit_P3 && _CurrentController == ControllerID.Controller3)
                || (inputSubmit_P4 && _CurrentController == ControllerID.Controller4))
            {
                _buttons[_currentButtonSelected].onClick.Invoke();
            }
        }
    }

    public void ResetToFirst()
    {
        _currentButtonSelected = 0;
        _buttons[0].Select();
    }

    public void SetActive(bool active)
    {
        _active = active;
    }
}
