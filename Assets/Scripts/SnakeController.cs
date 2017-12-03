using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour {

	public bool is_turn_left()
    {
        if (Input.GetKey(KeyCode.Q))
            return true;
        return false;
    }

    public bool is_turn_right()
    {
        if (Input.GetKey(KeyCode.E))
            return true;
        return false;
    }
}
