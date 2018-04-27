﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {

    int stateIndex = 0;
    List<Pair<GameObject, GameStates>> states = new List<Pair<GameObject, GameStates>>();
    public enum GameStates{
        MusicInstruction,
        Freestyle,
    }

    bool started = false;
    bool finished = false;
	
	// Update is called once per frame
	void Update () {
        if (!started || finished) return;
        switch (states[stateIndex].secondValue)
        {
            case GameStates.MusicInstruction:
                states[stateIndex].firstValue.GetComponent<MusicInstructions>().OnUpdate();
                if (states[stateIndex].firstValue.GetComponent<MusicInstructions>().stateFinished == true) NextState();
                break;
            case GameStates.Freestyle:
                states[stateIndex].firstValue.GetComponent<Freestyle>().OnUpdate();
                if (states[stateIndex].firstValue.GetComponent<Freestyle>().stateFinished == true) NextState();
                break;
            default:
                break;
        }
	}

    public void AddState (GameObject stateAdded, GameStates gameStateAdded)
    {
        Pair<GameObject, GameStates> temp = new Pair<GameObject, GameStates>(stateAdded, gameStateAdded);
        states.Add(temp);
        started = true;
        finished = false;
    }

    public GameObject GetCurrentState ()
    {
        return states[stateIndex].firstValue;
    }

    private void NextState()
    {
        stateIndex += 1;

        if (stateIndex > states.Count - 1)
        {
            finished = true;
            GetComponent<GameControlling>().GameOver();
        } else
        {
            switch (states[stateIndex].secondValue)
            {
                case GameStates.MusicInstruction:
                    states[stateIndex].firstValue.GetComponent<MusicInstructions>().OnStart();
                    break;
                case GameStates.Freestyle:
                    states[stateIndex].firstValue.GetComponent<Freestyle>().OnStart();
                    break;
                default:
                    break;
            }
        }
    }

}