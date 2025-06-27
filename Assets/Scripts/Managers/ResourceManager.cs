using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MNP.Core.DataStruct;


public class ResourceManager : MonoBehaviour
{
    public List<BezierCurve> CurveList = new();

    public List<EasingLerp> easings = new();
}
