using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts{
    public abstract class UIWidget : MonoBehaviour {
        
        public bool isActivated = false;
        public void Activate(){
            if(isActivated) return;
            isActivated = true;
            SongManager.Instance.playUIactivate();
            OnActivate();
        }

        public abstract void OnActivate();
        public void Deactivate(){
            if(!isActivated) return;
            isActivated = false;
            SongManager.Instance.playUIdeactivate();
            OnDeactivate();
        }

        public abstract void OnDeactivate();

    }

}