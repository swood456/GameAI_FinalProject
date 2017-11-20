using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeWorld : MonoBehaviour {

    [SerializeField]
    private int map_w = 10;

    [SerializeField]
    private int map_h = 10;

    [SerializeField]
    private GameObject Wall;

    [SerializeField]
    private int start_length;

    [SerializeField]
    private GameObject SnakeHead;

    // Use this for initialization
    void Start () {
        createMap();	
	}
	
	void createMap()
    {

    }
}
