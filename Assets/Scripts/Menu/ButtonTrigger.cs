﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTrigger : MonoBehaviour {

    public float chargeTime;
    public GameObject button;
    public GameObject chargeBar;
    public GameObject[] buttonDisplayObjects;
    public bool doesAccelerate;
    public float accelerationSpeed = 0.97f;
    public float minChargeTime = 0.2f;

    private float timeLeft;
    private bool isCharging;
    private Animator chargeAnimator;
    private Color32 highlightColor;
    private bool isColliding;
    private float acceleratedTime;

    private void Awake() {
        highlightColor = new Color32(180, 180, 180, 255);
        timeLeft = chargeTime;
        chargeAnimator = chargeBar.GetComponent<Animator>();
        chargeAnimator.speed = 1 / chargeTime;
        if (!isColliding) {
            isCharging = false;
            chargeBar.GetComponent<AudioSource>().Stop();
            acceleratedTime = chargeTime;
            foreach (GameObject button in buttonDisplayObjects) {
                if (button.GetComponent<SpriteRenderer>()) button.GetComponent<SpriteRenderer>().color = highlightColor;
                if (button.GetComponent<Image>()) button.GetComponent<Image>().color = highlightColor;
            }
        }
        isColliding = false;
    }

    private void Start() {
        foreach (GameObject button in buttonDisplayObjects) {
            if (button.GetComponent<SpriteRenderer>()) button.GetComponent<SpriteRenderer>().color = highlightColor;
            if (button.GetComponent<Image>()) button.GetComponent<Image>().color = highlightColor;
        }
        acceleratedTime = chargeTime;
    }

    void Update () {
        if (isCharging) {
            timeLeft -= Time.deltaTime;
        }
        else {
            timeLeft = chargeTime;
        }
        if (timeLeft <= 0 && isCharging) {
            timeLeft = chargeTime;
            button.GetComponent<Button>().onClick.Invoke();
            if (acceleratedTime >= minChargeTime)
            acceleratedTime *= accelerationSpeed;
            if (doesAccelerate) {
                timeLeft = acceleratedTime;
            }
        }
        SetChargeBarAnimation();
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "hand") {
            isCharging = true;
            timeLeft = chargeTime;
            chargeBar.GetComponent<AudioSource>().Play();
            foreach (GameObject button in buttonDisplayObjects) {
                if (button.GetComponent<SpriteRenderer>()) button.GetComponent<SpriteRenderer>().color = Color.white;
                if (button.GetComponent<Image>()) button.GetComponent<Image>().color = Color.white;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.tag == "hand") {
            isColliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.tag == "hand") {
            isCharging = false;
            chargeBar.GetComponent<AudioSource>().Stop();
            foreach (GameObject button in buttonDisplayObjects) {
                if (button.GetComponent<SpriteRenderer>()) button.GetComponent<SpriteRenderer>().color = highlightColor;
                if (button.GetComponent<Image>()) button.GetComponent<Image>().color = highlightColor;
            }
        }
        acceleratedTime = chargeTime;
    }

    public void SetChargeBarAnimation() {
        if (isCharging) {
            chargeAnimator.Play("charging");
        }
        else {
            chargeAnimator.Play("idle");
        }
    }
}
