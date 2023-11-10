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


public abstract class SheetTrackBase {
    protected int sheetIndex;
    protected float pixelPerSecond;
    protected float iconSize;
    protected float trackWidth;
    protected float trackHeight;
    protected GameObject trackLine;

    protected float sheetLeftTime {get {return - trackWidth / pixelPerSecond / 2;}}
    protected float sheetRightTime {get {return trackWidth / pixelPerSecond / 2;}}

    private List<SheetObject> sheetObjects = new List<SheetObject>();

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

    protected abstract SheetObject getNewObject(float musicTime);
    protected abstract void updateObject(float musicTime, SheetObject sheetObject);
    protected abstract bool shouldRemoveObject(float musicTime, SheetObject sheetObject);

    public void UpdateUsingMusicTime(float musicTime) {
        // push new object into list
        while (true) {
            SheetObject obj = getNewObject(musicTime);
            if (obj == null) break;
            sheetObjects.Add(obj);
        }

        // update and remove
        for (int i = sheetObjects.Count - 1; i >= 0; --i) {
            SheetObject obj = sheetObjects[i];
            updateObject(musicTime, obj);
            if (shouldRemoveObject(musicTime, obj)) {
                obj.Destroy();
                sheetObjects.RemoveAt(i);
            }
        }
    }


    protected class SheetObject {
        private GameObject _gameObject;
        public GameObject gameObject {get {return _gameObject;}}
        private float _timestamp;
        public float timestamp {get {return _timestamp;}}

        public SheetObject(GameObject gameObject, float timestamp) {
            this._gameObject = gameObject;
            this._timestamp = timestamp;
        }

        public void Destroy() {
            Object.Destroy(_gameObject);
        }
    }
}


public class SheetTrackSingle : SheetTrackBase {
    private GameObject prefab;

    public delegate void FuncIncr();
    public delegate float FuncGetNextTime();
    public FuncIncr funcIncr;
    public FuncGetNextTime funcGetNextTime;

    public SheetTrackSingle(int sheetIndex, GameObject prefab, float pixelPerSecond, float trackWidth, FuncIncr funcIncr, FuncGetNextTime funcGetNextTime) {
        this.sheetIndex = sheetIndex;
        this.prefab = prefab;
        this.pixelPerSecond = pixelPerSecond;
        this.trackWidth = trackWidth;
        this.funcIncr = funcIncr;
        this.funcGetNextTime = funcGetNextTime;
    }

    protected override SheetObject getNewObject(float musicTime) {
        float objectTime = funcGetNextTime();
        if (objectTime > musicTime + sheetRightTime) {
            return null;
        }

        GameObject obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, trackLine.transform);
        RectTransform tf = obj.GetComponent<RectTransform>();
        tf.localPosition = new Vector3(sheetRightTime * pixelPerSecond, 0, 0);
        tf.sizeDelta = new Vector2(iconSize, iconSize);
        funcIncr();

        return new SheetObject(obj, objectTime);
    }

    protected override void updateObject(float musicTime, SheetObject sheetObject) {
        RectTransform tf = sheetObject.gameObject.GetComponent<RectTransform>();
        tf.localPosition = new Vector3((sheetObject.timestamp - musicTime) * pixelPerSecond, 0, 0);
    }

    protected override bool shouldRemoveObject(float musicTime, SheetObject sheetObject) {
        return sheetObject.timestamp - musicTime < sheetLeftTime;
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
