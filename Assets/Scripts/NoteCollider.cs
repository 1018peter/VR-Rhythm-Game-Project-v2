using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts{
    public class NoteCollider : MonoBehaviour
    {
        public NoteController attachedNote;   
        // Start is called before the first frame update
        void Start()
        {
            if(attachedNote == null)
                attachedNote = transform.parent.GetComponentInChildren<NoteController>();
        }

        private void OnTriggerEnter(Collider other) {
            if(other.CompareTag("Player")){
                this.enabled = false;
                attachedNote.HitCheck();
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}

