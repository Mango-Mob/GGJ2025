using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreakoutUI : MonoBehaviour
{
    [SerializeField] private GameObject keyboardControls;
    [SerializeField] private GameObject controllerControls;
    [SerializeField] private Image healthFill;

    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        bool usingGamepad = InputManager.Instance.isInGamepadMode;
        keyboardControls.SetActive(!usingGamepad);
        controllerControls.SetActive(usingGamepad);
    }
    public void SetVisible(bool visible)
    {
        canvasGroup.alpha = visible ? 1.0f : 0.0f;
    }
    public void SetHealth(float _value)
    {
        healthFill.fillAmount = _value;
    }
}
