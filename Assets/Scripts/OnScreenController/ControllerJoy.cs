using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControllerJoy : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    Image ControllerArea;
    [Header("Joy Settings")]
    [Tooltip("Add image on the inspector first, then drug here")]
    //[SerializeField] Image JoyImage;
    [SerializeField] Image JoyImage;
    [Tooltip("Add image on the inspector first, then drug here")]
    [SerializeField] Image StickImage;

    [Tooltip("Joy will Not move Center to first touch position")]
    [SerializeField] bool DefaultPositionStatic = false;
    // [Tooltip("Joy will Wollow touch to avoid move far out the joy")]
    // [SerializeField] bool FollowTouch = false;

    Transform ControllerImageT;
    Transform JoyImageT;
    Transform StickImageT;

    Vector2 touchPosStart = new Vector2();
    Vector2 touchPosCurrent = new Vector2();

    Vector2 defaultJoyImageLocalPos = new Vector2();
    Vector2 defaultJoyImageGlobalPos = new Vector2();

    float joyCircleDiametr = 0;
    float joyCircleRadius = 0;

    //***********************Data for public
    [Header("Joy Values")]
    [SerializeField] bool InTouch = false;
    [SerializeField] Vector2 JoyValueRaw = new Vector2();
    [SerializeField] Vector2 JoyValueWalkRun = new Vector2();
    [SerializeField] Vector2 JoyValueNormalised = new Vector2();
    [Tooltip("Joy will return only certain Vectors - (0,0); (1,0); (0,1); (0,-1); (-1,0)")]
    [SerializeField] Vector2 JoyValueFourDirections = new Vector2();
    //*************************Keyboard
    [Header("Keyboard Settings")]
    [Tooltip("Will start Take value from keyboard if area not in touch")]
    [SerializeField] bool EmulateKeyboard = false;

    [Header("Keyboard Values (Check EmulateKeyboard)")]
    [SerializeField] Vector2 KeyboardValue = new Vector2();
    [SerializeField] Vector2 KeyboardValueRaw = new Vector2();

    public enum ControllerState { }

    string Name = "";
    //***********************************************
    private void NoDebugWarning()
    {
        if (InTouch) { };
        if (JoyValueRaw == new Vector2()) { };
        if (JoyValueNormalised == new Vector2()) { };
        if (JoyValueWalkRun == new Vector2()) { };
        if (JoyValueFourDirections == new Vector2()) { };
    }
    private void OnEnable()
    {
        CleanTouch(false);
    }
    private void OnDisable()
    {
        CleanTouch(false);
    }

    private void Awake()
    {

        Name = name;
        ControllerHub.AddToHub(gameObject);
        ControllerHub.AddJoyToHub(this);

        ControllerArea = gameObject.GetComponent<Image>();

        IfNotJoySetup();

        ControllerImageT = transform;
        JoyImageT = JoyImage.transform;
        StickImageT = StickImage.transform;

        JoyImageT.transform.SetParent(ControllerImageT);
        StickImage.transform.SetParent(ControllerImageT);

        defaultJoyImageGlobalPos = JoyImageT.position;
        defaultJoyImageLocalPos = JoyImageT.localPosition;

        //joyCircleDiametr = JoyAreaT.localToWorldMatrix[0] * JoyAreaT.GetComponent<RectTransform>().rect.height;
        joyCircleDiametr = JoyImage.rectTransform.sizeDelta.x;
        joyCircleRadius = joyCircleDiametr / 2;
    }

    void IfNotJoySetup()
    {
        if (JoyImage == null)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(transform);
            JoyImage = go.AddComponent<Image>();
            JoyImage.GetComponent<Image>().color = new Color32(35, 85, 35, 140);

            go.transform.localPosition = Vector2.zero;
            go.name = "JoyArea";

            Vector2 sizeD = ControllerArea.rectTransform.sizeDelta;
            JoyImage.rectTransform.sizeDelta = new Vector2(sizeD.x / 3, sizeD.x / 3);

        }
        if (StickImage == null)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(transform);
            StickImage = go.AddComponent<Image>();
            StickImage.color = new Color32(35, 85, 145, 170);

            go.transform.localPosition = Vector2.zero;
            go.name = "StickArea";

            Vector2 sizeD = ControllerArea.rectTransform.sizeDelta;
            StickImage.rectTransform.sizeDelta = new Vector2(sizeD.x / 9, sizeD.x / 9);
        }
    }

    private void Update()
    {
        if (EmulateKeyboard)
        {// Keyboard  value magnitude always 1
            KeyboardValue.x = Input.GetAxis("Horizontal");
            KeyboardValue.y = Input.GetAxis("Vertical");
            float distance = KeyboardValue.magnitude;
            KeyboardValue = (distance > 1) ? KeyboardValue.normalized : KeyboardValue;

            KeyboardValueRaw.x = Input.GetAxisRaw("Horizontal");
            KeyboardValueRaw.y = Input.GetAxisRaw("Vertical");
            distance = KeyboardValueRaw.magnitude;
            KeyboardValueRaw = (distance > 1) ? KeyboardValueRaw.normalized : KeyboardValueRaw;
        }
    }

    public void CleanTouch(bool touched = false)
    {
        InTouch = touched;

        JoyValueRaw = new Vector2();
        JoyValueWalkRun = new Vector2();
        JoyValueNormalised = new Vector2();
        JoyValueFourDirections = new Vector2();

        if (!touched)
        {
            touchPosCurrent = new Vector2();
            JoyImageT.localPosition = defaultJoyImageLocalPos;
            StickImageT.localPosition = defaultJoyImageLocalPos;
        }
    }

    //Vector2 FollowTouchCalculation(PointerEventData data)
    //{

    //    touchPosCurrent = data.position;
    //    Vector2 stickPos = touchPosCurrent;
    //    Vector2 direction = touchPosCurrent - touchPosStart;
    //    float distance = Mathf.Clamp(direction.magnitude, -joyCircleRadius, joyCircleRadius);

    //    return (direction.normalized * distance + touchPosStart);

    //}

    #region "Touch Events"    
    public void OnPointerDown(PointerEventData data)
    {
        if (DefaultPositionStatic)
        {
            touchPosCurrent = data.position;
            touchPosStart = defaultJoyImageGlobalPos;
            //touchPosStart = (FollowTouch) ? FollowTouchCalculation(data) : defaultJoyImageGlobalPos;

            JoyImageT.localPosition = defaultJoyImageLocalPos;

            InTouch = true;
            OnDrag(data);
        }
        else
        {
            touchPosCurrent = new Vector2();
            touchPosStart = data.position;
            //touchPosStart = (FollowTouch) ? FollowTouchCalculation(data) : data.position;

            JoyImageT.position = data.position;
            StickImageT.position = data.position;
            CleanTouch(true);
        }
    }
    public void OnPointerUp(PointerEventData data)
    {
        CleanTouch(false);
    }
    public void OnDrag(PointerEventData data)
    {
        touchPosCurrent = data.position;
        Vector2 stickPos = touchPosCurrent;
        Vector2 direction = touchPosStart - touchPosCurrent;
        //Vector2 direction = (FollowTouch) ? FollowTouchCalculation(data) - touchPosCurrent : touchPosStart - touchPosCurrent;

        float distance = Mathf.Clamp(direction.magnitude, -joyCircleRadius, joyCircleRadius);

        stickPos = -direction.normalized * distance + touchPosStart;

        StickImageT.position = stickPos;

        Vector2 joy = -1 * (touchPosStart - stickPos) / (joyCircleRadius);

        JoyValueRaw = joy;
        JoyValueNormalised = joy.normalized;

        JoyValueWalkRun = joy;
        if (joy.magnitude > 0.5f)
        {
            JoyValueWalkRun = joy.normalized;
        }

        JoyValueFourDirections = TransformToFourDirections(joy);
    }


    #endregion

    //**************GET
    #region "Get Values"
    public string GetControllerName()
    {
        return Name;
    }
    /// <summary>
    /// Returns bool value if joy pressed or not   
    /// </summary>
    public bool GetControllerInTouch()
    {
        return InTouch;
    }

    /// <summary>
    /// Returns keyboard value with magnitude less than 1 . (params Smooth, Raw)
    /// </summary>
    /// <param name="modifier"></param>
    /// <returns></returns>
    public Vector2 GetKeyboardAxis(string modifier = "Smooth")
    {
        Vector2 result = new Vector2();
        switch (modifier)
        {
            case "Raw":
                result = KeyboardValueRaw;
                break;
            case "Smooth":
            default:
                result = KeyboardValue;
                break;

        }
        return result;
    }

    Vector2 TransformToFourDirections(Vector2 value)
    {
        Vector2 result = Vector2.zero;
        if (value.SqrMagnitude() > 0)
        {
            if (Mathf.Abs(value.x) > Mathf.Abs(value.y))
                result = (value.x > 0) ? Vector2.right : Vector2.left;
            else
                result = (value.y > 0) ? Vector2.up : Vector2.down;
        }
        return result;
    }

    /// <summary>
    /// Returns Joy Value as Vector2 with mentioned modifier   
    /// </summary>
    /// <example> 
    /// Vector3 myValue = ControllerJoy.GetAxis();
    /// Vector3 myValue = ControllerJoy.GetAxis("Raw");
    /// Vector3 myValue = ControllerJoy.GetAxis("Normalized");
    /// Vector3 myValue = ControllerJoy.GetAxis("WalkRun");
    /// </example>
    public Vector2 GetAxis(string modifier = "Raw")
    {
        Vector2 result = new Vector2();
        switch (modifier)
        {
            case "Normalized":
                result = JoyValueNormalised;
                break;
            case "WalkRun":
                result = JoyValueWalkRun;
                break;
            case "Four":
                result = JoyValueFourDirections;
                break;
            case "Raw":
            default:
                result = JoyValueRaw;
                break;
        }

        if (!InTouch)
        {
            string mod = (modifier == "Normalized" || modifier == "Four") ? "Raw" : "Smooth"; // revert Modifier value acording Keyboard standart modifier
            result = GetKeyboardAxis(mod);
        }

        return result;
    }

    /// <summary>
    /// Returns Joy Value as Vector3 with mentioned modifier   
    /// </summary>
    /// <example> 
    /// Vector3 myValue = ControllerJoy.GetAxisDirectioned(camera.transform);
    /// Vector3 myValue = ControllerJoy.GetAxisDirectioned(camera.transform,"Raw");
    /// Vector3 myValue = ControllerJoy.GetAxisDirectioned(camera.transform,"Normalized",false);
    /// Vector3 myValue = ControllerJoy.GetAxisDirectioned(camera.transform,"WalkRun",false);
    /// </example>
    public Vector3 GetAxisRelated(Transform transform, string modifier = "Raw", bool parallelToGroundLine = true)
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        return GetAxisRelatedCalculation(forward,  modifier, parallelToGroundLine);
    }

    /// <summary>
    /// Returns Joy Value as Vector3 with mentioned modifier   
    /// </summary>
    /// <example> 
    /// Vector3 myValue = ControllerJoy.GetAxisDirectioned(camera.transform.forward,camera.transform.right);
    /// Vector3 myValue = ControllerJoy.GetAxisDirectioned(camera.transform.forward,camera.transform.right,"Raw");
    /// Vector3 myValue = ControllerJoy.GetAxisDirectioned(camera.transform.forward,camera.transform.right,"Normalized",false);
    /// Vector3 myValue = ControllerJoy.GetAxisDirectioned(camera.transform.forward,camera.transform.right,"WalkRun",false);
    /// </example>
    public Vector3 GetAxisRelated(Vector3 forward, Vector3 right, string modifier = "Raw", bool parallelToGroundLine = true)
    {
        return GetAxisRelatedCalculation(forward, modifier, parallelToGroundLine);
    }


    /// <summary>
    /// the calculation
    /// </summary>
    /// <param name="forwardToRelate"></param>
    /// <param name="modifier"></param>
    /// <param name="parallelToGroundLine"></param>
    /// <returns></returns>
    private Vector3 GetAxisRelatedCalculation(Vector3 forwardToRelate, string modifier = "Raw", bool parallelToGroundLine = true)
    {
        Vector2 joy = GetAxis(modifier); ;
        Vector3 joyV3 = new Vector3(joy.x, 0, joy.y);

        if (parallelToGroundLine)
            forwardToRelate = Vector3.ProjectOnPlane(forwardToRelate, Vector3.up); // the rezult is same as make Vector3.y=0; // =)

        float angle = Vector3.SignedAngle(Vector3.forward, forwardToRelate, Vector3.up);

        Vector3 joyV3modified = Quaternion.Euler(0, angle, 0) * joyV3;

        return joyV3modified;
    }
    #endregion
}
