using System.IO;
using UnityEngine;
using System;
public class PathReader
{
    private PickPaths paths;
    private PickPath[] patharr;
    private int pathId;
    private PickPath currentPath;
    /*public PathReader(PickPaths paths) {
        if (this.paths == null) {
            throw new NullReferenceException("PickPaths cannot be set null");
        }
        this.paths = paths;
        patharr = paths.pickPaths;
    }*/
    public PathReader(string filePath) {
        if (File.Exists(filePath))
        {
            string dataAsJSON = File.ReadAllText(filePath);
            paths = JsonUtility.FromJson<PickPaths>(dataAsJSON);
            patharr = paths.pickPaths;
        }
        else
        {
            throw new FileNotFoundException(filePath + " cannot be found.");
        }
    }
    public int getPathId() {
        return pathId;
    }
    public bool setPathId(int id) {
        if (id < 1 || id > patharr.Length)
        {
            return false;
        }
        else if (patharr[id - 1].pathId == id)
        {
            currentPath = patharr[id - 1];
            pathId = id;
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
public class PickPaths
{
    public PickPath[] pickPaths;
}
[System.Serializable]
public class PickPath {
    public int pathId;
    public string pathType;
    public PickPathInformation pickPathInformation;
}
[System.Serializable]
public class PickPathInformation {
    public BookWithLocation[] unorderedBooksAndLocations;
    public OrderedPickPath[] orderedPickPath;
    public BookWithLocation[] orderedBooksAndLocations;
}

[System.Serializable]
public class OrderedPickPath {
    public int[][] cellByCellPathToTargetBookLocation;
    public BookWithLocation targetBookAndTargetBookLocation;
    public int stepNumber;
}

[System.Serializable]
public class BookWithLocation {
    public Book book;
    public int[] location;
}
[System.Serializable]
public class Book
{
    public string author;
    public string tag;
    public string title;
}


