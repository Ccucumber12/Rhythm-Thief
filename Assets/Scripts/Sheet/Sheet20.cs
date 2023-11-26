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
    [SerializeField] private GameObject PrefabDurationIndicator;

    // TODO : Cetranlize the TimestampManager?
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
            new SheetTrackSingle(3, PrefabMoveIcon, pixelPerSecond, fullWidth, timeManager.IncrMoveIndex, timeManager.GetNextMoveTimestamp),
        };
        trackDuration = new [] {
            new SheetTrackDuration(2, PrefabDoorIcon, PrefabDurationIndicator, Color.cyan, pixelPerSecond, fullWidth, timeManager.IncrGateOpenIndex, timeManager.GetNextGateOpenTimestamp, timeManager.IncrGateCloseIndex, timeManager.GetNextGateCloseTimestamp),
            new SheetTrackDuration(1, PrefabLightIcon, PrefabDurationIndicator, Color.yellow, pixelPerSecond, fullWidth, timeManager.IncrLightsOffIndex, timeManager.GetNextLightsOffTimestamp, timeManager.IncrLightsOnIndex, timeManager.GetNextLightsOnTimestamp),
            new SheetTrackDuration(0, PrefabBellIcon, PrefabDurationIndicator, Color.magenta,  pixelPerSecond, fullWidth, timeManager.IncrBellRingIndex, timeManager.GetNextBellRingTimestamp, timeManager.IncrBellStopIndex, timeManager.GetNextBellStopTimestamp),
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

    public void HittedUsingHitTime(float hitTime)
    {
        for (int i = 0; i < trackSingle.Length; ++i)
            trackSingle[i].HittedUsingHitTime(hitTime);
        //for (int i = 0; i < trackDuration.Length; ++i)
        //    trackSingle[i].HittedUsingHitTime(hitTime);
    }
}


public interface SheetObjectInterface {
    public void Update(float musicTime);
    public void DestroyIfShould(float musicTime);
    public int getState();
    public void HittedCheck(float hitTime);
}


public abstract class SheetTrackBase {
    protected int sheetIndex;
    protected float pixelPerSecond;
    protected float iconSize;
    protected float trackWidth;
    protected float trackHeight;
    protected GameObject trackLine;

    protected float sheetLeftTime {get {return - 10;}}
    protected float sheetRightTime {get {return trackWidth / pixelPerSecond;}}

    private List<SheetObjectInterface> sheetObjects = new List<SheetObjectInterface>();

    public void setTrackHeightAndDrawTrackLine(float trackHeight, float iconSize, GameObject prefabTrackLine, Transform parentTransform) {
        this.trackHeight = trackHeight;
        this.iconSize = iconSize;

        // draw the track lines
        trackLine = GameObject.Instantiate(prefabTrackLine, Vector3.zero, Quaternion.identity, parentTransform);
        RectTransform tf = trackLine.GetComponent<RectTransform>();
        tf.localPosition = new Vector3(0, trackHeight * (0.5f + sheetIndex), tf.localPosition.z);
        // tf.offsetMin = new Vector2(0, tf.offsetMin.y);
        // tf.offsetMax = new Vector2(0, tf.offsetMax.y);
    }

    protected abstract SheetObjectInterface getNewObject(float musicTime);

    public void UpdateUsingMusicTime(float musicTime) {
        // push new object into list
        while (true) {
            SheetObjectInterface obj = getNewObject(musicTime);
            if (obj == null) break;
            sheetObjects.Add(obj);
        }

        // update and remove
        for (int i = sheetObjects.Count - 1; i >= 0; --i) {
            SheetObjectInterface obj = sheetObjects[i];
            obj.Update(musicTime);
            if (obj.getState() == 1) {
                sheetObjects.RemoveAt(i);
            }
        }
    }

    public void HittedUsingHitTime(float hitTime) {
        // update and remove
        for (int i = sheetObjects.Count - 1; i >= 0; --i) {
            SheetObjectInterface obj = sheetObjects[i];
            obj.HittedCheck(hitTime);
        }
    }
}


public class SheetTrackSingle : SheetTrackBase {
    private GameObject prefab;

    public delegate void FuncIncr();
    public delegate float FuncGetNextTime();
    public FuncIncr funcIncr;
    public FuncGetNextTime funcGetNextTime;

    public SheetTrackSingle(
        int sheetIndex,
        GameObject prefab,
        float pixelPerSecond,
        float trackWidth,
        FuncIncr funcIncr,
        FuncGetNextTime funcGetNextTime
    ) {
        this.sheetIndex = sheetIndex;
        this.prefab = prefab;
        this.pixelPerSecond = pixelPerSecond;
        this.trackWidth = trackWidth;
        this.funcIncr = funcIncr;
        this.funcGetNextTime = funcGetNextTime;
    }

    protected override SheetObjectInterface getNewObject(float musicTime) {
        float objectTime = funcGetNextTime();
        if (objectTime > musicTime + sheetRightTime) {
            return null;
        }

        GameObject obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, trackLine.transform);
        RectTransform tf = obj.GetComponent<RectTransform>();
        tf.localPosition = new Vector3(sheetRightTime * pixelPerSecond, 0, 0);
        tf.sizeDelta = new Vector2(iconSize, iconSize);
        funcIncr();

        return new SheetObject(obj, objectTime, pixelPerSecond, sheetLeftTime);
    }


    private class SheetObject : SheetObjectInterface {
        private GameObject gameObject;
        private float expireTime;
        private float pixelPerSecond;
        private float sheetLeftTime;
        private int state = 0; // active: 0, shouldBeRemoved: 1, hitted: 2
        private float animationExpireTime;
        private float animationDuration = 0.25f;

        public SheetObject(GameObject gameObject, float expireTime, float pixelPerSecond, float sheetLeftTime) {
            this.gameObject = gameObject;
            this.expireTime = expireTime;
            this.pixelPerSecond = pixelPerSecond;
            this.sheetLeftTime = sheetLeftTime;
        }

        public void DestroyIfShould(float musicTime) {
            bool shouldDestroy;
           
            shouldDestroy = ( state == 0  ) ? (expireTime - musicTime < sheetLeftTime)
                                            : (animationExpireTime <= musicTime);
            if (shouldDestroy) {
                Object.Destroy(gameObject);
                state = 1;
            }
        }

        public void HittedCheck(float hitTime)
        {
            if (state == 0 && hitTime == expireTime)
            {
                // Object.Destroy(gameObject);
                gameObject.GetComponent<Animator>().SetBool("Hitted", true);
                state = 2;
                animationExpireTime = hitTime + animationDuration;
            }
        }

        public void Update(float musicTime) {
            RectTransform tf = gameObject.GetComponent<RectTransform>();
            tf.localPosition = new Vector3((expireTime - musicTime) * pixelPerSecond, 0, 0);
            DestroyIfShould(musicTime);
        }

        public int getState()
        {
            return state;
        }
    }
}

public class SheetTrackDuration : SheetTrackBase {
    private GameObject prefab;
    private GameObject prefabDurationIndicator;
    private Color stripeColor;

    public delegate void FuncStartIncr();
    public delegate float FuncStartGetNextTime();
    public delegate void FuncEndIncr();
    public delegate float FuncEndGetNextTime();
    public FuncStartIncr funcStartIncr;
    public FuncStartGetNextTime funcStartGetNextTime;
    public FuncEndIncr funcEndIncr;
    public FuncEndGetNextTime funcEndGetNextTime;

    public SheetTrackDuration(
        int sheetIndex,
        GameObject prefab,
        GameObject prefabDurationIndicator,
        Color stripeColor,
        float pixelPerSecond,
        float trackWidth,
        FuncStartIncr funcStartIncr,
        FuncStartGetNextTime funcStartGetNextTime,
        FuncEndIncr funcEndIncr,
        FuncEndGetNextTime funcEndGetNextTime
    ) {
        this.sheetIndex = sheetIndex;
        this.prefab = prefab;
        this.stripeColor = stripeColor;
        this.prefabDurationIndicator = prefabDurationIndicator;
        this.pixelPerSecond = pixelPerSecond;
        this.trackWidth = trackWidth;
        this.funcStartIncr = funcStartIncr;
        this.funcStartGetNextTime = funcStartGetNextTime;
        this.funcEndIncr = funcEndIncr;
        this.funcEndGetNextTime = funcEndGetNextTime;
    }


    protected override SheetObjectInterface getNewObject(float musicTime) {
        float objectStartTime = funcStartGetNextTime();
        float objectEndTime = funcEndGetNextTime();
        if (objectStartTime > musicTime + sheetRightTime) {
            return null;
        }

        // parent: the background stripe for the duration
        GameObject durationIndicator = GameObject.Instantiate(prefabDurationIndicator, Vector3.zero, Quaternion.identity, trackLine.transform);
        {
            RectTransform tf = durationIndicator.GetComponent<Image>().rectTransform;
            tf.localPosition = new Vector3(sheetRightTime * pixelPerSecond, 0, 0);
            tf.sizeDelta = new Vector2((objectEndTime - objectStartTime) * pixelPerSecond, iconSize * 0.5f);
        }

        // child: the icon itself
        GameObject icon = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, durationIndicator.transform);
        {
            RectTransform tf = icon.GetComponent<RectTransform>();
            tf.localPosition = new Vector3(0, 0, 0);
            tf.sizeDelta = new Vector2(iconSize, iconSize);
        }

        funcStartIncr();
        funcEndIncr();

        return new SheetObject(durationIndicator, icon, objectStartTime, objectEndTime, pixelPerSecond, sheetLeftTime, stripeColor);
    }

    private class SheetObject : SheetObjectInterface {
        private GameObject gameObject;
        private GameObject icon;
        private Color stripeColor;
        private float startTime, endTime;
        private float pixelPerSecond;
        private float sheetLeftTime;
        private int state = 0;
        // active: 0, shouldBeRemoved: 1, hitted: 2

        public SheetObject(GameObject gameObject, GameObject icon, float startTime, float endTime, float pixelPerSecond, float sheetLeftTime, Color stripeColor) {
            this.gameObject = gameObject;
            this.icon = icon;
            this.startTime = startTime;
            this.endTime = endTime;
            this.pixelPerSecond = pixelPerSecond;
            this.sheetLeftTime = sheetLeftTime;
            this.stripeColor = stripeColor;
        }

        public void DestroyIfShould(float musicTime) {
            bool shouldDestroy = endTime < musicTime;
            if (shouldDestroy) {
                Object.Destroy(gameObject);
                state = 1;
            }
        }

        public void Update(float musicTime) {
            RectTransform tf = gameObject.GetComponent<RectTransform>();
            if (musicTime > startTime){
                // Hitted for duration
                float originalSize = tf.sizeDelta[1];
                tf.localPosition = new Vector3(0, 0, 0);
                tf.sizeDelta = new Vector2(Mathf.Max((endTime - musicTime) * pixelPerSecond, 0), originalSize);
                gameObject.GetComponentsInChildren<RectTransform>()[1].localPosition = new Vector3(0, 0, 0);
                gameObject.GetComponent<Image>().color = stripeColor;
                icon.GetComponent<Animator>().SetBool("Hitted", true);
            }
            else{
                tf.localPosition = new Vector3((startTime - musicTime) * pixelPerSecond, 0, 0);
            }
            DestroyIfShould(musicTime);
        }

        public void HittedCheck(float musicTime)
        {
            // Hitted for single -> pass
            return;
        }

        public int getState()
        {
            return state;
        }
    }
}


// background stripe -> animation?
// fire -> ???
// Disco ball ->
// Player's helper animation?