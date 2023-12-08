using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private SO_Vector2Event _onMovementDeltaChange; 
    [SerializeField] private InputActionReference _currentActionMap;

    private void MovementDeltaPerformed(InputAction.CallbackContext context){
        _onMovementDeltaChange.Invoke(context.ReadValue<Vector2>());
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