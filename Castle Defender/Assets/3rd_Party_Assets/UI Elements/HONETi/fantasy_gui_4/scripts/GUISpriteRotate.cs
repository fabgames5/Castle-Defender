﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUISpriteRotate : MonoBehaviour {

	public float rotateSpeed = 1f;
	float rotation;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//rotation += 1f * rotateSpeed;
		gameObject.transform.Rotate (0, 0, rotateSpeed);
	}
}
