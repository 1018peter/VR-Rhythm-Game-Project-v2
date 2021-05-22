using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class BeatmapHelper : EditorWindow
{
    [MenuItem("Window/BeatmapHelper")]
    public static void ShowExample()
    {
        // Whitespaces are substituted with %20
        Application.OpenURL($"file:///{Application.dataPath}/Beatmap%20Helper/index.html");

    }


}