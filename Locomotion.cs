using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Locomotion : LocomotionProvider
{
    // Start is called before the first frame update
    private PlayerInput playerinput;
    private CharacterController characterController = null;
    private GameObject head = null;

    [Header("Moving Section")]
    [SerializeField]
    float m_speedAmout = 3.0f;
    public float speedAmount { get { return m_speedAmout; } set { m_speedAmout = value; } }

    [SerializeField]
    float m_gravityMultiplier = 1.0f;
    public float gravityMultiplierAmount { get { return m_gravityMultiplier; } set { m_gravityMultiplier = value; } }

    [SerializeField]
    float m_runAccelerationAmount = 3.0f;
    public float runAccelerationAmount { get { return m_runAccelerationAmount; } set { m_runAccelerationAmount = value; } }

    [Header("Turning Section")]
    [SerializeField]
    [Tooltip("The number of degrees clockwise to rotate when snap turning clockwise.")]
    float m_TurnAmount = 45.0f;
    /// <summary>
    /// The number of degrees clockwise to rotate when snap turning clockwise.
    /// </summary>
    public float turnAmount { get { return m_TurnAmount; } set { m_TurnAmount = value; } }

    [SerializeField]
    [Tooltip("The amount of time that the system will wait before starting another snap turn.")]
    float m_DebounceTime = 0.5f;
    /// <summary>
    /// The amount of time that the system will wait before starting another snap turn.
    /// </summary>
    public float debounceTime { get { return m_DebounceTime; } set { m_DebounceTime = value; } }

    [SerializeField]
    [Tooltip("The deadzone that the controller movement will have to be above to trigger a snap turn.")]
    float m_DeadZone = 0.75f;
    /// <summary>
    /// The deadzone that the controller movement will have to be above to trigger a snap turn.
    /// </summary>
    public float deadZone { get { return m_DeadZone; } set { m_DeadZone = value; } }

    // state data
    float m_CurrentTurnAmount = 0.0f;
    float m_TimeStarted = 0.0f;

    protected override void Awake()
    {
        characterController = GetComponent<CharacterController>();
        head = GetComponent<XRRig>().cameraGameObject;
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
        StartMove(playerinput.GetLeftHandInputData().commonAxisStatus.thumb2DAxis);
        UpdateTurn();
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

    private void StartMove(Vector2 position)
    {
        Vector3 direction = new Vector3(position.x, 0, position.y);
        Vector3 headRotation = new Vector3(0, head.transform.eulerAngles.y, 0);

        direction = Quaternion.Euler(headRotation) * direction;

        Vector3 movement = direction * m_speedAmout * GetMultipliedRunAccelerationAmount();
        characterController.Move(movement * Time.deltaTime);
    }

    private float GetMultipliedRunAccelerationAmount()
    {
        return 1 + m_runAccelerationAmount * playerinput.GetLeftHandInputData().commonAxisStatus.triggerAxis;
    }

    //TODO: need some change to netural gravity
    private void ApplyGravity()
    {
        Vector3 gravity = new Vector3(0, Physics.gravity.y * m_gravityMultiplier, 0);
        gravity.y *= Time.deltaTime;

        characterController.Move(gravity);
    }
    //TODO: JUMP?
    private void Jump()
    {

    }

    private void UpdateTurn()
    {
        // wait for a certain amount of time before allowing another turn.
        if (m_TimeStarted > 0.0f && (m_TimeStarted + m_DebounceTime < Time.time))
        {
            m_TimeStarted = 0.0f;
            return;
        }

        Vector2 currentState = playerinput.GetRightHandInputData().commonAxisStatus.thumb2DAxis;

        if (currentState.x > deadZone)
        {
            StartTurn(m_TurnAmount);
        }
        else if (currentState.x < -deadZone)
        {
            StartTurn(-m_TurnAmount);
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
    }
    internal void FakeStartTurn(bool isLeft)
    {
        StartTurn(isLeft ? -m_TurnAmount : m_TurnAmount);
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
