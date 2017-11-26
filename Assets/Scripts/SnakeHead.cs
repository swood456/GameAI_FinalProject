using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 v = transform.position;
        v.x += 1.0f;
        transform.position = v;
        //transform.position += 50;
	}
}
