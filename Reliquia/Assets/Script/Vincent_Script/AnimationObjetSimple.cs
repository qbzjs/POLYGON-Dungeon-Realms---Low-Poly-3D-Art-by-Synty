using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AnimationObjetSimple : MonoBehaviour
{
    public Axis axis;
    private Quaternion _angleAller;
    private Quaternion _angleRetour;
    [SerializeField]
    private float _angleRotation = 10.0f;
    [SerializeField]
    private float _vitesseRotation = 2.0f;
    public AudioSource audioSource = null;
    private void Awake()
    {
        switch (axis)
        {
            case Axis.None:
                break;
            case Axis.X:
                _angleAller = Quaternion.AngleAxis(_angleRotation, Vector3.right) * transform.rotation;
                _angleRetour = Quaternion.AngleAxis(-_angleRotation, Vector3.right) * transform.rotation;
                break;
            case Axis.Y:
                _angleAller = Quaternion.AngleAxis(_angleRotation, Vector3.up) * transform.rotation;
                _angleRetour = Quaternion.AngleAxis(-_angleRotation, Vector3.up) * transform.rotation;
                break;
            case Axis.Z:
                _angleAller = Quaternion.AngleAxis(_angleRotation, Vector3.forward) * transform.rotation;
                _angleRetour = Quaternion.AngleAxis(-_angleRotation, Vector3.forward) * transform.rotation;
                break;
            default:
                break;
        }
    }
    void Start()
    {
        StartCoroutine(LancerAnimation());
    }
    IEnumerator LancerAnimation()
    {
        yield return new WaitForSeconds(Random.Range(0.0f, 5.0f));
        AnimationAller();
    }
    private void AnimationAller()
    {
        JouerSon();
        transform.DORotateQuaternion(_angleAller, _vitesseRotation).OnComplete(() => AnimationRetour());
    }
    private void AnimationRetour()
    {
        JouerSon();
        transform.DORotateQuaternion(_angleRetour, _vitesseRotation).OnComplete(() => AnimationAller());
    }
    private void JouerSon()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
