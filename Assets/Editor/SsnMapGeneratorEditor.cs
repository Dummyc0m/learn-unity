using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (SsnMapGenerator))]
public class SsnMapGeneratorEditor : Editor {
    public override void OnInspectorGUI () {
        var mapGen = (SsnMapGenerator) target;

        if (DrawDefaultInspector ()) {
            if (mapGen.AutoUpdate) {
                mapGen.GenerateMap ();
            }
        }

        if (GUILayout.Button ("Generate")) {
            mapGen.GenerateMap ();
        }
    }
}