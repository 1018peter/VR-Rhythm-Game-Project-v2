using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Assets.Scripts{
    public class IngameUIController : MonoBehaviour
    {
        public TextMeshPro missText, badText, goodText, perfectText, comboText;
        public static IngameUIController Instance;
        public void WriteMiss(int newCount){
            missText.text = newCount.ToString("D3");
        }

        public void WriteBad(int newCount){
            badText.text = newCount.ToString("D3");
        }

        public void WriteGood(int newCount){
            goodText.text = newCount.ToString("D3");
        }

        public void WritePerfect(int newCount){
            perfectText.text = newCount.ToString("D3");
        }

        public void WriteCombo(int newCount){
            comboText.text = newCount.ToString("D3");
        }

        // Start is called before the first frame update
        void Start()
        {
            if(Instance == null){
                Instance = this;
            }
            else{
                throw new UnityException("Singleton ResultsUIController instantiated more than once.");
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }


}