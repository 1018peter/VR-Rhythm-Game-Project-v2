using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
namespace Assets.Scripts
{

    public enum HitRank{
        Early, OK, Nice, Great, Perfect, Late
    }

    /// <summary>
    /// Singleton class that provides an interface to beatmap script execution.
    /// 
    /// </summary>
    public class SongManager : MonoBehaviour
    {
        // handle to the singleton class.
        public static SongManager handle;

        // handle to the orbit system.
        public OrbitManager orbitManager;

        // handle to audio source
        [HideInInspector]
        public AudioSource globalAudio;

        [Header("Note Prefabs")]

        public GameObject orbitNote;

        public GameObject tapNote;

        #region Readonly Variables
        //the current position of the song (in seconds)
        public float songPosition { get; private set; }

        //the current position of the song (in beats)
        public float songPosInBeats { get; private set; }

        //the duration of a beat
        public float SecPerBeat { get => 60.0f / bpm; }

        //how much time (in seconds) has passed since the song started
        public float dsptimesong { get; private set; }

        #endregion
        //beats per minute of a song
        [HideInInspector]
        public float bpm;

        [Header("Configurations")]

        [Tooltip("The number of beats to show in advance by default.")]
        public float beatsShownInAdvance;

        [Tooltip("For testing: Toggle to load and execute the beatmap specified by the Default Beatmap Name field immediately.")]
        public bool loadDefaultBeatmapOnLoad = true;
        
        [Tooltip("For testing: The default beatmap to execute.")]
        public string defaultBeatmapName = "Chirupa";



        #region Private Variables
        //the index of the next note to be spawned
        int nextNoteIndex = 0;

        int score = 0;


        // maintains a sequence of commands scripted to be executed.
        private readonly List<ScriptedCommand> notes = new List<ScriptedCommand>();

        int nextCommandIndex = 0;

        // maintains a sequence of commands scripted to be executed -immediately-.
        private readonly List<ScriptedCommand> immediateCommands = new List<ScriptedCommand>();

        //use for note coroutines.
        bool NextNoteReady
        {
            get
            {
                return nextNoteIndex < notes.Count && (notes[nextNoteIndex].timestamp < songPosInBeats + beatsShownInAdvance);
            }
        }


        /// <summary>
        /// A class that describes -when- to execute a given command and -what- command to execute.
        /// 
        /// </summary>
        class ScriptedCommand
        {
            /// <summary>
            /// When to execute the command, in units of beats. Set to less than 0 to execute immediately.
            /// </summary>
            public int timestamp;

            /// <summary>
            /// What to execute. 
            /// The action acts as a delegate for the lambda function that captures all parameters needed.
            /// </summary>
            public Action action;
            public ScriptedCommand(int t, Action a)
            {
                timestamp = t;
                action = a;
            }

        }

        #endregion

        public IEnumerator RotateOrbitForward(Transform orbit, int beats, float endAngle){
            // Internal: Rotates around the forward axis.
            float duration = beats / bpm * 60;
            float t = 0;
            float startAngle = orbit.rotation.eulerAngles.z;
            while(t / duration < 1){
                orbit.rotation = Quaternion.Euler(
                    orbit.rotation.eulerAngles.x,
                    orbit.rotation.eulerAngles.y,
                    Mathf.LerpAngle(startAngle, endAngle, t / duration));
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }

        }
        public IEnumerator RotateOrbitForwardOffset(Transform orbit, int beats, float offsetAngle){
             // Internal: Rotates around the forward axis.
            float duration = beats / bpm * 60;
            float t = 0;
            float startAngle = orbit.rotation.eulerAngles.z;
            float endAngle = startAngle + offsetAngle;
            while(t / duration < 1){
                orbit.rotation = Quaternion.Euler(
                    orbit.rotation.eulerAngles.x,
                    orbit.rotation.eulerAngles.y,
                    Mathf.LerpAngle(startAngle, endAngle, t / duration));
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
        }

        public IEnumerator RotateOrbitUp(Transform orbit, int beats, float endAngle){
            // Internal: Rotates around the up axis. (The axis that goes through the flat side.)
            float duration = beats / bpm * 60;
            float t = 0;
            float startAngle = orbit.rotation.eulerAngles.y;
            while(t / duration < 1){
                orbit.rotation = Quaternion.Euler(
                    orbit.rotation.eulerAngles.x,
                    Mathf.LerpAngle(startAngle, endAngle, t / duration),
                    orbit.rotation.eulerAngles.z);
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
        }
        public IEnumerator RotateOrbitUpOffset(Transform orbit, int beats, float offsetAngle){
            // Internal: Rotates around the up axis. (The axis that goes through the flat side.)
            float duration = beats / bpm * 60;
            float t = 0;
            float startAngle = orbit.rotation.eulerAngles.y;
            float endAngle = startAngle + offsetAngle;
            while(t / duration < 1){
                orbit.rotation = Quaternion.Euler(
                    orbit.rotation.eulerAngles.x,
                    Mathf.LerpAngle(startAngle, endAngle, t / duration),
                    orbit.rotation.eulerAngles.z);
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
        }

        public IEnumerator RotateRedOrbitForward(int beats, float endAngle){
            // Internal: Rotates around the forward axis.
            float duration = beats / bpm * 60;
            float t = 0;
            float startAngle = orbitManager.orbitTransformR.rotation.eulerAngles.z;
            while(t / duration < 1){
                orbitManager.orbitTransformR.rotation = Quaternion.Euler(
                    orbitManager.orbitTransformR.rotation.eulerAngles.x,
                    orbitManager.orbitTransformR.rotation.eulerAngles.y,
                    Mathf.LerpAngle(startAngle, endAngle, t / duration));
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
        }
        public IEnumerator RotateRedOrbitForwardOffset(int beats, float offsetAngle){
            // Internal: Rotates around the forward axis.
            float duration = beats / bpm * 60;
            float t = 0;
            float startAngle = orbitManager.orbitTransformR.rotation.eulerAngles.z;
            float endAngle = startAngle + offsetAngle;
            while(t / duration < 1){
                orbitManager.orbitTransformR.rotation = Quaternion.Euler(
                    orbitManager.orbitTransformR.rotation.eulerAngles.x,
                    orbitManager.orbitTransformR.rotation.eulerAngles.y,
                    Mathf.LerpAngle(startAngle, endAngle, t / duration));
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
        }
        public void SpawnOrbitNodeInPlace(Transform anchor, float orbitAngle){
            //  The note is first rotated to the same rotation as the anchor orbit.
            // Then, the note is assigned to be parent to the anchor orbit.
            // Finally, the note is guaranteed to stay on the orbit as long as only its z-rotation is changed.
            // So, we can specify its position on the orbit in euler angles.
            var newNote = Instantiate(orbitNote);
            newNote.transform.SetParent(anchor, true);
            newNote.transform.localScale = Vector3.one;
            newNote.transform.localRotation = Quaternion.Euler(0, 0, orbitAngle);
        }
        IEnumerator MoveOrbitNode(GameObject note, float endAngle){
            float beatPos = songPosInBeats;
            float startAngle = note.transform.localRotation.z;
            for(float t = (songPosInBeats - beatPos) / beatsShownInAdvance; t < 1; t = (songPosInBeats - beatPos) / beatsShownInAdvance){
                note.transform.localRotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(startAngle, endAngle, t));
                yield return new WaitForEndOfFrame();
            }
        }
        public void SpawnOrbitNodeMoving(Transform anchor, float orbitAngleStart, float orbitAngleEnd){
            var newNote = Instantiate(orbitNote);
            newNote.transform.SetParent(anchor, true);
            newNote.transform.localScale = Vector3.one;
            newNote.transform.localRotation = Quaternion.Euler(0, 0, orbitAngleStart);
            StartCoroutine(MoveOrbitNode(newNote, orbitAngleEnd));
        }
        public void SpawnTapNote(float rotationY, float rotationZ){
            // For simplicity, only two rotations can be specified.
            var newNote = Instantiate(tapNote);
            newNote.transform.SetParent(orbitManager.transform, true);
            newNote.transform.localScale = Vector3.one;
            newNote.transform.localRotation = Quaternion.Euler(0, rotationY, rotationZ);
        }
        #region TODO



        #endregion

        Transform decodeOrbitColor(string encode){
            switch(encode){
                case "R": return orbitManager.orbitTransformR;
                case "G": return orbitManager.orbitTransformG;
                case "B": return orbitManager.orbitTransformB;
            }

            return null;
        }

        /// <summary>
        /// Load the beatmap specified by name.
        /// The script is interpreted line by line.
        /// Basic syntax checking is performed.
        /// For more details, refer to the BeatScript Spec.md document.
        /// </summary>
        /// <param name="beatmapName">The name of the beatmap</param>
        public void LoadBeatmap(string beatmapName)
        {
            bool BPMdefined = false;
            Beatmap beatmap = gameObject.transform.Find(beatmapName).GetComponent<Beatmap>();
            Assert.IsNotNull(beatmap, $"Beatmap of name '{beatmapName}' not found in children of {gameObject.name}.");
            string[] lines = beatmap.script.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int lineCount = 0;
            foreach (var line in lines)
            {
                string[] tokens = line.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 0) continue; // Skip empty lines.
                string funcCode = tokens[0];
                if (funcCode.StartsWith("#") || funcCode.StartsWith("//")) continue; // Skip comments.
                int parameterCount = tokens.Length - 1;
                Transform targetOrbit = null;
                switch (funcCode)
                {
                    case "BPM":
                        Assert.IsTrue(parameterCount == 1, $"Line {lineCount}: BPM expects exactly one parameter (beats-per-minute of the song). {parameterCount} was found.");
                        bpm = Convert.ToInt32(tokens[1]);
                        BPMdefined = true;
                        break;
                    case "ROTATE_FORWARD_TO":
                        Assert.IsTrue(parameterCount == 4, $"Line {lineCount}: ROTATE_FORWARD_TO expects exactly 4 parameters (timestamp, orbit color(R/G/B), rotation duration in beats, resulting rotation in degrees). {parameterCount} was found.");
                        targetOrbit = decodeOrbitColor(tokens[2]);
                        Assert.IsNotNull(targetOrbit, $"Line {lineCount}: Invalid orbit color code. Must be R, G, or B.");
                        immediateCommands.Add(new ScriptedCommand(Convert.ToInt32(tokens[1]), () => { StartCoroutine(RotateOrbitForward(targetOrbit, Convert.ToInt32(tokens[3]), Convert.ToSingle(tokens[4]))); }));

                        break;
                    case "ROTATE_FORWARD_BY":
                        Assert.IsTrue(parameterCount == 4, $"Line {lineCount}: ROTATE_FORWARD_BY expects exactly 4 parameters (timestamp, orbit color(R/G/B), rotation duration in beats, offset rotation in degrees). {parameterCount} was found.");
                        targetOrbit = decodeOrbitColor(tokens[2]);
                        Assert.IsNotNull(targetOrbit, $"Line {lineCount}: Invalid orbit color code. Must be R, G, or B.");
                        immediateCommands.Add(new ScriptedCommand(Convert.ToInt32(tokens[1]), () => { StartCoroutine(RotateOrbitForwardOffset(targetOrbit, Convert.ToInt32(tokens[3]), Convert.ToSingle(tokens[4]))); }));

                        break;
                    case "ROTATE_UP_TO":
                        Assert.IsTrue(parameterCount == 4, $"Line {lineCount}: ROTATE_UP_TO expects exactly 4 parameters (timestamp, orbit color(R/G/B), rotation duration in beats, resulting rotation in degrees). {parameterCount} was found.");
                        targetOrbit = decodeOrbitColor(tokens[2]);
                        Assert.IsNotNull(targetOrbit, $"Line {lineCount}: Invalid orbit color code. Must be R, G, or B.");
                        immediateCommands.Add(new ScriptedCommand(Convert.ToInt32(tokens[1]), () => { StartCoroutine(RotateOrbitUp(targetOrbit, Convert.ToInt32(tokens[3]), Convert.ToSingle(tokens[4]))); }));

                        break;
                    case "ROTATE_UP_BY":
                        Assert.IsTrue(parameterCount == 4, $"Line {lineCount}: ROTATE_UP_BY expects exactly 4 parameters (timestamp, orbit color(R/G/B), rotation duration in beats, offset rotation in degrees). {parameterCount} was found.");
                        targetOrbit = decodeOrbitColor(tokens[2]);
                        Assert.IsNotNull(targetOrbit, $"Line {lineCount}: Invalid orbit color code. Must be R, G, or B.");
                        immediateCommands.Add(new ScriptedCommand(Convert.ToInt32(tokens[1]), () => { StartCoroutine(RotateOrbitUpOffset(targetOrbit, Convert.ToInt32(tokens[3]), Convert.ToSingle(tokens[4]))); }));

                        break;
                   
                    case "ORBITNOTE_FIXED":
                        Assert.IsTrue(parameterCount == 3, $"Line {lineCount}: ORBITNOTE_FIXED expects exactly 3 parameters (timestamp, orbit color(R/G/B), position on orbit in degrees). {parameterCount} was found.");
                        targetOrbit = decodeOrbitColor(tokens[2]);
                        Assert.IsNotNull(targetOrbit, $"Line {lineCount}: Invalid orbit color code. Must be R, G, or B.");
                        notes.Add(new ScriptedCommand(Convert.ToInt32(tokens[1]), () => { SpawnOrbitNodeInPlace(targetOrbit, Convert.ToSingle(tokens[3])); }));

                        break;
                    case "ORBITNOTE_MOVING":
                        Assert.IsTrue(parameterCount == 3, $"Line {lineCount}: ORBITNOTE_MOVING expects exactly 4 parameters (timestamp, orbit color(R/G/B), start position on orbit in degrees, end position on orbit in degrees). {parameterCount} was found.");
                        targetOrbit = decodeOrbitColor(tokens[2]);
                        Assert.IsNotNull(targetOrbit, $"Line {lineCount}: Invalid orbit color code. Must be R, G, or B.");
                        notes.Add(new ScriptedCommand(Convert.ToInt32(tokens[1]), () => { SpawnOrbitNodeMoving(targetOrbit, Convert.ToSingle(tokens[3]), Convert.ToSingle(tokens[4])); }));

                        break;
                    case "TAPNOTE":
                        Assert.IsTrue(parameterCount == 3, $"Line {lineCount}: TAPNOTE expects exactly 3 parameters (timestamp, X-rotation relative to origin, Z-position relative to origin). {parameterCount} was found.");
                        notes.Add(new ScriptedCommand(Convert.ToInt32(tokens[1]), () => { SpawnTapNote(Convert.ToSingle(tokens[2]), Convert.ToSingle(tokens[3])); }));
                        break;
                    default:
                        Debug.LogError($"Line {lineCount}: Unknown command '{tokens[0]}'.");
                        break;
                }

                lineCount++;
            }

            Assert.IsTrue(BPMdefined, $"Fatal Error: The script file of beatmap '{beatmapName}' does not define BPM!");


            globalAudio.clip = beatmap.song;

        }

        public IEnumerator ExecuteImmediateEvents(){
            nextCommandIndex = 0;
            while(nextCommandIndex < immediateCommands.Count){
                yield return new WaitUntil(() => songPosInBeats >= immediateCommands[nextCommandIndex].timestamp);
                immediateCommands[nextCommandIndex++].action();
            }
        }

        /// <summary>
        /// Execute the currently-loaded beatmap.
        /// </summary>
        public IEnumerator ExecuteBeatmap()
        {
            // reset positions
            nextNoteIndex = 0;

            //reset score
            score = 0;

            //record the time when the song starts
            dsptimesong = (float)AudioSettings.dspTime;

            //calculate the position in seconds
            songPosition = (float)(AudioSettings.dspTime - dsptimesong);

            //calculate the position in beats
            songPosInBeats = songPosition / SecPerBeat;
            Debug.Log("Start of beatmap!");

            globalAudio.Play();

            StartCoroutine(ExecuteImmediateEvents());

            while (nextNoteIndex < notes.Count)
            {
                if(!NextNoteReady)
                    yield return new WaitUntil(() => NextNoteReady);
                notes[nextNoteIndex++].action();
            }

            Debug.Log("End of beatmap!");
        }

        /// <summary>
        /// Helper function to execute a beatmap immediately.
        /// For actual usage, loading and executing should be done separately to split the lag.
        /// </summary>
        /// <param name="beatmapName">The name of the beatmap.</param>
        public void LoadAndExecuteBeatmap(string beatmapName)
        {
            LoadBeatmap(beatmapName);
            StartCoroutine(ExecuteBeatmap());
        }

        public void ScoreHit(HitRank rank){
            switch(rank){
                case HitRank.Early:
                    score += 500;
                    break;
                case HitRank.OK:
                    score += 1000;
                    break;
                case HitRank.Nice:
                    score += 1250;
                    break;
                case HitRank.Great:
                    score += 1750;
                    break;
                case HitRank.Perfect:
                    score += 2000;
                    break;
                case HitRank.Late:
                    score += 750;
                    break;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (handle == null) handle = this;
            else
            {
                throw new UnityException("Singleton SongManager instantiated twice!");
            }

            globalAudio = GetComponent<AudioSource>();

            Assert.IsNotNull(globalAudio, "Audio Source of Song Manager not set!");

            if(loadDefaultBeatmapOnLoad)
                LoadAndExecuteBeatmap(defaultBeatmapName);

            score = 0;

        }

        // OnGUI is used for debug displays.
        private void OnGUI()
        {
            GUI.Box(new Rect(0, Screen.height - 200, 200, 200), "");
            GUI.Label(new Rect(0, Screen.height - 200, 200, 200), 
            $"Song Position: {songPosition.ToString("0.00")} seconds ({songPosInBeats.ToString("0.00")} beats)\nNote Index: {nextNoteIndex}\nScore: {score}\n"
            + $"Red Orbit Rotation: {orbitManager.orbitTransformR.rotation.eulerAngles}"
            + $"Green Orbit Rotation: {orbitManager.orbitTransformG.rotation.eulerAngles}"
            + $"Blue Orbit Rotation: {orbitManager.orbitTransformR.rotation.eulerAngles}");
        
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnAudioFilterRead(float[] data, int channels)
        {

            //calculate the position in seconds
            songPosition = (float)(AudioSettings.dspTime - dsptimesong);

            //calculate the position in beats
            songPosInBeats = songPosition / SecPerBeat;
        }
    }

}
