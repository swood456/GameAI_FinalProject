using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public enum ControllType { Player, SimpleAI, QLearn};
    public ControllType controll_tye = ControllType.Player;

    private char[,] map;
    SnakeController controller;
    public GameObject tailObj;
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

    List<Vector2> tailPos;
    List<GameObject> tailObjs;
    int starting_length = 2;

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

        // set up tail
        Vector2 tail_vec2 = controller.transform.position - (Vector3)snake_dir;
        tailPos = new List<Vector2>();
        tailPos.Add(tail_vec2);

        GameObject tail = Instantiate(tailObj, tail_vec2, Quaternion.identity);
        tailObjs = new List<GameObject>();
        tailObjs.Add(tail);

        map[(int)tailPos[0].x, (int)tailPos[0].y] = 't';


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
    bool addTailPiece = false;
    Vector2 oldAppleLoc;

    void update_world()
    {
        if (snake_dead)
            return;
        // move the snake
        //if (controller.is_turn_left()) // now we do it in our code
        if(isTurnLeft())
        {
            // todo: make it turn left
            //snake_dir = Vector2.up;

            // gotta be a better way but brains off team
            if(snake_dir == Vector2.up)
            {
                snake_dir = Vector2.left;
            }
            else if (snake_dir == Vector2.right)
            {
                snake_dir = Vector2.up;
            }
            else if (snake_dir == Vector2.down)
            {
                snake_dir = Vector2.right;
            }
            else if (snake_dir == Vector2.left)
            {
                snake_dir = Vector2.down;
            }
        }
        //else if (controller.is_turn_right())
        else if(isTurnRight())
        {
            // todo: make it turn right
            //snake_dir = Vector2.down;

            if (snake_dir == Vector2.up)
            {
                snake_dir = Vector2.right;
            }
            else if (snake_dir == Vector2.right)
            {
                snake_dir = Vector2.down;
            }
            else if (snake_dir == Vector2.down)
            {
                snake_dir = Vector2.left;
            }
            else if (snake_dir == Vector2.left)
            {
                snake_dir = Vector2.up;
            }
        }
        else
        {
            //map[(int)snake_pos.x, (int)snake_pos.y] = 'e';
            // move forward
        }
        snake_old_pos = snake_pos;
        snake_pos += snake_dir;

        check_snake_pos();

        // update board
        map[(int)snake_pos.x, (int)snake_pos.y] = 'h';
        //map[(int)snake_old_pos.x, (int)snake_old_pos.y] = 'e';

        // update tail
        
        //for (int i = 1; i < tailPos.Count; ++i)
        for(int i = tailPos.Count - 1; i >=1; --i)
        {
            tailPos[i] = tailPos[i-1];
            tailObjs[i].transform.position = tailObjs[i-1].transform.position;
            map[(int)tailPos[i].x, (int)tailPos[i].y] = 't';
        }

        tailPos[0] = snake_old_pos;
        tailObjs[0].transform.position = snake_old_pos;
        map[(int)tailPos[0].x, (int)tailPos[0].y] = 't';

        if (addTailPiece)
        {
            //Vector2 tail_vec2 = controller.transform.position - (Vector3)snake_dir;
            //tailPos = new List<Vector2>();
            tailPos.Add(oldAppleLoc);

            GameObject tail = Instantiate(tailObj, oldAppleLoc, Quaternion.identity);
            tailObjs.Add(tail);

            map[(int)oldAppleLoc.x, (int)oldAppleLoc.y] = 't';
        }

        map[(int)tailPos[tailPos.Count-1].x, (int)tailPos[tailPos.Count-1].y] = 'e';

        controller.transform.position = snake_pos;

    }

    bool isTurnLeft()
    {
        if(controll_tye == ControllType.Player)
        {
            // there HAS to be a good way to do this but I don't want to think
            if( snake_dir == Vector2.right && Input.GetKey(KeyCode.UpArrow) ||
                snake_dir == Vector2.down && Input.GetKey(KeyCode.RightArrow) ||
                snake_dir == Vector2.left && Input.GetKey(KeyCode.DownArrow) ||
                snake_dir == Vector2.up && Input.GetKey(KeyCode.LeftArrow))
            {
                return true;
            }
        }
        else if (controll_tye == ControllType.SimpleAI)
        {

        }
        if (controll_tye == ControllType.QLearn)
        {
            // todo: make Q learning work
        }

        return false;
    }

    bool isTurnRight()
    {
        if (controll_tye == ControllType.Player)
        {
            // there HAS to be a good way to do this but I don't want to think
            if (snake_dir == Vector2.right && Input.GetKey(KeyCode.DownArrow) ||
                snake_dir == Vector2.down && Input.GetKey(KeyCode.LeftArrow) ||
                snake_dir == Vector2.left && Input.GetKey(KeyCode.UpArrow) ||
                snake_dir == Vector2.up && Input.GetKey(KeyCode.RightArrow))
            {
                return true;
            }
        }
        else if (controll_tye == ControllType.SimpleAI)
        {

        }
        if (controll_tye == ControllType.QLearn)
        {
            // todo: make Q learning work
        }

        return false;
    }

    void check_snake_pos()
    {
        addTailPiece = false;
        char current_space = map[(int)snake_pos.x, (int)snake_pos.y];
        if (current_space == 'e')
            return;

        // hit apple, move it on map
        if(current_space == 'a')
        {
            score++;
            addTailPiece = true;
            oldAppleLoc = tailPos[tailPos.Count - 1];

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
        //else if(current_space == 'w')
        else
        {
            // do something real here other than just setting it to be dead
            snake_dead = true;
            print("DEAD!");
        }
    }
}
