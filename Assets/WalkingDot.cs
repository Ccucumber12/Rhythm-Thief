using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalkingDot : MonoBehaviour
{
    public float duraiton;
    public GameObject trackLine;
    [SerializeField] private GameObject PrefabWalkingDotCircle;

    // TODO : Cetranlize the TimestampManager?
    [SerializeField] private StageMusicData musicData;
    private TimestampManager timeManager = new TimestampManager();

    private DotTrack WalkingDotTrack;

    void Start()
    {
        //float fullWidth = sheetArea.rectTransform.rect.width;
        //float fullHeight = sheetArea.rectTransform.rect.height;

        timeManager.ParseFromJSON(musicData.timestamp.text);
        WalkingDotTrack = new DotTrack(PrefabWalkingDotCircle, duraiton, timeManager.IncrMoveIndex, timeManager.GetNextMoveTimestamp, trackLine);
        float circleSize= 1f;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateUsingMusicTime(float musicTime)
    {
        WalkingDotTrack.UpdateUsingMusicTime(musicTime);
    }

    //public void HittedUsingHitTime(float hitTime)
    //{
    //    for (int i = 0; i < trackSingle.Length; ++i)
    //        trackSingle[i].HittedUsingHitTime(hitTime);
    //    //for (int i = 0; i < trackDuration.Length; ++i)
    //    //    trackSingle[i].HittedUsingHitTime(hitTime);
    //}
}


public interface DotInterface
{
    public void Update(float musicTime);
    public void DestroyIfShould(float musicTime);
    public int getState();
    public void HittedCheck(float hitTime);
    public float getProgress(float musicTime);
}



public class DotTrack
{
    private GameObject prefab;
    public delegate void FuncIncr();
    public delegate float FuncGetNextTime();
    public FuncIncr funcIncr;
    public FuncGetNextTime funcGetNextTime;
    public float pixelPerSecond;
    public float duration;
    private List<DotInterface> sheetObjects = new List<DotInterface>();
    protected GameObject trackLine;

    public DotTrack(
        GameObject prefab,
        float duraiton,
        FuncIncr funcIncr,
        FuncGetNextTime funcGetNextTime,
        GameObject trackLine
    )
    {
        this.prefab = prefab;
        this.duration = duraiton;
        this.funcIncr = funcIncr;
        this.funcGetNextTime = funcGetNextTime;
        this.trackLine = trackLine;
    }

    public void UpdateUsingMusicTime(float musicTime)
    {
        // push new object into list
        while (true)
        {
            DotInterface obj = getNewObject(musicTime);
            if (obj == null) break;
            sheetObjects.Add(obj);
            Debug.Log("SomethingIn!");
        }

        // update and remove
        for (int i = sheetObjects.Count - 1; i >= 0; --i)
        {
            DotInterface obj = sheetObjects[i];
            obj.Update(musicTime);
            if (obj.getState() == 1)
            {
                sheetObjects.RemoveAt(i);
            }
        }
    }

    protected DotInterface getNewObject(float musicTime)
    {
        float objectTime = funcGetNextTime();
        if (objectTime > musicTime + duration)
        {
            return null;
        }

        GameObject obj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, trackLine.transform);

        RectTransform tf = obj.GetComponent<RectTransform>();
        tf.localPosition = new Vector3(0, 0, 0);
        tf.sizeDelta = new Vector2(10, 10);
        funcIncr();

        return new SheetObject(obj, objectTime, duration);
    }


    private class SheetObject : DotInterface
    {
        private GameObject gameObject;
        private float expireTime;
        private int state = 0; // active: 0, shouldBeRemoved: 1, hitted: 2
        private float duration;

        public SheetObject(GameObject gameObject, float expireTime, float duration)
        {
            this.gameObject = gameObject;
            this.expireTime = expireTime;
            this.duration = duration;
            // this.shrinkRate = shrinkRate;
        }

        public float getProgress(float musicTime)
        {
            return (expireTime - musicTime) / duration;
            // return from 1 (appear) -> 0 (disappear)

        }

        public void DestroyIfShould(float musicTime)
        {
            bool shouldDestroy;
            shouldDestroy = (getProgress(musicTime) <= 0);
            if (shouldDestroy)
            {
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
            }
        }

        public void Update(float musicTime)
        {
            float scale = getProgress(musicTime);
            gameObject.GetComponent<DrawCircle>().draw_circle(100, scale);
            //tf.sizeDelta = new Vector2(10 * scale, 10 * scale);
            DestroyIfShould(musicTime);
        }

        public int getState()
        {
            return state;
        }
    }
}
