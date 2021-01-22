using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class MobileInputController : MonoBehaviour
    {
        [SerializeField] private UnityInputManager m_Character;
        [SerializeField] private bool m_AutoTargetCharacter = false;
        
        private void Awake()
        {
            if (m_Character == null && m_AutoTargetCharacter)
                m_Character = FindObjectOfType<UnityInputManager>();
        }

        private void Start()
        {
            if (m_Character == null)
                return;

            m_Character.manualUpdate = true;

            MobileButton[] buttons = GetComponentsInChildren<MobileButton>();
            foreach (MobileButton button in buttons)
                button.SetInputManager(m_Character);


            MobileJoystick[] analogics = GetComponentsInChildren<MobileJoystick>();
            foreach (MobileJoystick analog in analogics)
                analog.SetInputManager(m_Character);
        }
    }
}