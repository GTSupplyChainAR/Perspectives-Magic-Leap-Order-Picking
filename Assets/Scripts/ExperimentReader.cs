using System.IO;
using UnityEngine;
using System;

public class ExperimentReader {
    private Participants participants;
    private Participant[] participantsArr;
    private int participantId;
    private PathIds pathIds;
    private PickPaths paths;
    private PickPath[] patharr;

    public ExperimentReader(string filePath)
    {
        filePath = Path.Combine(Application.streamingAssetsPath, filePath);
        if (File.Exists(filePath))
        {
            string dataAsJSON = File.ReadAllText(filePath);
            paths = JsonUtility.FromJson<PickPaths>(dataAsJSON);
            Debug.Log(paths);
            patharr = paths.pickPaths;
            Debug.Log(patharr);

            Debug.Log(patharr.Length);
            foreach (PickPath path in patharr) {
                Debug.Log(path);
            }
        } else
        {
            throw new FileNotFoundException(filePath + " cannot be found.");
        }
    }
}

[System.Serializable]
public class Participants
{
    public Participant[] participants;
}

[System.Serializable]
public class Participant
{
    public int participantId;
    public TrainingPathOrder[] trainingPathOrder;
    public TestingPathOrder[] testingPathOrder;
}

[System.Serializable]
public class TrainingPathOrder
{
    public string position;
    public PathIds[] pathIds;
}

[System.Serializable]
public class TestingPathOrder
{
    public string position;
    public PathIds[] pathIds;
}

[System.Serializable]
public class PathIds
{
    public int pathId;
}