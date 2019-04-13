using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class JSON : MonoBehaviour {

    // Use this for initialization
    private string JSONFileName = "JSONFiles/pick-paths.json";
	void Start () {
        Debug.Log("JSON");
        ParseJSONDemo();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private bool ParseJSONDemo() {
        PathReader pr;
        try
        {
            pr = new PathReader(Path.Combine(Application.dataPath, JSONFileName));
            Debug.Log("parse");
        }
        catch (Exception e) {
            Debug.Log(e.Message);
            return false;
        }
        pr.setPathId(0, 0, 2);
        pr.printBookWithLocation(pr.getBookWithLocation(2));
        return true;

    }
}

