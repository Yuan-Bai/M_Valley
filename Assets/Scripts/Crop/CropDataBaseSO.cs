using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crop DataBase", menuName = "SO/Crop DataBase")]
public class CropDataBase_SO : ScriptableObject
{
    public List<CropModel> cropModelList = new();

    public CropModel GetCropModelByID(int id)
    {
        return cropModelList.Find(x => x.seedItemID == id);
    }
}
