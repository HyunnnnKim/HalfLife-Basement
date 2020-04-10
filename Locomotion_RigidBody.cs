using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VRcustom;

public class Locomotion_RigidBody : LocomotionProvider
{

 // Start is called before the first frame update
    public enum MoveDirectionType
    {
        HeadBased,
        ControllerBased,
        HeadAndController
    }
    public enum TurnType
    {
        SnapTurn,
        SmoothTurn
    }


    [Header("Moving Section")]

    public MoveDirectionType moveDirectionType = MoveDirectionType.HeadBased;

    [SerializeField]
    float m_SpeedAmout = 4.0f;
    public float SpeedAmount { get { return m_SpeedAmout; } set { m_SpeedAmout = value; } }

    [SerializeField]
    float m_RunAccelerationAmount = 2.0f;
    public float RunAccelerationAmount { get { return m_RunAccelerationAmount; } set { m_RunAccelerationAmount = value; } }

    [Header("Turning Section")]

    public TurnType turnType = TurnType.SnapTurn;

    [SerializeField]
    float m_SmoothTurnSpeed = 80f;
    public float SmoothTurnSpeed { get { return m_SmoothTurnSpeed; } set { m_SmoothTurnSpeed = value; } }

    [SerializeField]
    [Tooltip("The number of degrees clockwise to rotate when snap turning clockwise.")]
    float m_TurnAmount = 45.0f;
    /// <summary>
    /// The number of degrees clockwise to rotate when snap turning clockwise.
    /// </summary>
    public float TurnAmount { get { return m_TurnAmount; } set { m_TurnAmount = value; } }

    [SerializeField]
    [Tooltip("The deadzone that the controller movement will have to be above to trigger a snap turn.")]
    float m_DeadZone = 0.75f;
    /// <summary>
    /// The deadzone that the controller movement will have to be above to trigger a snap turn.
    /// </summary>
    public float DeadZone { get { return m_DeadZone; } set { m_DeadZone = value; } }

    [Header("Gravity And Jump Section")]

    [SerializeField]
    float m_gravityAmount = -9.81f;
    public float GravityAmount { get { return m_gravityAmount; } set { m_gravityAmount = value; } }
    Vector3 playerYVelocity;
    readonly int layerMask = -1 - (1 << 8);

    [SerializeField]
    float m_jumpHeight = 2f;
    public float JumpHeight { get { return m_jumpHeight; } set { m_jumpHeight = value; } }


    // state data

    private GameObject leftHandController = null;
    private GameObject rightHandController = null;
    private GameObject neck = null;
    private GameObject head = null;
    private GameObject cameraOffset = null;

    private CapsuleCollider BodyCapsuleCollider;
    private PlayerInput playerinput;
    private Rigidbody rb;


    protected override void Awake()
    {
        cameraOffset = GameObject.Find("Camera Offset");
        head = GameObject.Find("Camera Offset/Main Camera");
        neck = GameObject.Find("Camera Offset/Neck");
        leftHandController = GameObject.Find("Camera Offset/LeftHand Controller");
        rightHandController = GameObject.Find("Camera Offset/RightHand Controller");
    }

    private void Start()
    {
        playerinput = PlayerInput.Instance;
        rb = GetComponent<Rigidbody>();
        BodyCapsuleCollider = GetComponent<CapsuleCollider>();
        updateCollider();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTurn(playerinput.GetRightHandInputData().commonAxisStatus.thumb2DAxis);
        updateCollider();
        if (playerinput.GetRightHandInputData().commonButtonStatus.thumbButton && IsGrounded())
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        UpdateMove(playerinput.GetLeftHandInputData().commonAxisStatus.thumb2DAxis,
                playerinput.GetLeftHandInputData().commonButtonStatus.thumbButton);
    }

    private void UpdateMove(Vector2 position, bool IsCliked)
    {
        Vector3 direction = new Vector3(position.x, 0, position.y);
        switch (moveDirectionType)
        {
            case MoveDirectionType.HeadBased:
            default:
                Vector3 headRotation = new Vector3(0, neck.transform.eulerAngles.y, 0);
                direction = Quaternion.Euler(headRotation) * direction;
                break;
            case MoveDirectionType.ControllerBased:
                Vector3 controllerRotation = new Vector3(0, leftHandController.transform.eulerAngles.y, 0);
                direction = Quaternion.Euler(controllerRotation) * direction;
                break;
            case MoveDirectionType.HeadAndController:
                Vector3 middleOfHands = (leftHandController.transform.position + rightHandController.transform.position) / 2;
                Vector3 headLookAtMidOfH = Quaternion.LookRotation(middleOfHands - neck.transform.position).eulerAngles;
                headLookAtMidOfH.x = headLookAtMidOfH.z = 0;
                direction = Quaternion.Euler(headLookAtMidOfH) * direction;
                break;
        }

        Vector3 movement = direction * m_SpeedAmout;

        if (IsCliked) movement *= m_RunAccelerationAmount;

        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
        //rb.AddForce(movement * Time.fixedDeltaTime);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(neck.transform.position, Vector3.down, head.transform.localPosition.y + neck.transform.localPosition.y + 0.45f, layerMask);
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
    }

    bool isTurnedInLastFrame = false;
    private void UpdateTurn(Vector2 currentState)
    {
        if (!CanBeginLocomotion())
            return;

        switch (turnType)
        {
            case TurnType.SnapTurn:
                if (currentState.x > DeadZone || currentState.x < -DeadZone)
                {
                    if (isTurnedInLastFrame == true)
                        return;
                    isTurnedInLastFrame = true;

                    if (BeginLocomotion())
                    {
                        var xrRig = system.xrRig;
                        if (xrRig != null)
                        {
                            xrRig.RotateAroundCameraUsingRigUp(currentState.x > 0 ? m_TurnAmount : -m_TurnAmount);
                        }
                        EndLocomotion();
                    }
                }
                else
                {
                    isTurnedInLastFrame = false;
                }
                break;

            case TurnType.SmoothTurn:
                if (currentState.x > DeadZone || currentState.x < -DeadZone)
                {
                    if (BeginLocomotion())
                    {
                        var xrRig = system.xrRig;
                        if (xrRig != null)
                        {
                            xrRig.RotateAroundCameraUsingRigUp((currentState.x > 0 ? m_SmoothTurnSpeed : -m_SmoothTurnSpeed) * Time.deltaTime);
                        }
                        EndLocomotion();
                    }
                }
                break;
        }
    }

    private void updateCollider()
    {
        Vector3 headPosition = neck.transform.localPosition;
        BodyCapsuleCollider.height = headPosition.y;
        BodyCapsuleCollider.center = new Vector3(headPosition.x, headPosition.y / 2, headPosition.z);
    }

}
