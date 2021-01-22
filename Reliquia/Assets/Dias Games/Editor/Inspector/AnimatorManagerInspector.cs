using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AnimatorManager))]
    public class AnimatorManagerInspector : BaseInspector
    {
        protected override void FormatLabel()
        {
            label = "Animator Manager";

        }
    }
}