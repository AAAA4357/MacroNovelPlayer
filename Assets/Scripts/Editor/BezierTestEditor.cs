using System.Collections.Generic;
using System.Linq;
using MNP.Helpers;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierTest))]
public class BezierTestEditor : Editor
{
    float t;

    List<Vector2> points = new();
    List<Vector2> point1s = new();

    bool ready;
    bool animationStart;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        t = EditorGUILayout.Slider("插值t", t, 0, 1);
        if (GUILayout.Button("生成表格"))
        {
            points.Clear();
            point1s.Clear();
            float totalLength = PathLerpHelper.GetLengthAtParameter2D(((BezierTest)target).P0, ((BezierTest)target).C0, ((BezierTest)target).C1, ((BezierTest)target).P1);
            for (int i = 0; i < 15; i++)
            {
                float t = (float)i / 14;
                float curveLength = PathLerpHelper.GetLengthAtParameter2D(((BezierTest)target).P0, ((BezierTest)target).C0, ((BezierTest)target).C1, ((BezierTest)target).P1, 0, t);
                points.Add(new(t, curveLength / totalLength));
            }
            for (int i = 0; i < 15; i++)
            {
                float t1 = (float)i / 14;
                UtilityHelper.GetFloorIndexInContainer(points, x => x.y, t1, out int mapIndex);
                Vector2 start = new(points[mapIndex].y, points[mapIndex].x);
                Vector2 end = new(points[mapIndex + 1].y, points[mapIndex + 1].x);
                float delta = end.y - start.y;
                float averageT = start.y + (t1 - start.x) / (end.x - start.x) * delta;
                point1s.Add(new(t1, averageT));
            }
            ready = true;
        }
        EditorGUI.BeginDisabledGroup(!ready);
        if (GUILayout.Button("开始动画"))
        {
            animationStart = true;
        }
        EditorGUI.EndDisabledGroup();
    }

    void OnSceneGUI()
    {
        if (animationStart)
        {
            t += 0.005f;
            if (t >= 1)
            {
                t = 0;
                animationStart = false;
            }
        }
        ((BezierTest)target).P0 = Handles.PositionHandle(((BezierTest)target).P0, Quaternion.identity);
        ((BezierTest)target).P1 = Handles.PositionHandle(((BezierTest)target).P1, Quaternion.identity);
        ((BezierTest)target).C0 = Handles.PositionHandle(((BezierTest)target).C0, Quaternion.identity);
        ((BezierTest)target).C1 = Handles.PositionHandle(((BezierTest)target).C1, Quaternion.identity);

        Handles.color = Color.yellow;
        Handles.DrawSolidDisc(((BezierTest)target).P0, Vector3.back, 0.5f);
        Handles.DrawSolidDisc(((BezierTest)target).P1, Vector3.back, 0.5f);
        Handles.color = Color.blue;
        Handles.DrawSolidDisc(((BezierTest)target).C0, Vector3.back, 0.5f);
        Handles.DrawSolidDisc(((BezierTest)target).C1, Vector3.back, 0.5f);
        Handles.color = Color.white;
        Handles.DrawBezier(((BezierTest)target).P0, ((BezierTest)target).P1, ((BezierTest)target).C0, ((BezierTest)target).C1, Color.white, null, 4);
        Handles.color = Color.red;
        Handles.DrawSolidDisc((Vector2)PathLerpHelper.GetBezierPoint2D(((BezierTest)target).P0, ((BezierTest)target).C0, ((BezierTest)target).C1, ((BezierTest)target).P1, t), Vector3.back, 0.25f);
        DrawTable0();
        DrawTable1();
        if (point1s.Count == 0)
            return;
        UtilityHelper.GetFloorIndexInContainer(point1s, x => x.x, t, out int mapIndex);
        Vector2 start = point1s[mapIndex];
        Vector2 end = point1s[mapIndex + 1];
        float delta = end.y - start.y;
        float averageT = start.y + (t - start.x) / (end.x - start.x) * delta;
        Vector2 p = PathLerpHelper.GetBezierPoint2D(((BezierTest)target).P0, ((BezierTest)target).C0, ((BezierTest)target).C1, ((BezierTest)target).P1, averageT);
        Handles.color = Color.green;
        Handles.DrawSolidDisc(p, Vector3.back, 0.25f);
    }

    void DrawTable0()
    {
        if (points.Count == 0)
            return;
        Handles.color = Color.yellow;
        List<Vector3> lineList = new()
        {
            points[0]
        };
        for (int i = 0; i < points.Count; i++)
        {
            if (i == 0)
            {
                Handles.DrawSolidDisc(points[i], Vector3.back, 0.01f);
                continue;
            }
            lineList.Add(points[i]);
            lineList.Add(points[i]);
            Handles.DrawSolidDisc(points[i], Vector3.back, 0.01f);
        }
        lineList.RemoveAt(lineList.Count - 1);
        Handles.DrawLines(lineList.ToArray());
    }

    void DrawTable1()
    {
        if (point1s.Count == 0)
            return;
        Handles.color = Color.cyan;
        List<Vector3> lineList = new()
        {
            point1s[0]
        };
        for (int i = 0; i < point1s.Count; i++)
        {
            if (i == 0)
            {
                Handles.DrawSolidDisc(point1s[i], Vector3.back, 0.01f);
                continue;
            }
            lineList.Add(point1s[i]);
            lineList.Add(point1s[i]);
            Handles.DrawSolidDisc(point1s[i], Vector3.back, 0.01f);
        }
        lineList.RemoveAt(lineList.Count - 1);
        Handles.DrawLines(lineList.ToArray());
    }
}
