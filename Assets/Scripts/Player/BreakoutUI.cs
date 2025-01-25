using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreakoutUI : MonoBehaviour
{
    [SerializeField] private Animator keyboardControls;
    [SerializeField] private Animator controllerControls;
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
        keyboardControls.gameObject.SetActive(!usingGamepad);
        controllerControls.gameObject.SetActive(usingGamepad);
    }
    public void SetNextInput(int _target)
    {
        keyboardControls.SetInteger("Target", _target);
        controllerControls.SetInteger("Target", _target);
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
