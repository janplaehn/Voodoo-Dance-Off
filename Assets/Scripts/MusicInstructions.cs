﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicInstructions : State {
    [SerializeField]public DanceMovePair[] timingPairs;
    public Sprite voidSprite;
    public Sprite[] instructionImageArray;
    public Image instruction;
    private SpriteRenderer timingP1;
    private SpriteRenderer timingP2;
    public GameObject timingObjectP1;
    public GameObject timingObjectP2;
    public GameObject timingObjectP1Small;
    public GameObject timingObjectP2Small;
    public Image nextInstruction;
    private float accumulatedTime = 0f;
    public AudioSource musicSource;
    private ScoringSystem scoringSystem;
    private InputCheck inputCheck;
    private DanceMove lastMove;
    private int lastPairIndex = 0;
    private bool started = true;
    public bool stateFinished = false;
    private bool moveRatedP1 = false;
    private bool moveRatedP2 = false;
    private bool intro = true;
    public float introTime;
    public static float tempo = 1;
    public Sprite timingSprite;
    [Range(0f, 4f)] public float minTimingSpriteSize = 1.34f;
    [Range(1f, 4f)] public float maxTimingSpriteSize = 4f;

    private float errorMargin = 0.5f;
    public bool isPaused = false;
    private float scaleTiming = 1;

   

	// Use this for initialization
	public override void OnStart () {
        timingP1 = timingObjectP1.GetComponent<SpriteRenderer>();
        timingP2 = timingObjectP2.GetComponent<SpriteRenderer>();
        lastMove = timingPairs[lastPairIndex].firstValue;
        nextInstruction.sprite = lastMove.moveInstruction;
        inputCheck = GameObject.FindGameObjectWithTag("GameController").GetComponent<InputCheck>();
        scoringSystem = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScoringSystem>();
        if (timingPairs.Length == 0)
        {
            started = false;
        }
        instruction.sprite = timingPairs[lastPairIndex].firstValue.moveInstruction;
        timingP1.sprite = instruction.sprite;
        timingP1.gameObject.transform.localScale = new Vector3(scaleTiming, scaleTiming, 1);
        timingP2.gameObject.transform.localScale = timingP1.gameObject.transform.localScale;
        timingP2.sprite = timingP1.sprite;
        if (lastPairIndex + 1 <= timingPairs.Length - 1)
        {
            nextInstruction.sprite = timingPairs[lastPairIndex + 1].firstValue.moveInstruction;
        }
        else
        {
            nextInstruction.sprite = voidSprite;
        }
        timingP1.gameObject.transform.position = new Vector3(-3, -3.75f, 1);
        timingP2.gameObject.transform.position = new Vector3(3, -3.75f, 1);
        timingObjectP1Small.transform.position = timingP1.gameObject.transform.position;
        timingObjectP2Small.transform.position = timingP2.gameObject.transform.position;
        if (musicSource != null) musicSource.Play();
        if (introTime > 0) started = false;
    }

    public override bool OnUpdate ()
    {
        if (started == true && isPaused == false)
        {
            timingP1.enabled = true;
            timingP2.enabled = true;
            accumulatedTime += Time.deltaTime;
            if (accumulatedTime <= GetTiming())
            {
                AnimateTiming(GetTiming(), accumulatedTime);
                CheckTiming(accumulatedTime);
            }
            else
            {
                if (moveRatedP1 == false) scoringSystem.AddFirstPlayerScore(inputCheck.CheckScore(lastMove, 1, InputCheck.Players.PlayerOne), inputCheck.GetMaxScore(lastMove),false);
                if (moveRatedP2 == false) scoringSystem.AddSecondPlayerScore(inputCheck.CheckScore(lastMove, 1, InputCheck.Players.PlayerTwo), inputCheck.GetMaxScore(lastMove),false);
                lastPairIndex++;
                if (timingPairs.Length <= lastPairIndex)
                {
                    started = false;
                    stateFinished = true;
                    instruction.sprite = voidSprite;
                    nextInstruction.sprite = voidSprite;
                    lastPairIndex = 0;
                    accumulatedTime = 0f;
                    return false;
                }

                moveRatedP1 = false;
                moveRatedP2 = false;
                accumulatedTime = 0f;
                instruction.sprite = timingPairs[lastPairIndex].firstValue.moveInstruction;
                
                if (lastPairIndex + 1 <= timingPairs.Length - 1)
                {
                    nextInstruction.sprite = timingPairs[lastPairIndex + 1].firstValue.moveInstruction;
                }
                else
                {
                    nextInstruction.sprite = voidSprite;
                }
                timingP1.sprite = voidSprite;
                timingP2.sprite = voidSprite;
                //timingObjectP1Small.GetComponent<SpriteRenderer>().sprite = voidSprite;
                //timingObjectP2Small.GetComponent<SpriteRenderer>().sprite = voidSprite;
                timingP1.gameObject.transform.localScale = new Vector3(scaleTiming, scaleTiming, 1);
                timingP1.color = new Color(1f, 1f, 1f, 0.25f);
                timingP2.gameObject.transform.localScale = timingP1.gameObject.transform.localScale;
                timingP2.color = timingP1.color;
                timingP1.sprite = instruction.sprite;
                timingP2.sprite = timingP1.sprite;
                timingObjectP1Small.GetComponent<SpriteRenderer>().sprite = timingP1.sprite;
                timingObjectP2Small.GetComponent<SpriteRenderer>().sprite = timingP2.sprite;
                lastMove = timingPairs[lastPairIndex].firstValue;

            }
        }
        else if (intro == true)
        {
            timingP1.sprite = voidSprite;
            timingP2.sprite = voidSprite;
            accumulatedTime += Time.deltaTime;
            if (accumulatedTime >= introTime)
            {
                timingP1.enabled = true;
                timingP2.enabled = true;
                timingP1.sprite = instruction.sprite;
                timingP2.sprite = instruction.sprite;
                timingObjectP1Small.GetComponent<SpriteRenderer>().sprite = timingP1.sprite;
                timingObjectP2Small.GetComponent<SpriteRenderer>().sprite = timingP2.sprite;
                timingP1.gameObject.transform.localScale = new Vector3(scaleTiming, scaleTiming, 1);
                timingP1.color = new Color(1f, 1f, 1f, 0.25f);
                timingP2.gameObject.transform.localScale = new Vector3(scaleTiming, scaleTiming, 1);
                timingP2.color = new Color(1f, 1f, 1f, 0.25f);
                started = true;
                intro = false;
                accumulatedTime = 0f;
            }
        }

        return true;
    }

    public override void OnEnd()
    {
        timingP1.sprite = voidSprite;
        timingP2.sprite = voidSprite;
        timingObjectP1Small.GetComponent<SpriteRenderer>().sprite = voidSprite;
        timingObjectP2Small.GetComponent<SpriteRenderer>().sprite = voidSprite;

    }

    public void SetMusic(AudioClip givenMusic, DanceMovePair[] givenPairs)
    {
        musicSource.clip = givenMusic;
        musicSource.Play();
        timingPairs = givenPairs;
        started = true;
    }

    void AnimateTiming (float perfectTime, float accumulatedTime)
    {
        timingP1.gameObject.transform.localScale = new Vector3(1.8f + (3 * (perfectTime - accumulatedTime)) / perfectTime, 1.8f + (3 * (perfectTime - accumulatedTime)) / perfectTime, 0);
        //timingP1.color = new Color(1f, 1f, 1f, 0.25f * accumulatedTime);
        if ((1 + (2 * (perfectTime - accumulatedTime)) / perfectTime) <= 1f) {
            timingP1.sprite = voidSprite;
            timingP2.sprite = voidSprite;
        }
        timingP2.gameObject.transform.localScale = timingP1.gameObject.transform.localScale;
        timingP2.color = timingP1.color;
    }

    void CheckTiming (float accTime)
    {
        if (inputCheck.CheckLimbs(lastMove, InputCheck.Players.PlayerOne) == 1 && moveRatedP1 == false)
        {
            if (accTime > GetTiming() - errorMargin && accTime < GetTiming() + errorMargin)
            {
                scoringSystem.AddFirstPlayerScore(inputCheck.CheckScore(lastMove, 2, InputCheck.Players.PlayerOne), inputCheck.GetMaxScore(lastMove), false);

            }
            else
            {
                scoringSystem.AddFirstPlayerScore(inputCheck.CheckScore(lastMove, 1, InputCheck.Players.PlayerOne), inputCheck.GetMaxScore(lastMove), true);
            }
            moveRatedP1 = true;
        }
        if (inputCheck.CheckLimbs(lastMove, InputCheck.Players.PlayerTwo) == 1 && moveRatedP2 == false)
        {
            if (accTime > GetTiming() - errorMargin && accTime < GetTiming() + errorMargin)
            {
                scoringSystem.AddSecondPlayerScore(inputCheck.CheckScore(lastMove, 2, InputCheck.Players.PlayerTwo), inputCheck.GetMaxScore(lastMove), false);

            }
            else
            {
                scoringSystem.AddSecondPlayerScore(inputCheck.CheckScore(lastMove, 1, InputCheck.Players.PlayerTwo), inputCheck.GetMaxScore(lastMove), true);
            }
            moveRatedP2 = true;
        }
        if (accTime >= GetTiming()) timingP1.sprite = voidSprite;
    }

    float GetTiming()
    {
        return timingPairs[lastPairIndex].secondValue * tempo;
    }
}