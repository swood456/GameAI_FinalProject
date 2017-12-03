using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public enum ControllType { Player, SimplAI, QLearn};
    public ControllType controll_tye = ControllType.Player;

    private char[,] map;
    SnakeController controller;
    Vector2 snake_dir;
    Vector2 snake_pos;
    Vector2 snake_old_pos;

    GameObject apple;

    bool snake_dead = false;
    int score = 0;

    int map_w;
    int map_h;

    public float desiredFrameRate;
    float curTime = 0.0f;

    // Use this for initialization
    void Start () {
        // set up the world
        MakeWorld makeWorld = GetComponent<MakeWorld>();
        map = makeWorld.createMap();
        map_w = makeWorld.map_w;
        map_h = makeWorld.map_h;
        apple = makeWorld.real_apple;

        controller = FindObjectOfType<SnakeController>();
        snake_dir.x = 1;
        snake_dir.y = 0;

        for(int i = 0; i < map_w + 2; ++i)
        {
            for(int j = 0; j < map_h + 2; ++j)
            {
                if(map[i,j] == 'h')
                {
                    snake_pos.x = i;
                    snake_pos.y = j;
                }
            }
        }

        // this drops framerate significantly (so that each movement takes 1 frame and is still easy to speed up)
        // this is probably not the best solution for making the game both run slow and fast
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 4;
    }
	
	// Update is called once per frame
	void Update () {
        curTime += Time.deltaTime;
        if(curTime >= 1.0f/desiredFrameRate)
        {
            update_world();
            curTime = 0.0f;
        }
        
    }

    void update_world()
    {
        if (snake_dead)
            return;
        // move the snake
        if (controller.is_turn_left())
        {
            // todo: make it turn left
            snake_dir = Vector2.up;
        }
        else if (controller.is_turn_right())
        {
            // todo: make it turn right
            snake_dir = Vector2.down;
        }
        else
        {
            //map[(int)snake_pos.x, (int)snake_pos.y] = 'e';
            

        }
        snake_old_pos = snake_pos;
        snake_pos += snake_dir;

        check_snake_pos();

        // update board
        map[(int)snake_old_pos.x, (int)snake_old_pos.y] = 'e';
        map[(int)snake_pos.x, (int)snake_pos.y] = 'h';

        controller.transform.position = snake_pos;
    }

    void check_snake_pos()
    {
        char current_space = map[(int)snake_pos.x, (int)snake_pos.y];
        if (current_space == 'e')
            return;

        // hit apple, move it on map
        if(current_space == 'a')
        {
            score++;

            int a_i = Random.Range(1, map_w);
            int a_j = Random.Range(1, map_h);

            while (map[a_i, a_j] != 'e')
            {
                a_i = Random.Range(1, map_w);
                a_j = Random.Range(1, map_h);
            }

            map[a_i, a_j] = 'a';

            // move apple in world
            apple.transform.position = new Vector3(a_i, a_j);
        }
        else if(current_space == 'w')
        {
            // do something real here other than just setting it to be dead
            snake_dead = true;
        }
    }
}
