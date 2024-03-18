using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform PlayerTransform;

    private Vector3 _cameraOffset;

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;

    public bool LookAtPlayer = false;

    public float RotationSpeed = 8.0f;

    public float PitchSpeed = 2.0f;

    public float CameraPitchMin = 1.5f;

    public float CameraPitchMax = 6.5f;

    public StickController CameraStick;

    void Awake()
    {
        if (CameraStick != null)
        {
            CameraStick.StickChanged += CameraStick_StickChanged;
        }
    }

    private Vector2 CameraStickPos = Vector2.zero;

    private void CameraStick_StickChanged(object sender, StickEventArgs e)
    {
        CameraStickPos = e.Position;
    }

    // Use this for initialization
    void Start()
    {
        _cameraOffset = transform.position - PlayerTransform.position;
    }

    // LateUpdate is called after Update methods
    void LateUpdate()
    {
        float h = CameraStickPos.x * RotationSpeed;
        float v = CameraStickPos.y * PitchSpeed;

        Quaternion camTurnAngle = Quaternion.AngleAxis(h, Vector3.up);

        Quaternion camTurnAngleY = Quaternion.AngleAxis(v, transform.right);

        Vector3 newCameraOffset = camTurnAngle * camTurnAngleY * _cameraOffset;

        // Limit camera pitch
        if (newCameraOffset.y < CameraPitchMin || newCameraOffset.y > CameraPitchMax)
        {
            newCameraOffset = camTurnAngle * _cameraOffset;
        }

        _cameraOffset = newCameraOffset;


        Vector3 newPos = PlayerTransform.position + _cameraOffset;

        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);

        if (LookAtPlayer)
            transform.LookAt(PlayerTransform);
    }
}
