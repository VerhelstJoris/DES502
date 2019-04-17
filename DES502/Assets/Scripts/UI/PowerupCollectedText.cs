using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupCollectedText : MonoBehaviour
{
    // I don't think we even need to cache this
    //private Text _text;
    private RectTransform _rectTransform;

    void Awake()
    {
        //_text = GetComponent<Text>();
        _rectTransform = GetComponent<RectTransform>();
        // test
        //SetWorldPosition(new Vector3(0, 0, 0));
    }

    // functions as a custom constructor??
    public void Activate(string powerupName, Vector3 powerupPosition)
    {
        // set initial position
        SetWorldPosition(powerupPosition);
        SetText(powerupName);
        //StartCoroutine(FloatUpwards());
        // TODO: this is created multiple times!!!!!! should only trigger once per pickup
        Debug.Log("POWERUP TEXT EXISTS");
    }

    private void SetText(string newText)
    {
        Text textComponent = GetComponent<Text>();
        textComponent.text = newText;
    }

    private void SetWorldPosition(Vector3 worldPosition)
    {
        //_rectTransform.anchoredPosition = new Vector2(0, 0);
    }

    /*
    private IEnumerator FloatUpwards()
    {
    }
    */
}
