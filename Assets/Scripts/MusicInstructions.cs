﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicInstructions : MonoBehaviour {
    private Pair<DanceMove, float>[] timingPairs;
    public Sprite voidSprite;
    public Sprite[] instructionImageArray;
    public Image instruction;
    public Image timing;
    //public Image timing;
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
    private bool intro;
    public float introTime;
    public Sprite timingSprite;
    [Range(1f, 4f)]  public float maxTimingImgSize = 1.5f;
    public float instructionTime;

    private float errorMargin = 0.2f;
    public bool isPaused;
    private float scaleTiming = 1;
    public enum DanceMoveEnum
    {
        LeftArmUp,
        RightArmUp,
        BothArmsUp,
        BothArmsDown,
        SplitArmsUp,
        SplitArmsDown,
        LeftArmLeftLegUp,
        RightArmRightLegUp,
    }
	// Use this for initialization
	void Start () {
        isPaused = false;
        intro = true;
        lastMove = timingPairs[0].firstValue;
        nextInstruction.sprite = instructionImageArray[lastMove.instructionImageIndex];
        inputCheck = GameObject.FindGameObjectWithTag("GameController").GetComponent<InputCheck>();
        scoringSystem = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScoringSystem>();
        if (timingPairs.Length == 0)
        {
            started = false;
        }

        instructionTime = timingPairs[lastPairIndex].secondValue + 1f;
        instruction.sprite = instructionImageArray[timingPairs[lastPairIndex].firstValue.instructionImageIndex];
        timing.sprite = timingSprite;
        timing.rectTransform.localScale = new Vector3(scaleTiming, scaleTiming, 1);
        if (lastPairIndex + 1 <= timingPairs.Length - 1)
        {
            nextInstruction.sprite = instructionImageArray[timingPairs[lastPairIndex + 1].firstValue.instructionImageIndex];
        }
        else
        {
            nextInstruction.sprite = voidSprite;
        }
        lastMove = timingPairs[lastPairIndex].firstValue;
    }

    public void OnUpdate ()
    {
        if (started == true && isPaused == false)
        {
            accumulatedTime += Time.deltaTime;
            if (accumulatedTime <= instructionTime)
            {
                animateTiming(timingPairs[lastPairIndex].secondValue, accumulatedTime);
                checkTiming(accumulatedTime);
            }
            else
            {
                if (moveRatedP1 == false) scoringSystem.AddFirstPlayerScore(inputCheck.CheckScore(lastMove, 1, InputCheck.Players.PlayerOne), inputCheck.GetMaxScore(lastMove));
                if (moveRatedP2 == false) scoringSystem.AddSecondPlayerScore(inputCheck.CheckScore(lastMove, 1, InputCheck.Players.PlayerTwo), inputCheck.GetMaxScore(lastMove));
                lastPairIndex++;
                if (timingPairs.Length <= lastPairIndex)
                {
                    started = false;
                    stateFinished = true;
                    instruction.sprite = voidSprite;
                    nextInstruction.sprite = voidSprite;
                    lastPairIndex = 0;
                    accumulatedTime = 0f;
                    
                    return;
                }

                moveRatedP1 = false;
                moveRatedP2 = false;
                accumulatedTime = 0f;
                instructionTime = timingPairs[lastPairIndex].secondValue + 1f;
                instruction.sprite = instructionImageArray[timingPairs[lastPairIndex].firstValue.instructionImageIndex];
                timing.sprite = timingSprite;
                timing.rectTransform.localScale = new Vector3(scaleTiming, scaleTiming, 1);
                if (lastPairIndex + 1 <= timingPairs.Length - 1)
                {
                    nextInstruction.sprite = instructionImageArray[timingPairs[lastPairIndex + 1].firstValue.instructionImageIndex];
                }
                else
                {
                    nextInstruction.sprite = voidSprite;
                }
                lastMove = timingPairs[lastPairIndex].firstValue;
            }
        }
        else if (intro == true)
        {
            accumulatedTime += Time.deltaTime;
            if (accumulatedTime >= introTime)
            {
                timing.enabled = true;
                started = true;
                intro = false;
                accumulatedTime = 0f;
            }
        }
    }

    public void SetMusic(AudioClip givenMusic, Pair<DanceMove, float>[] givenPairs)
    {
        musicSource.clip = givenMusic;
        musicSource.Play();
        timingPairs = givenPairs;
        started = true;
    }

    void animateTiming (float perfectTime, float accumulatedTime)
    {
        timing.rectTransform.localScale = new Vector3(1 + (maxTimingImgSize * (perfectTime - accumulatedTime)) / perfectTime, 1 + (maxTimingImgSize * (perfectTime - accumulatedTime)) / perfectTime, 0);
        if ((1 + (2 * (perfectTime - accumulatedTime)) / perfectTime) <= 1) {
            timing.sprite = voidSprite;
        }
    }

    void checkTiming (float accTime)
    {
        if (inputCheck.CheckLimbs(lastMove, InputCheck.Players.PlayerOne) == 1 && moveRatedP1 == false)
        {
            if (accTime > timingPairs[lastPairIndex].secondValue - errorMargin && accTime < timingPairs[lastPairIndex].secondValue + errorMargin)
            {
                scoringSystem.AddFirstPlayerScore(inputCheck.CheckScore(lastMove, 2, InputCheck.Players.PlayerOne), inputCheck.GetMaxScore(lastMove));

            }
            else
            {
                scoringSystem.AddFirstPlayerScore(inputCheck.CheckScore(lastMove, 1, InputCheck.Players.PlayerOne), inputCheck.GetMaxScore(lastMove));
            }
            moveRatedP1 = true;
        }
        if (inputCheck.CheckLimbs(lastMove, InputCheck.Players.PlayerTwo) == 1 && moveRatedP2 == false)
        {
            if (accTime > timingPairs[lastPairIndex].secondValue - errorMargin && accTime < timingPairs[lastPairIndex].secondValue + errorMargin)
            {
                scoringSystem.AddSecondPlayerScore(inputCheck.CheckScore(lastMove, 2, InputCheck.Players.PlayerTwo), inputCheck.GetMaxScore(lastMove));

            }
            else
            {
                scoringSystem.AddSecondPlayerScore(inputCheck.CheckScore(lastMove, 1, InputCheck.Players.PlayerTwo), inputCheck.GetMaxScore(lastMove));
            }
            moveRatedP2 = true;
        }
        if (accTime >= timingPairs[lastPairIndex].secondValue) timing.sprite = voidSprite;
    }

}