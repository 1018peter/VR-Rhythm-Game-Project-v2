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

        public bool selected = false;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        

        private void OnTriggerEnter(Collider other) {
            if(!activated) return;

            if(!selected){
                Debug.Log("UI Selected");
                onSelect.Invoke();
                selected = true;
                widget.Activate();
            }
            else{
                onConfirm.Invoke();
                selected = false;
                widget.Deactivate();
            }
        }

        private void OnTriggerExit(Collider other) {
            if(!activated) return;
            
            Debug.Log("UI Un-triggered");
            widget.Deactivate();
            selected = false;
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }

}