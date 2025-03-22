using UnityEngine;

[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 pos)
    {
        x = pos.x;
        y = pos.y;
        z = pos.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public Vector3Int ToVector3Int()
    {
        return new Vector3Int((int)x, (int)y, (int)z);
    }
}

[System.Serializable]
public class SceneItem
{
    public int itemID;
    public SerializableVector3 pos;
    public int quantity;
}
