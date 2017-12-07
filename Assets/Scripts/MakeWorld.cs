using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeWorld : MonoBehaviour {

    [SerializeField]
    public int map_w = 10;

    [SerializeField]
    public int map_h = 10;

    [SerializeField]
    private GameObject Wall;

    [SerializeField]
    public int start_length;
    // note: I am  now doing starting length in the game manager for ease

    [SerializeField]
    private GameObject SnakeHead;

    [SerializeField]
    private GameObject Apple;

    private char[,] map;
    public GameObject real_apple;

    public char[,] createMap(bool make_new_stuff)
    {
        // for now map is a 2d array of chars, might be better to be something simpler?
        map = new char[map_w + 2, map_h + 2];

        // set up center to be empty
        for(int i = 0; i < map_w; ++i)
        {
            for(int j = 0; j < map_h; ++j)
            {
                map[i + 1, j + 1] = 'e';
            }
        }

        for(int i = 0; i < map_w + 2; ++i)
        {
            map[i, 0] = 'w';
            map[i, map_h + 1] = 'w';

            if(make_new_stuff)
            {
                // place a wall block
                Instantiate(Wall, new Vector3(i, 0), Quaternion.identity);
                Instantiate(Wall, new Vector3(i, map_h + 1), Quaternion.identity);
            }
        }

        for(int j = 0; j < map_h; ++j)
        {
            map[0, j + 1] = 'w';
            map[map_w + 1, j + 1] = 'w';
            if (make_new_stuff)
            {
                Instantiate(Wall, new Vector3(0, j + 1), Quaternion.identity);
                Instantiate(Wall, new Vector3(map_w + 1, j + 1), Quaternion.identity);
            }
        }

        int s_i = Random.Range(1, map_w - 2);
        int s_j = Random.Range(1, map_h - 2);

        map[s_i + 1, s_j + 1] = 'h';
        if (make_new_stuff)
            Instantiate(SnakeHead, new Vector3(s_i + 1, s_j + 1), Quaternion.identity);
        else
        {
            SnakeController snake_head = FindObjectOfType<SnakeController>();
            snake_head.transform.position = new Vector3(s_i + 1, s_j + 1);
            snake_head.transform.rotation = Quaternion.identity;
        }

        // start position for the apple
        //  start it in the same spot as snake head then move it until free
        int a_i = s_i;
        int a_j = s_j;

        while(a_i == s_i && a_j == s_j)
        {
            a_i = Random.Range(0, map_w - 1);
            a_j = Random.Range(0, map_h - 1);
        }

        map[a_i + 1, a_j + 1] = 'a';
        if (make_new_stuff)
            real_apple = Instantiate(Apple, new Vector3(a_i + 1, a_j + 1), Quaternion.identity);
        else
        {
            real_apple.transform.position = new Vector3(a_i + 1, a_j + 1);
            real_apple.transform.rotation = Quaternion.identity;
        }


        return map;
    }
}
