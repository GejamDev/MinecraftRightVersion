using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowObject : MonoBehaviour
{

    public Custom_R_Bow bow;
    public LineRenderer lr;
    
    public Transform lineStrTr;
    public Transform lineMidTr;
    public Transform lineEndTr;
    public Vector3 originPos;
    public Vector3 chargedPos;
    public Vector3 originRot;
    public Vector3 chargedRot;
    public float posLerp;
    public float rotLerp;

    public Vector3 mid_originPos;
    public Vector3 mid_chargedPos;
    public float stringBendSpeed;
    public float stringReturnSpeed;



    void LateUpdate()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, bow.inUse ? chargedPos : originPos, posLerp * Time.deltaTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(bow.inUse ? chargedRot : originRot), rotLerp * Time.deltaTime);

        lineMidTr.localPosition = Vector3.Lerp(lineMidTr.localPosition, bow.inUse ? mid_chargedPos : mid_originPos, (bow.inUse ? stringBendSpeed : stringReturnSpeed) * Time.deltaTime);



        lr.SetPosition(0, lineStrTr.localPosition);
        lr.SetPosition(1, lineMidTr.localPosition);
        lr.SetPosition(2, lineEndTr.localPosition);
    }
}
