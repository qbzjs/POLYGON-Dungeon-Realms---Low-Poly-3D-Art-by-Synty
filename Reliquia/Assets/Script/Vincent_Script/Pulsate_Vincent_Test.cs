using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsate_Vincent_Test : MonoBehaviour
{
    public GameObject pulsatePointDepart;
    public GameObject pulsatePrefab;
    public GameObject pulsateInstance;
    public bool isCreated = false;
    public bool isCooldown = false;

    private void OnEnable()
    {

        William_Script.INTERACT_ACTIONS2 += LancerPouvoir;
    }

    private void OnDisable()
    {

        William_Script.INTERACT_ACTIONS2 += LancerPouvoir;
    }
    // Start is called before the first frame update

    private void LancerPouvoir()
    {
        StartCoroutine(Cooldown());
    }
    private IEnumerator Cooldown()
    {
        Instantiate(pulsatePrefab, transform.position, Quaternion.identity, transform);
        isCooldown = true;
        //pulsateInstance.SetActive(true);
        yield return new WaitForSeconds(7);
        isCooldown = false;
    }
}
