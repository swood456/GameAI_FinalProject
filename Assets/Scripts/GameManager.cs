using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct State{
	public Vector2 s_pos;
	public Vector2 a_pos;
	public List<Vector2> t;
}

public struct State2
{
    public bool is_left_valid;
    public bool is_fwd_valid;
    public bool is_right_valid;
    public int x_diff;
    public int y_diff;
}

public struct State3
{
    public bool is_left_valid;
    public bool is_fwd_valid;
    public bool is_right_valid;
    public int angle_degrees;
}

public struct State4
{
    public bool is_left_valid;
    public bool is_fwd_valid;
    public bool is_right_valid;
    public int angle_rounded;
}

public struct Key{
    public State4 s;
    //public State2 s;
    //public State s;
	//public int a;
}

public class GameManager : MonoBehaviour {

    Vector2[] directions = {Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    public enum ControllType { Player, SimpleAI, QLearn, RandomAI};
    public ControllType controll_tye = ControllType.Player;

    private char[,] map;
    SnakeController controller;
    public GameObject tailObj;
    //Vector2 snake_dir;
    int snake_dir_index = 1;
    Vector2 snake_pos;
    Vector2 snake_old_pos;
    private int num_moves_this_game = 0;

    public bool replay_on_death = true;

    GameObject apple;

    bool snake_dead = false;
    int score = 0;

    public bool print_debug = false;

    int map_w;
    int map_h;

    public float desiredFrameRate;
    public int updates_per_frame = 1;
    float curTime = 0.0f;

    List<Vector2> tailPos;
    List<GameObject> tailObjs;
    int starting_length = 2;

	//Q learning vars
	Dictionary<Key, float> QValueStore;
	public float rho;
	public float alpha;
	public float gamma;
	private bool learn = false;
	private Key last;
	private float oldQ = 0;
	private float last_score = 0;

    float prev_dist;

    // Use this for initialization
    void Start () {
        // set up the world
        MakeWorld makeWorld = GetComponent<MakeWorld>();
        map = makeWorld.createMap(true);
        map_w = makeWorld.map_w;
        map_h = makeWorld.map_h;
        apple = makeWorld.real_apple;

        controller = FindObjectOfType<SnakeController>();
        //snake_dir.x = 1;
        //snake_dir.y = 0;

		QValueStore = new Dictionary<Key, float>();

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
        prev_dist = Vector2.Distance(snake_pos, (Vector2)apple.transform.position);
        // set up tail
        Vector2 tail_vec2 = controller.transform.position - (Vector3)directions[snake_dir_index];//(Vector3)snake_dir;
        tailPos = new List<Vector2>();
        tailPos.Add(tail_vec2);

        GameObject tail = Instantiate(tailObj, tail_vec2, Quaternion.identity);
        tailObjs = new List<GameObject>();
        tailObjs.Add(tail);

        if(map[(int)tailPos[0].x, (int)tailPos[0].y] == 'a')
        {
            // find a new random place for apple
            map[(int)tailPos[0].x, (int)tailPos[0].y] = 't';

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
        map[(int)tailPos[0].x, (int)tailPos[0].y] = 't';

        //print("starting index: " + snake_dir_index);

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
            for(int i = 0; i < updates_per_frame; ++i)
                update_world();
            curTime = 0.0f;
        }
        //print (snake_pos);

        if (Input.GetKeyDown(KeyCode.Z))
            print("num things in dict: " + QValueStore.Count);
    }
    bool addTailPiece = false;
    Vector2 oldAppleLoc;

    void update_world()
    {
		if (snake_dead) {
			last_score = 0;

            // DEATH
            /*
            Key t_key;
            //t_key.s = getState();
            //t_key.s = getState2();
            t_key.s = getState3();
            //t_key.a = snake_dir_index;
            float n_Q = getQ(t_key);
            float reward =  -1;
            if (learn)
            {
                float newQ = (1 - alpha) * oldQ + alpha * (reward + gamma * n_Q);
                setQ(last, newQ);
            }
            */
            learn = false;
            
            if(!replay_on_death)
			    return;
            else
            {
                // set the board back up, but don't initialize new things
                MakeWorld makeWorld = GetComponent<MakeWorld>();
                map = makeWorld.createMap(false);
                map_w = makeWorld.map_w;
                map_h = makeWorld.map_h;
                apple = makeWorld.real_apple;

                for (int i = 0; i < map_w + 2; ++i)
                {
                    for (int j = 0; j < map_h + 2; ++j)
                    {
                        if (map[i, j] == 'h')
                        {
                            snake_pos.x = i;
                            snake_pos.y = j;
                        }
                    }
                }

                snake_dir_index = 1;

                // set up tail
                Vector2 tail_vec2 = controller.transform.position - (Vector3)directions[snake_dir_index];
                tailPos = new List<Vector2>();
                tailPos.Add(tail_vec2);

                // destroy all old tail objects
                foreach (GameObject g in tailObjs)
                {
                    Destroy(g);
                }

                GameObject tail = Instantiate(tailObj, tail_vec2, Quaternion.identity);                
                tailObjs = new List<GameObject>();
                tailObjs.Add(tail);

                map[(int)tailPos[0].x, (int)tailPos[0].y] = 't';

                print("score: "+ score);
                //print("num moves: " + num_moves_this_game);
                num_moves_this_game = 0;
                score = 0;
                snake_dead = false;
            }
		}
        

        if(controll_tye == ControllType.SimpleAI)
        {
            simple_AI_controlls();
        }
        else if(controll_tye == ControllType.RandomAI)
        {
            List<int> actions = new List<int>();

            //all possible actions from this state

            actions.Add((snake_dir_index + directions.Length - 1) % directions.Length);
            actions.Add(snake_dir_index);
            actions.Add((snake_dir_index + 1) % directions.Length);

            int rng_index = Random.Range(0, actions.Count);
            snake_dir_index = actions[rng_index];

        }
        else if (controll_tye == ControllType.Player)
        {
            if (isTurnLeft())
            {
                turn_left();
            }
            else if (isTurnRight())
            {
                turn_right();
            }
        }
        else if(controll_tye == ControllType.QLearn)
        {
			Key t_key;
            //t_key.s = getState();
            t_key.s = getState4();
            //t_key.a = getBestActionQ();
			float n_Q = getQ (t_key);
            float reward = score - last_score;
            if (learn) {
				float newQ = (1 - alpha) * oldQ + alpha * (reward + gamma * n_Q);
				setQ (last, newQ);
			} else {
				learn = true;
			}
			Q_learning_controlls();
        }
        

        snake_old_pos = snake_pos;
        //snake_pos += snake_dir;
        snake_pos += directions[snake_dir_index];

        check_snake_pos();

        // update board
        map[(int)snake_pos.x, (int)snake_pos.y] = 'h';

        // update tail
        map[(int)tailPos[tailPos.Count - 1].x, (int)tailPos[tailPos.Count - 1].y] = 'e';
        for (int i = tailPos.Count - 1; i >=1; --i)
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
            tailPos.Add(oldAppleLoc);

            GameObject tail = Instantiate(tailObj, oldAppleLoc, Quaternion.identity);
            tailObjs.Add(tail);

            map[(int)oldAppleLoc.x, (int)oldAppleLoc.y] = 't';
        }

        

        controller.transform.position = snake_pos;
        num_moves_this_game++;

        // debug: print all indexes with h
        //int count = 0;
        //for (int j = 0; j < map_h; ++j)
        //{
        //    for (int i = 0; i < map_w; ++i)
        //    {
        //        if (map[i, j] == 't')
        //        {
        //            print("t at : " + i + " , " + j);
        //            count++;
        //        }
        //    }
        //}
        //print("count: " + count);
    }

    void turn_left()
    {
        snake_dir_index = (snake_dir_index + directions.Length - 1) % directions.Length;
    }

    void turn_right()
    {
        snake_dir_index = (snake_dir_index + 1) % directions.Length;
    }

    void simple_AI_controlls()
    {
        // simple AI controller

        // list of possible directions, we will pick one at random
        List<int> pos_dirs = new List<int>();

        // if we can move left, add it as possible
        Vector2 left_pos = snake_pos + directions[(snake_dir_index + directions.Length - 1) % directions.Length];
        if (map[(int)left_pos.x, (int)left_pos.y] == 'a')
        {
            snake_dir_index = (snake_dir_index + directions.Length - 1) % directions.Length;
            return;
        }
        if (map[(int)left_pos.x, (int)left_pos.y] == 'e')
        {
            pos_dirs.Add((snake_dir_index + directions.Length - 1) % directions.Length);
        }


        Vector2 fwd_pos = snake_pos + directions[snake_dir_index];
        if (map[(int)fwd_pos.x, (int)fwd_pos.y] == 'a')
        {
            return;
        }
        if (map[(int)fwd_pos.x, (int)fwd_pos.y] == 'e')
        {
            pos_dirs.Add(snake_dir_index);
        }

        Vector2 right_pos = snake_pos + directions[(snake_dir_index + 1) % directions.Length];
        if (map[(int)right_pos.x, (int)right_pos.y] == 'a')
        {
            snake_dir_index = (snake_dir_index + 1) % directions.Length;
            return;
        }
        if (map[(int)right_pos.x, (int)right_pos.y] == 'e')
        {
            pos_dirs.Add((snake_dir_index + 1) % directions.Length);
        }
        
        if (pos_dirs.Count > 0)
        {
            int rng_index = Random.Range(0, pos_dirs.Count);
            snake_dir_index = pos_dirs[rng_index];
        }
    }


    bool isTurnLeft()
    {
        if (controll_tye == ControllType.Player)
        {
            if (snake_dir_index == 0 && Input.GetKey(KeyCode.LeftArrow) ||
                snake_dir_index == 1 && Input.GetKey(KeyCode.UpArrow) ||
                snake_dir_index == 2 && Input.GetKey(KeyCode.RightArrow) ||
                snake_dir_index == 3 && Input.GetKey(KeyCode.DownArrow)
                )
            {
                return true;
            }
        }
        return false;
    }

    bool isTurnRight()
    {
        if (controll_tye == ControllType.Player)
        {
            if (snake_dir_index == 0 && Input.GetKey(KeyCode.RightArrow) ||
                snake_dir_index == 1 && Input.GetKey(KeyCode.DownArrow) ||
                snake_dir_index == 2 && Input.GetKey(KeyCode.LeftArrow) ||
                snake_dir_index == 3 && Input.GetKey(KeyCode.UpArrow)
                )
            {
                return true;
            }
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
        
        else
        {
            // do something real here other than just setting it to be dead
            snake_dead = true;
        }
    }

	State getState(){
		State s = new State();
		s.s_pos = snake_pos;
		s.a_pos = apple.transform.position;
		s.t = tailPos;
		return s;
	}

    
    State2 getState2()
    {
        State2 s = new State2();
        Vector2 left_pos = snake_pos + directions[(snake_dir_index + directions.Length - 1) % directions.Length];
        s.is_left_valid = map[(int)left_pos.x, (int)left_pos.y] == 'e' || map[(int)left_pos.x, (int)left_pos.y] == 'a';

        Vector2 fwd_pos = snake_pos + directions[snake_dir_index];
        try
        {
            s.is_fwd_valid = map[(int)fwd_pos.x, (int)fwd_pos.y] == 'e' || map[(int)fwd_pos.x, (int)fwd_pos.y] == 'a';
        }
        catch
        {
            s.is_fwd_valid = false;
        }
        

        Vector2 right_pos = snake_pos + directions[(snake_dir_index + 1) % directions.Length];
        s.is_right_valid = map[(int)right_pos.x, (int)right_pos.y] == 'e' || map[(int)right_pos.x, (int)right_pos.y] == 'a';

        s.x_diff = (int)snake_pos.x - (int)apple.transform.position.x;
        s.y_diff = (int)snake_pos.y - (int)apple.transform.position.y;
        return s;
    }

    State3 getState3()
    {
        State3 s = new State3();

        Vector2 left_pos = snake_pos + directions[(snake_dir_index + directions.Length - 1) % directions.Length];
        s.is_left_valid = map[(int)left_pos.x, (int)left_pos.y] == 'e' || map[(int)left_pos.x, (int)left_pos.y] == 'a';

        Vector2 fwd_pos = snake_pos + directions[snake_dir_index];
        try
        {
            s.is_fwd_valid = map[(int)fwd_pos.x, (int)fwd_pos.y] == 'e' || map[(int)fwd_pos.x, (int)fwd_pos.y] == 'a';
        }
        catch
        {
            s.is_fwd_valid = false;
        }


        Vector2 right_pos = snake_pos + directions[(snake_dir_index + 1) % directions.Length];
        s.is_right_valid = map[(int)right_pos.x, (int)right_pos.y] == 'e' || map[(int)right_pos.x, (int)right_pos.y] == 'a';

        Vector2 appleVec = (Vector2)apple.transform.position - snake_pos;
        s.angle_degrees = (int)Vector2.Angle(directions[snake_dir_index], appleVec.normalized);

        if (Vector2.Dot(appleVec, directions[(snake_dir_index + directions.Length - 1) % directions.Length]) < 0)
            s.angle_degrees *= -1;

        return s;
    }

    State4 getState4()
    {
        State4 s = new State4();

        Vector2 left_pos = snake_pos + directions[(snake_dir_index + directions.Length - 1) % directions.Length];
        s.is_left_valid = map[(int)left_pos.x, (int)left_pos.y] == 'e' || map[(int)left_pos.x, (int)left_pos.y] == 'a';

        Vector2 fwd_pos = snake_pos + directions[snake_dir_index];
        try
        {
            s.is_fwd_valid = map[(int)fwd_pos.x, (int)fwd_pos.y] == 'e' || map[(int)fwd_pos.x, (int)fwd_pos.y] == 'a';
        }
        catch
        {
            s.is_fwd_valid = false;
        }


        Vector2 right_pos = snake_pos + directions[(snake_dir_index + 1) % directions.Length];
        s.is_right_valid = map[(int)right_pos.x, (int)right_pos.y] == 'e' || map[(int)right_pos.x, (int)right_pos.y] == 'a';

        Vector2 appleVec = (Vector2)apple.transform.position - snake_pos;
        s.angle_rounded = (int)(Vector2.Angle(directions[snake_dir_index], appleVec.normalized) / 15);

        if (Vector2.Dot(appleVec, directions[(snake_dir_index + directions.Length - 1) % directions.Length]) < 0)
        {
            s.angle_rounded *= -1;
            //s.angle_rounded = -1;
        }
        else
        {
            //s.angle_rounded = 1;
        }

        

        //s.angle_rounded /= 15;
        

        return s;
    }


    float getQ(Key k){
		float QVal;
		bool suc = QValueStore.TryGetValue(k, out QVal);
		if (suc) {
            //print("found the key");
			return QVal;
		}
		QValueStore.Add (k, 0.0f);
        //print("making new key");
		return 0.0f;
	}

	void setQ(Key k, float val){
		QValueStore.Remove(k);
		QValueStore.Add (k, val);
	}

	int getBestActionQ(){
        State4 s = getState4 ();
        //State2 s = getState2();
        List<int> actions = new List<int>();

        //all possible actions from this state
		actions.Add((snake_dir_index + directions.Length - 1) % directions.Length);
		actions.Add(snake_dir_index);
		actions.Add((snake_dir_index + 1) % directions.Length);

		int best = 0;
		float bestQval = float.NegativeInfinity;

		foreach (int act in actions) {
			Key k;
			k.s = s;
			//k.a = act;
			float newVal = getQ (k);
			if (newVal > bestQval) {
				best = act;
				bestQval = newVal;
			} else if (newVal == bestQval) {
				int r = Random.Range(0,2);
				if (r > 0) {
					best = act;
					bestQval = newVal;
				}
			}
		}
		Key k_prime;
		k_prime.s = s;
		//k_prime.a = best;
		last = k_prime;
        if (print_debug)
            print("best q: " + bestQval);
		return best;
    }


	void Q_learning_controlls()
	{
        /*
         * OLD VERSION - allowed snake to turn 2 times, which isn't allowed
         * 
		// list of possible directions, we will pick one at random
		List<int> actions = new List<int>();

		//all possible actions from this state
		actions.Add((snake_dir_index + directions.Length - 1) % directions.Length);
		actions.Add(snake_dir_index);
		actions.Add((snake_dir_index + 1) % directions.Length);

		if (Random.Range(0,1) < rho) {
			int rng_index = Random.Range(0, actions.Count);
			snake_dir_index = actions[rng_index];
		}

	    int a = getBestActionQ ();
		snake_dir_index = a;
		Key tmp;
		tmp.s = getState();
		tmp.a = a;
		oldQ = getQ (tmp);
        */

        // I don't know if this is a valid fix or not
        List<int> actions = new List<int>();

        //all possible actions from this state
        
        actions.Add((snake_dir_index + directions.Length - 1) % directions.Length);
        actions.Add(snake_dir_index);
        actions.Add((snake_dir_index + 1) % directions.Length);
        

        // only allow us to use ones that are not death
        /*
        Vector2 left_pos = snake_pos + directions[(snake_dir_index + directions.Length - 1) % directions.Length];
        if (map[(int)left_pos.x, (int)left_pos.y] == 'e' || map[(int)left_pos.x, (int)left_pos.y] == 'a')
            actions.Add((snake_dir_index + directions.Length - 1) % directions.Length);

        Vector2 fwd_pos = snake_pos + directions[snake_dir_index];
        if (map[(int)fwd_pos.x, (int)fwd_pos.y] == 'e' || map[(int)fwd_pos.x, (int)fwd_pos.y] == 'a')
            actions.Add(snake_dir_index);

        Vector2 right_pos = snake_pos + directions[(snake_dir_index + 1) % directions.Length];
        if (map[(int)right_pos.x, (int)right_pos.y] == 'e' || map[(int)right_pos.x, (int)right_pos.y] == 'a')
            actions.Add((snake_dir_index + 1) % directions.Length);
        */
        if (Random.Range(0.0f, 1.0f) < rho)
        {
            //print("doing random");
            if(actions.Count != 0)
            {
                int rng_index = Random.Range(0, actions.Count);
                snake_dir_index = actions[rng_index];
            }
            
            /*
            Key tmp;
            tmp.s = getState();
            //tmp.s = getState2();
            tmp.a = rng_index;
            oldQ = getQ(tmp);
            */
        }
        else
        {
            int a = getBestActionQ();
            snake_dir_index = a;
            Key tmp;
            tmp.s = getState4();
            //tmp.s = getState();
            //tmp.a = a;
            oldQ = getQ(tmp);
        }

        
        
    }

}
