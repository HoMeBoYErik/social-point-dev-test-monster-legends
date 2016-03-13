using UnityEngine;
using System.Collections;

public class MonsterRowFactory  {

    [Inject("MonsterRowPrefab")]
    public GameObject monsterRowPrefab { get; set; }

    public MonsterRowFactory() { }
    
    public GameObject Create()
    {
        return GameObject.Instantiate(monsterRowPrefab) as GameObject;
    }


}
