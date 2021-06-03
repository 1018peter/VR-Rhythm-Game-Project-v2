using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
namespace Assets.Scripts{
    public class OrbitUIController : MonoBehaviour
    {
        public UIWidget widget;
        public UnityEvent onSelect;
        public UnityEvent onConfirm;
        public bool activated = true;


        public static OrbitUIController rightSelected = null;
        public static OrbitUIController leftSelected = null;

        public static void RightConfirm(){
            if(rightSelected != null){
                rightSelected.onConfirm.Invoke();
                rightSelected.widget.Deactivate();
                leftSelected = null;
                rightSelected = null;
            }
        }

        public static void LeftConfirm(){
            if(leftSelected != null){
                leftSelected.onConfirm.Invoke();
                leftSelected.widget.Deactivate();
                leftSelected = null;
                rightSelected = null;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        

        private void OnTriggerEnter(Collider other) {
            if(!activated) return;

            Debug.Log("UI Selected");
            onSelect.Invoke();
            if(other.gameObject.name.StartsWith("Left")){
                leftSelected = this;
            }
            else{
                rightSelected = this;
            }
            widget.Activate();
        }

        private void OnTriggerExit(Collider other) {
            if(!activated) return;
            
            Debug.Log("UI Un-triggered");
            if(other.gameObject.name.StartsWith("Left")){
                widget.Deactivate();
                leftSelected = null;
            }
            else{
                widget.Deactivate();
                rightSelected = null;
            }

        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }

}