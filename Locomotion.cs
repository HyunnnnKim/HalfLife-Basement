using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VRcustom;

public class Locomotion : LocomotionProvider
{
    // Start is called before the first frame update
    public enum MoveDirectionType
    {
        HeadBased,
        ControllerBased
    }
    public enum TurnType
    {
        SnapTurn,
        SmoothTurn
    }


    private PlayerInput playerinput;
    private CharacterController characterController = null;
    private GameObject head = null;

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
    [Tooltip("The amount of time that the system will wait before starting another snap turn.")]
    float m_DebounceTime = 0.5f;
    /// <summary>
    /// The amount of time that the system will wait before starting another snap turn.
    /// </summary>
    public float DebounceTime { get { return m_DebounceTime; } set { m_DebounceTime = value; } }

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
    float m_jumpHeight = 3f;
    public float JumpHeight { get { return m_jumpHeight; } set { m_jumpHeight = value; }  }


    // state data
    float m_CurrentTurnAmount = 0.0f;
    float m_TimeStarted = 0.0f;

    GameObject leftHandController;
    protected override void Awake()
    {
        characterController = GetComponent<CharacterController>();
        head = GetComponent<XRRig>().cameraGameObject;
        leftHandController = GameObject.Find("LeftHand Controller");
    }

    private void Start()
    {
        playerinput = PlayerInput.Instance;
        PositionController();
    }

    // Update is called once per frame
    void Update()
    {
        PositionController();
        ApplyGravity();
        UpdateMove(playerinput.GetLeftHandInputData().commonAxisStatus.thumb2DAxis,
            playerinput.GetLeftHandInputData().commonButtonStatus.thumbButton);

        UpdateTurn(playerinput.GetRightHandInputData().commonAxisStatus.thumb2DAxis);

        if (playerinput.GetRightHandInputData().commonButtonStatus.thumbButton && IsGrounded())
        {
            Jump();
        }

        if (playerinput.GetRightHandInputData().otherButtonStatus.secondaryTouch)
        {
            playerinput.SendHapticImpulse(VRControllerNode.leftHand, 0.2f, 0.1f);
        }
        if (playerinput.GetRightHandInputData().otherButtonStatus.secondaryButton)
        {
            playerinput.SendHapticImpulse(VRControllerNode.leftHand, 0.6f, 10f);
        }

    }

    private void PositionController()
    {
        float headHeight = Mathf.Clamp(head.transform.localPosition.y, 1, 2);
        characterController.height = headHeight;

        Vector3 newCenter = Vector3.zero;
        newCenter.y = characterController.height / 2;
        newCenter.y += characterController.skinWidth;

        newCenter.x = head.transform.localPosition.x;
        newCenter.z = head.transform.localPosition.z;

        characterController.center = newCenter;
    }

    private void UpdateMove(Vector2 position, bool IsCliked)
    {
        Vector3 direction = new Vector3(position.x, 0, position.y);
        

        switch (moveDirectionType)
        {
            case MoveDirectionType.HeadBased:
            default:
                Vector3 headRotation = new Vector3(0, head.transform.eulerAngles.y, 0);
                direction = Quaternion.Euler(headRotation) * direction;
                break;
            case MoveDirectionType.ControllerBased:
                Vector3 controllerRotation = new Vector3(0, leftHandController.transform.eulerAngles.y, 0);
                direction = Quaternion.Euler(controllerRotation) * direction;
                break;
        }

        Vector3 movement = direction * m_SpeedAmout;

        if (IsCliked) movement *= m_RunAccelerationAmount;

        characterController.Move(movement * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(head.transform.position, Vector3.down, head.transform.localPosition.y + 0.45f, layerMask);
    }

    private void ApplyGravity()
    {
        if (IsGrounded())
            playerYVelocity.y = -2f;
        else
            playerYVelocity.y += m_gravityAmount * Time.deltaTime;
        characterController.Move(playerYVelocity * Time.deltaTime);
    }

    private void Jump()
    {
        playerYVelocity.y = Mathf.Sqrt(JumpHeight * -2f * GravityAmount);
        characterController.Move(playerYVelocity * Time.deltaTime);
    }

    bool IsTurnedInLastFrame = false;
    private void UpdateTurn(Vector2 currentState)
    {
        switch (turnType)
        {
            case TurnType.SnapTurn:
                // wait for a certain amount of time before allowing another turn.
                if (m_TimeStarted > 0.0f && (m_TimeStarted + m_DebounceTime < Time.time))
                {
                    m_TimeStarted = 0.0f;
                    return;
                }

                if (currentState.x > DeadZone || currentState.x < -DeadZone)
                {
                    if (IsTurnedInLastFrame == true)
                        return;
                    StartTurn(currentState.x > 0 ? m_TurnAmount : -m_TurnAmount);
                    IsTurnedInLastFrame = true;
                }
                else
                {
                    IsTurnedInLastFrame = false;
                }


                if (Math.Abs(m_CurrentTurnAmount) > 0.0f && BeginLocomotion())
                {
                    var xrRig = system.xrRig;
                    if (xrRig != null)
                    {
                        xrRig.RotateAroundCameraUsingRigUp(m_CurrentTurnAmount);
                    }
                    m_CurrentTurnAmount = 0.0f;
                    EndLocomotion();
                }
                break;

            case TurnType.SmoothTurn:
                if (currentState.x > DeadZone || currentState.x < -DeadZone)
                {
                    if (!CanBeginLocomotion())
                        return;
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

    private void StartTurn(float amount)
    {
        if (m_TimeStarted != 0.0f)
            return;

        if (!CanBeginLocomotion())
            return;

        m_TimeStarted = Time.time;
        m_CurrentTurnAmount = amount;
    }

}
