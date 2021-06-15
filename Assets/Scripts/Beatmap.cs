using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEditor;
using TMPro;
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

        private string dataKey{
            get{
                return $@"GameData/BeatmapRecords/{gameObject.name}";
            }
        }

        public void FetchRecords(TextMeshPro target){
            target.text = $"Local Record\n{localRecords.Min.score.ToString("D6")}";

        }

        public void LoadRecords(){
            localRecords.Clear();
            string records = PlayerPrefs.GetString(dataKey, "0 0 0 0 0 0");
            string[] lines = records.Split('\n');
            foreach(var line in lines){
                string[] tokens = line.Split(' ');
                if(tokens.Length != 6){
                    throw new Exception("Record data format error at " + dataKey);
                }
                localRecords.Add(new BeatmapRecord(
                    Convert.ToInt32(tokens[0]), 
                    Convert.ToInt32(tokens[1]), 
                    Convert.ToInt32(tokens[2]), 
                    Convert.ToInt32(tokens[3]), 
                    Convert.ToInt32(tokens[4]), 
                    Convert.ToInt32(tokens[5])));

            }
        }

        public void SaveRecords(){
            StringBuilder sb = new StringBuilder();
            foreach(var record in localRecords){
                sb.Append($"{record.missCount} {record.badCount} {record.goodCount} {record.perfectCount} {record.maxCombo} {record.score}\n");
            }
            PlayerPrefs.SetString(dataKey, sb.ToString());
        }

        public void ClearRecords(){
            localRecords.Clear();
            localRecords.Add(new BeatmapRecord(0, 0, 0, 0, 0, 0));
            SaveRecords();
        }


        // Use this for initialization
        void Start()
        {
            Assert.IsNotNull(song, $"Clip for {gameObject.name} is not set.");
            Assert.IsNotNull(script, $"Script for {gameObject.name} is not set.");
            LoadRecords();
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}

