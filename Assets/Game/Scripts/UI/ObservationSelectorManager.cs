using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class ObservationSelectorManager : MonoBehaviour
{
    [SerializeField] ObservationSelectorButton _buttonPrefab;
    [SerializeField] SO_ObservationPointEvent _onObservationPointChange;

    [SerializeField] ObservationPoint _defaultPoint; 
    [SerializeField] ObservationPoint[] _observationPoints;
    ObservationSelectorButton[] _buttons;

    int _activePoint;
    void Start(){
        _buttons = new ObservationSelectorButton[_observationPoints.Length];
        for (int i = 0; i < _observationPoints.Length; i++){
            _buttons[i] = Instantiate(_buttonPrefab,transform);
            _buttons[i].name = $"Здание {i+1}";
            _buttons[i].ID = i;
            _buttons[i].AddOnClickListener(ChangeActivePoint);
        }
        ChangeActivePoint(-1);
    }

    void ChangeActiveColors(int id, bool isDefaultActive){
        for (int i = 0; i < _buttons.Length; i++){
            _buttons[i].ActiveColorState = i == id ? !isDefaultActive : isDefaultActive;
        }
    }
    void ChangeActivePoint(int id){
        if(id == _activePoint){
            _onObservationPointChange.Event.Invoke(_defaultPoint);
            _activePoint = -1;
            ChangeActiveColors(-1, true);
            return;
        }
        else if(id >= 0 && id < _observationPoints.Length){
            _onObservationPointChange.Event.Invoke(_observationPoints[id]);
            _activePoint = id;
            ChangeActiveColors(id, false);

            return;
        }
        _onObservationPointChange.Event.Invoke(_defaultPoint);
        _activePoint = -1;
        ChangeActiveColors(-1, true);
        
    }
}
