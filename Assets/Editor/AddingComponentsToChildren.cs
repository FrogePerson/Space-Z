#if UNITY_EDITOR

using System;
using Network.NetworkJoint;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AddingComponentsToChildren : EditorWindow
{
    [MenuItem("Tools/Добавить компоненты на дочерние объекты")]
    static void Init()
    {
        AddingComponentsToChildren window = (AddingComponentsToChildren)EditorWindow.GetWindow(typeof(AddingComponentsToChildren));

        window.Show();
    }

    MonoScript tragetComponent;

    void OnGUI()
    {
        GUILayout.Label("Добавление компонента на дочерние объекты", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        if (Selection.activeGameObject == null)
        {
            EditorGUILayout.HelpBox("Выделите родительский объект", MessageType.Info);
        }
        else
        {
            if (GUILayout.Button("Добавить NetworkDetail"))
            {
                AddComponentsToChildren<NetworkDetail>();
            }

            if (GUILayout.Button("Удалить NetworkDetail"))
            {
                RemoveComponentsFromChildren<NetworkDetail>();
            }

            EditorGUILayout.Space();
            GUILayout.Label("Добавление другого компонента", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            tragetComponent = (MonoScript)EditorGUILayout.ObjectField(
                "Компонент",
                tragetComponent,
                typeof(MonoScript),
                true
                );
            EditorGUILayout.Space();

            if (tragetComponent != null)
            {
                Type scriptType = tragetComponent.GetClass();

                if (scriptType != null && typeof(Component).IsAssignableFrom(scriptType))
                {
                    if (GUILayout.Button("Добавить компонент"))
                    {
                        MethodInfo method = typeof(AddingComponentsToChildren)
                            .GetMethod("AddComponentsToChildren",
                                       BindingFlags.Public | BindingFlags.Static,
                                       null,
                                       Type.EmptyTypes,
                                       null);
                        MethodInfo genericMethod = method.MakeGenericMethod(scriptType);
                        genericMethod.Invoke(null, null);
                    }
                    else if (GUILayout.Button("Удалить компонент"))
                    {
                        MethodInfo method = typeof(AddingComponentsToChildren)
                            .GetMethod("RemoveComponentsFromChildren",
                                       BindingFlags.Public | BindingFlags.Static,
                                       null,
                                       Type.EmptyTypes,
                                       null);
                        MethodInfo genericMethod = method.MakeGenericMethod(scriptType);
                        genericMethod.Invoke(null, null);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Это не валидный компонент!", MessageType.Error);
                }
            }
        }
    }


    public static void AddComponentsToChildren<T>() where T : Component
    {
        GameObject selectedObj = Selection.activeGameObject;

        List<Transform> childrens = new List<Transform>();

        getChildrenRecursive(selectedObj.transform, childrens);

        foreach(Transform child in childrens)
        {
            if(!child.gameObject.GetComponent<T>())
                child.gameObject.AddComponent<T>();
        }
    }

    static void getChildrenRecursive(Transform parent, List<Transform> childrens)
    {
        foreach(Transform child in parent)
        {
            childrens.Add(child);

            getChildrenRecursive(child, childrens);
        }
    }

    public static void RemoveComponentsFromChildren<T>() where T : Component
    {
        GameObject selectedObj = Selection.activeGameObject;

        List<Transform> childrens = new List<Transform>();

        getChildrenRecursive(selectedObj.transform, childrens);

        foreach (Transform child in childrens)
        {
            T comp = child.GetComponent<T>();

            if (comp != null)
                Undo.DestroyObjectImmediate(comp);

        }
    }
}

#endif