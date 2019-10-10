using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DEPRICATED. The script was for TEST purposes 
/// </summary>
public class CamPointer : MonoBehaviour
{
    public Transform targT;
    public Transform bodyT;
     Transform _transform;
    public float offsetZ = -2f;

    Vector3 dir = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        _transform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (targT == null) return;
        if (bodyT == null) return;

        dir = targT.position - _transform.position;

        //float xEuler = Clamp_xEuler(camTransform.eulerAngles.x + joyLook.y);
        Vector3 dirRotated = Vector3.RotateTowards(_transform.forward, dir, Vector3.Angle(_transform.forward, dir), 1);
        Quaternion rotation = Quaternion.LookRotation(dirRotated); // Quaternion.Euler(xEuler, camTransform.eulerAngles.y + joyLook.x, 0f);

        Vector3 position = rotation*new Vector3(0,0,offsetZ) + bodyT.position;//rotation * new Vector3(bodyOffsetThirdPerson.x, bodyOffsetThirdPerson.y, zoomNow) + bodyPos;
        
        _transform.rotation = rotation;
        _transform.position = position;
    }
}
