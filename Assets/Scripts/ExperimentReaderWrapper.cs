using System.IO;
using UnityEngine;
using System;

public class ExperimentReaderWrapper: MonoBehaviour {

    void Start() {
        ExperimentReader reader = new ExperimentReader("experiments.json");
    }

}