using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using System.IO;
using UnityEngine.Networking;
using System;
using System.Threading;
//using System.Globalization;

public class NavigationController : MonoBehaviour {

    //MLInputController
    //Bumper for back, trigger for select
    private const float _triggerThreshold = 0.2f;
    private MLInputController _controller;

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

    enum OrderPickingMode {
        UserSelection,
        PhaseSelection,
        PathIdSelection,
        BookInfo,
        Shelf,
        Completion,
        Placement
    }

    private OrderPickingMode currentMode;

    // Use this for initialization
    void Start() {
        // data model init
        pr = new PathReader(Path.Combine(Application.streamingAssetsPath, "pick-paths.json"));
        pr.setPathId(selectedPathId);
        record_posted_book = new Dictionary<int, string>();
        userSelectionView = GameObject.Find("User Selection View");
        userSelectionView.SetActive(false);
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
        
        // Start with user selection
        setMode(OrderPickingMode.UserSelection);

        //controller
        MLInput.Start();
        _controller = MLInput.GetController(MLInput.Hand.Right);
        MLInput.OnControllerButtonDown += OnButtonDown;
        MLInput.OnControllerTouchpadGestureStart += OnGestureStart;
        MLInput.OnTriggerDown += OnTriggerDown;
    }

    private void OnDestroy()
    {
        MLInput.Stop();
    }

    //Bumper
    void OnButtonDown(byte controller_id, MLInputControllerButton button)
    {
        if (button == MLInputControllerButton.Bumper)
        {
            OnBumper();
        }
    }

    void OnBumper() {
        switch (currentMode) {
            case OrderPickingMode.UserSelection:
                userSelectionBumper();
                break;
            case OrderPickingMode.PhaseSelection:
                phaseSelectionBumper();
                break;
            case OrderPickingMode.PathIdSelection:
                pathIdSelectionBumper();
                break;
            case OrderPickingMode.BookInfo:
                bookInfoBumper();
                break;
            case OrderPickingMode.Shelf:
                shelfBumper();
                break;
            case OrderPickingMode.Completion:
                completionBumper();
                break;
            case OrderPickingMode.Placement:
                placementSelectionBumper();
                break;
            default:
                // do nothing
                break;
        }
    }

    void OnGestureStart(byte controller_id, MLInputControllerTouchpadGesture touchpad_gesture) {
        switch (currentMode) {
            case OrderPickingMode.UserSelection:
                userSelectionControl(touchpad_gesture);
                break;
            case OrderPickingMode.PhaseSelection:
                phaseSelectionControl(touchpad_gesture);
                break;
            case OrderPickingMode.PathIdSelection:
                pathIdSelectionControl(touchpad_gesture);
                break;
            case OrderPickingMode.BookInfo:
                bookInfoControl(touchpad_gesture);
                break;
            case OrderPickingMode.Shelf:
                shelfControl(touchpad_gesture);
                break;
            case OrderPickingMode.Completion:
                completionControl(touchpad_gesture);
                break;
            case OrderPickingMode.Placement:
                placementSelectionControl(touchpad_gesture);
                break;
            default:
                // do nothing
                break;
        }
    }

    private void OnTriggerDown(byte controller_id, float intensity)
    {
        switch (currentMode) {
            case OrderPickingMode.UserSelection:
                userSelectionTrigger();
                break;
            case OrderPickingMode.PhaseSelection:
                phaseSelectionTrigger();
                break;
            case OrderPickingMode.PathIdSelection:
                pathIdSelectionTrigger();
                break;
            case OrderPickingMode.BookInfo:
                bookInfoTrigger();
                break;
            case OrderPickingMode.Shelf:
                shelfTrigger();
                break;
            case OrderPickingMode.Completion:
                completionTrigger();
                break;
            case OrderPickingMode.Placement:
                placementSelectionTrigger();
                break;
            default:
                // do nothing
                break;
        }
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


    private void placementSelectionControl(MLInputControllerTouchpadGesture touchpad_gesture)
    {
        if (touchpad_gesture.Type == MLInputControllerTouchpadGestureType.Swipe)
        {
            //Down
            //if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Right)
            {
                Vector3 vec = new Vector3(0.01f, 0, 0);
                //userSelectionView.transform.position += vec;
                phaseSelectionView.transform.position += vec;
                pathIdSelectionView.transform.position += vec;
                bookInfoView.transform.position += vec;
                shelfView.transform.position += vec;
                completionView.transform.position += vec;
                placementView.transform.position += vec;
            }
            //Left
            //else if (Input.GetKey(KeyCode.Numlock))
            else if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Left)
            {
                Vector3 vec = new Vector3(-0.01f, 0, 0);
                //userSelectionView.transform.position += vec;
                phaseSelectionView.transform.position += vec;
                pathIdSelectionView.transform.position += vec;
                bookInfoView.transform.position += vec;
                shelfView.transform.position += vec;
                completionView.transform.position += vec;
                placementView.transform.position += vec;
            }
            //Up
            //else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            else if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Up)
            {
                Vector3 vec = new Vector3(0, 0.01f, 0);
                //userSelectionView.transform.position += vec;
                phaseSelectionView.transform.position += vec;
                pathIdSelectionView.transform.position += vec;
                bookInfoView.transform.position += vec;
                shelfView.transform.position += vec;
                completionView.transform.position += vec;
                placementView.transform.position += vec;
            }
            //Right
            //else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            else if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Down)
            {
                Vector3 vec = new Vector3(0, -0.01f, 0);
                //userSelectionView.transform.position += vec;
                phaseSelectionView.transform.position += vec;
                pathIdSelectionView.transform.position += vec;
                bookInfoView.transform.position += vec;
                shelfView.transform.position += vec;
                completionView.transform.position += vec;
                placementView.transform.position += vec;
            }
        }
    }

    private void placementSelectionTrigger() {
        setMode(OrderPickingMode.PathIdSelection);
        // setup next selection
        pathIdSelectionView.GetComponent<PathIdSelectionView>().setPhase(selectedPhase);
    }

    private void placementSelectionBumper() {
        // unused
    }

    private void userSelectionControl(MLInputControllerTouchpadGesture touchpad_gesture) {
        //Debug.Log("V " + Input.GetAxis("Vertical"));
        //Debug.Log("H " + Input.GetAxis("Horizontal"));

        //if (Input.GetKeyDown(KeyCode.B))
        if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Down)
        {
            userSelectionView.GetComponent<UserSelectionView>().selectNext();

        }

        else if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Up)
        {
            userSelectionView.GetComponent<UserSelectionView>().selectLast();
        }
    }

    private void userSelectionTrigger() {
        selectedUserId = userSelectionView.GetComponent<UserSelectionView>().getSelectedUserId();
        setMode(OrderPickingMode.PhaseSelection);
        // clear next selection
        selectedPhase = 0;
        phaseSelectionView.GetComponent<PhaseSelectionView>().setPhase(selectedPhase);
    }

    private void userSelectionBumper() {
        // unused
    }

    private void phaseSelectionControl(MLInputControllerTouchpadGesture touchpad_gesture)
    {
        //if (Input.GetKeyDown(KeyCode.B))
        if(touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Down)
        {
            phaseSelectionView.GetComponent<PhaseSelectionView>().selectTesting();
        }
        //else if (Input.GetKeyDown(KeyCode.D))
        else if(touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Up)
        {
            phaseSelectionView.GetComponent<PhaseSelectionView>().selectTraining();
        }
    }

    private void phaseSelectionTrigger() {
        selectedPhase = phaseSelectionView.GetComponent<PhaseSelectionView>().getSelectedPhase();
        setMode(OrderPickingMode.Placement);
    }

    private void phaseSelectionBumper() {
        // go back to user selection
        setMode(OrderPickingMode.UserSelection);
    }

    private void setMode(OrderPickingMode newMode) {
        // Disable current
        if (currentActiveView != null) {
            currentActiveView.SetActive(false);
        }

        // Determine new view
        GameObject newActiveView;
        switch (newMode) {
            case OrderPickingMode.UserSelection:
                newActiveView = userSelectionView;
                break;
            case OrderPickingMode.PhaseSelection:
                newActiveView = phaseSelectionView;
                break;
            case OrderPickingMode.PathIdSelection:
                newActiveView = pathIdSelectionView;
                break;
            case OrderPickingMode.BookInfo:
                newActiveView = bookInfoView;
                break;
            case OrderPickingMode.Shelf:
                newActiveView = shelfView;
                break;
            case OrderPickingMode.Completion:
                newActiveView = completionView;
                break;
            case OrderPickingMode.Placement:
                newActiveView = placementView;
                break;
            default:
                // use current
                newActiveView = currentActiveView;
                break;
        }

        // Set new view
        newActiveView.SetActive(true);
        currentActiveView = newActiveView;
        currentMode = newMode;
    }

    private void pathIdSelectionControl(MLInputControllerTouchpadGesture touchpad_gesture) {
        //if (Input.GetKeyDown(KeyCode.B))
        if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Down)
        {
            pathIdSelectionView.GetComponent<PathIdSelectionView>().selectNext();
        }
        //else if (Input.GetKeyDown(KeyCode.D))
        else if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Up)
        {
            pathIdSelectionView.GetComponent<PathIdSelectionView>().selectLast();
        }
    }

    private void pathIdSelectionTrigger() {
        setMode(OrderPickingMode.BookInfo);
        selectedPathId = pathIdSelectionView.GetComponent<PathIdSelectionView>().getSelectedPathId();

        // setup the next view
        if (selectedPathId != pr.getPathId())
        {
            pr.setPathId(selectedPathId);
            selectedBookNum = 0;
            record_posted_book.Clear();
        }
        bookInfoView.GetComponent<BookInfoView>().highlightBookInfo(pr.getBookWithLocation(selectedBookNum));
    }

    private void pathIdSelectionBumper() {
        setMode(OrderPickingMode.PhaseSelection);
    }

    private void bookInfoControl(MLInputControllerTouchpadGesture touchpad_gesture)
    {
        //if (Input.GetKeyDown(KeyCode.B))
        if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Up)
        {
            if (selectedBookNum + 1 < pr.getNumberOfBooksInPath())
            {
                selectedBookNum++;
                bookInfoView.GetComponent<BookInfoView>().highlightBookInfo(pr.getBookWithLocation(selectedBookNum));
            }
        }
        //else if (Input.GetKeyDown(KeyCode.D))
        else if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Down)
        {
            if (selectedBookNum > 0)
            {
                selectedBookNum--;
                bookInfoView.GetComponent<BookInfoView>().highlightBookInfo(pr.getBookWithLocation(selectedBookNum));
            }
        }
        //else if (Input.GetKeyDown(KeyCode.C))
        else if (Input.GetKeyDown(KeyCode.E))
        {
            setMode(OrderPickingMode.PathIdSelection);
        }
    }

    private void bookInfoTrigger() {
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
            setMode(OrderPickingMode.Completion);
        }
    }

    private void bookInfoBumper() {
        // switch to shelf view
        setMode(OrderPickingMode.Shelf);
        shelfView.GetComponent<ShelfView>().highlightBlock(pr.getBookWithLocation(selectedBookNum));
    }

    private void completionControl(MLInputControllerTouchpadGesture touchpad_gesture)
    {
        // unused
    }

    private void completionTrigger() {
        selectedUserId = 0;
        selectedPhase = 0; // 0 indicates training, 1 indicates testing
        selectedPathId = 1;
        selectedBookNum = 0;
        selectedBookTag = "";
        record_posted_book.Clear();
        setMode(OrderPickingMode.UserSelection);
    }

    private void completionBumper() {
        // unused
    }

    private void shelfControl(MLInputControllerTouchpadGesture touchpad_gesture)
    {
        //if (Input.GetKeyDown(KeyCode.B))
        if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Up)
        {
            if (selectedBookNum + 1 < pr.getNumberOfBooksInPath())
            {
                selectedBookNum++;
                shelfView.GetComponent<ShelfView>().highlightBlock(pr.getBookWithLocation(selectedBookNum));
            }
        }
        //else if (Input.GetKeyDown(KeyCode.D))
        else if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Down)
        {
            if (selectedBookNum > 0)
            {
                selectedBookNum--;
                shelfView.GetComponent<ShelfView>().highlightBlock(pr.getBookWithLocation(selectedBookNum));
            }
        }
    }

    private void shelfTrigger() {
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
            setMode(OrderPickingMode.Completion);
        }

        else if (Input.GetKeyDown(KeyCode.E))
        {
            setMode(OrderPickingMode.PathIdSelection);
        }
    }

    private void shelfBumper() {
        // switch to book info view
        setMode(OrderPickingMode.BookInfo);
        bookInfoView.GetComponent<BookInfoView>().highlightBookInfo(pr.getBookWithLocation(selectedBookNum));
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown(0)) {
            OnTriggerDown(0, 0);
        }
        else if (Input.GetMouseButtonDown(1)) {
            OnBumper();
        }
    }
}




//Code e?
