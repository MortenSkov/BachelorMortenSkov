using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    InputHandler inputHandler;

    /// <summary>
    /// The target transform that the camera will eventually be looking at
    /// </summary>
    public Transform targetTransform;
    /// <summary>
    /// The transform of our actual Camera GameObject
    /// </summary>
    public Transform cameraTransform;
    /// <summary>
    /// The transform of our Cameras pivot - used for making the camera rotate around our player character
    /// </summary>
    public Transform cameraPivotTransform;

    private Transform myTransform;
    private Vector3 cameraTransformPosition;
    private LayerMask ignoreLayers;
    private Vector3 cameraFollowVelocity = Vector3.zero;

    public static CameraHandler singleton;

    public float lookSpeed = 0.1f;
    public float followSpeed = 0.1f;
    public float pivotSpeed = 0.03f;

    private float targetPosition;
    private float defaultPosition;
    private float lookAngle;
    private float pivotAngle;
    public float minimumPivot = -35;
    public float maximumPivot = 35;

    public float cameraSphereRadius = 0.2f;
    public float cameraCollisionOffset = 0.2f;
    public float minimumCollisionOffset = 0.2f;

    public Transform currentLockOnTarget;

    List<CharacterManager> availableTargets = new List<CharacterManager>();
    public Transform nearestLockOnTarget;
    public Transform leftLockTarget;
    public Transform rightLockTarget;
    public float maximumLockOnDistance = 30;


    private void Awake()
    {
        singleton = this;
        myTransform = transform;
        defaultPosition = cameraTransform.localPosition.z;
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        inputHandler = FindObjectOfType<InputHandler>();
    }

    /// <summary>
    /// Causes our camera to follow the transform of our target position
    /// </summary>
    /// <param name="delta">Delta time passed since the last update called</param>
    public void FollowTarget(float delta)
    {
        //Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
        Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
        myTransform.position = targetPosition;

        HandleCameraCollision(delta);
    }

    /// <summary>
    /// Handles the camera's rotation around the player GameObject, through mouse input
    /// </summary>
    /// <param name="delta">fixed delta time passed since last call on the update method</param>
    /// <param name="mouseXInput">Mouse input on the X-Axis</param>
    /// <param name="mouseYInput">Mouse input on the Y-Axis</param>
    public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
    {
        if(!inputHandler.lockOnFlag && currentLockOnTarget == null) // If Player is NOT locked-on to a target
        {
            lookAngle += mouseXInput * lookSpeed * delta; // lookAngle += (mouseXInput * lookSpeed) / delta; -- (OLD LOGIC)
            pivotAngle -= mouseYInput * pivotSpeed * delta; // pivotAngle -= (mouseYInput * pivotSpeed) / delta; -- (OLD LOGIC)

            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            myTransform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;

            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
        else // Player IS lock-on to a target
        {
            //float velocity = 0;

            Vector3 dir = currentLockOnTarget.position - transform.position;
            dir.Normalize();
            dir.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = targetRotation;

            dir = currentLockOnTarget.position - cameraPivotTransform.position;
            dir.Normalize();

            targetRotation = Quaternion.LookRotation(dir);
            Vector3 eulerAngle = targetRotation.eulerAngles;
            eulerAngle.y = 0;
            cameraPivotTransform.localEulerAngles = eulerAngle;
        }
    }

    private void HandleCameraCollision(float delta)
    {
        targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
        direction.Normalize();

        if(Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
        {
            float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetPosition = -(dis - cameraCollisionOffset);
        }

        if(Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = -minimumCollisionOffset;
        }

        cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
        cameraTransform.localPosition = cameraTransformPosition;
    }

    public void HandleLockOn()
    {
        float shortestDistance = Mathf.Infinity; // used to measure the distance between targets and pick the shortest one, that is closest to us
        float shortestDistanceOfLeftTarget = Mathf.Infinity;
        float shortestDistanceOfRightTarget = Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager character = colliders[i].GetComponent<CharacterManager>();

            if(character != null)
            {
                Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);

                if(character.transform.root != targetTransform.transform.root 
                    && viewableAngle > -50 && viewableAngle < 50 
                    && distanceFromTarget <= maximumLockOnDistance)
                {
                    availableTargets.Add(character);
                }
            }
        }

        for (int k = 0; k < availableTargets.Count; k++)
        {
            float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);

            if(distanceFromTarget < shortestDistance)
            {
                shortestDistance = distanceFromTarget;
                nearestLockOnTarget = availableTargets[k].lockOnTransform;
            }

            if (inputHandler.lockOnFlag)
            {
                Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(availableTargets[k].transform.position);
                float distanceFromLeftTarget = currentLockOnTarget.transform.position.x - availableTargets[k].transform.position.x;
                float distanceFromRightTarget = currentLockOnTarget.transform.position.x + availableTargets[k].transform.position.x;

                if(relativeEnemyPosition.x > 0.00 && distanceFromLeftTarget < shortestDistanceOfLeftTarget)
                {
                    shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                    leftLockTarget = availableTargets[k].lockOnTransform;
                }

                if(relativeEnemyPosition.x < 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                {
                    shortestDistanceOfRightTarget = distanceFromRightTarget;
                    rightLockTarget = availableTargets[k].lockOnTransform;
                }
            }
        }
    }

    public void ClearLockOnTargets()
    {
        availableTargets.Clear();
        nearestLockOnTarget = null;
        currentLockOnTarget = null;
    }

}
