using UnityEngine;
using System.Collections;

public class CharacterPart{
    public Vector3 Scale = Vector3.one;
    public Vector3 Location = Vector3.zero; //Location is from the round point of 0,0,0;
    public string Name = "GenericPart";
    public int ID = 0;

    public CharacterPart(Vector3 scale, string name, Vector3 location)
    {
        Scale = scale;
        Name = name;
        Location = location;
    }

}
