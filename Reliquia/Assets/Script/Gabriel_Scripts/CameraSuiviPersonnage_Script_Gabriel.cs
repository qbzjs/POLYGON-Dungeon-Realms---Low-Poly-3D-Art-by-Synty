using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSuiviPersonnage_Script_Gabriel : MonoBehaviour
{
    MouvementWilliam_Script mouvementWilliam;
    GameManager gameManager;


    private const float Y_Angle_Min = -25.0f;
    private const float Y_Angle_Max = 25.0f;

    //private const float X_Angle_Min = -25.0f;
    //private const float X_Angle_Max = 25.0f;

    public Transform Personnage;
    public Transform cameraTransform;
    public GameObject followTransform;
    
  


    private Camera cam;

    private float distance = 2.5f;
    private float currentX = 0.0f;
    
    private float currentY = 0.0f;
    public Vector2 _look;

    [SerializeField]
    private float sensibility;

    private void Start()
    {
        mouvementWilliam = FindObjectOfType<MouvementWilliam_Script>();
        gameManager = FindObjectOfType<GameManager>();

        cameraTransform = transform;
        cam = Camera.main;
    }

    private void Update()
    {
        if (gameManager.voirMenu == false) 
        currentX += Input.GetAxis("Mouse X") * sensibility;
        currentY += Input.GetAxis("Mouse Y") * -sensibility;
        

        currentY = Mathf.Clamp(currentY, Y_Angle_Min, Y_Angle_Max);
        //if (!mouvementWilliam.enMouvement) currentX = Mathf.Clamp(currentX, X_Angle_Min, X_Angle_Max);
    }

    private void LateUpdate()
    {

        Vector3 dir = new Vector3(0, 1.7f, -distance);
        Quaternion rotation = Quaternion.Euler(0, currentX, 0);
        Quaternion rotationPivot = Quaternion.Euler(currentY, currentX, 0);
        cameraTransform.position = Personnage.position + rotation * dir;
        cameraTransform.rotation = Quaternion.Euler(9.5f, currentX, 0);
        followTransform.transform.localRotation = rotationPivot;

        

        

        /*//Clamp the Up/Down rotation
        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }*/







        if (mouvementWilliam.enMouvement) Personnage.localRotation = rotation;
            
            


         RaycastHit hitInfo;
        Vector3 originLine = mouvementWilliam.transform.position + Vector3.up * 2;
        if (Physics.Linecast(originLine, cameraTransform.position, out hitInfo))
        {

            cameraTransform.position = new Vector3(hitInfo.point.x, cameraTransform.position.y, hitInfo.point.z);

        }
    }
}
