using System.IO;
using UnityEngine;
using System;

public class PathReader {
    private Participants participants;
    private Participant[] participantsArr;
    private int participantId;
    private PathIds pathIds;
    private int pathId;
    private PickPath currentPath;

    public PathReader(string filePath)
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

    public int getPathId() {
        return pathId;
    }
    public bool setPathId(int userId, int pathId, int phase) {
        // Get participant
        if (userId < 1 || userId > participantsArr.Length) {
            return false;
        }
        Participant p = participantsArr[userId];

        // Get path arr
        // 0 indicates training, 1 indicates testing
        PickPath[] patharr = (phase == 1) ? p.trainingPathOrder : p.testingPathOrder;
        if (pathId < 1 || pathId > patharr.Length)
        {
            return false;
        }
        else if (patharr[pathId - 1].pathId == pathId)
        {
            // Set
            currentPath = patharr[pathId - 1];
            this.pathId = pathId;
            return true;
        }
        return false;
    }
    // no starts from zero.
    public BookWithLocation getBookWithLocation(int no) {
        if (currentPath == null) {
            return null;
        }
        if (no < 0 || no >= currentPath.pickPathInformation.orderedBooksAndLocations.Length) {
            return null;
        }
        return currentPath.pickPathInformation.orderedBooksAndLocations[no];
    }
    public void printBookWithLocation(BookWithLocation b) {
        Debug.Log("================");
        Debug.Log("Title: " + b.book.title + ", author: " + b.book.author + ", tag: " + b.book.tag);
        Debug.Log("Location: " + b.location[0] + ", " + b.location[1]);
    }

    public int getNumberOfBooksInPath() {
        return currentPath.pickPathInformation.orderedBooksAndLocations.Length;
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

[System.Serializable]
public class PickPaths
{
    public PickPath[] pickPaths;
}
[System.Serializable]
public class PickPath {
    public int pathId;
    public string pathType;
    public PickPathInformation pickPathInformation;
    
    public override string ToString() {
        return "PickPath: {\n"
            + "pathId = " + pathId.ToString() + "\n"
            + "pathType = " + pathType + "\n"
            + "pickPathInformation = " + pickPathInformation + "\n"
            + "}";
    }
}
[System.Serializable]
public class PickPathInformation {
    public BookWithLocation[] unorderedBooksAndLocations;
    public BookWithLocation[] orderedBooksAndLocations;
    public OrderedPickPath[] orderedPickPath;

    public override string ToString() {
        return "PickPathInformation: {\n"
            + "unorderedBooksAndLocations = " + unorderedBooksAndLocations.ToString() + "\n"
            + "orderedBooksAndLocations = " + orderedBooksAndLocations.ToString() + "\n"
            + "orderedPickPath = " + orderedPickPath.ToString() + "\n"
            + "}";
    }
}

[System.Serializable]
public class OrderedPickPath
{
    public int stepNumber;
    public int[][] cellByCellPathToTargetBookLocation;
    public BookWithLocation targetBookAndTargetBookLocation;

    public override string ToString() {
        return "OrderedPickPath: {\n"
            + "stepNumber = " + stepNumber.ToString() + "\n"
            + "targetBookAndTargetBookLocation = " + targetBookAndTargetBookLocation.ToString() + "\n"
            + "}";
    }
}

[System.Serializable]
public class BookWithLocation
{
    public Book book;
    public int[] location;

    public override string ToString() {
        string locationStr = "[ ";
        foreach (int loc in location) {
            locationStr += loc.ToString() + " ";
        }
        locationStr += " ]";
        return "BookWithLocation: {\n"
            + "book = " + book.ToString() + "\n"
            + "location = " + locationStr + "\n"
            + "}";
    }
}

[System.Serializable]
public class Book
{
    public string title;
    public string author;
    public string tag;

    public override string ToString() {
        return "TrainingPathOrder: {\n"
            + "title = " + title + "\n"
            + "author = " + author + "\n"
            + "tag = " + tag + "\n"
            + "}";
    }

}