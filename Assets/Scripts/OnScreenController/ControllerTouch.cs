using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControllerTouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler//, IDragHandler
{
    [Header("Touch Values")]
    [SerializeField] bool InTouch = false; // Touch area that used to control camera rotation
    [SerializeField] Vector2 TouchValue = new Vector2();

    [Tooltip("Touch valuse will be Lerped when finger UP")]
    [SerializeField] bool TouchValueSmooth = true;
    private Vector2 TouchValuePrevious = new Vector2();

    [Header("Mouse Settings")]
    [Tooltip("Will start Take value from mouse if area not in touch")]
    [SerializeField] bool EmulateMouse = false;
    [Tooltip("Multiply MouseValue to get same params as with Touch")]
    [SerializeField] float MouseMultiply = 20;

    [Header("Mouse Values (Check EmulateMouse)")]
    [SerializeField] Vector2 MouseValue = new Vector2();
    [SerializeField] float MouseWheel = 0;

    string Name = "";
    //**********************       
    Vector2 PreJoy = new Vector2();
    PointerEventData TouchData;

    //*************Check Boxes
    [Header("Reverce Touch values if required")]
    [SerializeField] bool TouchReverceX = true;
    [SerializeField] bool TouchReverceY = true;
    [SerializeField] bool MouseReverceX = false;
    [SerializeField] bool MouseReverceY = false;
    [SerializeField] bool MouseReverceWheel = false;
    //**********************

    private void OnEnable()
    {
        ResetTouch(false);
    }
    private void OnDisable()
    {
        ResetTouch(false);
    }

    private void Awake()
    {
        Name = name;
        ControllerHub.AddToHub(gameObject);
        ControllerHub.AddTouchToHub(this);
    }

    public void ResetTouch(bool touched = false)
    {
        InTouch = touched;
        //if (!TouchValueSmooth)
        TouchValue = new Vector2();
    }

    public void OnPointerDown(PointerEventData data)
    {
        ResetTouch(true);
        TouchData = data;
        PreJoy = TouchData.position;
    }

    public void OnPointerUp(PointerEventData data)
    {
        ResetTouch(false);
    }

    //public void OnDrag(PointerEventData data)
    //{/*
    //    joy = PreJoy - data.position;
    //    PreJoy = data.position;

    //   // if (DisableMouseLook)
    //        ControllerHub.SetTouchValue(joy);
    //    */
    //}

    private void Update()
    {
        if (InTouch)
        {
            TouchValue = PreJoy - TouchData.position;
            TouchValue.x = TouchReverceX ? -TouchValue.x : TouchValue.x;
            TouchValue.y = TouchReverceY ? -TouchValue.y : TouchValue.y;

            PreJoy = TouchData.position;

            // Debug.LogFormat("x values= {0}  {1}; y values={2} , {3}", TouchValue.x, MouseValue.x, TouchValue.y, MouseValue.y);
        }
        else
        {
            if (TouchValueSmooth)
                TouchValue = Vector2.Lerp(TouchValuePrevious, Vector2.zero, Time.deltaTime * 10);
        }

        TouchValuePrevious = TouchValue;

        if (EmulateMouse)
        {
            MouseValue.x = MouseReverceX ? -Input.GetAxis("Mouse X") * MouseMultiply : Input.GetAxis("Mouse X") * MouseMultiply;
            MouseValue.y = MouseReverceY ? -Input.GetAxis("Mouse Y") * MouseMultiply : Input.GetAxis("Mouse Y") * MouseMultiply;

            MouseWheel = MouseReverceWheel ? -Input.GetAxis("Mouse ScrollWheel") : Input.GetAxis("Mouse ScrollWheel");
        }
    }

    #region "GET"
    public string GetControllerName()
    {
        return Name;
    }

    public bool GetControllerInTouch()
    {
        return InTouch;
    }
    /// <summary>
    /// /// Returns GetAxis with params Vector2(x,y)
    /// </summary>
    /// <returns></returns>
    public Vector2 GetAxis()
    {
        if (EmulateMouse && !InTouch)
            return MouseValue;

        return TouchValue;
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
        return MouseWheel;
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
