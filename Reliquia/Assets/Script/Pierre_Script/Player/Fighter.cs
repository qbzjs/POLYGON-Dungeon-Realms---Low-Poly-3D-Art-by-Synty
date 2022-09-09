using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFighter 
{
    int GetStrength();
    void Hurt(int damage);

    void Die();

}
