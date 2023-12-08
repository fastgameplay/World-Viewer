using System;
using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] SO_ObservationPointEvent _onTargetChange;
    [SerializeField] SO_Vector2Event _onInputDeltaChage;
    public float rotationSpeed = 2.0f;
    public float minVerticalAngle = -80.0f;
    public float maxVerticalAngle = 80.0f;
    public float duration = 1.0f;

    private Transform target;
    private bool isTransitioning;
    private float defaultDistance = 15.0f;
    private float currentRotationX = 0.0f;
    private Vector2 rotationDelta;
    [SerializeField] ObservationPoint point;

    void Start(){
        OnTargetChange(point);
    }
    
    void Update()
    {
        if (isTransitioning)
            return;

        currentRotationX = Mathf.Clamp(currentRotationX + rotationDelta.y, minVerticalAngle, maxVerticalAngle);

        transform.rotation = Quaternion.Euler(currentRotationX, transform.rotation.eulerAngles.y - rotationDelta.x, 0);

        Vector3 newPosition = target.position - transform.forward * defaultDistance;

        // Apply the new position
        transform.position = newPosition;

        // Ensure the camera is always looking at the target
        transform.LookAt(target.position);


    }
    // Method to smoothly change the target over time
    public void OnTargetChange(ObservationPoint observationPoint){
        // if(target == newTarget) return;
        if (isTransitioning){
            StopAllCoroutines();
        }
        StartCoroutine(SmoothTargetChange(observationPoint.Target, observationPoint.DistanceFromTarget));
    }
    // Coroutine to smoothly change the target
    private IEnumerator SmoothTargetChange(Transform newTarget, float distance){
        isTransitioning = true;
        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
        {
            // Interpolate both position and rotation
            transform.position = Vector3.Lerp(initialPosition, newTarget.position - transform.forward * distance, t);
            transform.rotation = Quaternion.Slerp(initialRotation, Quaternion.LookRotation(newTarget.position - transform.position), t);

            yield return null;
        }

        // Ensure the final position and rotation match the new target
        transform.position = newTarget.position - transform.forward * distance;
        transform.rotation = Quaternion.LookRotation(newTarget.position - transform.position);

        target = newTarget;
        currentRotationX = transform.rotation.eulerAngles.x;
        isTransitioning = false;
        defaultDistance = distance;
    }
    private void OnInputDeltaChage(Vector2 value) => rotationDelta = value * rotationSpeed;
    private void OnEnable(){
        _onTargetChange.AddListener(OnTargetChange);
        _onInputDeltaChage.AddListener(OnInputDeltaChage);
    }


    private void OnDisable(){
        _onTargetChange.RemoveListener(OnTargetChange);
        _onInputDeltaChage.AddListener(OnInputDeltaChage);
    }
}