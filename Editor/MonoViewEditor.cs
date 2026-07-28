using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoView), true)]
public sealed class MonoViewEditor : UnityEditor.Editor
{
    private static readonly GUIContent PresenterHeaderContent = new GUIContent("Presenter Binding Info");

    public override void OnInspectorGUI()
    {
        var monoView = (MonoView)target;

        DrawPresenterInfo(monoView);

        EditorGUILayout.Space(4f);
        DrawSeparator();
        EditorGUILayout.Space(4f);

        if (monoView.TryGetPresenter<DisposablePoco>(out _) && GUILayout.Button("Clear Presenter"))
        {
            monoView.ClearPresenter();
        }

        base.OnInspectorGUI();
    }

    private static void DrawPresenterInfo(MonoView monoView)
    {
        if (!monoView.TryGetPresenter<DisposablePoco>(out var presenter))
            return;

        Type presenterType = presenter.GetType();

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField(PresenterHeaderContent, EditorStyles.boldLabel);
            EditorGUILayout.Space(2f);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.TextField("Presenter Type", presenterType.FullName ?? presenterType.Name);
                EditorGUILayout.TextField("View", monoView.GetType().Name);
                EditorGUILayout.TextField("Bound", "Yes");
            }

            DrawPresenterMembers(presenterType);
        }
    }

    private static void DrawPresenterMembers(Type presenterType)
    {
        var fields = presenterType.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        var properties = presenterType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

        int visibleFieldCount = 0;
        for (int i = 0; i < fields.Length; i++)
        {
            if (!fields[i].IsStatic)
            {
                visibleFieldCount++;
            }
        }

        int visiblePropertyCount = 0;
        for (int i = 0; i < properties.Length; i++)
        {
            if (properties[i].CanRead && properties[i].GetIndexParameters().Length == 0)
            {
                visiblePropertyCount++;
            }
        }

        EditorGUILayout.Space(2f);
        EditorGUILayout.LabelField("Members", $"Fields {visibleFieldCount} / Properties {visiblePropertyCount}", EditorStyles.miniBoldLabel);
    }

    private static void DrawSeparator()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 1f);
        EditorGUI.DrawRect(rect, new Color(0.25f, 0.25f, 0.25f, 1f));
    }
}
