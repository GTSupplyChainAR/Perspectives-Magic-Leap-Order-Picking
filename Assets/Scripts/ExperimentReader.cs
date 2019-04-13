using System.IO;
using UnityEngine;
using System;

public class ExperimentReader {
    public Participants participants;
    public Participant[] participantsArr;
    public int participantId;
    public PathIds pathIds;

    public ExperimentReader(string filePath)
    {
        filePath = Path.Combine(Application.streamingAssetsPath, filePath);
        if (File.Exists(filePath))
        {
            string dataAsJSON = File.ReadAllText(filePath);
            participants = JsonUtility.FromJson<Participants>(dataAsJSON);
            participantsArr = participants.participants;
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
        string str = "Participant[]: [\n";
        foreach (Participant p in participants) {
            str += p.ToString() + "\n";
        }
        str += "]";

        return str;
    }
}

[System.Serializable]
public class Participant
{
    public int participantId;
    public TrainingPathOrder[] trainingPathOrder;
    public TestingPathOrder[] testingPathOrder;
    
    public override string ToString() {
        string trainStr = "trainingPathOrder[]: [\n";
        foreach (TrainingPathOrder t in trainingPathOrder) {
            trainStr += t.ToString() + "\n";
        }
        trainStr += "]";

        string testingStr = "testingPathOrder[]: [\n";
        foreach (TestingPathOrder t in testingPathOrder) {
            testingStr += t.ToString() + "\n";
        }
        testingStr += "]";

        return "Participant: {\n"
            + "ID = " + participantId.ToString() + "\n"
            + "trainingPathOrder = " + trainStr + "\n"
            + "testingPathOrder = " + testingStr
            + "}";
    }
}

[System.Serializable]
public class TrainingPathOrder
{
    public string position;
    public int[] pathIds;

    public override string ToString() {
        string pathIdStr = "pathIds[]: [\n";
        foreach (int p in pathIds) {
            pathIdStr += p.ToString() + "\n";
        }
        pathIdStr += "]";

        return "TrainingPathOrder: {\n"
            + "position = " + position + "\n"
            + "pathIds = " + pathIdStr + "\n"
            + "}";
    }
}

[System.Serializable]
public class TestingPathOrder
{
    public string position;
    public int[] pathIds;

    public override string ToString() {
        string pathIdStr = "pathIds[]: [\n";
        foreach (int p in pathIds) {
            pathIdStr += p.ToString() + "\n";
        }
        pathIdStr += "]";

        return "TestingPathOrder: {\n"
            + "position = " + position + "\n"
            + "pathIds = " + pathIdStr
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