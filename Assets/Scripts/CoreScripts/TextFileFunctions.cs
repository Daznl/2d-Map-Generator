using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TextFileFunctions : MonoBehaviour
{
    public static string GetRandomLineFromTextFile(string filePath)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(filePath);
        if (textAsset != null)
        {
            string[] lines = textAsset.text.Split('\n');
            int randomIndex = UnityEngine.Random.Range(0, lines.Length);
            return lines[randomIndex].Trim();
        }
        else
        {
            //Debug.LogError("Text file not found at: " + filePath);
            return "NotFound";
        }
    }
}
