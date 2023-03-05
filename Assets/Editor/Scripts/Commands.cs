using UnityEngine;

public class Commands : MonoBehaviour
{
    [UnityEditor.MenuItem("Stuff/Stuff")]
    private static void DoStuff()
    {
        for(int i = 0; i < 50; ++i)
            Debug.Log("<><><><><><><><>" + "ABCDEGKNPSTXZ"[TotallyNotSussyServiceScript.FMRandom(0, 13, "ABCDEGKNPSTXZ")]);
    }
}