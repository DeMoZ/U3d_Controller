using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControllerTest : MonoBehaviour
{

    //Joy
    [Header("Joy data")]
    [SerializeField] bool JoyTouched = false;

    [SerializeField] Vector2 JoyAxisRaw = new Vector2();
    [SerializeField] Vector2 JoyAxisWalkRun = new Vector2();
    [SerializeField] Vector2 JoyAxisNormalized = new Vector2();
    [SerializeField] Vector2 JoyAxisFourDirections = new Vector2();

    //Joy (1)
    [Header("Joy(1) data")]
    [SerializeField] bool Joy1Touched = false;

    [SerializeField] Vector2 Joy1AxisRaw = new Vector2();

    // touch
    [Header("Touch data")]
    [SerializeField] bool TouchTouched = false;

    [SerializeField] Vector2 TouchAxis = new Vector2();

    private void Update()
    {
        //******************** show  how to Controllers values for your code
        // Joy Values
        JoyAxisRaw = ControllerHub.GetControllerJoy("JoyArea").GetAxis(); // .GetAxis("Raw");
        JoyAxisNormalized = ControllerHub.GetControllerJoy("JoyArea").GetAxis("Normalized");
        JoyAxisWalkRun = ControllerHub.GetControllerJoy("JoyArea").GetAxis("WalkRun");
        JoyAxisFourDirections = ControllerHub.GetControllerJoy("JoyArea").GetAxis("Four");
        
        JoyTouched = ControllerHub.GetControllerJoy("JoyArea").GetControllerInTouch();

        // Joy Values
        //Joy1AxisRaw = ControllerHub.GetControllerJoy("JoyArea (1)").GetAxis(); // .GetAxis("Raw");
        //Joy1Touched = ControllerHub.GetControllerJoy("JoyArea (1)").GetControllerInTouch();
        // Touch Values
        TouchAxis = ControllerHub.GetControllerTouch("TouchArea").GetAxis();
        TouchTouched = ControllerHub.GetControllerTouch("TouchArea").GetControllerInTouch();
    }

    private void NoDebugWarning() // I am not using variables so Unity shows warnings. I Use this func to pretend of using variables
    {
        if (JoyTouched) { };
        if (JoyAxisRaw == new Vector2()) { };
        if (JoyAxisNormalized == new Vector2()) { };
        if (JoyAxisWalkRun == new Vector2()) { };
        if(JoyAxisFourDirections == new Vector2()) { };

        if (Joy1Touched) { };
        if (Joy1AxisRaw == new Vector2()) { };

        if (TouchTouched) { };
        if (TouchAxis == new Vector2()) { };
    }

    private void Start()
    {

        //Invoke("InvokedFunc1", 1);
        //Invoke("InvokedFunc2", 2);
        //Invoke("InvokedFunc3", 3);
        //Invoke("InvokedFunc4", 4);

    }
    //******************** show  how to Enable/Disable Controllers while in game
    private void InvokedFunc1()
    {
        ControllerHub.ControllerSwitcher(false);
    }

    private void InvokedFunc2()
    {
        ControllerHub.ControllerSwitcher(true);
    }
    private void InvokedFunc3()
    {
        ControllerHub.ControllerSwitcher(false, "JoyArea");
    }
    private void InvokedFunc4()
    {
        ControllerHub.ControllerSwitcher(true, "JoyArea");
    }
}
