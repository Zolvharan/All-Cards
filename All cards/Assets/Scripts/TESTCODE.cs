using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTCODE : MonoBehaviour
{
    public LevelGenerator generator;
    public PlayerControl player;

    // Start is called before the first frame update
    void Start()
    {
        //generator.GenerateLevel(15, 10);
        //player.StartTurn();
        SaveData.LoadData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
