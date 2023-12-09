using System;
using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _rotationSpeed = 2.0f;
    [SerializeField] private float _minVerticalAngle = 5.0f;
    [SerializeField] private float _maxVerticalAngle = 80.0f;
    [SerializeField] private float _transitionDuration = 1.0f;

    [Space(10)]
    [Header("Events")]
    [SerializeField] private SO_ObservationPointEvent _onTargetChange;
    [SerializeField] private SO_Vector2Event _onInputDeltaChage;

    private Transform _target;
    private bool _isTransitioning;
    private float _defaultDistance = 15.0f;
    private float _currentRotationX = 0.0f;
    private Vector2 _rotationDelta;

    
    private void Update()
    {
        if (_isTransitioning)
            return;

        _currentRotationX = Mathf.Clamp(_currentRotationX + _rotationDelta.y, _minVerticalAngle, _maxVerticalAngle);

        transform.rotation = Quaternion.Euler(_currentRotationX, transform.rotation.eulerAngles.y - _rotationDelta.x, 0);

        Vector3 newPosition = _target.position - transform.forward * _defaultDistance;

        // Apply the new position
        transform.position = newPosition;

        // Ensure the camera is always looking at the target
        transform.LookAt(_target.position);
    }
    // Method to smoothly change the target over time
    public void OnTargetChange(ObservationPoint observationPoint){
        // if(target == newTarget) return;
        if (_isTransitioning){
            StopAllCoroutines();
        }
        StartCoroutine(SmoothTargetChange(observationPoint.Target, observationPoint.DistanceFromTarget));
    }
    // Coroutine to smoothly change the target
    private IEnumerator SmoothTargetChange(Transform newTarget, float distance){
        _isTransitioning = true;
        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / _transitionDuration)
        {
            // Interpolate both position and rotation
            transform.position = Vector3.Lerp(initialPosition, newTarget.position - transform.forward * distance, t);
            transform.rotation = Quaternion.Slerp(initialRotation, Quaternion.LookRotation(newTarget.position - transform.position), t);

            yield return null;
        }

        // Ensure the final position and rotation match the new target
        transform.position = newTarget.position - transform.forward * distance;
        transform.rotation = Quaternion.LookRotation(newTarget.position - transform.position);

        _target = newTarget;
        _currentRotationX = transform.rotation.eulerAngles.x;
        _isTransitioning = false;
        _defaultDistance = distance;
    }
    private void OnInputDeltaChage(Vector2 value) => _rotationDelta = value * _rotationSpeed;

    private void OnEnable(){
        _onTargetChange.AddListener(OnTargetChange);
        _onInputDeltaChage.AddListener(OnInputDeltaChage);
    }
    private void OnDisable(){
        _onTargetChange.RemoveListener(OnTargetChange);
        _onInputDeltaChage.AddListener(OnInputDeltaChage);
    }
}