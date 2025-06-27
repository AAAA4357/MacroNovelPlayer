using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using MNP.Core.DataStruct;
using MNP.Managers;
using MNP.Helper;
using Unity.VisualScripting;
using System.Collections.Generic;

[CustomEditor(typeof(ResourceManager))]
public class ResourceManagerEditor : Editor
{
    private SerializedProperty curveProperty;
    private ReorderableList curveList;

    private AnimBool debugEnabled;
    private bool showList;
    private bool showDebugCurve;
    private int selectedCurveIndex = -1;
    private float debugT;
    private float debugD;

    private SearchType searchType;
    private string searchQuery;

    private AddCurveType addCurveType;
    private AnimBool addCircular;
    private AnimBool addCircularWarn;
    private AnimBool addCustom;

    private Vector2 addP0;
    private Vector2 addP1;
    private float addRadius;
    private Vector2 addP2;
    private Vector2 addP3;

    private void OnEnable()
    {
        curveProperty = serializedObject.FindProperty("CurveList");
        curveList = new(serializedObject, curveProperty, true, true, true, true)
        {
            list = ((ResourceManager)target).CurveList,
            drawHeaderCallback = rect =>
            {
                GUI.Label(rect, "曲线列表");
            },
            onAddCallback = list =>
            {
                if (addCircularWarn.target)
                    return;
                list.serializedProperty.arraySize++;
                list.index = list.serializedProperty.arraySize - 1;
                SerializedProperty item = list.serializedProperty.GetArrayElementAtIndex(list.index);
                BezierCurve curve = addCurveType switch
                {
                    AddCurveType.Line => new(addP0, addP3),
                    AddCurveType.Circular => new(addP0, addP3, addRadius),
                    AddCurveType.Custom => new(addP0, addP1, addP2, addP3),
                    _ => new(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero)
                };
                item.boxedValue = curve;
            },
            onRemoveCallback = list =>
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                selectedCurveIndex = -1;
                list.Select(-1);
            },
            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty element = curveList.serializedProperty.GetArrayElementAtIndex(index);

                EditorGUI.LabelField(new(rect.x, rect.y, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight), "贝塞尔曲线#" + index);
                EditorGUI.PropertyField(new(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 5, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("P0"), new GUIContent("起始点"));
                EditorGUI.PropertyField(new(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2 + 10, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("P3"), new GUIContent("结束点"));
                EditorGUI.PropertyField(new(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 3 + 15, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("P1"), new GUIContent("控制点1"));
                EditorGUI.PropertyField(new(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 4 + 20, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("P2"), new GUIContent("控制点2"));
            },
            elementHeightCallback = index =>
            {
                return EditorGUIUtility.singleLineHeight * 5 + 25;
            },
            onSelectCallback = list =>
            {
                selectedCurveIndex = list.selectedIndices[0];
            }
        };

        debugEnabled = new(false);
        addCircular = new(false);
        addCircularWarn = new(false);
        addCustom = new(false);
    }

    public override void OnInspectorGUI()
    {
        GUIStyle h1 = new(GUI.skin.label)
        {
            fontSize = 20
        };
        GUIStyle h2 = new(GUI.skin.label)
        {
            fontSize = 16
        };
        GUILayout.Label("VNP曲线管理器", h1);
        EditorGUILayout.Separator();
        GUILayout.Label("管理器属性", h2);
        EditorGUILayout.Separator();
        EditorGUILayout.HelpBox("列表元素可能很多，展开后列表将会很长", MessageType.Warning);
        showList = EditorGUILayout.BeginFoldoutHeaderGroup(showList, "显示列表");
        if (showList)
        {
            if (EditorGUILayout.BeginFadeGroup(debugEnabled.faded))
            {
                searchType = (SearchType)EditorGUILayout.EnumPopup("搜索类型", searchType);
                searchQuery = EditorGUILayout.TextField("搜索关键词", searchQuery);
                GUILayout.BeginHorizontal();
                List<BezierCurve> list = ((ResourceManager)target).CurveList;
                if (GUILayout.Button("向后搜索"))
                {
                    switch (searchType)
                    {
                        case SearchType.Index:
                            if (int.TryParse(searchQuery, out int value))
                            {
                                curveList.Select(value);
                            }
                            break;
                    }
                }
                if (GUILayout.Button("向前搜索"))
                {
                    switch (searchType)
                    {
                        case SearchType.Index:
                            if (int.TryParse(searchQuery, out int value))
                            {
                                curveList.Select(value);
                            }
                            break;
                    }
                }
                GUILayout.EndHorizontal(); 
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUI.BeginDisabledGroup(!debugEnabled.target);
            serializedObject.Update();
            curveList.DoLayoutList();
            EditorGUI.EndDisabledGroup();
            GUILayout.Label($"共有{((ResourceManager)target).CurveList.Count}条曲线");
            if (EditorGUILayout.BeginFadeGroup(debugEnabled.faded))
            {
                addCurveType = (AddCurveType)EditorGUILayout.EnumPopup("新建曲线类型", addCurveType);
                switch (addCurveType)
                {
                    case AddCurveType.Line:
                        addCircular.target = false;
                        addCustom.target = false;
                        break;
                    case AddCurveType.Circular:
                        addCircular.target = true;
                        addCustom.target = false;
                        break;
                    case AddCurveType.Custom:
                        addCircular.target = false;
                        addCustom.target = true;
                        break;
                }
                addP0 = EditorGUILayout.Vector2Field("起始点", addP0);
                addP3 = EditorGUILayout.Vector2Field("结束点", addP3);
                if (EditorGUILayout.BeginFadeGroup(addCircular.faded))
                {
                    addRadius = EditorGUILayout.FloatField("圆弧半径", addRadius);
                    addCircularWarn.target = Mathf.Abs(addRadius * 2) < Vector2.Distance(addP3, addP0);
                    if (EditorGUILayout.BeginFadeGroup(addCircularWarn.faded))
                    {
                        EditorGUILayout.HelpBox("圆弧直径不能小于起始点与结束点连线长度", MessageType.Warning);
                    }
                    EditorGUILayout.EndFadeGroup();
                }
                EditorGUILayout.EndFadeGroup();
                if (EditorGUILayout.BeginFadeGroup(addCustom.faded))
                {
                    addP1 = EditorGUILayout.Vector2Field("控制点1：", addP1);
                    addP2 = EditorGUILayout.Vector2Field("控制点2：", addP2);
                }
                EditorGUILayout.EndFadeGroup();
            }
            EditorGUILayout.EndFadeGroup();
        }
        EditorGUILayout.Separator();
        debugEnabled.target = EditorGUILayout.ToggleLeft("启用调试操作", debugEnabled.target, h2);
        EditorGUILayout.Separator();
        if (EditorGUILayout.BeginFadeGroup(debugEnabled.faded))
        {
            showDebugCurve = EditorGUILayout.ToggleLeft("显示选择的曲线", showDebugCurve);
            EditorGUI.BeginDisabledGroup(!showDebugCurve);
            debugT = EditorGUILayout.Slider("比值插值", debugT, 0, 1);
            debugD = EditorGUILayout.Slider("距离插值", debugD, 0, 1);
            EditorGUI.EndDisabledGroup();
        }
        EditorGUILayout.EndFadeGroup();
        serializedObject.ApplyModifiedProperties();
    }

    void OnSceneGUI()
    {
        if (!showDebugCurve | selectedCurveIndex < 0 | selectedCurveIndex >= ((ResourceManager)target).CurveList.Count)
            return;
        BezierCurve curve = ((ResourceManager)target).CurveList[selectedCurveIndex];
        Handles.color = Color.white;
        for (int i = 0; i <= 100; i++)
        {
            if (i == 100)
                break;
            float t0 = i / 100f;
            float t1 = (i + 1) / 100f;
            Handles.DrawLine(curve.GetPoint(t0), curve.GetPoint(t1), 6f);
        }
        Handles.color = Color.blue;
        Handles.DrawLine(curve.P0, curve.P1, 4f);
        Handles.DrawLine(curve.P2, curve.P3, 4f);
        Handles.color = Color.yellow;
        Handles.DrawSolidDisc(curve.P0, Vector3.back, 0.1f);
        Handles.DrawSolidDisc(curve.P1, Vector3.back, 0.1f);
        Handles.DrawSolidDisc(curve.P2, Vector3.back, 0.1f);
        Handles.DrawSolidDisc(curve.P3, Vector3.back, 0.1f);

        Handles.color = Color.green;
        Handles.DrawSolidDisc(curve.GetPoint(debugT), Vector3.back, 0.2f);
        Handles.color = Color.red;
        Handles.DrawSolidDisc(curve.GetPointAtDistanceRatio(debugD), Vector3.back, 0.2f);

        Transform transform = ((ResourceManager)target).transform;
        curve.P0 = Handles.PositionHandle(curve.P0, transform.rotation * Quaternion.LookRotation(Vector3.forward));
        curve.P1 = Handles.PositionHandle(curve.P1, transform.rotation * Quaternion.LookRotation(Vector3.forward));
        curve.P2 = Handles.PositionHandle(curve.P2, transform.rotation * Quaternion.LookRotation(Vector3.forward));
        curve.P3 = Handles.PositionHandle(curve.P3, transform.rotation * Quaternion.LookRotation(Vector3.forward));
    }

    private enum SearchType
    {
        Index
    }

    private enum AddCurveType
    {
        Line,
        Circular,
        Custom
    }
}
