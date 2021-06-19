using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
namespace Assets.Scripts
{
    public enum NoteType{
        OrbitNote, TapNote
    }
    public class NoteController : MonoBehaviour
    {
        Animator anim;
        float beatPos;

        public NoteType noteType;

        private bool hasBeenHit = false;

        public GameObject destroyTarget;

        // Use this for initialization
        void Start()
        {
            beatPos = SongManager.Instance.songPosInBeats;
            anim = GetComponent<Animator>();
            Assert.IsNotNull(anim);
        }


        public void HitCheck(){
            if(hasBeenHit) return;
            hasBeenHit = true;
            if(noteType == NoteType.OrbitNote){
                SongManager.Instance.createONParticlesOnHit(transform.position, transform.rotation);
            }
            else {
                SongManager.Instance.createTNParticlesOnHit(transform.position, transform.rotation);
            }
            
            float delta = (SongManager.Instance.songPosInBeats - beatPos) / SongManager.Instance.beatsShownInAdvance;
            if(delta > 0.9 || delta < -0.5){
                SongManager.Instance.RegisterBad();
            }
            else if(delta > 0.5 || delta < -0.3){
                SongManager.Instance.RegisterGood();
            }
            else{
                SongManager.Instance.RegisterPerfect();
            }
            
            if(destroyTarget == null)
                Destroy(this.gameObject);
            else Destroy(destroyTarget);

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