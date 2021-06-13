using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace Assets.Scripts{

    /// <summary>
    /// This component will activate and deactivate all children UIWidgets if you activate/deactivate it.
    /// </summary>
    public class UIWidgetGroup : UIWidget
    {

        private List<UIWidget> widgets;
        public override void OnActivate(){
            foreach(var widget in widgets){
                widget.Activate();
            }
        }

        public override void OnDeactivate(){
            foreach(var widget in widgets){
                widget.Deactivate();
            }
            
        }



        // Start is called before the first frame update
        void Start()
        {
            widgets = GetComponentsInChildren<UIWidget>().Where((UIWidget widget) => widget != this).ToList<UIWidget>();
            foreach(var widget in widgets){
                widget.OnDeactivate();
            }
            // OnDeactivate();
        }


        // Update is called once per frame
        void Update()
        {
            
        }
    }

}