using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSelectionView : MonoBehaviour {

    // Use this for initialization
    private string []usernames;
    private int sliding_window_left = 0;
    private int window_size = 6;
    private int selected_user_index = 0;
    private int character_width = 0;
    public void init() {
        usernames = new string[] {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"};
       
    }
    private string alignUsername(string username) {
        if (username == null) {
            return "";
        }
        if (username.Length > character_width) {
            return username;
        }
        int left_padding = (character_width - username.Length) / 2;
        string result = "";
        for (; left_padding > 0; left_padding--) {
            result += " ";
        }
        result += username;
        while(result.Length < character_width) {
            result += " ";
        }
        return result;
    }
    private void refreshView() {
        int count = 0;
        for (int i = sliding_window_left; i - sliding_window_left < window_size && i < usernames.Length; i++) {
            GameObject.Find("user_" + count).GetComponent<TextMesh>().text = alignUsername(usernames[i]);
            if (i == selected_user_index) {
                GameObject.Find("user_" + count).GetComponent<TextMesh>().color = Color.blue;
            } else {
                GameObject.Find("user_" + count).GetComponent<TextMesh>().color = Color.white;
            }
            count++;
        }
    }
    public void selectNext() {
        // 1. add 1 to selected_user_index
        // 2. check if need to moving the sliding window
        if (selected_user_index + 1 >= usernames.Length) {
            return;
        }
        selected_user_index++;
        if (selected_user_index - sliding_window_left >= window_size) {
            sliding_window_left++;
        }
        refreshView();
    }
    public void selectLast() {
        if (selected_user_index <= 0) {
            return;
        }
        selected_user_index--;
        if (selected_user_index < sliding_window_left) {
            sliding_window_left = selected_user_index;
        }
        refreshView();
    }
    public int getSelectedUserId() {
        return selected_user_index;
    }
	void Start () {
        init();
        character_width = GameObject.Find("prompt").GetComponent<TextMesh>().text.Length;
        refreshView();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
