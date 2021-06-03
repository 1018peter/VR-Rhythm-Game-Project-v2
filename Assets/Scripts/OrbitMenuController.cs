using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.EventSystems;

namespace Assets.Scripts{
    public class OrbitMenuController : MonoBehaviour
    {
        private List<OrbitUIController> UIelements = new List<OrbitUIController>();
        public bool isOpen = false;
        
        public void Open(){
            if(isOpen) return;
            gameObject.SetActive(true);
            // animator.Play("Menu Appear", 0);
            System.Action<ITween<Vector3>> updatePos = (t) => {
                transform.localPosition = t.CurrentValue;
            };
            System.Action<ITween<Vector3>> callback = t => {
                // AfterOpen();
            };
            
            foreach(var UIelement in UIelements){
                UIelement.activated = true;
            }
            var start = new Vector3(0, -11.32f, 0);
            gameObject.Tween("OpenMenu", start, Vector3.zero, 0.5f, TweenScaleFunctions.QuadraticEaseInOut, updatePos, callback);
            isOpen = true;

        }

        public void AfterOpen(){
            foreach(var UIelement in UIelements){
                UIelement.activated = true;
            }

        }

        public void Close(System.Action callback){
            if(!isOpen) return;
            foreach(var UIelement in UIelements){
                UIelement.activated = false;
            }
            
            System.Action<ITween<Vector3>> updatePos = (t) => {
                transform.localPosition = t.CurrentValue;
            };

            System.Action<ITween<Vector3>> disableAfterClose = (t) => {
                gameObject.SetActive(false);
                callback();
            };


            var end = new Vector3(0, -11.32f, 0);
            gameObject.Tween("CloseMenu", Vector3.zero, end, 0.5f, TweenScaleFunctions.QuadraticEaseInOut, updatePos, disableAfterClose);
            isOpen = false;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            if(isOpen){
                transform.localPosition = Vector3.zero;
                gameObject.SetActive(true);
            }
            else{
                transform.localPosition = new Vector3(0, -11.32f, 0);
                gameObject.SetActive(false);
            }
            UIelements = new List<OrbitUIController>(GetComponentsInChildren<OrbitUIController>());
            Debug.Log($"Found {UIelements.Count} UI elements in {gameObject.name}");

        }


        // Update is called once per frame
        void Update()
        {
            
        }

    }

}
