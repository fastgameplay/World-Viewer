using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] SO_ObservationPointEvent _onTargetChange;
    public float rotationSpeed = 2.0f;
    public float minVerticalAngle = -80.0f;
    public float maxVerticalAngle = 80.0f;
    public float duration = 1.0f;

    private Transform target;
    private bool isTransitioning;
    private float defaultDistance = 15.0f;
    private float currentRotationX = 0.0f;

    [SerializeField] ObservationPoint point;

    void Start(){
        OnTargetChange(point);
    }
    
    void Update()
    {
       
        // Check if transitioning; if so, return and don't handle input
        if (isTransitioning)
            return;
        // // Check for touch input
        // {
        //     float rotationX = Input.GetAxis("Mouse Y") * rotationSpeed;
        //     float rotationY = Input.GetAxis("Mouse X") * rotationSpeed;

        //     // Update the current vertical rotation angle
        //     currentRotationX = Mathf.Clamp(currentRotationX - rotationX, minVerticalAngle, maxVerticalAngle);

        //     // Rotate around the target based on the updated vertical rotation angle
        //     transform.rotation = Quaternion.Euler(currentRotationX, transform.rotation.eulerAngles.y + rotationY, 0);
        // }
        // else
        // {
            float rotationX;
            float rotationY;
                    // {
        if (Input.GetMouseButton(0)){
            rotationX = -Input.GetAxis("Mouse Y") * rotationSpeed * 2;
            rotationY = -Input.GetAxis("Mouse X") * rotationSpeed * 2;
            Debug.Log(Input.GetAxis("Mouse X") + " : " + Input.GetAxis("Mouse Y") );
        }
        else{
            rotationX = Input.GetAxis("Vertical") * rotationSpeed;
            rotationY = Input.GetAxis("Horizontal") * rotationSpeed;
        }

            // Update the current vertical rotation angle
            currentRotationX = Mathf.Clamp(currentRotationX + rotationX, minVerticalAngle, maxVerticalAngle);

            // Rotate around the target based on the updated vertical rotation angle
            transform.rotation = Quaternion.Euler(currentRotationX, transform.rotation.eulerAngles.y - rotationY, 0);
        // }

        // Calculate the new position maintaining the default distance
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
    private void OnEnable(){
        _onTargetChange.AddListener(OnTargetChange);
    }
    private void OnDisable(){
        _onTargetChange.RemoveListener(OnTargetChange);
    }
}