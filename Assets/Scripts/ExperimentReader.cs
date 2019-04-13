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
                Debug.Log(participant.testingPathOrder[0].pathIds[0].pathId);
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