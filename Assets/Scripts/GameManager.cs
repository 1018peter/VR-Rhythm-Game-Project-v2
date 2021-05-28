using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

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

        public void GoToMainMenu(){
            Debug.Log("Go to main menu");
            if(currentMenu == mainMenu) return;
            if(currentMenu != null) currentMenu.Close();
            currentMenu = mainMenu;
            state = GameState.MainMenu;
            currentMenu.Open();
        }

        public void GoToSettings(){
            Debug.Log("Go to settings");
            if(currentMenu == settings) return;
            if(currentMenu != null) currentMenu.Close();
            currentMenu = settings;
            state = GameState.Settings;
            currentMenu.Open();
        }

        public void GoToTrackSelect(){
            Debug.Log("Go to track select");
            if(currentMenu == trackSelect) return;
            if(currentMenu != null) currentMenu.Close();
            currentMenu = trackSelect;
            state = GameState.SelectTrack;
            currentMenu.Open();
        }

        public void GoToIngame(){
            Debug.Log("Go to ingame");
            if(currentMenu != null) currentMenu.Close();
            if(currentMenu == ingame) return;
            state = GameState.Ingame;
            currentMenu = ingame;
            StartCoroutine(SongManager.Instance.orbitManager.RotateAllToInitial(3.0f, delegate () {
                StartCoroutine(SongManager.Instance.ExecuteBeatmap());
            }));
            currentMenu.Open();

        }

        public void GoToResults(){
            Debug.Log("Go to results");
            if(currentMenu != null) currentMenu.Close();
            if(currentMenu == results) return;
            state = GameState.Results;
            currentMenu = results;
            
            StartCoroutine(SongManager.Instance.orbitManager.RotateAllToSideways(1.0f, delegate () {
                results.Open();
            }));
        }

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

        }
    }
}