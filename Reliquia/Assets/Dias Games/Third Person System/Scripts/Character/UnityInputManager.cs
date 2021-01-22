/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

namespace DiasGames.ThirdPersonSystem
{
    public class InputButton
    {
        public bool WasPressed { get; private set; }
        public bool WasReleased { get; private set; }
        public bool IsPressed { get; private set; }

        public string InputName;

        public InputButton(string input)
        {
            WasPressed = false;
            WasReleased = false;
            IsPressed = false;

            InputName = input;
        }

        public void Update()
        {
            if (string.IsNullOrEmpty(InputName))
                return;

            IsPressed = Input.GetButton(InputName);
            WasPressed = Input.GetButtonDown(InputName);
            WasReleased = Input.GetButtonUp(InputName);
        }


        public void SetButtonState(bool wasPressed, bool wasReleased, bool pressing)
        {
            WasPressed = wasPressed;
            WasReleased = wasReleased;
            IsPressed = pressing;
        }
    }

    public enum InputReference
    {
        Jump, Walk, Roll, Crouch, Crawl, Drop, Interact,
        Toggle, RightWeapon, LeftWeapon, Zoom,
        Fire, Reload, Action01, Action02, Action03
    }

    public class UnityInputManager : MonoBehaviour
    {        
        // Camera reference
        [Tooltip("Camera used in the scene")][SerializeField] private Transform m_Camera;

        // --------------------- INPUT BUTTONS --------------------- //

        public InputButton jumpButton { get; private set; }
        public InputButton walkButton { get; private set; }
        public InputButton rollButton { get; private set; }
        public InputButton crouchButton { get; private set; }
        public InputButton crawlButton { get; private set; }
        public InputButton dropButton { get; private set; }

        public InputButton toggleWeaponButton { get; private set; }
        public InputButton rightWeaponButton { get; private set; }
        public InputButton leftWeaponButton { get; private set; }
        public InputButton zoomButton { get; private set; }
        public InputButton fireButton { get; private set; }
        public InputButton reloadButton { get; private set; }

        public InputButton interactButton { get; private set; }

        public InputButton action01 { get; private set; }
        public InputButton action02 { get; private set; }
        public InputButton action03 { get; private set; }


        // String names
        [Space()]
        [SerializeField] private string m_JumpInputName = "Jump";
        [Space()]
        [SerializeField] private string m_WalkInputName = "Walk";
        [Space()]
        [SerializeField] private string m_RollInputName = "Roll";
        [Space()]
        [SerializeField] private string m_DropInputName = "Drop";
        [Space()]
        [SerializeField] private string m_CrouchInputName = "Crouch";
        [Space()]
        [SerializeField] private string m_CrawlInputName = "Crawl";
        [Space()]
        [SerializeField] private string m_ZoomInputName = "Zoom";
        [Space()]

        [SerializeField] private string m_InteractInputName = "Interact";
        [Space()]
        [SerializeField] private string m_ToggleWeaponInputName = "Toggle";
        [Space()]
        [SerializeField] private string m_RightWeaponInputName = "RightWeapon";
        [Space()]
        [SerializeField] private string m_LeftWeaponInputName = "LeftWeapon";
        [Space()]
        [SerializeField] private string m_FireInputName = "Fire";
        [Space()]
        [SerializeField] private string m_ReloadInputName = "Reload";
        [Space()]
        [SerializeField] private string m_ExtraAction01 = string.Empty;
        [Space()]
        [SerializeField] private string m_ExtraAction02 = string.Empty;
        [Space()]
        [SerializeField] private string m_ExtraAction03 = string.Empty;

        // --------------------------------------------------------- //

        private Vector2 m_Move;
        private Vector2 m_ScrollView;
        private Vector3 m_RelativeInput;

        public Vector3 Move { get { return m_Move; }  set { m_Move = value; } }
        public Vector2 ScrollView { get { return m_ScrollView; }  set { m_ScrollView = value; } }
        public Vector3 RelativeInput { get { return m_RelativeInput; } }

        [HideInInspector] public bool manualUpdate = false;

        CinemachineFreeLook[] m_FreeLookCameras;

        private bool m_HideCursor = true;
        private float waitTimeToHide = 0f;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Find main camera if it was not attached in hierarchy
            if (m_Camera == null)
            {
                if (Camera.main == null)
                {
                    Debug.LogError("There is no Camera in the scene. Please add a camera!");
                }
                else
                    m_Camera = Camera.main.transform;
            }

            m_FreeLookCameras = FindObjectsOfType<CinemachineFreeLook>();

            // Initialize buttons
            jumpButton = new InputButton(m_JumpInputName);
            walkButton = new InputButton(m_WalkInputName);
            rollButton = new InputButton(m_RollInputName);
            crouchButton = new InputButton(m_CrouchInputName);
            crawlButton = new InputButton(m_CrawlInputName);
            dropButton = new InputButton(m_DropInputName);

            toggleWeaponButton = new InputButton(m_ToggleWeaponInputName);
            rightWeaponButton = new InputButton(m_RightWeaponInputName);
            leftWeaponButton = new InputButton(m_LeftWeaponInputName);
            fireButton = new InputButton(m_FireInputName);
            reloadButton = new InputButton(m_ReloadInputName);
            zoomButton = new InputButton(m_ZoomInputName);

            interactButton = new InputButton(m_InteractInputName);

            action01 = new InputButton(m_ExtraAction01);
            action02 = new InputButton(m_ExtraAction02);
            action03 = new InputButton(m_ExtraAction03);
        }

        private void FixedUpdate()
        {
            if (!manualUpdate)
            {
                m_Move.x = Input.GetAxis("Horizontal");
                m_Move.y = Input.GetAxis("Vertical");

                m_ScrollView.x = Input.GetAxis("Mouse X");
                m_ScrollView.y = Input.GetAxis("Mouse Y");
            }

            // calculate camera relative direction to move:
            Vector3 CamForward = Vector3.Scale(m_Camera.forward, new Vector3(1, 0, 1)).normalized;
            m_RelativeInput = m_Move.y * CamForward + m_Move.x * m_Camera.right;

        }

        private void Update()
        {
            if (m_HideCursor)
            {
                if (waitTimeToHide <= 0)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                    waitTimeToHide -= Time.unscaledDeltaTime;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                m_HideCursor = true;
                waitTimeToHide = 0.1f;
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
                m_HideCursor = false;

            foreach (CinemachineFreeLook freeLook in m_FreeLookCameras)
            {
                if (freeLook.IsValid)
                {
                    freeLook.m_XAxis.m_InputAxisValue = m_ScrollView.x;
                    freeLook.m_YAxis.m_InputAxisValue = m_ScrollView.y;
                }
            }

            if (manualUpdate)
                return;

            jumpButton.Update();
            walkButton.Update();
            rollButton.Update();
            crouchButton.Update();
            crawlButton.Update();
            dropButton.Update();

            toggleWeaponButton.Update();
            rightWeaponButton.Update();
            leftWeaponButton.Update();
            fireButton.Update();
            reloadButton.Update();
            zoomButton.Update();
            interactButton.Update();

            action01.Update();
            action02.Update();
            action03.Update();
        }

        public InputButton GetInputReference(InputReference reference)
        {
            switch (reference)
            {
                case InputReference.Jump:
                    return jumpButton;
                case InputReference.Walk:
                    return walkButton;
                case InputReference.Roll:
                    return rollButton;
                case InputReference.Crouch:
                    return crouchButton;
                case InputReference.Crawl:
                    return crawlButton;
                case InputReference.Drop:
                    return dropButton;
                case InputReference.Interact:
                    return interactButton;
                case InputReference.Toggle:
                    return toggleWeaponButton;
                case InputReference.RightWeapon:
                    return rightWeaponButton;
                case InputReference.LeftWeapon:
                    return leftWeaponButton;
                case InputReference.Zoom:
                    return zoomButton;
                case InputReference.Fire:
                    return fireButton;
                case InputReference.Reload:
                    return reloadButton;
                case InputReference.Action01:
                    return action01;
                case InputReference.Action02:
                    return action02;
                case InputReference.Action03:
                    return action03;
                default:
                    return walkButton;
            }
        }
    }
}
