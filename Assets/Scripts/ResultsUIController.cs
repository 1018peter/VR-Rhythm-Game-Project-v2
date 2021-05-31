using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Assets.Scripts{
public class ResultsUIController : MonoBehaviour
{
    public TextMeshPro scoreText, missText, badText, goodText, perfectText, maxComboText;
    public static ResultsUIController Instance;

    /// <summary>
    /// Write results of SongManager into Results UI.
    /// </summary>
    public void Write(){
        BeatmapRecord record = SongManager.Instance.results;
        scoreText.text = record.score.ToString("D6");
        missText.text = record.missCount.ToString("D3");
        badText.text = record.badCount.ToString("D3");
        goodText.text = record.goodCount.ToString("D3");
        perfectText.text = record.perfectCount.ToString("D3");
        maxComboText.text = record.maxCombo.ToString("D3");
    }

    private void Awake() {
        
        if(Instance == null){
            Instance = this;
        }
        else{
            throw new UnityException("Singleton ResultsUIController instantiated more than once.");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}


}