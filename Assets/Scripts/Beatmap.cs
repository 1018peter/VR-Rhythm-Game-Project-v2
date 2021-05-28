using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEditor;
namespace Assets.Scripts
{
    public class BeatmapRecord : IComparable {
        public int missCount = 0, badCount = 0, goodCount = 0, perfectCount = 0, maxCombo = 0, score = 0;
        public int CompareTo(object rhs){
            var rhsRecord = rhs as BeatmapRecord;
            return -score + rhsRecord.score;
        }

        public BeatmapRecord(int _miss, int _bad, int _good, int _perfect, int _maxCombo, int _score){
            missCount = _miss;
            badCount = _bad;
            goodCount = _good;
            perfectCount = _perfect;
            maxCombo = _maxCombo;
            score = _score;
        }
        
    }

    /// <summary>
    /// Beatmap component that contains the song and script associated with it.
    /// </summary>
    public class Beatmap : MonoBehaviour
    {
        [Tooltip("The song associated with the Beatmap.")]
        public AudioClip song;

        [Tooltip("The BeatScript (.txt) associated with the Beatmap.")]
        public TextAsset script;

        public SortedSet<BeatmapRecord> localRecords = new SortedSet<BeatmapRecord>();

        // Use this for initialization
        void Start()
        {
            Assert.IsNotNull(song, $"Clip for {gameObject.name} is not set.");
            Assert.IsNotNull(script, $"Script for {gameObject.name} is not set.");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}

