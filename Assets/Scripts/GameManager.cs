using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


    private char[,] map;

    // Use this for initialization
    void Start () {
        // set up the world
        MakeWorld makeWorld = GetComponent<MakeWorld>();
        map = makeWorld.createMap();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
