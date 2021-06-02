﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts{
    public class NoteCollider : MonoBehaviour
    {
        public NoteController attachedNote;   
        public bool scaleWithAttachedNote = true;
        // Start is called before the first frame update
        void Start()
        {
            float scaleFactor = GameManager.Instance.playerMaxReach / Mathf.Abs(transform.localPosition.y);
            transform.parent.localScale = new Vector3(1, scaleFactor, 1);
            if(attachedNote == null)
                attachedNote = transform.parent.GetComponentInChildren<NoteController>();
            if(scaleWithAttachedNote)
                transform.localScale = attachedNote.transform.localScale * scaleFactor;
            else
                transform.localScale = transform.parent.localScale;
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
            float scaleFactor = GameManager.Instance.playerMaxReach / Mathf.Abs(transform.localPosition.y);
            transform.parent.localScale = new Vector3(1, scaleFactor, 1);
            if(scaleWithAttachedNote)
                transform.localScale = attachedNote.transform.localScale * scaleFactor;
            else
                transform.localScale = transform.parent.localScale;
        }
    }
}
