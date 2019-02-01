using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System;
using System.Threading;
//using System.Globalization;

public class NavigationController : MonoBehaviour {


    // data model;
    private int selectedUserId = 0;
    private int selectedPhase = 0; // 0 indicates training, 1 indicates testing
    private int selectedPathId = 1;
    private int selectedBookNum = 0;
    private string selectedBookTag = "";
   
    private Dictionary<int, string> record_posted_book;
    private PathReader pr;
    private const string url = "https://eyegaze4605api.herokuapp.com/api/userData";
    /* view style config
    private Color selected_color = Color.blue;
    private Color unselected_color = Color.white;*/

    // views
    GameObject userSelectionView;
    GameObject phaseSelectionView;
    GameObject pathIdSelectionView;
    GameObject bookInfoView;
    GameObject shelfView;
    GameObject completionView;
    GameObject placementView;

    // active view
    GameObject currentActiveView;

    // Use this for initialization
    void Start () {
        // data model init
        pr = new PathReader(Path.Combine(Application.streamingAssetsPath, "pick-paths.json"));
        pr.setPathId(selectedPathId);
        record_posted_book = new Dictionary<int, string>();
        userSelectionView = GameObject.Find("User Selection View");
        userSelectionView.SetActive(true);
        phaseSelectionView = GameObject.Find("Phase Selection View");
        phaseSelectionView.SetActive(false);
        pathIdSelectionView = GameObject.Find("PathId Selection View");
        pathIdSelectionView.SetActive(false);
        bookInfoView = GameObject.Find("Book Info View");
        bookInfoView.SetActive(false);
        shelfView = GameObject.Find("Shelf View");
        shelfView.GetComponent<ShelfView>().init();
        placementView = GameObject.Find("Placement Selection View");
        placementView.SetActive(false);
        shelfView.SetActive(false);
        
        completionView = GameObject.Find("Completion View");
        completionView.SetActive(false);
        currentActiveView = userSelectionView;



    }
    private void postdata() {
        WWWForm form = new WWWForm();
        form.AddField("userId", selectedUserId);
        form.AddField("phase", selectedPhase);
        form.AddField("time", (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        form.AddField("pathId", selectedPathId);
        form.AddField("bookTag", selectedBookTag);
        form.AddField("device", 2);
        form.AddField("viewPosition", 1);
        StartCoroutine(Upload(form));
    }
    private IEnumerator Upload(WWWForm form) {
        var download = UnityWebRequest.Post(url, form);
        yield return download.SendWebRequest();
        if (download.isNetworkError || download.isHttpError)
        {

        }
        else {

        }
    }

    /*if ((Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick2Button0)) && bookNum < pr.getNumberOfBooksInPath() - 1)
    {
    }*/
    private IEnumerator holdOn()
    {
        print("S " + Time.time);
        yield return new WaitForSeconds(1);
        print("E " + Time.time);
    }


    private void placementSelectionControl()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            Vector3 vec = new Vector3(0.01f, 0, 0);
            userSelectionView.transform.position += vec;
            phaseSelectionView.transform.position += vec;
            pathIdSelectionView.transform.position += vec;
            bookInfoView.transform.position += vec;
            shelfView.transform.position += vec;
            completionView.transform.position += vec;
            placementView.transform.position += vec;
        }
        //left
        else if (Input.GetKey(KeyCode.Numlock))
        {
            Vector3 vec = new Vector3(-0.01f, 0, 0);
            userSelectionView.transform.position += vec;
            phaseSelectionView.transform.position += vec;
            pathIdSelectionView.transform.position += vec;
            bookInfoView.transform.position += vec;
            shelfView.transform.position += vec;
            completionView.transform.position += vec;
            placementView.transform.position += vec;
        }
        else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            Vector3 vec = new Vector3(0, 0.01f, 0);
            userSelectionView.transform.position += vec;
            phaseSelectionView.transform.position += vec;
            pathIdSelectionView.transform.position += vec;
            bookInfoView.transform.position += vec;
            shelfView.transform.position += vec;
            completionView.transform.position += vec;
            placementView.transform.position += vec;
        }
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            Vector3 vec = new Vector3(0, -0.01f, 0);
            userSelectionView.transform.position += vec;
            phaseSelectionView.transform.position += vec;
            pathIdSelectionView.transform.position += vec;
            bookInfoView.transform.position += vec;
            shelfView.transform.position += vec;
            completionView.transform.position += vec;
            placementView.transform.position += vec;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            currentActiveView.SetActive(false);
            pathIdSelectionView.SetActive(true);
            currentActiveView = pathIdSelectionView;
            // setup next selection
            pathIdSelectionView.GetComponent<PathIdSelectionView>().setPhase(selectedPhase);
        }
    }
    private void userSelectionControl() {
        //Debug.Log("V " + Input.GetAxis("Vertical"));
        //Debug.Log("H " + Input.GetAxis("Horizontal"));

        //if (Input.GetAxis("Vertical") == 1)

        if (Input.GetKeyDown(KeyCode.B))
        {

            userSelectionView.GetComponent<UserSelectionView>().selectNext();

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {

            userSelectionView.GetComponent<UserSelectionView>().selectLast();

        }
        else if (Input.GetKeyDown(KeyCode.C))
        {

            selectedUserId = userSelectionView.GetComponent<UserSelectionView>().getSelectedUserId();
            currentActiveView.SetActive(false);
            phaseSelectionView.SetActive(true);
            // clear next selection
            selectedPhase = 0;
            phaseSelectionView.GetComponent<PhaseSelectionView>().setPhase(selectedPhase);
            currentActiveView = phaseSelectionView;
            
        }
    }

    private void phaseSelectionControl()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            phaseSelectionView.GetComponent<PhaseSelectionView>().selectTesting();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            phaseSelectionView.GetComponent<PhaseSelectionView>().selectTraining();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            selectedPhase = phaseSelectionView.GetComponent<PhaseSelectionView>().getSelectedPhase();
            currentActiveView.SetActive(false);
            placementView.SetActive(true);
            currentActiveView = placementView;
        }
        //else if (Input.GetKeyDown(KeyCode.LeftArrow))
        else if (Input.GetKeyDown(KeyCode.A))
        {
            // go back to user selection
            currentActiveView.SetActive(false);
            userSelectionView.SetActive(true);
            currentActiveView = userSelectionView;
        }
    }

    private void pathIdSelectionControl() {
        if (Input.GetKeyDown(KeyCode.B))
        {
            pathIdSelectionView.GetComponent<PathIdSelectionView>().selectNext();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            pathIdSelectionView.GetComponent<PathIdSelectionView>().selectLast();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            selectedPathId = pathIdSelectionView.GetComponent<PathIdSelectionView>().getSelectedPathId();
            
            currentActiveView.SetActive(false);
            bookInfoView.SetActive(true);
            currentActiveView = bookInfoView;
            // setup the next view
            if (selectedPathId != pr.getPathId())
            {
                pr.setPathId(selectedPathId);
                selectedBookNum = 0;
                record_posted_book.Clear();
            }
            bookInfoView.GetComponent<BookInfoView>().highlightBookInfo(pr.getBookWithLocation(selectedBookNum));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            currentActiveView.SetActive(false);
            phaseSelectionView.SetActive(true);
            currentActiveView = phaseSelectionView;

        }

    }
    private void bookInfoControl()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (selectedBookNum + 1 < pr.getNumberOfBooksInPath())
            {
                selectedBookNum++;
                bookInfoView.GetComponent<BookInfoView>().highlightBookInfo(pr.getBookWithLocation(selectedBookNum));
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (selectedBookNum > 0)
            {
                selectedBookNum--;
                bookInfoView.GetComponent<BookInfoView>().highlightBookInfo(pr.getBookWithLocation(selectedBookNum));
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            // switch to shelf view
            currentActiveView.SetActive(false);
            shelfView.SetActive(true);
            currentActiveView = shelfView;
            shelfView.GetComponent<ShelfView>().highlightBlock(pr.getBookWithLocation(selectedBookNum));
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            // get the book, send server data
            if (!record_posted_book.ContainsKey(selectedBookNum))
            {
                selectedBookTag = pr.getBookWithLocation(selectedBookNum).book.tag;
                //Debug.Log(selectedBookTag + " " + (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
                postdata();
                record_posted_book.Add(selectedBookNum, "pick");
            }
            if (selectedBookNum + 1 < pr.getNumberOfBooksInPath())
            {
                selectedBookNum++;
                bookInfoView.GetComponent<BookInfoView>().highlightBookInfo(pr.getBookWithLocation(selectedBookNum));
            }
            if (record_posted_book.Count >= pr.getNumberOfBooksInPath())
            {
                // go to next, or notify completion.
                currentActiveView.SetActive(false);
                completionView.SetActive(true);
                currentActiveView = completionView;
            }
            else
            {
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            currentActiveView.SetActive(false);
            pathIdSelectionView.SetActive(true);
            currentActiveView = pathIdSelectionView;
        }
    }
    private void completionControl()
    {
        if (Input.anyKeyDown)
        {
            selectedUserId = 0;
            selectedPhase = 0; // 0 indicates training, 1 indicates testing
            selectedPathId = 1;
            selectedBookNum = 0;
            selectedBookTag = "";
            record_posted_book.Clear();
            currentActiveView.SetActive(false);
            userSelectionView.SetActive(true);
            currentActiveView = userSelectionView;
        }
    }
    private void shelfControl()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (selectedBookNum + 1 < pr.getNumberOfBooksInPath())
            {
                selectedBookNum++;
                shelfView.GetComponent<ShelfView>().highlightBlock(pr.getBookWithLocation(selectedBookNum));
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (selectedBookNum > 0)
            {
                selectedBookNum--;
                shelfView.GetComponent<ShelfView>().highlightBlock(pr.getBookWithLocation(selectedBookNum));
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            // switch to book info view
            currentActiveView.SetActive(false);
            bookInfoView.SetActive(true);
            currentActiveView = bookInfoView;
            bookInfoView.GetComponent<BookInfoView>().highlightBookInfo(pr.getBookWithLocation(selectedBookNum));
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            // get the book, send server data
            if (!record_posted_book.ContainsKey(selectedBookNum))
            {
                selectedBookTag = pr.getBookWithLocation(selectedBookNum).book.tag;
                postdata();
                record_posted_book.Add(selectedBookNum, "pick");
            }
            if (selectedBookNum + 1 < pr.getNumberOfBooksInPath())
            {
                selectedBookNum++;
                bookInfoView.GetComponent<BookInfoView>().highlightBookInfo(pr.getBookWithLocation(selectedBookNum));
            }
            if (record_posted_book.Count >= pr.getNumberOfBooksInPath())
            {
                // go to next, or notify completion.
                currentActiveView.SetActive(false);
                completionView.SetActive(true);
                currentActiveView = completionView;
            }
            else
            {
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            currentActiveView.SetActive(false);
            pathIdSelectionView.SetActive(true);
            currentActiveView = pathIdSelectionView;
        }
    }
    // Update is called once per frame
    void Update () {
        if (currentActiveView == userSelectionView)
        {
            userSelectionControl();
        }
        else if (currentActiveView == phaseSelectionView)
        {
            phaseSelectionControl();
        }
        else if (currentActiveView == pathIdSelectionView)
        {
            pathIdSelectionControl();
        }
        else if (currentActiveView == bookInfoView)
        {
            bookInfoControl();
        }
        else if (currentActiveView == shelfView)
        {
            shelfControl();
        }
        else if (currentActiveView == completionView)
        {
            completionControl();

        }
        else if (currentActiveView == placementView)
        {
            placementSelectionControl();
        }
    }
}
