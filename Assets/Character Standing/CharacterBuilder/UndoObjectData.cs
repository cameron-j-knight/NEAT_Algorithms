using UnityEngine;
using System.Collections;

public class UndoObjectData{
    //Transform Info
    public Vector3 _rotation;
    public Vector3 _scale;
    public Vector3 _position;

    //SettingsInfromation
    public bool _selected;
    

    public UndoObjectData(Transform SaveObject, bool Selected)
    {

        _rotation = SaveObject.eulerAngles;
        _scale = SaveObject.localScale;
        _position = SaveObject.position;

        _selected = Selected;
    }

}
