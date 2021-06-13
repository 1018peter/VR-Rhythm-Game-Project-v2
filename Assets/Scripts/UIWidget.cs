using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts{
    public abstract class UIWidget : MonoBehaviour {
        public int activation = 0;
        public bool isActivated{
            get => activation > 0;
        }
        public void Activate(){
            if(!isActivated){
                SongManager.Instance.playUIactivate();
                OnActivate();
            }
            activation++;
        }

        public abstract void OnActivate();
        public void Deactivate(){
            activation--;
            if(activation < 0) activation = 0;
            if(!isActivated) {
                SongManager.Instance.playUIdeactivate();
                OnDeactivate();
            }
        }

        public abstract void OnDeactivate();

    }

}