using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEditor;
namespace Assets.Scripts
{

    /// <summary>
    /// Beatmap component that contains the song and script associated with it.
    /// </summary>
    public class Beatmap : MonoBehaviour
    {
        [Tooltip("The song associated with the Beatmap.")]
        public AudioClip song;

        [Tooltip("The BeatScript (.txt) associated with the Beatmap.")]
        public TextAsset script;

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

