using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithMe : MonoBehaviour
{
    [SerializeField] private Transform withWho;

    // Update is called once per frame
    void Update()
    {
        Debug.Log(withWho.eulerAngles);
        transform.eulerAngles = new Vector3 (0,withWho.eulerAngles.y,0);
    }
}
