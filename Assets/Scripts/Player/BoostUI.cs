using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostUI : MonoBehaviour
{
    [SerializeField] private Image[] boostOrbs;
    private Color boostOrbColor;
    public float maxValue { private set; get; }
    [SerializeField] private Color emptyColorMult;
    public float currentValue { 
        set
        {
            _currentValue = value;
            _currentValue = Mathf.Clamp(_currentValue, 0.0f, maxValue);

            for (int i = 0; i < boostOrbs.Length; i++)
            {
                boostOrbs[i].fillAmount = Mathf.Clamp(currentValue - i, 0.0f, 1.0f);
                if (boostOrbs[i].fillAmount >= 1.0f)
                {
                    boostOrbs[i].color = boostOrbColor;
                }
                else
                {
                    boostOrbs[i].color = boostOrbColor * emptyColorMult;
                }
            }
        }
        get
        {
            return _currentValue;
        }
    }
    private float _currentValue;
    // Start is called before the first frame update
    void Awake()
    {
        boostOrbColor = boostOrbs[0].color;
        maxValue = boostOrbs.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
