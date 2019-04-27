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

    public bool LightspeedMode = false;

    //MLInputController
    //Bumper for back, trigger for select
    public const float _triggerThreshold = 0.2f;
    private MLInputController _controller;

    // data model;
    public int selectedUserId = 0;
    public int selectedPhase = 0; // 0 indicates training, 1 indicates testing
    public int selectedPathId = 1;
    public int selectedBookNum = 0;
    public string selectedBookTag = "";
    public int[,] mergeArr = new int[4, 15];
    public int positionRound = 0; //0-4 keep track of how many positions the user has done
    public int placeRound = 0;


    private Dictionary<int, string> record_posted_book;
    private PathReader pr;
    public const string url = "https://eyegaze4605api.herokuapp.com/api/userData";
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
    GameObject restorePrompt;

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
        pr.setUserId(selectedUserId);
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

        // Prompt restore
        restorePrompt = GameObject.Find("Restore Prompt");
        restorePrompt.SetActive(true);

        // Start with user selection
        setMode(OrderPickingMode.UserSelection);

        // step 1. select user
        // step 2. select phase
        // step 3. select view position (initial)
        // 4 positions
        //      5 paths
        //          1 start per path
        //          10 clicks per path
        //          1 quit per path
        //      select view position (new)
        int numClicks = 3 + 4 * ((5 * 12) + 1);
        if (LightspeedMode) {
            DebugClick(numClicks);
        }

        StartCoroutine(Yolo());

        //controller
        MLInput.Start();
        _controller = MLInput.GetController(MLInput.Hand.Right);
        MLInput.OnControllerButtonDown += OnButtonDown;
        MLInput.OnControllerTouchpadGestureStart += OnGestureStart;
        MLInput.OnTriggerDown += OnTriggerDown;
    }

    private IEnumerator Yolo() {
        Debug.Log("yolo 1");
        yield return new WaitForSeconds(3.0f);
        Debug.Log("yolo 2");
        Debug.Log(restorePrompt);
        restorePrompt.SetActive(false);
        Debug.Log("yolo 3");
    }

    private void DebugClick(int numClicks) {
        StartCoroutine(DebugClickC(numClicks));
    }

    private IEnumerator DebugClickC(int numClicks) {
        // initial delay
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < numClicks; i++) {
            OnTriggerDown(0, 0);
            yield return new WaitForSeconds(0.0001f);
        }
        yield return null;
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
            Debug.Log("-- ON BUMPER --");
        }
    }

    void OnBumper() {
        /*if (restorePrompt.active) {
            restorePrompt.SetActive(false);
            return;
        }*/

        switch (currentMode) {
            case OrderPickingMode.UserSelection:
                userSelectionBumper();
                break;
            case OrderPickingMode.PhaseSelection:
                phaseSelectionBumper();
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
            //case OrderPickingMode.PathIdSelection:
            //    pathIdSelectionControl(touchpad_gesture);
            //    break;
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
        if (restorePrompt.active) {
            loaddata();
            restorePrompt.SetActive(false);
            return;
        }

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
        if (LightspeedMode) {
            return;
        }

        savedata();

        WWWForm form = new WWWForm();
        form.AddField("userId", selectedUserId);
        form.AddField("phase", selectedPhase);
        form.AddField("time", (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        form.AddField("pathId", selectedPathId);
        form.AddField("bookTag", selectedBookTag);
        form.AddField("device", 2);
        form.AddField("viewPosition", 1);
        Debug.Log(selectedUserId);
        Debug.Log(selectedPhase);
        Debug.Log(selectedPathId);
        Debug.Log(selectedBookTag);
        StartCoroutine(Upload(form));
    }

    private void savedata() {
        PlayerPrefs.SetInt("userId", selectedUserId);
        PlayerPrefs.SetInt("phase", selectedPhase);
        PlayerPrefs.SetInt("positionRound", positionRound);
        PlayerPrefs.SetInt("placeRound", placeRound);
    }

    private void loaddata() {
        selectedUserId = PlayerPrefs.GetInt("userId");
        selectedPhase = PlayerPrefs.GetInt("phase");
        positionRound = PlayerPrefs.GetInt("positionRound");
        placeRound = PlayerPrefs.GetInt("placeRound");

        if (selectedPathId > 0) {
            pathIdSelectionTrigger();
        } else {
            Debug.Log("Ignoring RESTORE b/c selectedPathId was read as 0!");
        }
    }

    private IEnumerator Upload(WWWForm form) {
        var download = UnityWebRequest.Post(url, form);
        yield return download.SendWebRequest();
        if (download.isNetworkError || download.isHttpError)
        {
            Debug.LogError("HTTP UPLOAD ERROR:");
            Debug.LogError(download.error);
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
            Vector3 vec = Vector3.zero;

            //if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Left)
            {
                vec = Vector3.left;
            }
            //else if (Input.GetKey(KeyCode.Numlock))
            else if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Right)
            {
                vec = Vector3.right;
            }
            //else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            else if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Up)
            {
                vec = Vector3.up;
            }
            //else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            else if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Down)
            {
                vec = Vector3.down;
            }
            vec /= 10;

            moveView(userSelectionView, vec);
            moveView(phaseSelectionView, vec);
            moveView(pathIdSelectionView, vec);
            moveView(bookInfoView, vec);
            moveView(shelfView, vec);
            moveView(completionView, vec);
            moveView(placementView, vec);
        }
    }

    private void moveView(GameObject o, Vector3 delta) {
        RectTransform t = o.GetComponent<RectTransform>();
        Vector3 pos = t.localPosition;
        float origZ = pos.z;
        pos += delta;
        pos.z = origZ;
        t.localPosition = pos;
    }

    private void placementSelectionTrigger() {
        setMode(OrderPickingMode.PathIdSelection);
        // setup next selection        
        pathIdSelectionView.GetComponent<PathIdSelectionView>().setPhase(selectedPhase);
    }

    private void placementSelectionBumper() {
        // unused
    }

    private void keyAltInput() {
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            switch (currentMode) {
                case OrderPickingMode.UserSelection:
                    userSelectionView.GetComponent<UserSelectionView>().selectNext();
                    break;
                case OrderPickingMode.PhaseSelection:
                    phaseSelectionView.GetComponent<PhaseSelectionView>().selectTesting();
                    break;
                default:
                    break;
            }
            
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            switch (currentMode) {
                case OrderPickingMode.UserSelection:
                    userSelectionView.GetComponent<UserSelectionView>().selectLast();
                    break;
                case OrderPickingMode.PhaseSelection:
                    phaseSelectionView.GetComponent<PhaseSelectionView>().selectTraining();
                    break;
                default:
                    break;
            }
        }
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
                placeRound++;
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
                positionRound++;
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

    //private void pathIdSelectionControl(MLInputControllerTouchpadGesture touchpad_gesture)
    //{
    //    if (Input.GetKeyDown(KeyCode.B))
    //        if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Down)
    //        {
    //            pathIdSelectionView.GetComponent<PathIdSelectionView>().selectNext();
    //        }
    //        else if (Input.GetKeyDown(KeyCode.D))
    //    else if (touchpad_gesture.Direction == MLInputControllerTouchpadGestureDirection.Up)
    //        {
    //            pathIdSelectionView.GetComponent<PathIdSelectionView>().selectLast();
    //        }
    //}

    private void pathIdSelectionTrigger() {
        setMode(OrderPickingMode.BookInfo);
        mergeArr = pr.getMergedArry();
        
        if(selectedPhase == 0)
        {
            selectedPathId = mergeArr[positionRound - 1, placeRound - 1];

        } else if (selectedPhase == 1)
        {
            selectedPathId = mergeArr[positionRound - 1, (placeRound - 1) + 10];
            Debug.Log((placeRound - 1) + 10);


        }

        // setup the next view
        if (selectedPathId != pr.getPathId())
        {
            pr.setPathId(selectedPathId);
            selectedBookNum = 0;
            record_posted_book.Clear();
        }
        bookInfoView.GetComponent<BookInfoView>().highlightBookInfo(pr.getBookWithLocation(selectedBookNum));

        selectedBookTag = "A-0-0";
        postdata();
    }


    private void bookInfoControl(MLInputControllerTouchpadGesture touchpad_gesture)
    {
        // unused
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
        
        selectedBookNum = 0;
        selectedBookTag = "";
        record_posted_book.Clear();
 
        if (selectedPhase == 0)
        {
            if (placeRound == 5)
            {
                placeRound = 0;
                if (positionRound == 4)
                {
                    positionRound = 0;
                    setMode(OrderPickingMode.UserSelection);
                } else
                {
                setMode(OrderPickingMode.Placement);

                }
            } else
            {
                setMode(OrderPickingMode.PathIdSelection);
            }
        }
        else if (selectedPhase == 1)
        {
            Debug.Log(placeRound);
            if (placeRound == 5)
            {
                placeRound = 0;
                if (positionRound == 4)
                {
                    positionRound = 0;
                    setMode(OrderPickingMode.UserSelection);
                    selectedUserId = 0;
                    selectedPhase = 0; // 0 indicates training, 1 indicates testing
                    //selectedPathId = 1;
                    
                }
                else
                {
                    setMode(OrderPickingMode.Placement);
                }
            }
            else
            {
                setMode(OrderPickingMode.PathIdSelection);
            }
        }
        
    }

    private void completionBumper() {
        // unused
    }

    private void shelfControl(MLInputControllerTouchpadGesture touchpad_gesture)
    {
        // unused
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
            shelfView.GetComponent<ShelfView>().highlightBlock(pr.getBookWithLocation(selectedBookNum));
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
        if (Input.GetKeyDown(KeyCode.B)) {
            OnBumper();
        }
        // on desktop only
        keyAltInput();
    }
}




//Code e?
