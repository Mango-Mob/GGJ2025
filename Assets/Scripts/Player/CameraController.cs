using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private float followFOV = 90.0f;
    [SerializeField] private float zoomedFOV = 90.0f;
    private Vector3 velocity;
    private float fovVelocity;
    
    AnimationCurve curve;
    Camera thisCamera;
    private CameraState cameraState;

    public enum CameraState
    {
        FOLLOW,
        ZOOMED,
        DEAD,
    }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        thisCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPos = Vector3.zero;
        switch (cameraState)
        {
            case CameraState.FOLLOW:
                targetPos = new Vector3(0.0f, target.position.y, transform.position.z);
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
                thisCamera.fieldOfView = Mathf.SmoothDamp(thisCamera.fieldOfView, followFOV, ref fovVelocity, 0.1f);
                break;
            case CameraState.ZOOMED:
                targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
                thisCamera.fieldOfView = Mathf.SmoothDamp(thisCamera.fieldOfView, zoomedFOV, ref fovVelocity, 0.1f);
                break;
            case CameraState.DEAD:
                break;
            default:
                break;
        }
    }
    public void SetCameraState(CameraState _state)
    {
        cameraState = _state;
    }
}
