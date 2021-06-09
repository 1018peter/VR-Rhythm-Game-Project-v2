using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using System.Linq;
namespace Assets.Scripts
{

    /// <summary>
    /// Singleton class that provides an interface to beatmap script execution.
    /// 
    /// </summary>
    public class SongManager : MonoBehaviour
    {
        // handle to the singleton class.
        public static SongManager Instance;

        // handle to the orbit system.
        public OrbitManager orbitManager;

        // handle to audio source
        [HideInInspector]
        public AudioSource globalAudio;

        public AudioClip onUIActivate;
        public AudioClip onUIConfirm;
        public AudioClip onUIDeactivate;

        #region Audio Utilities
        public void playUIactivate(){
            globalAudio.PlayOneShot(onUIActivate);
        }

        public void playUIconfirm(){
            globalAudio.PlayOneShot(onUIConfirm);
        }

        public void playUIdeactivate(){
            globalAudio.PlayOneShot(onUIDeactivate);
        }
        #endregion

        [Header("Note Prefabs")]

        public GameObject orbitNote;

        public GameObject tapNote;

        [Header("Particles")]
        public GameObject particlesOnHit;
        public float particlesOnHitLifespan = 0.5f;

        #region Readonly Variables
        
        //the offset time until the first beat of the song (in seconds)
        public float offsetFromFirstBeat { get; private set; } = 0;


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
        public Beatmap defaultBeatmap;
        [Tooltip("The parent gameObject of all beatmaps.")]
        public GameObject beatmapGroup;



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

        #region FX Utilities

        private IEnumerator destroyParticlesAfterSeconds(float t, GameObject targetParticles){
            yield return new WaitForSecondsRealtime(t);
            Destroy(targetParticles);
        }
        public void createParticlesOnHit(Vector3 position, Quaternion rotation){
            StartCoroutine(destroyParticlesAfterSeconds(particlesOnHitLifespan, Instantiate(particlesOnHit, position, rotation)));
            
        }
        #endregion

        #region Rotation Utilities

        public IEnumerator RotateOrbitForward(Transform orbit, int beats, float endAngle){
            // Internal: Rotates around the forward axis.
            float duration = beats / bpm * 60;
            float t = 0;
            float startAngle = orbit.rotation.eulerAngles.z;
            float offsetAngle = endAngle - startAngle;
            Quaternion endRotation = Quaternion.AngleAxis(offsetAngle, orbit.forward);
            Quaternion startRotation = orbit.rotation;
            while(t / duration < 1){
                orbit.rotation = Quaternion.Slerp(startRotation, endRotation, t);
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
            orbit.rotation = endRotation;
        }
        public IEnumerator RotateOrbitForwardOffset(Transform orbit, int beats, float offsetAngle){
             // Internal: Rotates around the forward axis.
            float duration = beats / bpm * 60;
            float t = 0;
            Quaternion endRotation = Quaternion.AngleAxis(offsetAngle, orbit.forward);
            Quaternion startRotation = orbit.rotation;
            while(t / duration < 1){
                orbit.rotation = Quaternion.Slerp(startRotation, endRotation, t);
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
            orbit.rotation = endRotation;
        }

        public IEnumerator RotateOrbitUp(Transform orbit, int beats, float endAngle){
            // Internal: Rotates around the up axis. (The axis that goes through the flat side.)
            float duration = beats / bpm * 60;
            float t = 0;
            float startAngle = orbit.rotation.eulerAngles.y;
            float offsetAngle = endAngle - startAngle;
            Quaternion endRotation = Quaternion.AngleAxis(offsetAngle, orbit.up);
            Quaternion startRotation = orbit.rotation;
            while(t / duration < 1){
                orbit.rotation = Quaternion.Slerp(startRotation, endRotation, t);
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
            orbit.rotation = endRotation;
        }
        public IEnumerator RotateOrbitUpOffset(Transform orbit, int beats, float offsetAngle){
            // Internal: Rotates around the up axis. (The axis that goes through the flat side.)
            float duration = beats / bpm * 60;
            float t = 0;
            Quaternion endRotation = Quaternion.AngleAxis(offsetAngle, orbit.up);
            Quaternion startRotation = orbit.rotation;
            while(t / duration < 1){
                orbit.rotation = Quaternion.Slerp(startRotation, endRotation, t);
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
            orbit.rotation = endRotation;
        }

        public IEnumerator RotateOrbitRight(Transform orbit, int beats, float endAngle){
            // Internal: Rotates around the right axis.
            float duration = beats / bpm * 60;
            float t = 0;
            float startAngle = orbit.rotation.eulerAngles.x;
            float offsetAngle = endAngle - startAngle;
            Quaternion endRotation = Quaternion.AngleAxis(offsetAngle, orbit.right);
            Quaternion startRotation = orbit.rotation;
            while(t / duration < 1){
                orbit.rotation = Quaternion.Slerp(startRotation, endRotation, t);
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
            orbit.rotation = endRotation;
        }
        public IEnumerator RotateOrbitRightOffset(Transform orbit, int beats, float offsetAngle){
            // Internal: Rotates around the right axis.
            float duration = beats / bpm * 60;
            float t = 0;
            Quaternion endRotation = Quaternion.AngleAxis(offsetAngle, orbit.right);
            Quaternion startRotation = orbit.rotation;
            while(t / duration < 1){
                orbit.rotation = Quaternion.Slerp(startRotation, endRotation, t);
                yield return new WaitForEndOfFrame();
                t += Time.deltaTime;
            }
            orbit.rotation = endRotation;
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
                if(note == null) break;
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

        #endregion

        #region Beatmap I/O
        Transform decodeOrbitColor(string encode){
            switch(encode){
                case "R": return orbitManager.orbitTransformR;
                case "G": return orbitManager.orbitTransformG;
                case "B": return orbitManager.orbitTransformB;
            }

            return null;
        }

        Beatmap currentBeatmap;

        /// <summary>
        /// The object version of LoadBeatmap.
        /// The script is interpreted line by line.
        /// Basic syntax checking is performed.
        /// For more details, refer to the BeatScript Spec.md document.
        /// </summary>
        /// <param name="beatmap">A beatmap component, which should belong to a gameObject</param>
        public void LoadBeatmap(Beatmap beatmap){
            immediateCommands.Clear();
            notes.Clear();
            bool BPMdefined = false;
            string[] lines = beatmap.script.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int lineCount = 1;
            foreach (var line in lines)
            {
                if(line.StartsWith("#") || line.StartsWith("//")) {
                    lineCount++;
                    continue; // Skip comments.
                }
                string[] tokens = line.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 0) continue; // Skip empty lines.
                string funcCode = tokens[0];
                int parameterCount = tokens.Length - 1;
                Transform targetOrbit = null;
                try{
                    switch (funcCode)
                    {
                    case "BPM":
                        Assert.IsTrue(parameterCount == 1, $"Line {lineCount}: BPM expects exactly one parameter (beats-per-minute of the song). {parameterCount} was found.");
                        bpm = Convert.ToInt32(tokens[1]) * globalAudio.pitch;
                        BPMdefined = true;
                        break;
                    case "FIRST_BEAT_OFFSET":
                        Assert.IsTrue(parameterCount == 1, $"Line {lineCount}: FIRST_BEAT_OFFSET expects exactly one parameter (seconds until the first beat of the song). {parameterCount} was found.");
                        offsetFromFirstBeat = Convert.ToSingle(tokens[1]);
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
                    case "ROTATE_RIGHT_TO":
                        Assert.IsTrue(parameterCount == 4, $"Line {lineCount}: ROTATE_UP_TO expects exactly 4 parameters (timestamp, orbit color(R/G/B), rotation duration in beats, resulting rotation in degrees). {parameterCount} was found.");
                        targetOrbit = decodeOrbitColor(tokens[2]);
                        Assert.IsNotNull(targetOrbit, $"Line {lineCount}: Invalid orbit color code. Must be R, G, or B.");
                        immediateCommands.Add(new ScriptedCommand(Convert.ToInt32(tokens[1]), () => { StartCoroutine(RotateOrbitRight(targetOrbit, Convert.ToInt32(tokens[3]), Convert.ToSingle(tokens[4]))); }));

                        break;
                    case "ROTATE_RIGHT_BY":
                        Assert.IsTrue(parameterCount == 4, $"Line {lineCount}: ROTATE_UP_BY expects exactly 4 parameters (timestamp, orbit color(R/G/B), rotation duration in beats, offset rotation in degrees). {parameterCount} was found.");
                        targetOrbit = decodeOrbitColor(tokens[2]);
                        Assert.IsNotNull(targetOrbit, $"Line {lineCount}: Invalid orbit color code. Must be R, G, or B.");
                        immediateCommands.Add(new ScriptedCommand(Convert.ToInt32(tokens[1]), () => { StartCoroutine(RotateOrbitRightOffset(targetOrbit, Convert.ToInt32(tokens[3]), Convert.ToSingle(tokens[4]))); }));

                        break;
                    case "ORBITNOTE_FIXED":
                        Assert.IsTrue(parameterCount == 3, $"Line {lineCount}: ORBITNOTE_FIXED expects exactly 3 parameters (timestamp, orbit color(R/G/B), position on orbit in degrees). {parameterCount} was found.");
                        targetOrbit = decodeOrbitColor(tokens[2]);
                        Assert.IsNotNull(targetOrbit, $"Line {lineCount}: Invalid orbit color code. Must be R, G, or B.");
                        notes.Add(new ScriptedCommand(Convert.ToInt32(tokens[1]), () => { SpawnOrbitNodeInPlace(targetOrbit, Convert.ToSingle(tokens[3])); }));

                        break;
                    case "ORBITNOTE_MOVING":
                        Assert.IsTrue(parameterCount == 4, $"Line {lineCount}: ORBITNOTE_MOVING expects exactly 4 parameters (timestamp, orbit color(R/G/B), start position on orbit in degrees, end position on orbit in degrees). {parameterCount} was found.");
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
                }
                catch(System.Exception e){
                    Debug.LogError($"Error occurred while parsing line {lineCount}.\nException: {e.ToString()}\nStackTrace: {e.StackTrace}");
                }
                

                lineCount++;
            }

            Assert.IsTrue(BPMdefined, $"Fatal Error: The script file of beatmap '{beatmap.gameObject.name}' does not define BPM!");
            Debug.Log($"Successfully loaded beatmap '{beatmap.gameObject.name}'");

            globalAudio.clip = beatmap.song;
            currentBeatmap = beatmap;
            ResetCounts();
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
            Beatmap beatmap = gameObject.transform.Find(beatmapName).GetComponent<Beatmap>();
            Assert.IsNotNull(beatmap, $"Beatmap of name '{beatmapName}' not found in children of {gameObject.name}.");
            LoadBeatmap(beatmap);

        }

        public void PlayPreview(){
            globalAudio.Stop();
            globalAudio.Play();
            globalAudio.loop = true;
        }

        public void StopPreview(){
            globalAudio.Stop();
            globalAudio.loop = false;
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
            yield return new WaitUntil(() => !globalAudio.isPlaying);
            Debug.Log("End of song");
            results = new BeatmapRecord(
                missCount, 
                badCount, 
                goodCount, 
                perfectCount, 
                maxCombo, 
                ComputeScore());
            currentBeatmap.localRecords.Add(results);
            ResultsUIController.Instance.Write();
            currentBeatmap.SaveRecords();
            GameManager.Instance.GoToResults();

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

        public void LoadAndExecuteBeatmap(Beatmap beatmap){
            LoadBeatmap(beatmap);
            StartCoroutine(ExecuteBeatmap());
        }

        #endregion

        #region Scoring Utilities
        [NonSerialized]
        public BeatmapRecord results;
        [NonSerialized]
        public int missCount = 0, badCount = 0, goodCount = 0, perfectCount = 0, comboCount = 0, maxCombo = 0;
        private List<int> comboSegmentScores = new List<int>();
        private void ResetCounts(){
            missCount = badCount = goodCount = perfectCount = comboCount = maxCombo = 0;
            comboSegmentScores.Clear();
            IngameUIController.Instance.WriteMiss(0);
            IngameUIController.Instance.WriteBad(0);
            IngameUIController.Instance.WriteGood(0);
            IngameUIController.Instance.WritePerfect(0);
            IngameUIController.Instance.WriteCombo(0);
        }
        public void RegisterMiss(){
            missCount++;
            comboSegmentScores.Add(comboCount * (comboCount - 1) / 2);
            maxCombo = Math.Max(maxCombo, comboCount);
            comboCount = 0;
            IngameUIController.Instance.WriteMiss(missCount);
            IngameUIController.Instance.WriteCombo(0);
        }

        public void RegisterBad(){
            badCount++;
            comboCount++;
            IngameUIController.Instance.WriteBad(badCount);
            IngameUIController.Instance.WriteCombo(comboCount);

        }

        public void RegisterGood(){
            goodCount++;
            comboCount++;
            IngameUIController.Instance.WriteGood(goodCount);
            IngameUIController.Instance.WriteCombo(comboCount);

        }

        public void RegisterPerfect(){
            perfectCount++;
            comboCount++;
            IngameUIController.Instance.WritePerfect(perfectCount);
            IngameUIController.Instance.WriteCombo(comboCount);
        }

        /// <summary>
        /// Compute total score. The implementation follows the specifications. Note that Linq is used for easy summation.
        /// </summary>
        public int ComputeScore(){
            comboSegmentScores.Add(comboCount * (comboCount - 1) / 2); // Case where the player ends the song with an active combo.
            int totalNotes = notes.Count();
            const int comboCoefficient = 20000;
            const int noteCoefficient = 180000;
            float noteScore = (1.0f * perfectCount + 0.7f * goodCount + 0.3f * badCount) / totalNotes * noteCoefficient;
            float comboScoreSum = comboSegmentScores.Sum();
            float comboScore = comboScoreSum / (totalNotes * (totalNotes - 1) / 2) * comboCoefficient;
            return (int) Mathf.Floor(comboScore + noteScore);
        }

        #endregion

        // Start is called before the first frame update
        void Start()
        {
            if (Instance == null) Instance = this;
            else
            {
                throw new UnityException("Singleton SongManager instantiated twice!");
            }

            globalAudio = GetComponent<AudioSource>();

            Assert.IsNotNull(globalAudio, "Audio Source of Song Manager not set!");

            if(loadDefaultBeatmapOnLoad)
                LoadAndExecuteBeatmap(defaultBeatmap);

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
            songPosition = (float)(AudioSettings.dspTime - dsptimesong - offsetFromFirstBeat);

            //calculate the position in beats
            songPosInBeats = songPosition / SecPerBeat;
        }
    }

}
