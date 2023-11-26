using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingDot : MonoBehaviour
{
    public float upperBoundInSec;
    public GameObject DotIcon;
    [SerializeField] private StageMusicData musicData;
    private TimestampManager timeManager = new TimestampManager();
    private Vector3 DafaultSize = new Vector3(3f, 0.9f, 1f);
    // Start is called before the first frame update
    void Start()
    {
        timeManager.ParseFromJSON(musicData.timestamp.text);
    }

    public void UpdateUsingMusicTime(float musicTime)
    {
        while ( musicTime >= timeManager.GetNextMoveTimestamp())
        {
            timeManager.IncrMoveIndex();
        }

        float PrevMoveTimeDis = musicTime - timeManager.GetPrevMoveTimestamp();
        float NextMoveTimeDis = timeManager.GetNextMoveTimestamp() - musicTime;
        float delta = 0;
        if ( PrevMoveTimeDis < 0.1 )
        {
            delta = PrevMoveTimeDis;
        }
        else if( NextMoveTimeDis  < 0.2 )
        {
            delta = NextMoveTimeDis + 0.1f;
        }
        else
        {
            delta = 0;
        }
        delta /= 0.3f;

        DotIcon.transform.localScale = DafaultSize * (delta * 0.3f + 0.3f);
        DotIcon.GetComponent<SpriteRenderer>().color = new Color(delta, 0, 1 - delta, 0.3f);
    }
}
