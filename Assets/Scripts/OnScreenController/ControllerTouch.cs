using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
public class ControllerTouch :
    MonoBehaviour,
    //IPointerDownHandler, 
    //IPointerUpHandler, 
    IDragHandler, // required for IBeginDragHandler to work
    IEndDragHandler, // // issue - OnEnDrag called on pointer up instead of on end moving, so get delta value in update
    IBeginDragHandler
{
    //**********************
    [Header("Touch Values")]
    [SerializeField] private bool _inDrag;
    private PointerEventData _pointer = null;
    [SerializeField] private Vector2 _delta = Vector2.zero;

    [SerializeField] private bool _smoothTouch;
    [SerializeField] private int _frameCount = 10;
    [SerializeField] private float _touchSpeed = 10;
    /// <summary>
    /// last 20 delta values for smoothnes
    /// </summary>
    private List<Vector2> _deltaList = new List<Vector2>();
    //**********************
    [Header("Mouse Settings")]
    [Tooltip("Will start Take value from mouse if area not in touch")]
    [SerializeField] bool _emulateMouse = false;
    [Tooltip("Multiply MouseValue to get same params as with Touch")]
    [SerializeField] float _mouseSpeed = 20;

    [Header("Mouse Values (Check EmulateMouse)")]
    [SerializeField] Vector2 MouseValue = new Vector2();
    [SerializeField] float _mouseWheelValue = 0;

    //*************Check Boxes
    [Header("Reverce Touch values if required")]
    [SerializeField] bool _touchReverceX = true;
    [SerializeField] bool _touchReverceY = true;
    [SerializeField] bool MouseReverceX = false;
    [SerializeField] bool MouseReverceY = false;
    [SerializeField] bool MouseReverceWheel = false;

    //*******************

    private void Awake()
    {
        ControllerHub.AddToHub(gameObject);
        ControllerHub.AddTouchToHub(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _pointer = eventData;
    }
    // required for OnBeginDrag to work
    public void OnDrag(PointerEventData eventData) { }

    // issue - OnEnDrag called on pointer up instead of on end moving
    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");
        _pointer = null;
    }

    private void Update()
    {
        if (_pointer != null)
        {
            _inDrag = true;
            _delta = _pointer.delta;
        }
        else
        {
            _inDrag = false;
            _delta = Vector2.zero;
        }

        // smothing _delta values by finding average from last _frameCOunt frames
        if (_smoothTouch)
        {
            _deltaList.Add(_delta);

            if (_deltaList.Count > _frameCount && _deltaList.Count > 0)
                _deltaList.RemoveAt(0);

            Vector2 sum = Vector2.zero;

            for (int i = 0; i < _deltaList.Count; i++)
            {
                sum.x += _deltaList[i].x;
                sum.y += _deltaList[i].y;
            }

            _delta.x = sum.x;
            _delta.y = sum.y;
        }

        // upply touch inverse
        _delta.x = (_touchReverceX) ? -_delta.x : _delta.x;
        _delta.y = (_touchReverceY) ? -_delta.y : _delta.y;

        // upply touch speed
        _delta.x *= Time.deltaTime * _touchSpeed;
        _delta.y *= Time.deltaTime * _touchSpeed;

        if (_emulateMouse)
        {
            MouseValue.x = MouseReverceX ? -Input.GetAxis("Mouse X") * _mouseSpeed : Input.GetAxis("Mouse X") * _mouseSpeed;
            MouseValue.y = MouseReverceY ? -Input.GetAxis("Mouse Y") * _mouseSpeed : Input.GetAxis("Mouse Y") * _mouseSpeed;

            _mouseWheelValue = MouseReverceWheel ? -Input.GetAxis("Mouse ScrollWheel") : Input.GetAxis("Mouse ScrollWheel");
        }
    }

    #region "GET"
    public string GetControllerName()
    {
        return name;
    }

    public bool GetControllerInTouch()
    {
        return _inDrag;
    }
    /// <summary>
    /// /// Returns GetAxis with params Vector2(x,y)
    /// </summary>
    /// <returns></returns>
    public Vector2 GetAxis()
    {
        if (_emulateMouse && !_inDrag)
            return MouseValue;

        return _delta;
    }
    ///// <summary>
    ///// Returns GetAxis with params Vector3(x,0,y)
    ///// </summary>
    ///// <returns></returns>
    //public Vector3 GetAxisV3()
    //{
    //    if (EmulateMouse && !InTouch)
    //        return new Vector3(MouseValue.x, 0, MouseValue.y);
    //    //return MouseValue;

    //    return new Vector3( TouchValue.x,0,TouchValue.y);
    //}
    public Vector2 GetMouseAxis()
    {
        return MouseValue;
    }
    /// <summary>
    /// returns value from mouse wheel
    /// </summary>
    /// <returns></returns>
    public float GetMouseWheel()
    {
        return _mouseWheelValue;
    }
    #endregion

    /// <summary>
    /// Returns look direction vector3(X,Y,Z) acording Transform tm forward
    /// </summary>
    /// <param name="tm"></param>
    /// <returns></returns>
    public Vector3 GetAxisRelated(Transform tm)
    {
        return GetAxisRelatedCalculation(tm.forward, tm.right);
    }

    /// <summary>
    /// Returns look direction vector3(X,Y,Z) acording  forward and right vectors
    /// </summary>
    /// <param name="forward"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public Vector3 GetAxisRelated(Vector3 forward, Vector3 right)
    {
        return GetAxisRelatedCalculation(forward, right);
    }

    private Vector3 GetAxisRelatedCalculation(Vector3 forward, Vector3 right)
    {
        Vector3 camForw = new Vector3();
        Vector3 camRigh = new Vector3();
        Vector2 joy = GetAxis();

        Vector2 axisDt = new Vector2(joy.x, joy.y);//, 0);

        //axisDt.z = MouseWheel;

        // modify joy value to camera forvard
        camForw = forward;
        camForw.y = 0;
        camForw = camForw.normalized;
        camForw = camForw * axisDt.y;

        camRigh = right;
        camRigh.y = 0;
        camRigh = camRigh.normalized;
        camRigh = camRigh * axisDt.x;

        return (camForw + camRigh);
    }
}
