using System.IO;
using UnityEngine;
using System;

public class ExperimentReader {
    private Participants participants;
    private Participant[] participantsArr;
    private int participantId;
    private PathIds pathIds;

    public ExperimentReader(string filePath)
    {
        filePath = Path.Combine(Application.streamingAssetsPath, filePath);
        if (File.Exists(filePath))
        {
            string dataAsJSON = File.ReadAllText(filePath);
            participants = JsonUtility.FromJson<Participants>(dataAsJSON);
            Debug.Log(participants);
            participantsArr = participants.participants;
            Debug.Log(participantsArr);

            Debug.Log(participantsArr.Length);
            foreach (Participant participant in participantsArr) {
                Debug.Log(participant);
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

    public override string ToString() {
        return "boi";
    }
}

[System.Serializable]
public class Participant
{
    public int participantId;
    public TrainingPathOrder[] trainingPathOrder;
    public TestingPathOrder[] testingPathOrder;
    
    public override string ToString() {
        return "Participant: {\n"
            + "ID = " + participantId.ToString() + "\n"
            + "trainingPathOrder = " + trainingPathOrder.ToString() + "\n"
            + "testingPathOrder = " + testingPathOrder.ToString()
            + "}";
    }
}

[System.Serializable]
public class TrainingPathOrder
{
    public string position;
    public PathIds[] pathIds;

    public override string ToString() {
        return "TrainingPathOrder: {\n"
            + "position = " + position + "\n"
            + "pathIds = " + pathIds.ToString()
            + "}";
    }
}

[System.Serializable]
public class TestingPathOrder
{
    public string position;
    public PathIds[] pathIds;

    public override string ToString() {
        return "TestingPathOrder: {\n"
            + "position = " + position + "\n"
            + "pathIds = " + pathIds.ToString()
            + "}";
    }
}

[System.Serializable]
public class PathIds
{
    public int pathId;

    public override string ToString() {
        return "TrainingPathOrder: {\n"
            + "pathId = " + pathId.ToString()
            + "}";
    }
}