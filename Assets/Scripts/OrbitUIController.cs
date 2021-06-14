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

        public UnityEvent onDeselect;
        public UnityEvent onConfirm;
        public bool activated = true;

        public bool deactivateWidgetOnConfirm = true;


        public static OrbitUIController rightSelected = null;
        public static OrbitUIController leftSelected = null;

        public static void RightConfirm(){
            if(rightSelected != null){
                SongManager.Instance.playUIconfirm();
                rightSelected.onConfirm.Invoke();
                if(rightSelected.deactivateWidgetOnConfirm){
                    rightSelected.widget.Deactivate();
                    leftSelected = null;
                    rightSelected = null;
                }
            }
            else{
                GameManager.Instance.debugDisplay.text = "Right Confirm rejected: Nothing selected.";
            }
        }

        public static void LeftConfirm(){
            if(leftSelected != null){
                SongManager.Instance.playUIconfirm();
                leftSelected.onConfirm.Invoke();
                if(leftSelected.deactivateWidgetOnConfirm){
                    leftSelected.widget.Deactivate();
                    leftSelected = null;
                    rightSelected = null;
                }
            }
            else{
                GameManager.Instance.debugDisplay.text = "Left Confirm rejected: Nothing selected.";
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        

        private void OnTriggerEnter(Collider other) {
            if(!activated) return;
            if(other.gameObject.name.StartsWith("Left") && leftSelected != this){
                GameManager.Instance.debugDisplay.text = "UI Selected";
                Debug.Log("UI Selected");
                onSelect.Invoke();
                leftSelected = this;
                widget.Activate();
            }
            else if(other.gameObject.name.StartsWith("Right") && rightSelected != this){
                GameManager.Instance.debugDisplay.text = "UI Selected";
                Debug.Log("UI Selected");
                onSelect.Invoke();
                rightSelected = this;
                widget.Activate();
            }
        }

        private void OnTriggerExit(Collider other) {
            
            GameManager.Instance.debugDisplay.text = "UI Un-triggered";
            Debug.Log("UI Un-triggered");
            onDeselect.Invoke();
            if(other.gameObject.name.StartsWith("Left")){
                widget.Deactivate();
                if(leftSelected == this)
                    leftSelected = null;
            }
            else if(other.gameObject.name.StartsWith("Right")){
                widget.Deactivate();
                if(rightSelected == this)
                    rightSelected = null;
            }

        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }

}