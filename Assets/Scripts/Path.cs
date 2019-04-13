﻿using System.IO;
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


