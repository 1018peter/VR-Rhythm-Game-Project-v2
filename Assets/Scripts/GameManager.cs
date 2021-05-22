using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;


namespace Assets.Scripts
{

    /// <summary>
    /// Singleton class that manages global variables and references.
    /// Access with GameManager.handle
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // handle to the singleton class.
        public static GameManager Instance;

        public OrbitManager orbitManager;

        [Tooltip("The player GameObject.")]
        public GameObject player;
        public Transform playerLeftController;

        public Transform playerRightController;

        [Tooltip("The reference transform of the player's VR device.")]
        public Transform playerDeviceTransform;


        [Tooltip("The maximum distance the player can reach with their hands, relative to the player gameObject. (A rough estimate)")]
        public float playerMaxReach = 1.0f;

        // The distance that separates "Near" and "Far" areas.
        public float nearBorder => playerMaxReach / 2;
    

        private float ComputeDistanceToSource(Transform t){
            return (t.position - SongManager.handle.transform.position).magnitude;
        }

        public bool IsNear(Transform t){
            return ComputeDistanceToSource(t) <= nearBorder;
        }

        public bool IsLeftControllerNear(){
            return IsNear(playerLeftController);
        }

        public bool IsRightControllerNear(){
            return IsNear(playerRightController);
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

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}