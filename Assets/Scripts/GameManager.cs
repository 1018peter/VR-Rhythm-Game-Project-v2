using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using OVR;

namespace Assets.Scripts
{
    /// <summary>
    /// The global game state. Allowing single-scene game structure.
    /// </summary>
    public enum GameState{
        Title, 
        MainMenu, 
        SelectTrack, 
        Settings, 
        Ingame, 
        Results,
    }

    /// <summary>
    /// Singleton class that manages global variables and references.
    /// Access with GameManager.handle
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // handle to the singleton class.
        public static GameManager Instance;

        public static GameState state = GameState.Title;

        public OrbitMenuController mainMenu;
        public OrbitMenuController settings;
        public OrbitMenuController trackSelect;

        public OrbitMenuController ingame;
        public OrbitMenuController results;
        private OrbitMenuController _currentMenu = null;
        public OrbitMenuController currentMenu{
            get{
                return _currentMenu;
            }
            private set{
                _currentMenu = value;
            }
        }

        public OrbitManager orbitManager;

        [Tooltip("The player GameObject.")]
        public GameObject player;
        public Transform playerLeftController;

        public Transform playerRightController;

        [Tooltip("The reference transform of the player's VR device.")]
        public Transform playerDeviceTransform;


        [Tooltip("The maximum distance the player can reach with their hands, relative to the player gameObject. (A rough estimate)")]
        public float playerMaxReach = 1.0f;

        #region Menu Controls

        public float gripRotateAngularSpeed = 10;

        private Quaternion gripRotateStart = Quaternion.Euler(0, 0, 0);
        private Vector3 rightGripRotateOrigin = new Vector3(0, 0, -2.15f);
        private Vector3 leftGripRotateOrigin = new Vector3(0, 0, -2.15f);
        private bool rightGripRotateEnabled = false;
        private bool leftGripRotateEnabled = false;
        public void RightGripRotate(InputAction.CallbackContext callbackContext){
            if(currentMenu == ingame) return;
            if(callbackContext.started && !leftGripRotateEnabled){ // Start
                Debug.Log("RightGripRotate Start");
                rightGripRotateEnabled = true;
                rightGripRotateOrigin = playerRightController.position;
                gripRotateStart = currentMenu.transform.localRotation;
            }
            else if(callbackContext.canceled){ // End
                Debug.Log("RightGripRotate End");
                rightGripRotateEnabled = false;
            }
        }
        public void LeftGripRotate(InputAction.CallbackContext callbackContext){
            if(currentMenu == ingame) return;
            if(callbackContext.started && !rightGripRotateEnabled){ // Start
                Debug.Log("LeftGripRotate Start");
                leftGripRotateEnabled = true;
                leftGripRotateOrigin = playerLeftController.position;
                gripRotateStart = currentMenu.transform.localRotation;
            }
            else if(callbackContext.canceled){ // End
                Debug.Log("LeftGripRotate End");
                leftGripRotateEnabled = false;
            }
        }

        #endregion

        #region Menu State Transition Functions

        public void LeftConfirm(){
            OrbitUIController.LeftConfirm();
        }

        public void RightConfirm(){
            OrbitUIController.RightConfirm();
        }

        public void GoToMainMenu(){
            Debug.Log("Go to main menu");
            leftGripRotateEnabled = false;
            rightGripRotateEnabled = false;
            if(currentMenu == mainMenu) return;
            if(currentMenu != null){ 
                currentMenu.Close(delegate () {
                    currentMenu = mainMenu;
                    state = GameState.MainMenu;
                    currentMenu.Open();
                });
            }
            else{
                currentMenu = mainMenu;
                state = GameState.MainMenu;
                currentMenu.Open();
            }
            
        }

        public void GoToSettings(){
            Debug.Log("Go to settings");
            leftGripRotateEnabled = false;
            rightGripRotateEnabled = false;
            if(currentMenu == settings) return;
            if(currentMenu != null){ 
                currentMenu.Close(delegate () {
                    currentMenu = settings;
                    state = GameState.Settings;
                    currentMenu.Open();
                });
            }
            else{
                currentMenu = settings;
                state = GameState.Settings;
                currentMenu.Open();
            }
        }

        public void GoToTrackSelect(){
            Debug.Log("Go to track select");
            leftGripRotateEnabled = false;
            rightGripRotateEnabled = false;
            if(currentMenu == trackSelect) return;
            if(currentMenu != null){ 
                currentMenu.Close(delegate () {
                    currentMenu = trackSelect;
                    state = GameState.SelectTrack;
                    currentMenu.Open();
                });
            }
            else{
                currentMenu = trackSelect;
                state = GameState.SelectTrack;
                currentMenu.Open();
            }
            
        }

        public void GoToIngame(){
            Debug.Log("Go to ingame");
            leftGripRotateEnabled = false;
            rightGripRotateEnabled = false;
            if(currentMenu == ingame) return;
            if(currentMenu != null){ 
                currentMenu.Close(delegate () {
                    state = GameState.Ingame;
                    currentMenu = ingame;
                    currentMenu.Open();
                });
            }
            else{
                state = GameState.Ingame;
                currentMenu = ingame;
                currentMenu.Open();
            }
            StartCoroutine(SongManager.Instance.orbitManager.RotateAllToInitial(3.0f, delegate () {
                StartCoroutine(SongManager.Instance.ExecuteBeatmap());
            }));

        }

        public void GoToResults(){
            Debug.Log("Go to results");
            leftGripRotateEnabled = false;
            rightGripRotateEnabled = false;
            if(currentMenu == results) return;
            if(currentMenu != null){ 
                currentMenu.Close(delegate () {
                    state = GameState.Results;
                    currentMenu = results;
                    
                    StartCoroutine(SongManager.Instance.orbitManager.RotateAllToSideways(1.0f, delegate () {
                        results.Open();
                    }));
                });
            }
            else{
                state = GameState.Results;
                currentMenu = results;
                
                StartCoroutine(SongManager.Instance.orbitManager.RotateAllToSideways(1.0f, delegate () {
                    results.Open();
                }));
            }
        }
        #endregion

        void Awake(){
            Debug.Log("Awake");
            if (Instance == null) Instance = this;
            else
            {
                throw new UnityException("Singleton GameManager instantiated twice!");
            }

        }

        // Use this for initialization
        void Start()
        {
            Assert.IsNotNull(player);
            GameManager.state = GameState.Title;
            SongManager.Instance.orbitManager.orbitTransformB.rotation = 
            SongManager.Instance.orbitManager.orbitTransformG.rotation = 
            SongManager.Instance.orbitManager.orbitTransformR.rotation = Quaternion.Euler(-90, 0, 0);
            GoToMainMenu();
        }

        // Update is called once per frame
        void Update()
        {
            if(rightGripRotateEnabled && !leftGripRotateEnabled){ // RightGripRotate
                var theta = Vector3.SignedAngle(new Vector3(rightGripRotateOrigin.x, 0, rightGripRotateOrigin.z), new Vector3(playerRightController.position.x, 0, playerRightController.position.z), Vector3.up);
                currentMenu.transform.localRotation = Quaternion.Euler(0, gripRotateStart.eulerAngles.y + theta, 0);
                
            }
            else if(leftGripRotateEnabled && !rightGripRotateEnabled){ // LeftGripRotate
                var theta = Vector3.SignedAngle(new Vector3(leftGripRotateOrigin.x, 0, leftGripRotateOrigin.z), new Vector3(playerLeftController.position.x, 0, playerLeftController.position.z), Vector3.up);
                currentMenu.transform.localRotation = Quaternion.Euler(0, gripRotateStart.eulerAngles.y + theta, 0);
                
            }

        }
    }
}