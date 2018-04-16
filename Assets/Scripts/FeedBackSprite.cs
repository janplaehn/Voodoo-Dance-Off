﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FeedBackSprite : MonoBehaviour {
    SpriteRenderer spriteRenderer;
    private Vector2 direction = Vector2.zero;
    public float speed = 2f;
    public float lifeTime = 1f;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        direction.x = Random.Range(-1f, 1f);
        direction.y = Random.Range(0.1f, -1f);
        direction.Normalize();
        Destroy(gameObject, lifeTime);
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += new Vector3(direction.x * speed * Time.deltaTime, direction.y * speed * Time.deltaTime, 0);
	}

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}
