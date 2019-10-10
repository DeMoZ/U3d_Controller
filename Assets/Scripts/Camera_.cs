using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
//[RequireComponent(typeof(CharacterController))]
//[RequireComponent(typeof(Rigidbody))]
public class Camera_ : MonoBehaviour
{
    //public string clampObjName = "";
    [Tooltip("How Camera will work")]
    public CameraState cameraState;

    public enum CameraState
    {
        TestMethod,
        ThirdPerson,
        Future_ThirdPersonTargetFocused,  // can be used to focusing player moving aroun target object. For mele fight.
        FirstPerson,
        Future_FirstPersonTargetFocused,  // can be used to focusing player moving aroun target object. For mele fight.
        Future_Free,                // no object to follow, moving horizontaly
        Future_FreeFly,             // no object to follow, move and fly
        Future_BirdEyeThirdPerson,
        Future_BirdEyeFree,
        Off,                        // no behaviour.        
    }

    [Tooltip("Transform of the object to follow")]
    public Transform bodyTransform;
    private Transform camTransform;
    private Vector3 bodyPos;

    [Tooltip("Offcet for Third person camera. Camera will keep object not in the center of the screen")]
    [SerializeField] private Vector2 bodyOffsetThirdPerson = new Vector2(2, 2);

    [Tooltip("Offcet for First person camera. To move camera in the body eyes view")]
    [SerializeField] private Vector3 bodyOffsetFirstPerson = new Vector3(0, 0.7f, 0.4f);

    [Tooltip("Camera will start focusing on objects with tags from tagTargetList")]
    public bool FocuseTarget = false;
    [Tooltip("list of Tags for Camera focusing")]
    public List<string> tagFocusetList = new List<string>();

    [Tooltip("list of Tags for Camera Ignoring Collision")]
    public List<string> tagCollisionIgnoreList = new List<string>();
    //private CharacterController controller;


    //private 
    public Vector3 joyLook = new Vector3();
    private Vector3 joyMove = new Vector3();

    [Header("Zoom properties")]
    [Tooltip("Start zoom distance (0 is closest (object eyes))")]
    [Range(0, -30)]
    [SerializeField] private float zoomDefault = -4f;
    private float zoomFromController;

    [Tooltip("Zoom Speed / distance camera fly with zooming")]
    [Range(0, 10)]
    [SerializeField] private float zoomStep = 1;
    private float zoomClamped;
    private float zoomPrevious;
    private float zoomNow;

    private float joyZoom = 0;


    [Tooltip("Minimum close to body value")]
    [SerializeField] private float zoomMin = -0.3f;
    [Tooltip("Maximum far from body value")]
    [SerializeField] private float zoomMax = -20f;


    [Space]
    [Range(0, 89.9f)]
    [Tooltip("Maximum angle look Down")]
    [SerializeField] private float xClampMax = 89.9f;
    [Range(-89.9f, 0)]
    [Tooltip("Minimum angle look Up")]
    [SerializeField] private float xClampMin = -89.9f;

    [Tooltip("Zoom from/to object with Lerp")]
    [SerializeField] private bool zoomSmooth = true;

    //private Camera cam;
    private void Start()
    {
        camTransform = transform;
        // controller = GetComponent<CharacterController>();
        // cam = GetComponent<Camera>();
        ReSet();

        if (joyMove == Vector3.zero) { } // заглушка  
    }

    public void ReSet()
    {
        zoomFromController = zoomDefault;
        zoomPrevious = zoomDefault;
    }

    void Update()
    {
        if (cameraState != CameraState.Off)
            TakeValuesFromControl();
    }
    void FixedUpdate()
    {
        switch (cameraState)
        {
            case CameraState.TestMethod:
                //CameraMoveTestMethod();
                //CameraMoveTestMethod2();
                CameraMoveTestMethod3();
                break;
            case CameraState.ThirdPerson:
                CameraMoveUpdateThirdPerson();
                //CameraMoveUpdateThirdPerson2();
                break;
            case CameraState.Future_ThirdPersonTargetFocused:
                // focused
                //CameraMoveUpdateThirdPersonFocused();
                CameraMoveUpdateThirdPersonFocused();
                break;
            case CameraState.FirstPerson:
                CameraMoveUpdateFirstPerson();
                break;
            case CameraState.Future_FirstPersonTargetFocused:
                // focused
                break;
            case CameraState.Future_Free:            // no object to follow, moving horizontaly

                break;
            case CameraState.Future_FreeFly:         // no object to follow, move and fly

                break;
            case CameraState.Future_BirdEyeThirdPerson:

                break;
            case CameraState.Future_BirdEyeFree:

                break;
            default:                                   // CameraState.Off,      
                break;
        }
    }
    private void TakeValuesFromControl()
    {
        switch (cameraState)
        {
            case CameraState.TestMethod:
                TouchMouse();
                ZoomButtonsMouseWheel();
                break;
            case CameraState.ThirdPerson:
                TouchMouse();
                ZoomButtonsMouseWheel();
                break;
            case CameraState.Future_ThirdPersonTargetFocused:
                TouchMouse();
                ZoomButtonsMouseWheel();
                break;
            case CameraState.FirstPerson:
                TouchMouse();
                break;
            case CameraState.Future_Free:            // no object to follow, moving horizontaly
                TouchMouse();
                JoyKeyboard();
                UpDownButtonsMouseWheel();
                break;
            case CameraState.Future_FreeFly:         // no object to follow, move and fly
                TouchMouse();
                JoyKeyboard(false);
                UpDownButtonsMouseWheel();
                break;
            case CameraState.Future_BirdEyeThirdPerson:
                UpDownButtonsMouseWheel();
                break;
            case CameraState.Future_BirdEyeFree:
                UpDownButtonsMouseWheel();
                JoyKeyboard();
                break;
            default:                                   // CameraState.Off,      
                break;
        }

        // > Change camera state logic is here

        // <
    }

    void TouchMouse()
    {
        // Get looking joy
        joyLook = ControllerHub.GetControllerTouch("TouchArea").GetAxis();

        // unmark this code and region "Smooth Rotation" if you use your Touch controller and want smooth rotation 
        // ControllerHub.GetControllerTouch controller has own implementation for smoothing
        //SmoothedTouchMouseValues();
    }

    #region "Smooth Rotation"
    /*
    [Tooltip("camera rotation with Lerp")]
    [SerializeField] private bool smoothRotation = true;
    private Vector3 joyLookPrevious = new Vector2();

    /// <summary>
    /// If you want to add smooth to rotation
    /// </summary>
    void SmoothedTouchMouseValues()
    {
        if (smoothRotation && (joyLook.x == 0 || joyLook.z == 0))
        {
            joyLook = Vector3.Lerp(joyLookPrevious, joyLook, Time.deltaTime * 10);
            joyLookPrevious = joyLook;
        }
    }
    */
    #endregion

    void ZoomButtonsMouseWheel()
    {
        // Get zoom. Need to also get zoom from on screen controls
        joyZoom = ControllerHub.GetControllerTouch("TouchArea").GetMouseWheel();

        joyZoom = BtnZooming();

        if (joyZoom > 0) joyZoom += zoomStep;
        if (joyZoom < 0) joyZoom -= zoomStep;
        zoomFromController += joyZoom;

        zoomFromController = Mathf.Clamp(zoomFromController, zoomMax, zoomMin);
    }
    void JoyKeyboard(bool yLocked = true)
    {
        if (yLocked)
            joyMove = ControllerHub.GetControllerJoy("JoyArea").GetAxisRelated(camTransform);
        else
            joyMove = ControllerHub.GetControllerJoy("JoyArea").GetAxisRelated(camTransform, parallelToGroundLine: false);
    }

    void UpDownButtonsMouseWheel()
    {

    }

    void CameraMoveUpdateThirdPerson()
    {
        if (bodyTransform == null) return;
        bodyPos = bodyTransform.position;

        float xEuler = Clamp_xEuler(camTransform.eulerAngles.x + joyLook.y);
        xEulerPublic = xEuler;
        Quaternion rotation = Quaternion.Euler(xEuler, camTransform.eulerAngles.y + joyLook.x, 0f);

        zoomNow = ZoomNow();

        Vector3 position = rotation * new Vector3(bodyOffsetThirdPerson.x, bodyOffsetThirdPerson.y, zoomNow) + bodyPos;

        position = Clamp_Colision(bodyPos, position);

        camTransform.rotation = rotation;
        camTransform.position = position;
    }
    void CameraMoveUpdateThirdPerson2()
    {
        if (bodyTransform == null) return;
        bodyPos = bodyTransform.position;

        Vector3 _pointOnCamForward = camTransform.position + camTransform.forward.normalized * 10;
        gismoTPCyellow = _pointOnCamForward;

        Vector3 _dirTargToBody = (bodyPos - _pointOnCamForward);
        Vector3 _right = Vector3.Cross(_dirTargToBody, Vector3.up).normalized;
        Vector3 _up = Vector3.Cross(_dirTargToBody, _right).normalized;

        Vector3 _offsetedPos = bodyPos + _right * bodyOffsetThirdPerson.x + _up * bodyOffsetThirdPerson.y + _dirTargToBody.normalized * -ZoomNow();

        Debug.DrawRay(bodyPos, _right, Color.red);
        Debug.DrawRay(bodyPos, _up, Color.yellow);

        // Vector3 _targetGismoPoint = _bodyPosOffsettedY + (_dirBodyToTarget * (_disBodyToTarg + 2));// targettingDistance;

        Quaternion rotation = Quaternion.LookRotation(_pointOnCamForward - _offsetedPos);


        camTransform.rotation = rotation;
        camTransform.position = _offsetedPos;

        // gismoTPCyellow = _targetGismoPoint;
        gismoTPCred = _offsetedPos;
    }

    Transform targetObjectTransform;
    public float angle = 0;
    public float angleY = 0;// { get; set; }
    public float angleX = 0;
    public float angleBody = 0;
    public bool readX = true;
    public bool readY = true;

    public float xEulerPublic = 0;

    public Vector3 _directProjectionToTarg;
    public Vector2 joyEmulator;

    public Vector2 screenProjecPoint;
    public Vector2 screenTargetPoint;
    public Vector2 screenBodyPoint;

    // камера не глючит, если цель возле тела, но дрожит, если цель далеко. Вращается по градусам
    void CameraMoveTestMethod()
    {
        if (bodyTransform == null) return;
        bodyPos = bodyTransform.position;

        bool _joyLookTouched = ControllerHub.GetControllerTouch("TouchArea").GetControllerInTouch();
        bool _autoLook = false;
        if (!_joyLookTouched)//поиск цели, или слежение за целью
        {
            Vector3 _bodyPosOffsettedY = new Vector3(0, bodyOffsetThirdPerson.y, 0) + bodyPos;

            float _distance = targettingDistance + Vector3.Distance(camTransform.position, _bodyPosOffsettedY); // distance to the target to enable focusing 
            Ray _ray = new Ray(camTransform.position, camTransform.forward);

            RaycastHit _hit;
            if (Physics.Raycast(_ray, out _hit, _distance))
            {
                if ((Vector3.Distance(bodyPos, _hit.point) <= targettingDistance) && (TagInFocuseList(_hit.transform.gameObject.tag)))
                {
                    targetObjectTransform = _hit.transform;
                }
            }

            //If target persist and distance to target is ok then ok, else remove target
            if ((targetObjectTransform != null) && (Vector3.Distance(bodyPos, targetObjectTransform.position) > targettingDistance))
            {
                targetObjectTransform = null;
            }

            if (targetObjectTransform != null)
            {

                _autoLook = true;
                //Vector3 _dirBodToTarget = (targetObjectTransform.position - bodyPos);
                Vector3 _dirCamToTarget = (targetObjectTransform.position - camTransform.position);
                Vector3 _dirCamToTargetN = _dirCamToTarget.normalized;
                Vector3 _dirCamForw = camTransform.forward;

                float _distCamToTarg = Vector3.Distance(camTransform.position, targetObjectTransform.position);

                angle = Vector3.Angle(_dirCamForw, _dirCamToTarget.normalized);

                Vector3 pointProjection = camTransform.position + camTransform.forward * _distCamToTarg;
                gismoCamPos = pointProjection;
                gismoBodyOfsetPos = _bodyPosOffsettedY;
                float _distCamToProjection = Vector3.Distance(camTransform.position, pointProjection);

                _directProjectionToTarg = (targetObjectTransform.position - pointProjection);


                Debug.DrawRay(pointProjection, _directProjectionToTarg, Color.blue);
                Debug.DrawRay(camTransform.position, _dirCamToTargetN * (_distCamToTarg + 2), Color.red);
                Debug.DrawRay(camTransform.position, camTransform.forward * (_distCamToTarg + 2), Color.yellow);

                screenProjecPoint = gameObject.GetComponent<Camera>().WorldToScreenPoint(pointProjection);
                screenTargetPoint = gameObject.GetComponent<Camera>().WorldToScreenPoint(targetObjectTransform.position);
                screenBodyPoint = gameObject.GetComponent<Camera>().WorldToScreenPoint(_bodyPosOffsettedY);

                Vector3 directionScrenProfectToTarget = (screenTargetPoint - screenProjecPoint).normalized;

                angleBody = -Vector2.Angle(screenProjecPoint - screenBodyPoint, Vector2.right);

                joyEmulator.y = -directionScrenProfectToTarget.y;
                joyEmulator.x = directionScrenProfectToTarget.x;

                // поворот вектора на градус (крестовина ординат меняется от + к х)
                //if (_distCamToTarg < _distCamToProjection)
                joyEmulator.x = RotateVector(joyEmulator, angleBody).x; // применяю по х поворот

                // для опытов ограничиваю один из векторов
                if (!readX)
                    joyEmulator.x = 0;
                if (!readY)
                    joyEmulator.y = 0;



                float xEuler = Clamp_xEuler(camTransform.eulerAngles.x + joyEmulator.y);
                xEulerPublic = xEuler;

                Quaternion rotation = Quaternion.Euler(xEuler, camTransform.eulerAngles.y + joyEmulator.x, 0f);
                zoomNow = ZoomNow();

                Vector3 position = rotation * new Vector3(bodyOffsetThirdPerson.x, bodyOffsetThirdPerson.y, zoomNow) + bodyPos;
                position = Clamp_Colision(bodyPos, position);

                camTransform.rotation = rotation;
                camTransform.position = position;

            }
        }
        else
        {
            targetObjectTransform = null;
        }

        if (!_autoLook)
        {
            CameraMoveUpdateThirdPerson();
            gismoCamPos = camTransform.position;

        }
    }

    // TPCF по вектору от цели ищем положение с офсетом на теле
    void CameraMoveTestMethod2()
    {
        if (bodyTransform == null) return;
        bodyPos = bodyTransform.position;

        bool _joyLookTouched = ControllerHub.GetControllerTouch("TouchArea").GetControllerInTouch();
        bool _autoLook = false;

        if (!_joyLookTouched)//поиск цели, или слежение за целью
        {
            Vector3 _bodyPosOffsettedY = new Vector3(0, bodyOffsetThirdPerson.y, 0) + bodyPos;

            float _distance = targettingDistance + Vector3.Distance(camTransform.position, _bodyPosOffsettedY); // distance to the target to enable focusing 
            Ray _ray = new Ray(camTransform.position, camTransform.forward);

            RaycastHit _hit;
            if (Physics.Raycast(_ray, out _hit, _distance))
            {
                if ((Vector3.Distance(bodyPos, _hit.point) <= targettingDistance) && (TagInFocuseList(_hit.transform.gameObject.tag)))
                {
                    targetObjectTransform = _hit.transform;
                }
            }

            //If target persist and distance to target is ok then ok, else remove target
            if ((targetObjectTransform != null) && (Vector3.Distance(bodyPos, targetObjectTransform.position) > targettingDistance))
            {
                targetObjectTransform = null;
            }

            if (targetObjectTransform != null)
            {
                /**************************************
                 * Vector3 side = Vector3.Cross (direction, Vector3.up)
                Vector3 up = Vector3.Cross (direction, side) 
                 *************************************/
                _autoLook = true;

                float _disBodyToTarg = Vector3.Distance(_bodyPosOffsettedY, targetObjectTransform.position);

                //Vector3 _dirCamToTarget = (targetObjectTransform.position - camTransform.position).normalized;
                Vector3 _dirBodyToTarget = (targetObjectTransform.position - _bodyPosOffsettedY).normalized;

                Vector3 _dirTargToBody = (bodyPos - targetObjectTransform.position);
                Vector3 _right = Vector3.Cross(_dirTargToBody, Vector3.up).normalized;
                // Vector3 _up = Vector3.Cross(_dirBodyToTarget, _right).normalized;
                Vector3 _up = Vector3.Cross(-_dirTargToBody, _right).normalized;
                Vector3 _offsetedPos = bodyPos + _right * bodyOffsetThirdPerson.x + _up * bodyOffsetThirdPerson.y + _dirTargToBody.normalized * -ZoomNow();

                Debug.DrawRay(bodyPos, _right, Color.red);
                Debug.DrawRay(bodyPos, _up, Color.yellow);

                //Vector3 _ToTargetForC

                Vector3 _targetGismoPoint = _bodyPosOffsettedY + (_dirBodyToTarget * (_disBodyToTarg + 2));// targettingDistance;

                Quaternion rotation = Quaternion.LookRotation(targetObjectTransform.position - _offsetedPos);


                camTransform.rotation = rotation;
                camTransform.position = _offsetedPos;

                gismoTPCyellow = _targetGismoPoint;
                gismoTPCred = _offsetedPos;
            }
        }
        else
        {
            targetObjectTransform = null;
        }

        if (!_autoLook)
        {
            CameraMoveUpdateThirdPerson();
        }
    }
    public float distanceBodyToTarget;
    public float xEuler;
    void CameraMoveTestMethod3()
    {
        if (bodyTransform == null) return;
        bodyPos = bodyTransform.position;

        bool _joyLookTouched = ControllerHub.GetControllerTouch("TouchArea").GetControllerInTouch();
        bool _autoLook = false;

        if (!_joyLookTouched)//поиск цели, или слежение за целью
        {
            Vector3 _bodyPosOffsettedY = new Vector3(0, bodyOffsetThirdPerson.y, 0) + bodyPos;

            float _distance = targettingDistance + Vector3.Distance(camTransform.position, _bodyPosOffsettedY); // distance to the target to enable focusing 
            Ray _ray = new Ray(camTransform.position, camTransform.forward);

            RaycastHit _hit;
            if (Physics.Raycast(_ray, out _hit, _distance))
            {
                if ((Vector3.Distance(bodyPos, _hit.point) <= targettingDistance) && (TagInFocuseList(_hit.transform.gameObject.tag)))
                {
                    targetObjectTransform = _hit.transform;
                }
            }

            //If target persist and distance to target is ok then ok, else remove target
            if ((targetObjectTransform != null) && (Vector3.Distance(bodyPos, targetObjectTransform.position) > targettingDistance))
            {
                targetObjectTransform = null;
            }

            if (targetObjectTransform != null)
            {
                /**************************************
                 * Vector3 side = Vector3.Cross (direction, Vector3.up)
                Vector3 up = Vector3.Cross (direction, side) 
                 *************************************/
                _autoLook = true;

                Vector3 _dirBodyToTarg = targetObjectTransform.position - bodyPos;
                Vector3 _targetPosVirtualPoint = targetObjectTransform.position;

                distanceBodyToTarget = Vector3.Distance(bodyPos, targetObjectTransform.position);
                // если обьект ближе этого расстояния, то точку слежения отдаляю на разницу
                float _closeLimit = 4;
                if (distanceBodyToTarget < _closeLimit)//2.83
                {
                    _targetPosVirtualPoint = _targetPosVirtualPoint + (_dirBodyToTarg).normalized * (_closeLimit - distanceBodyToTarget);
                }
                gismoTPCred = _targetPosVirtualPoint;
                // получаю желаемый поворот в эулерах (градусах)
                Vector3 _rotationEuler = Quaternion.LookRotation(_targetPosVirtualPoint - camTransform.position).eulerAngles;

                // получаю градусы между текущим вектором форвард камеры и вектором на таргет

                // получаю направление довотора вектора ???

                // клами поворота 
                // float xEuler = Clamp_xEuler(camTransform.eulerAngles.x + joyLook.y);
                xEuler = Clamp_xEuler(_rotationEuler.x);



                // доворачиваю вектор форвард камеры на этот градус, в направлении на таргет 
                // Quaternion rotation = Quaternion.Euler(xEuler, camTransform.eulerAngles.y + joyLook.x, 0f);
                Quaternion rotation = Quaternion.Euler(xEuler, _rotationEuler.y, 0f);
                //Quaternion rotation = Quaternion.Euler(xEuler, camTransform.eulerAngles.y, 0f);
                //Quaternion rotation = Quaternion.Euler(camTransform.eulerAngles.x, _rotationEuler.y, 0f);     // нет отслеживания по вертикали
                //Quaternion rotation = Quaternion.Euler(_rotationEuler.x, _rotationEuler.y, 0f); // нет клампа по вертикали

                zoomNow = ZoomNow();

                Vector3 position = rotation * new Vector3(bodyOffsetThirdPerson.x, bodyOffsetThirdPerson.y, zoomNow) + bodyPos;

                //position = Clamp_Colision(bodyPos, position);

                // считаю положение второй раз, если коллизии с перерасчетом на коллизии
                position = rotation * Clamp_CollisionOffset(bodyPos, position) + bodyPos;

                camTransform.rotation = rotation;
                camTransform.position = position;

            }
        }
        else
        {
            targetObjectTransform = null;
        }

        if (!_autoLook)
        {
            CameraMoveUpdateThirdPerson();
        }

    }



    private Vector2 RotateVector(Vector2 v, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float _x = v.x * Mathf.Cos(radian) - v.y * Mathf.Sin(radian);
        float _y = v.x * Mathf.Sin(radian) + v.y * Mathf.Cos(radian);
        return new Vector2(_x, _y);
    }

    Vector3 gismoCamPos { get; set; }
    Vector3 gismoBodyOfsetPos { get; set; }//float distToTarg { get; set; }

    Vector3 gismoTPCyellow { get; set; }
    Vector3 gismoTPCred { get; set; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawCube(gismoCamPos, new Vector3(0.5f, 0.5f, 0.5f));
        Gizmos.DrawSphere(gismoCamPos, 0.2f);
        Gizmos.DrawSphere(gismoBodyOfsetPos, 0.2f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gismoTPCyellow, 0.15f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(gismoTPCred, 0.3f);
    }
    //Transform targetObjectTransform;
    float targettingDistance = 10;

    void CameraMoveUpdateThirdPersonFocused()
    {
        if (bodyTransform == null) return;
        bodyPos = bodyTransform.position;

        bool _joyLookTouched = ControllerHub.GetControllerTouch("TouchArea").GetControllerInTouch();
        bool _autoLook = false;

        if (!_joyLookTouched)//поиск цели, или слежение за целью
        {
            Vector3 _bodyPosOffsetted = new Vector3(bodyOffsetThirdPerson.x, bodyOffsetThirdPerson.y, 0) + bodyPos;

            float _distance = targettingDistance + Vector3.Distance(camTransform.position, _bodyPosOffsetted); // distance to the target to enable focusing 
            Ray _ray = new Ray(camTransform.position, camTransform.forward);

            RaycastHit _hit;
            if (Physics.Raycast(_ray, out _hit, _distance))
            {
                if ((Vector3.Distance(bodyPos, _hit.point) <= targettingDistance) && (TagInFocuseList(_hit.transform.gameObject.tag)))
                {
                    targetObjectTransform = _hit.transform;
                }
            }

            //If target persist and distance to target is ok then ok, else remove target
            if ((targetObjectTransform != null) && (Vector3.Distance(bodyPos, targetObjectTransform.position) > targettingDistance))
            {
                targetObjectTransform = null;
            }

            if (targetObjectTransform != null)
            {
                _autoLook = true;

                // // // // SemiWorkedTPCF(Vector3 bodyPos)

                //Vector3 _dirBodToTarget = (targetObjectTransform.position - bodyPos);
                Vector3 _dirCamToTarget = (targetObjectTransform.position - camTransform.position);

                //Vector3 _dirRotated = Vector3.RotateTowards(camTransform.forward, _dirBodToTarget, Vector3.Angle(camTransform.forward, _dirBodToTarget), 0.01f);

                Quaternion _rotation = Quaternion.LookRotation(_dirCamToTarget, camTransform.up);
                //Quaternion _rotation = Quaternion.LookRotation(_dirBodToTarget);
                //Quaternion _rotation = Quaternion.LookRotation(_dirRotated);

                zoomNow = ZoomNow();
                Vector3 position = _rotation * new Vector3(bodyOffsetThirdPerson.x, bodyOffsetThirdPerson.y, zoomNow) + bodyPos;
                //Vector3 position = _rotation * new Vector3(xEuler, bodyOffsetThirdPerson.y, zoomNow) + bodyPos;


                //position = Clamp_Colision(bodyPos, position);
                camTransform.position = position;
                camTransform.rotation = Quaternion.LookRotation(targetObjectTransform.position - camTransform.position);
            }
        }
        else
        {
            targetObjectTransform = null;
        }

        if (!_autoLook)
        {
            CameraMoveUpdateThirdPerson();

        }

    }

    void SemiWorkedTPCF(Vector3 bodyPos)
    {
        Vector3 _dirBodToTarget = (targetObjectTransform.position - bodyPos);

        Vector3 _dirRotated = Vector3.RotateTowards(camTransform.forward, _dirBodToTarget, Vector3.Angle(camTransform.forward, _dirBodToTarget), 0.01f);

        // Quaternion _rotation = Quaternion.LookRotation(_dirBodToTarget);
        Quaternion _rotation = Quaternion.LookRotation(_dirRotated);

        zoomNow = ZoomNow();
        float xEuler = Clamp_xEuler(camTransform.eulerAngles.x + joyLook.y);

        //Vector3 position = _rotation * new Vector3(bodyOffsetThirdPerson.x, bodyOffsetThirdPerson.y, zoomNow) + bodyPos;
        Vector3 position = _rotation * new Vector3(xEuler, bodyOffsetThirdPerson.y, zoomNow) + bodyPos;
        // !!! Clamp Position is Required!!!
        position = Clamp_Colision(bodyPos, position);
        camTransform.position = position;
        camTransform.rotation = Quaternion.LookRotation(targetObjectTransform.position - camTransform.position);
    }
    void CameraMoveUpdateFirstPerson()
    {
        if (bodyTransform == null) return;
        bodyPos = bodyTransform.position;

        float xEuler = Clamp_xEuler(camTransform.eulerAngles.x + joyLook.y);

        Quaternion rotation = Quaternion.Euler(xEuler, camTransform.eulerAngles.y + joyLook.x, 0f);

        //Vector3 position = bodyPos; // добавить смещение положения по Y и Z
        Vector3 position = rotation * new Vector3(bodyOffsetFirstPerson.x, bodyOffsetFirstPerson.y, bodyOffsetFirstPerson.z) + bodyPos;

        camTransform.rotation = rotation;
        camTransform.position = position;
    }


    //public float rez;
    private float ZoomNow()
    {
        float rez;
        if (zoomSmooth)
        {
            rez = Mathf.Lerp(zoomPrevious, zoomFromController, Time.deltaTime * 10);
        }
        else
        {
            rez = zoomFromController;
        }
        zoomPrevious = rez;
        return rez;
    }
    private Vector3 Clamp_Colision(Vector3 _bodyPos, Vector3 _position)
    {
        float _distance = Vector3.Distance(_bodyPos, _position);
        Vector3 _direction = (_position - _bodyPos);
        Ray _ray = new Ray(_bodyPos, _direction);
        RaycastHit _hit;

        if (Physics.Raycast(_ray, out _hit, _distance))
        {
            if ((_hit.distance < _distance) && (!TagInIgnoreList(_hit.transform.gameObject.tag)))
            {
                //_distance = _hit.distance;
                return _hit.point;
            }
            else
            {
                Debug.DrawRay(_bodyPos, camTransform.position - _bodyPos, Color.blue);
            }
        }

        return _position;
    }

    // спроэцировать точку коллизии в параметры офсета и применить на камеру.
    private Vector3 Clamp_CollisionOffset(Vector3 _bodyPos, Vector3 _position)
    {

        float _distance = Vector3.Distance(_bodyPos, _position);
        Vector3 _direction = (_position - _bodyPos);
        Ray _ray = new Ray(_bodyPos, _direction);
        RaycastHit _hit;

        if (Physics.Raycast(_ray, out _hit, _distance))
        {
            if ((_hit.distance < _distance) && (!TagInIgnoreList(_hit.transform.gameObject.tag)))
            {
                _distance = _hit.distance;
                return (_bodyPos - _hit.point);
            }
            else
            {
                Debug.DrawRay(_bodyPos, camTransform.position - _bodyPos, Color.blue);
            }
        }

        return (_bodyPos - _position);
    }

    /// <summary>
    /// returns true if tagCollisionIgnoreList contains the string
    /// </summary>
    /// <returns></returns>
    private bool TagInIgnoreList(string tg)
    {
        if (tagCollisionIgnoreList.Contains(tg))
            return true;

        return false;
    }

    /// <summary>
    /// returns true if tagFocuseList contains the string
    /// </summary>
    /// <returns></returns>
    private bool TagInFocuseList(string tg)
    {
        if (tagFocusetList.Contains(tg))
            return true;

        return false;
    }

    //float Clamp_xEuler(float angle, out bool clamp)
    //{
    //    clamp = (angle < xClampMin || angle > xClampMax) ? true : false;
    //    return Clamp_xEuler(angle);
    //}
    float Clamp_xEuler(float angle)
    {
        // иногда проскакивает значение угла ниже 0 и выше 360. это правлю
        if (angle < 0)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        // Debug.Log("angle<360 = " + angle);
        // запретить камере быть ниже  360+minimumY (минимум должен быть отрицательным)
        // запретить камере быть выше maximumY
        if (angle > 180)
            return Mathf.Clamp(angle, 360 + xClampMin, 360);
        else
            return Mathf.Clamp(angle, xClampMin, xClampMax);
    }

    private float btnZoom = 0;
    private float BtnZooming()
    {
        float rez = joyZoom;
        if (Mathf.Abs(joyZoom) < Mathf.Abs(btnZoom))
            rez = btnZoom;

        btnZoom = 0;
        return rez;
    }

    /// <summary>
    /// Increaze zoom. 
    /// Zoom camera from the object
    /// </summary>
    public void ZoomPlus()
    {
        btnZoom = zoomStep;
    }

    /// <summary>
    /// Decreaze zoom. 
    /// Zoom camera to the object
    /// </summary>
    public void ZoomMinus()
    {
        btnZoom = -zoomStep;
    }

    /// <summary>
    /// Increaze Y position of the camera. 
    /// Fly camera up
    /// </summary>
    public void FlyUp()
    {

    }

    /// <summary>
    /// Decreaze Y position of the camera.
    /// Fly camera down
    /// </summary>
    public void FlyDown()
    {

    }
    //--------------------------------------------------------------------------------------------------------------







    //-----------------------------------base . first attemt that works

    /// <summary>
    /// Depricated
    /// </summary>        
    private void CameraPlayerRelativeMovementBase(Vector3 moveDirection, Vector3 _bodyPos)
    {
        //Transform angle in degree in quaternion form used by Unity for rotation.
        Quaternion rotation = Quaternion.Euler(camTransform.eulerAngles.x + moveDirection.y, camTransform.eulerAngles.y + moveDirection.x, 0.0f);

        //The new position is the target position + the distance vector of the camera
        //rotated at the specified angle.
        Vector3 position = rotation * new Vector3(0, 0, -2) + _bodyPos;

        //Update the rotation and position of the camera.
        camTransform.rotation = rotation;
        camTransform.position = position;
    }

    /*
    float GetDistance(Vector3 _direction, Vector3 _bodyPos, float _zoom, bool _smoothZ)
    {
        Ray ray = new Ray(_bodyPos, _direction);
        Vector3 point = ray.GetPoint(rangeOld);
        distanceOld = Vector3.Distance(point, camTransform.position);

        zoomOld += _zoom * CamZoomStepOld;
        rangesumOld = rangeOld - zoomOld;

        if (_smoothZ)
        {
            float res = Mathf.Lerp(distanceOld, rangesumOld, Time.fixedDeltaTime * camSpeedOld.z);
            return rangeOld - res;
        }
        else
            return rangeOld - rangesumOld;
    }
    */


    //[Tooltip("Camera follows body with dalay")]
    //[SerializeField] 
    //private bool latensyZ = true;
    /*
        if (latensyZ)
        {
            if ((bodyPos != bodyPosPrevious) && (ControllerHub.GetControllerJoy("JoyArea").GetAxis().y > 0))
            {
                lateTimer = Mathf.Clamp((lateTimer - Time.deltaTime), 0, 1);
                if (lateTimer > 0)
                {
                    _zoomNow = 0.3f-Vector3.Distance(bodyPos, camTransform.position);
                }
            }
            else
            {
                _zoomNow = ZoomNow();
            }

            if (bodyPos == bodyPosPrevious)
            {
                lateTimer = 1;
            }
        }
        else
        {
            _zoomNow = ZoomNow();
        }
        testdistancenow= -Vector2.Distance(new Vector2(bodyPos.x,bodyPos.z),new Vector2( camTransform.position.x, camTransform.position.y));
        */
}
