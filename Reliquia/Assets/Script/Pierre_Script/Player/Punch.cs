using System.Collections;
using System.Collections.Generic;
using DiasGames.ThirdPersonSystem;
using UnityEngine;

public class Punch : ThirdPersonAbility
{
    public override void Initialize(ThirdPersonSystem mainSystem, AnimatorManager animatorManager, UnityInputManager inputManager)
    {
        base.Initialize(mainSystem, animatorManager, inputManager);
    }

    public override bool TryEnterAbility()
    {
        if(m_System.IsGrounded && William_Script.instance.BoutonAttaquer && !William_Script.instance.UsingPower)
        {
            William_Script.instance.BoutonAttaquer = false;
            return true;
        }
        return false;
    }

    public override bool TryExitAbility()
    {
        return (!m_System.IsGrounded);
    }

    public override void OnEnterAbility()
    {
        base.OnEnterAbility();

    }
    public override void OnExitAbility()
    {
        base.OnExitAbility();

    }
}
