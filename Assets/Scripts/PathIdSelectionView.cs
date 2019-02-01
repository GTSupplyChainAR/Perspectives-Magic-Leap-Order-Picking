using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathIdSelectionView : MonoBehaviour {

    private int phase = 0; // 0 indicates training, 1 indiciates testing
    private int sliding_window_left = 1;
    private int selected_pathid = 1; // from 1
    private int window_size = 10;
    private int training_num = 20;
    private int testing_num = 20;
    // Use this for initialization

    private void refreshView() {
        int count = 0;
        for (int i = sliding_window_left; i - sliding_window_left < window_size; i++) {
            GameObject.Find("path_" + count).GetComponent<TextMesh>().text = "Path - " + i;
            if (i == selected_pathid)
            {
                GameObject.Find("path_" + count).GetComponent<TextMesh>().color = Color.blue;
            }
            else
            {
                GameObject.Find("path_" + count).GetComponent<TextMesh>().color = Color.white;
            }
            count++;
        }
    }
    public void setPhase(int phase) {
        this.phase = phase;
        if (this.phase == 0)
        {
            this.selected_pathid = 1;
            this.sliding_window_left = 1;
        }
        else {
            this.selected_pathid = this.training_num + 1;
            this.sliding_window_left = this.selected_pathid;
        }
        refreshView();
    }
    public void selectNext() {
        if (this.phase == 0 && this.selected_pathid >= this.training_num) {
            return;
        }
        if (this.phase == 1 && this.selected_pathid >= this.training_num + this.testing_num) {
            return;
        }
        this.selected_pathid += 1;
        if (selected_pathid - sliding_window_left >= window_size) {
            sliding_window_left++;
        }
        

        refreshView();
    }
    public void selectLast() {
        if (this.phase == 0 && this.selected_pathid <= 1)
        {
            return;
        }
        if (this.phase == 1 && this.selected_pathid <= this.training_num + 1)
        {
            return;
        }
        this.selected_pathid -= 1;
        if (selected_pathid < sliding_window_left)
        {
            sliding_window_left = selected_pathid;
        }

        refreshView();
    }
    public int getSelectedPathId() {
        return this.selected_pathid;
    }
	void Start () {
        refreshView();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
