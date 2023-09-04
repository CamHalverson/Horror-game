using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wallpaper : MonoBehaviour
{

    [SerializeField]
    private Transform pos1;
    [SerializeField]
    private Transform pos2;    
    
    private bool isitFirstAnimation = true;
    private bool posOK = false;

   
    private void LateUpdate()
    {        
        if (isitFirstAnimation)
        {
            if (!posOK)
            {
                transform.SetPositionAndRotation(pos1.position, pos1.rotation);
                posOK = true;
            }
            if (transform.position.x > -72)
            {
                transform.Translate(new Vector3(-0.5f, 0, 0) * Time.deltaTime, Space.World);
            }
            else
            {
                isitFirstAnimation = false;
                posOK = false;
            }
        }

        else
        {
            if (!posOK)
            {
                transform.SetPositionAndRotation(pos2.position, pos2.rotation);
                posOK = true;
            }
            if (transform.position.z < -35)
            {
                transform.Translate(new Vector3(0, 0, 0.5f) * Time.deltaTime, Space.World);
            }
            else
            {
                isitFirstAnimation = true;
                posOK = false;
            }
        }
    }
}
