using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts{

    public class HoloScreen : UIWidget
    {
        private Animator animator;

        public override void OnActivate(){
            animator.Play("Panel Appear", 0);
        }

        public override void OnDeactivate(){
            animator.Play("Panel Disappear", 0);
        }



        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            transform.localScale = new Vector3(0, 0, 1);
        }


        // Update is called once per frame
        void Update()
        {
            
        }
    }

}