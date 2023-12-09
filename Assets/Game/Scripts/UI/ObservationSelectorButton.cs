using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
[RequireComponent(typeof(Button))]
public class ObservationSelectorButton : MonoBehaviour
{
    public bool ActiveColorState{
        set{ UpdateButtonColor(value); }
    }   
    public string Text{
        get => _text.text;
        set { _text.text = value; }
    }
    public int ID{ get; set; }
    [SerializeField] private TextMeshProUGUI _text;
    private Button _currentButton;

    private void Awake(){
        _currentButton = GetComponent<Button>();
    }
    public void AddOnClickListener(Action<int> action){
        _currentButton.onClick.AddListener(() => action.Invoke(ID));
    }
    private void UpdateButtonColor(bool state){
        ColorBlock colorBlock = _currentButton.colors;

        Color targetColor = state ? colorBlock.normalColor : colorBlock.disabledColor;

        Graphic graphic = _currentButton.targetGraphic ?? GetComponentInChildren<Graphic>();
        
        if (graphic != null) graphic.color = targetColor;
    }
}
