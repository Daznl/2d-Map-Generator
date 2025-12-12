using System.Diagnostics;
using UnityEngine;

public class StopwatchUtility : MonoBehaviour
{
    private Stopwatch stopwatch = new Stopwatch();

    public void StartTimer()
    {
        stopwatch.Reset();
        stopwatch.Start();
    }

    public void StopAndLogTimer(string functionName)
    {
        stopwatch.Stop();
        UnityEngine.Debug.Log($"{functionName} took {stopwatch.ElapsedMilliseconds} ms");
    }
}