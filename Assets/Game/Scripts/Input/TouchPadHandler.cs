using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchPadHandler : MonoBehaviour
{   
    [SerializeField] private Vector2 _deltaMultiplyer;
    
    [Space(10)]    
    [Header("Reference")]
    [SerializeField] private Camera _uiCamera;
    [SerializeField] private RectTransform _touchPad;
    
    [Space(10)]    
    [Header("Inputs")]
    [SerializeField] private InputActionReference _touchPosition;
    [SerializeField] private InputActionReference _touchDelta;
    [SerializeField] private InputActionReference _tapDetection;
    
    [Space(10)]
    [Header("Events")]
    [SerializeField] private SO_Vector2Event _onMovementDeltaChange; 

    private bool isInsideRect;
    private bool isTapDetected;

    private void MovementDeltaPerformed(InputAction.CallbackContext context){
        if(!isTapDetected) return;
        if(isInsideRect){
            _onMovementDeltaChange.Invoke(context.ReadValue<Vector2>() * _deltaMultiplyer);
        }
    }
    private void MovementDeltaCanceled(InputAction.CallbackContext context){
        _onMovementDeltaChange.Invoke(Vector2.zero);
    }
    private void TouchPositionPerformed(InputAction.CallbackContext context){
        isInsideRect = IsTouchInsideRectTransform(context.ReadValue<Vector2>(), _touchPad); 
    }
    private void TouchPositionCanceled(InputAction.CallbackContext context) => isInsideRect = false;

    private void TapPerformed(InputAction.CallbackContext context) => isTapDetected = true;
    private void TapCanceled(InputAction.CallbackContext context) => isTapDetected = false;

    private bool IsTouchInsideRectTransform(Vector2 touchPosition, RectTransform rt){
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, touchPosition, _uiCamera, out localPoint);
        return rt.rect.Contains(localPoint);
    }
    
    private void OnEnable(){
        _touchPosition.action.Enable();
        _touchDelta.action.Enable();
        _tapDetection.action.Enable();

        _tapDetection.action.performed += TapPerformed;
        _tapDetection.action.canceled += TapCanceled;

        _touchPosition.action.performed += TouchPositionPerformed;
        _touchPosition.action.canceled += TouchPositionCanceled;

        _touchDelta.action.performed += MovementDeltaPerformed;
        _touchDelta.action.canceled += MovementDeltaCanceled;
    }


    private void OnDisable(){
        _tapDetection.action.performed -= TapPerformed;
        _tapDetection.action.canceled -= TapCanceled;
        
        _touchPosition.action.performed -= TouchPositionPerformed;
        _touchPosition.action.canceled -= TouchPositionCanceled;

        _touchDelta.action.performed -= MovementDeltaPerformed;
        _touchDelta.action.canceled -= MovementDeltaCanceled;

        _tapDetection.action.Disable();
        _touchPosition.action.Disable();
        _touchDelta.action.Disable();
    }
}
