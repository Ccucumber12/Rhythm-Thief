using UnityEngine;
using UnityEngine.UI;

public class Sheet20 : MonoBehaviour {
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
    private int numOfTrack;

    // Start is called before the first frame update
    void Start() {
        timeManager.ParseFromJSON(musicData.timestamp.text);
        trackSingle = new [] {
            new SheetTrackSingle(0, PrefabMoveIcon, timeManager.IncrMoveIndex, timeManager.GetNextMoveTimestamp, timeManager.GetPrevMoveTimestamp),
        };
        trackDuration = new [] {
            new SheetTrackDuration(1, PrefabDoorIcon, timeManager.IncrGateOpenIndex, timeManager.GetNextGateOpenTimestamp, timeManager.GetPrevGateOpenTimestamp, timeManager.IncrGateCloseIndex, timeManager.GetNextGateCloseTimestamp, timeManager.GetPrevGateCloseTimestamp),
            new SheetTrackDuration(2, PrefabDoorIcon, timeManager.IncrLightsOnIndex, timeManager.GetNextLightsOnTimestamp, timeManager.GetPrevLightsOnTimestamp, timeManager.IncrLightsOffIndex, timeManager.GetNextLightsOffTimestamp, timeManager.GetPrevLightsOffTimestamp),
        };
        numOfTrack = trackSingle.Length + trackDuration.Length;

        // draw the track lines
        float fullHeight = GetComponentInChildren<RectMask2D>().rectTransform.rect.height;
        float trackHeight = fullHeight / numOfTrack;
        for (int i = 0; i < numOfTrack; ++i) {
            GameObject line = Instantiate(PrefabTrackLine, Vector3.zero, Quaternion.identity, this.GetComponentInChildren<RectMask2D>().transform);
            RectTransform tf = line.GetComponent<RectTransform>();
            tf.localPosition = new Vector3(tf.position.x, trackHeight * (0.5f + i), tf.position.z);
            tf.offsetMin = new Vector2(0, tf.offsetMin.y);
            tf.offsetMax = new Vector2(0, tf.offsetMax.y);
        }
    }

    // Update is called once per frame
    void Update() {
    }
}


public class SheetTrackSingle {
    public int sheetIndex;
    public GameObject prefab;
    public delegate void FuncIncr();
    public delegate float FuncGetNextTime();
    public delegate float FuncGetPrevTime();

    public FuncIncr funcIncr;
    public FuncGetNextTime funcGetNextTime;
    public FuncGetPrevTime funcGetPrevTime;


    public SheetTrackSingle(int sheetIndex, GameObject prefab, FuncIncr funcIncr, FuncGetNextTime funcGetNextTime, FuncGetPrevTime funcGetPrevTime) {
        this.sheetIndex = sheetIndex;
        this.prefab = prefab;
        this.funcGetNextTime = funcGetNextTime;
        this.funcGetPrevTime = funcGetPrevTime;
    }
}

public class SheetTrackDuration {
    public int sheetIndex;
    public GameObject prefab;
    public delegate void FuncStartIncr();
    public delegate float FuncStartGetNextTime();
    public delegate float FuncStartGetPrevTime();
    public delegate void FuncEndIncr();
    public delegate float FuncEndGetNextTime();
    public delegate float FuncEndGetPrevTime();

    public FuncStartIncr funcStartIncr;
    public FuncStartGetNextTime funcStartGetNextTime;
    public FuncStartGetPrevTime funcStartGetPrevTime;
    public FuncEndIncr funcEndIncr;
    public FuncEndGetNextTime funcEndGetNextTime;
    public FuncEndGetPrevTime funcEndGetPrevTime;


    public SheetTrackDuration(int sheetIndex, GameObject prefab, FuncStartIncr funcStartIncr, FuncStartGetNextTime funcStartGetNextTime, FuncStartGetPrevTime funcStartGetPrevTime, FuncEndIncr funcEndIncr, FuncEndGetNextTime funcEndGetNextTime, FuncEndGetPrevTime funcEndGetPrevTime) {
        this.sheetIndex = sheetIndex;
        this.prefab = prefab;
        this.funcStartIncr = funcStartIncr;
        this.funcStartGetNextTime = funcStartGetNextTime;
        this.funcStartGetPrevTime = funcStartGetPrevTime;
        this.funcEndIncr = funcEndIncr;
        this.funcEndGetNextTime = funcEndGetNextTime;
        this.funcEndGetPrevTime = funcEndGetPrevTime;
    }
}
