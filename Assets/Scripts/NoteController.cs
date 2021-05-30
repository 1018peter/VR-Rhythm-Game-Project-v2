using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
namespace Assets.Scripts
{
    public class NoteController : MonoBehaviour
    {
        Animator anim;
        float beatPos;

        private bool hasBeenHit = false;

        public GameObject destroyTarget;

        // Use this for initialization
        void Start()
        {
            beatPos = SongManager.Instance.songPosInBeats;
            anim = GetComponent<Animator>();
            Assert.IsNotNull(anim);
        }


        private void HitCheck(){
            float delta = (SongManager.Instance.songPosInBeats - beatPos) / SongManager.Instance.beatsShownInAdvance;
            if(delta > 2 || delta < -1){
                SongManager.Instance.RegisterMiss();
            }
            else if(delta > 1.25 || delta < -0.5){
                SongManager.Instance.RegisterBad();
            }
            else if(delta > 0.5 || delta < -0.25){
                SongManager.Instance.RegisterGood();
            }
            else{
                SongManager.Instance.RegisterPerfect();
            }
            

        }

        // Use this for note hit logic. Whatever the player is using to hit the note with should be tagged with "Player" and have a collider.
        private void OnTriggerEnter(Collider other) {
            if(other.CompareTag("Player") && !hasBeenHit){
                hasBeenHit = true;
                HitCheck();
                if(destroyTarget == null)
                    Destroy(this.gameObject);
                else Destroy(destroyTarget);
            }   
        }

        // Update is called once per frame
        void Update()
        {
            float t = (SongManager.Instance.songPosInBeats - beatPos) / SongManager.Instance.beatsShownInAdvance;
            anim.SetFloat("EntryTime", t);
            anim.SetFloat("ExitTime", (t - 1) * 2);
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("End"))
            {
                SongManager.Instance.RegisterMiss();
                if(destroyTarget == null)
                    Destroy(this.gameObject);
                else Destroy(destroyTarget);
            }


        }
    }
}