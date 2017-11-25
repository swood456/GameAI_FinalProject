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

    private char[,] map;

    // Use this for initialization
    void Start () {
        createMap();
	}
	
	void createMap()
    {
        // for now map is a 2d array of chars, might be better to be something simpler?
        map = new char[map_w + 2, map_h + 2];

        for(int i = 0; i < map_w + 2; ++i)
        {
            map[i, 0] = 'w';
            map[i, map_h + 1] = 'w';
        }

        for(int j = 0; j < map_h; ++j)
        {
            map[0, j + 1] = 'w';
            map[map_w + 1, j + 1] = 'w';
        }

        int s_i = Random.Range(0, map_w - 1);
        int s_j = Random.Range(0, map_h - 1);

        map[s_i + 1, s_j + 1] = 'h';
    }
}
