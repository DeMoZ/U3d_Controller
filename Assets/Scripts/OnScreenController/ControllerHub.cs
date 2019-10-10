using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ControllerHub : MonoBehaviour
{
    static List<GameObject> controllers = new List<GameObject>();
    static List<ControllerJoy> controllersJoy = new List<ControllerJoy>();
    static List<ControllerTouch> controllersTouch = new List<ControllerTouch>();
    //NotImplemented 
    //static List<ControllerButton> controllersButton = new List<ControllerButton>();


    /// <summary>
    /// SetActive to enable/disable  to a chosen controller by name or for all the list
    /// </summary>
    public static void ControllerSwitcher(bool enable, string name = "All")
    {
        if (name == "All")
        {
            foreach (GameObject go in controllers)
            {
                go.SetActive(enable);
            }
        }
        else
        {
            var linqController = controllers.Where(c => c.name == name);

            foreach (GameObject go in linqController)
            {
                go.SetActive(enable);
            }
        }
    }

    /// <summary>
    /// Add controller as Game Object to activate/deactivate
    /// </summary>
    /// <param name="go"></param>
    public static void AddToHub(GameObject go)
    {
        if (!controllers.Contains(go))
            controllers.Add(go);
    }

    /// <summary>
    /// Add controller Joy to list
    /// </summary>
    /// <param name="c"></param>
    public static void AddJoyToHub(ControllerJoy c)
    {
        if (!controllersJoy.Contains(c))
            controllersJoy.Add(c);
    }

    /// <summary>
    /// Add controller Touch to list
    /// </summary>
    /// <param name="c"></param>
    public static void AddTouchToHub(ControllerTouch c)
    {
        if (!controllersTouch.Contains(c))
            controllersTouch.Add(c);
    }

    //NotImplemented  Add controller Button to list
    //public static void AddButtonToHub(ControllerTouch c)
    //{
    //    if (!controllersButton.Contains(c))
    //        controllersButton.Add(c);
    //}
    //---

    #region "GET"

    /// <summary>
    /// Get controller Joy values
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static ControllerJoy GetControllerJoy(string name)
    {
        var linqC = controllersJoy.Where(c => c.GetControllerName() == name);
        if (linqC == null)
        {
            Debug.LogError("You are trying to get Joy Controller with name that doesn't exist : " + name);
        }
        return linqC.First();
    }

    /// <summary>
    /// Get controller Touch values
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static ControllerTouch GetControllerTouch(string name)
    {
        var linqC = controllersTouch.Where(c => c.GetControllerName() == name);
        if (linqC == null)
        {
            Debug.LogError("You are trying to get Touch Controller with name that doesn't exist : " + name);
        }
        return linqC.First();
    }

    //NotImplemented Get controller Button values
    //public static ControllerJoy GetControllerButton(string name)
    //{
    //    var linqC = controllersButton.Where(c => c.GetControllerName()  == name);
    //    if (linqC == null)
    //    {
    //        Debug.LogError("You are trying to get Button Controller with name that doesn't exist : "+name);
    //    }
    //    return linqC.First();
    //}
    //---

    #endregion
}
