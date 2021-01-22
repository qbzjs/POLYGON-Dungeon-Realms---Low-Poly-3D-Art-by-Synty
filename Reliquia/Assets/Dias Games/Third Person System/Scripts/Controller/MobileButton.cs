using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DiasGames.ThirdPersonSystem {

    public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        // InputButton that should change states
        [Tooltip("Input Button that should change states")][SerializeField] private InputReference m_InputReference = InputReference.Action01;

        [SerializeField] private Image m_TargetGraphic = null;
        [SerializeField] private Color NormalColor = Color.white;
        [SerializeField] private Color PressedColor = Color.white;

        // Components
        private UnityInputManager m_InputManager;
        private InputButton m_Button;

        // Internal controller variables
        bool wasPressed = false;
        bool wasReleased = false;
        bool pressing = false;

        public void SetInputManager(UnityInputManager manager)
        {
            m_InputManager = manager;
            m_Button = manager.GetInputReference(m_InputReference);
        }

        private void Update()
        {
            if (m_Button == null)
                return;

            m_Button.SetButtonState(wasPressed, wasReleased, pressing);

            wasPressed = false;
            wasReleased = false;

            m_TargetGraphic.color = (pressing) ? PressedColor : NormalColor;
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            wasPressed = true;
            pressing = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            wasReleased = true;
            pressing = false;
        }
    }
}