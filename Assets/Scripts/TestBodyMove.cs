using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class TestBodyMove : MonoBehaviour {
    public Camera _camera;
    Transform _transform;
    CharacterController cc;
    Vector3 joyMove;
	
	void Start () {
        _transform = transform;
        cc = GetComponent<CharacterController>();
	}

   
    void FixedUpdate () {
        if (_camera != null)
        {
            joyMove = ControllerHub.GetControllerJoy("JoyArea").GetAxisRelated(_camera.transform);
            Debug.DrawRay(_transform.position, joyMove*3,Color.red);
        }
        else
        {
            Debug.LogError("no camera attached to script " + this); 
        }

        //cc.Move(new Vector3(joyMove.x,0,joyMove.y)*10*Time.fixedDeltaTime); 
        cc.Move(joyMove * 10 * Time.fixedDeltaTime);
    }
}
