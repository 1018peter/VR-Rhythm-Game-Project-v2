using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
namespace Assets.Scripts
{
    public class NoteController : MonoBehaviour
    {
        Animator anim;
        float beatPos;

        static readonly float[] intervalList = { -0.8f, -0.5f, -0.3f, -0.1f, 0.1f, 0.2f };
        // Use this for initialization
        void Start()
        {
            beatPos = SongManager.handle.songPosInBeats;
            anim = GetComponent<Animator>();
            Assert.IsNotNull(anim);
        }


        private void HitCheck(){
            // delta = t - 1
            float delta = (SongManager.handle.songPosInBeats - beatPos) / SongManager.handle.beatsShownInAdvance - 1;
            int lowerBound = 0;
            int upperBound = intervalList.Length;
            // Performs an upper bound search.
            while(lowerBound < upperBound){
                int middle = (lowerBound + upperBound) / 2;
                if(intervalList[middle] < delta){
                    lowerBound = middle + 1;
                }
                else {
                    upperBound = middle;
                }
            }
            HitRank h = HitRank.Late;
            switch(upperBound){
                case 0: // delta < -0.8f, Early
                Debug.Log("Hit: Early");
                h = HitRank.Early;
                break;
                case 1: // -0.8f <= delta < -0.5f, OK
                Debug.Log("Hit: OK");
                h = HitRank.OK;
                break;
                case 2: // -0.5f <= delta < -0.3f, Nice
                Debug.Log("Hit: Nice");
                h = HitRank.OK;
                break;
                case 3: // -0.3f <= delta < -0.1f, Great
                Debug.Log("Hit: Great");
                h = HitRank.Great;
                break;
                case 4: // -0.1f <= delta < 0.1f, Perfect
                Debug.Log("Hit: Perfect");
                h = HitRank.Perfect;
                break;
                case 5: // 0.1f <= delta < 0.2f, Great
                Debug.Log("Hit: Great");
                h = HitRank.Great;

                break;
                case 6: // 0.2f <= delta, Late
                Debug.Log("Hit: Late");
                h = HitRank.Late;

                break;
                default:
                Debug.LogError("Interval search failed with invalid upperBound: " + upperBound);
                break;
            }
            SongManager.handle.ScoreHit(h);
        }

        // Use this for note hit logic. Whatever the player is using to hit the note with should be tagged with "Player" and have a collider.
        private void OnTriggerEnter(Collider other) {
            Debug.Log("trigger");
            if(other.CompareTag("Player")){
                HitCheck();
                Destroy(this.gameObject);
            }   
        }

        // Update is called once per frame
        void Update()
        {
            float t = (SongManager.handle.songPosInBeats - beatPos) / SongManager.handle.beatsShownInAdvance;
            anim.SetFloat("EntryTime", t);
            anim.SetFloat("ExitTime", (t - 1) * 2);
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("End"))
            {
                Destroy(this.gameObject);
            }


        }
    }
}