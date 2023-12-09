using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private Vector2 _deltaMultiplyer;

    [Space(10)]
    [Header("Inputs")]
    [SerializeField] private InputActionReference _currentActionMap;

    [Space(10)]
    [Header("Events")]
    [SerializeField] private SO_Vector2Event _onMovementDeltaChange; 

    private void MovementDeltaPerformed(InputAction.CallbackContext context){
        _onMovementDeltaChange.Invoke(context.ReadValue<Vector2>() * _deltaMultiplyer);
    }
    
    private void MovementDeltaCanceled(InputAction.CallbackContext context){
        _onMovementDeltaChange.Invoke(Vector2.zero);
    }
    
    private void OnEnable(){
        _currentActionMap.action.Enable();
        _currentActionMap.action.performed += MovementDeltaPerformed;
        _currentActionMap.action.canceled += MovementDeltaCanceled;
    }

    private void OnDisable(){
        _currentActionMap.action.performed += MovementDeltaPerformed;
        _currentActionMap.action.canceled += MovementDeltaCanceled;
        _currentActionMap.action.Disable();
    }
}