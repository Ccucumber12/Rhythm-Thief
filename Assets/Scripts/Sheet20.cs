using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sheet20 : MonoBehaviour {
    public float pixelPerSecond;
    [Range(0f, 1f)]
    public float iconSizeToTrackHeightRatio;

    [SerializeField] private GameObject PrefabMoveIcon;
    [SerializeField] private GameObject PrefabDoorIcon;
    [SerializeField] private GameObject PrefabGunIcon;
    [SerializeField] private GameObject PrefabLightIcon;
    [SerializeField] private GameObject PrefabBellIcon;
    [SerializeField] private GameObject PrefabTrackLine;
    // [SerializeField] private GameObject PrefabDiscoIcon; // TODO

    [SerializeField] private StageMusicData musicData;
    private TimestampManager timeManager = new TimestampManager();

    private SheetTrackSingle[] trackSingle;
    private SheetTrackDuration[] trackDuration;

    // Start is called before the first frame update
    void Start() {
        RectMask2D sheetArea = GetComponentInChildren<RectMask2D>();
        float fullWidth = sheetArea.rectTransform.rect.width;
        float fullHeight = sheetArea.rectTransform.rect.height;

        timeManager.ParseFromJSON(musicData.timestamp.text);
        trackSingle = new [] {
            new SheetTrackSingle(0, PrefabMoveIcon, pixelPerSecond, fullWidth, timeManager.IncrMoveIndex, timeManager.GetNextMoveTimestamp),
        };
        trackDuration = new [] {
            new SheetTrackDuration(1, PrefabDoorIcon, timeManager.IncrGateOpenIndex, timeManager.GetNextGateOpenTimestamp, timeManager.IncrGateCloseIndex, timeManager.GetNextGateCloseTimestamp),
            new SheetTrackDuration(2, PrefabDoorIcon, timeManager.IncrLightsOnIndex, timeManager.GetNextLightsOnTimestamp, timeManager.IncrLightsOffIndex, timeManager.GetNextLightsOffTimestamp),
        };

        int numOfTrack = trackSingle.Length + trackDuration.Length;
        float trackHeight = fullHeight / numOfTrack;
        float iconSize = trackHeight * iconSizeToTrackHeightRatio;
        for (int i = 0; i < trackSingle.Length; ++i)
            trackSingle[i].setTrackHeightAndDrawTrackLine(trackHeight, iconSize, PrefabTrackLine, sheetArea.transform);
        for (int i = 0; i < trackDuration.Length; ++i)
            trackDuration[i].setTrackHeightAndDrawTrackLine(trackHeight, iconSize, PrefabTrackLine, sheetArea.transform);
    }

    // Update is called once per frame
    void Update() {
    }

    public void UpdateUsingMusicTime(float musicTime) {
        for (int i = 0; i < trackSingle.Length; ++i)
            trackSingle[i].UpdateUsingMusicTime(musicTime);
        for (int i = 0; i < trackDuration.Length; ++i)
            trackDuration[i].UpdateUsingMusicTime(musicTime);
    }
}


public class SheetTrackSingle {
    private int sheetIndex;
    private GameObject prefab;
    private GameObject trackLine;
    private float pixelPerSecond;
    private float trackWidth;
    private float trackHeight;
    private float iconSize;

    public delegate void FuncIncr();
    public delegate float FuncGetNextTime();

    public FuncIncr funcIncr;
    public FuncGetNextTime funcGetNextTime;

    private List<GameObject> instances = new List<GameObject>();
    private List<float> instancesTimestamp = new List<float>();

    private float sheetLeftTime {get {return - trackWidth / pixelPerSecond / 2;}}
    private float sheetRightTime {get {return trackWidth / pixelPerSecond / 2;}}

    public SheetTrackSingle(int sheetIndex, GameObject prefab, float pixelPerSecond, float trackWidth, FuncIncr funcIncr, FuncGetNextTime funcGetNextTime) {
        this.sheetIndex = sheetIndex;
        this.prefab = prefab;
        this.pixelPerSecond = pixelPerSecond;
        this.trackWidth = trackWidth;
        this.funcIncr = funcIncr;
        this.funcGetNextTime = funcGetNextTime;
    }

    public void setTrackHeightAndDrawTrackLine(float trackHeight, float iconSize, GameObject prefabTrackLine, Transform parentTransform) {
        this.trackHeight = trackHeight;
        this.iconSize = iconSize;

        // draw the track lines
        trackLine = GameObject.Instantiate(prefabTrackLine, Vector3.zero, Quaternion.identity, parentTransform);
        RectTransform tf = trackLine.GetComponent<RectTransform>();
        tf.localPosition = new Vector3(tf.localPosition.x, trackHeight * (0.5f + sheetIndex), tf.localPosition.z);
        tf.offsetMin = new Vector2(0, tf.offsetMin.y);
        tf.offsetMax = new Vector2(0, tf.offsetMax.y);
    }

    public void UpdateUsingMusicTime(float musicTime) {
        // push new object into list
        while (true) {
            if (funcGetNextTime() > musicTime + sheetRightTime) break;
            Debug.Log("instantiate at time = " + funcGetNextTime());
            GameObject obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, trackLine.transform);
            RectTransform tf = obj.GetComponent<RectTransform>();
            tf.localPosition = new Vector3(sheetRightTime * pixelPerSecond, 0, 0);
            tf.sizeDelta = new Vector2(iconSize, iconSize);
            instances.Add(obj);
            instancesTimestamp.Add(funcGetNextTime());
            funcIncr();
        }

        // update object position
        for (int i = 0; i < instances.Count; ++i) {
            GameObject obj = instances[i];
            float time = instancesTimestamp[i];
            RectTransform tf = obj.GetComponent<RectTransform>();
            tf.localPosition = new Vector3((time - musicTime) * pixelPerSecond, 0, 0);
        }

        // remove object if out of sight
        for (int i = instances.Count - 1; i >= 0; --i) {
            GameObject obj = instances[i];
            float time = instancesTimestamp[i];
            if (time - musicTime < sheetLeftTime) {
                Object.Destroy(obj);
                instances.RemoveAt(i);
                instancesTimestamp.RemoveAt(i);
            }
        }
    }
}

public class SheetTrackDuration {
    public int sheetIndex;
    public GameObject prefab;
    private float pixelPerSecond;
    private float trackWidth;
    private float trackHeight;
    private float iconSize;

    public delegate void FuncStartIncr();
    public delegate float FuncStartGetNextTime();
    public delegate void FuncEndIncr();
    public delegate float FuncEndGetNextTime();
    public FuncStartIncr funcStartIncr;
    public FuncStartGetNextTime funcStartGetNextTime;
    public FuncEndIncr funcEndIncr;
    public FuncEndGetNextTime funcEndGetNextTime;

    private List<GameObject> instances;
    private List<GameObject> instancesTimestamp;

    private float sheetLeftTime {get {return - trackWidth / pixelPerSecond / 2;}}
    private float sheetRightTime {get {return trackWidth / pixelPerSecond / 2;}}


    public SheetTrackDuration(int sheetIndex, GameObject prefab, FuncStartIncr funcStartIncr, FuncStartGetNextTime funcStartGetNextTime, FuncEndIncr funcEndIncr, FuncEndGetNextTime funcEndGetNextTime) {
        this.sheetIndex = sheetIndex;
        this.prefab = prefab;
        this.funcStartIncr = funcStartIncr;
        this.funcStartGetNextTime = funcStartGetNextTime;
        this.funcEndIncr = funcEndIncr;
        this.funcEndGetNextTime = funcEndGetNextTime;
    }

    public void setTrackHeightAndDrawTrackLine(float trackHeight, float iconSize, GameObject prefabTrackLine, Transform parentTransform) {
        this.trackHeight = trackHeight;
        this.iconSize = iconSize;

        // draw the track lines
        GameObject line = GameObject.Instantiate(prefabTrackLine, Vector3.zero, Quaternion.identity, parentTransform);
        RectTransform tf = line.GetComponent<RectTransform>();
        tf.localPosition = new Vector3(tf.position.x, trackHeight * (0.5f + sheetIndex), tf.position.z);
        tf.offsetMin = new Vector2(0, tf.offsetMin.y);
        tf.offsetMax = new Vector2(0, tf.offsetMax.y);
    }

    public void UpdateUsingMusicTime(float musicTime) {
    }
}
