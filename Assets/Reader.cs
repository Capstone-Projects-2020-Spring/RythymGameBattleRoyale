﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChartParser;

public class Reader : MonoBehaviour
{
    public GameObject[] inputButtonObjects;
    public GameObject noteObject;
    public AudioSource audioData;
    public float songTime;

    private InstantiatorScript[] noteSpawnerScripts;
    Song Holder;
    List<Note> track;
    float startTime;
    float oldTime = 0f;
    public float b = 120.0f;
    public float res = 192.0f;
    bool songStarted = false;
    private int measure = 0;
    


    // Start is called before the first frame update
    void Start()
    {
        startTime = 0.0f;
        noteSpawnerScripts = new InstantiatorScript[inputButtonObjects.Length];

        

        string path = Application.dataPath;

        Holder = Parser.ChartReader(path + "\\MEGALOVANIA.chart");
        track = Holder.NoteTracksList.getExpert();

        for (int i = 0; i < inputButtonObjects.Length; i++)
        {
            noteSpawnerScripts[i] = inputButtonObjects[i].GetComponent<InstantiatorScript>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        startTime += Time.deltaTime;
        
        if (track.Count>0 && startTime >= calcTime(track[0].getTimeStamp()))
        {
            check(track[0].getChord());
            oldTime = startTime;
        }
        if (startTime >= calcTime((res * measure)))
        {
            measure++;
            noteSpawnerScripts[2].SpawnBar();
        }


        float playSongTime = songTime;// + noteObject.GetComponent<NoteScript>().speed*-.003f + 6.45f;
        if (!songStarted && startTime >= playSongTime) {
            startSong();
        }
    }

    public float calcTime(float value) {
        return (((value / res) * (60000.0f / b)) / 1000.0f);
    }

    public void startSong(){
        if (!songStarted) {
            audioData.Play(0);
        }
        songStarted = true;
    }


    public void check(List<ButtonColor> chord)
    {
        foreach (ButtonColor color in chord)
        {
            switch (color)
            {
                case ButtonColor.Green:
                    noteSpawnerScripts[0].SpawnNote();
                    print("green");
                    break;
                case ButtonColor.Red:
                    noteSpawnerScripts[1].SpawnNote();
                    print("red");
                    break;
                case ButtonColor.Yellow:
                    noteSpawnerScripts[2].SpawnNote();
                    print("yellow");
                    break;
                case ButtonColor.Blue:
                    noteSpawnerScripts[3].SpawnNote();
                    print("blue");
                    break;
                case ButtonColor.Orange:
                    noteSpawnerScripts[4].SpawnNote();
                    print("orange");
                    break;
                default:

                    break;
            }
        }
        track.RemoveAt(0);

    }






}
