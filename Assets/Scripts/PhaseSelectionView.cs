using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseSelectionView : MonoBehaviour {

    // 0 indicates training, 1 indicates testing
    private int selected_phase = 0;
    public void setPhase(int phase) {
        this.selected_phase = phase;
        refreshView();
    }
    private void refreshView() {
        if (selected_phase == 0)
        {
            GameObject.Find("training").GetComponent<TextMesh>().color = Color.blue;
            GameObject.Find("testing").GetComponent<TextMesh>().color = Color.white;
        }
        else {
            GameObject.Find("training").GetComponent<TextMesh>().color = Color.white;
            GameObject.Find("testing").GetComponent<TextMesh>().color = Color.blue;
        }
    }
    public void selectTraining() {
        selected_phase = 0;
        refreshView();
    }
    public void selectTesting() {
        selected_phase = 1;
        refreshView();
    }
    public int getSelectedPhase() {
        return selected_phase;
    }
	// Use this for initialization
	void Start () {
        selected_phase = 0;
        refreshView();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
