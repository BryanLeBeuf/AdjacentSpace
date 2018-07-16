using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/** Custom Classes **/
[CustomEditor(typeof(VignetteConfiguration))]
[CanEditMultipleObjects]
public class VignetteConfigurationEditor : StructListEditor {
}
[CustomEditor(typeof(GameManager))]
[CanEditMultipleObjects]
public class GameManagerEditor : StructListEditor {
}


//* Code *//
// https://forum.unity.com/threads/display-a-list-class-with-a-custom-editor-script.227847/
public class StructListEditor : Editor {
	private const int m_ButtonSizeWidth = 19;
	private const int m_ButtonSizeHeight = 15;

    public override void OnInspectorGUI()
    {
        //EditorGUIUtility.fieldWidth = EditorGUIUtility.currentViewWidth/3;
        IEnumerable<SerializedProperty> iterator = serializedObject.GetChildren();
        using (var sequenceEnum = iterator.GetEnumerator())
        {
            while (sequenceEnum.MoveNext())
            {
                SerializedProperty prop = sequenceEnum.Current;
                
                if(prop.isArray){
                    DisplayStructList(prop);
                }else if((prop.type.StartsWith("PPtr") && prop.objectReferenceValue != null)
                    || prop.propertyType == SerializedPropertyType.Boolean
                    || prop.propertyType == SerializedPropertyType.Integer
                    || prop.propertyType == SerializedPropertyType.Float
                    || prop.propertyType == SerializedPropertyType.String
                    || prop.propertyType == SerializedPropertyType.Color
                    || prop.propertyType == SerializedPropertyType.Enum
                    || prop.propertyType == SerializedPropertyType.Vector2
                    || prop.propertyType == SerializedPropertyType.Vector3) {
                    EditorGUILayout.PropertyField(prop);
                }else {
                    DisplayElement(prop, null, 0);
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayStructList(SerializedProperty structList){

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(structList);
		EditorGUILayout.EndHorizontal();

        if(!structList.isExpanded){
            return;
        }

        for(int i = 0; i < structList.arraySize; i++){
            SerializedProperty MyListRef = structList.GetArrayElementAtIndex(i);

            DisplayElement(MyListRef, structList, i);
		}

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("New:");
        if(GUI.Button(ButtonRect(0),"+",GUI.skin.button)){
            structList.InsertArrayElementAtIndex(structList.arraySize);
            structList.GetArrayElementAtIndex(structList.arraySize -1);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DisplayElement(SerializedProperty MyListRef, SerializedProperty structList, int i){
        bool hasArray = false;
        IEnumerable<SerializedProperty> iterator = MyListRef.GetChildren();
        if(MyListRef.hasVisibleChildren){
            using (var sequenceEnum = iterator.GetEnumerator())
            {
                while (sequenceEnum.MoveNext())
                {
                    SerializedProperty prop = sequenceEnum.Current;
                    if(prop.isArray && prop.hasVisibleChildren){
                       hasArray = true;
                       break;
                    }
                }
            }
        }

        if(hasArray){
            IEnumerable<SerializedProperty> iterator2 = MyListRef.GetChildren();
            using (var sequenceEnum = iterator2.GetEnumerator())
            {
                while (sequenceEnum.MoveNext())
                {
                    SerializedProperty prop = sequenceEnum.Current;
                    if(!prop.isArray || !prop.hasVisibleChildren){
                        EditorGUILayout.BeginHorizontal();
                        DisplayDynamicLabel(prop.name + ":", 0);
                        EditorGUILayout.PropertyField( prop, GUIContent.none);
                        if(structList!=null && GUI.Button(ButtonRect(),"-",GUI.skin.button)){
                            structList.DeleteArrayElementAtIndex(i);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            int indentLevel = 1;
            EditorGUI.indentLevel+=indentLevel;
            iterator2 = MyListRef.GetChildren();
            using (var sequenceEnum = iterator2.GetEnumerator())
            {
                while (sequenceEnum.MoveNext())
                {
                    SerializedProperty prop = sequenceEnum.Current;
                    if(prop.isArray && prop.hasVisibleChildren){
                        DisplayStructList(prop);
                    }
                }
            }
            EditorGUI.indentLevel-=indentLevel;
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }else{
            DisplayArray(MyListRef, structList, i);
        }  
    }

    public void DisplayArray(SerializedProperty listRef, SerializedProperty structList, int index){
            IEnumerable<SerializedProperty> iterator = listRef.GetChildren();
            if(listRef.hasVisibleChildren){   
                using (var sequenceEnum = iterator.GetEnumerator())
                {
                    while (sequenceEnum.MoveNext())
                    {
                        EditorGUILayout.BeginHorizontal();
                        SerializedProperty prop = sequenceEnum.Current;
                        DisplayDynamicLabel(prop.name + ":", 0);
                        EditorGUILayout.PropertyField( prop, GUIContent.none);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                
                EditorGUILayout.BeginHorizontal();
                DisplayDynamicLabel("Remove:", 0);
                if(structList != null && GUI.Button(ButtonRect(),"-",GUI.skin.button)){
                    structList.DeleteArrayElementAtIndex(index);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }else{
                EditorGUILayout.BeginHorizontal();      
                EditorGUILayout.PropertyField( listRef, GUIContent.none);
                
                if(GUI.Button(ButtonRect(),"-",GUI.skin.button)){
                    structList.DeleteArrayElementAtIndex(index);
                }
                EditorGUILayout.EndHorizontal();
            }
    }

    private void DisplayDynamicLabel(string text, float paddingRight = 0){
        float indentLevelCompensation = EditorGUI.indentLevel * 16;
        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(text));
        EditorGUILayout.LabelField(text, GUILayout.Width(textDimensions.x + paddingRight + indentLevelCompensation));
    }

    private Rect ButtonRect(int offsetX = 0){
        GUIStyle myStyle = new GUIStyle (GUI.skin.button);
        myStyle.fixedWidth =m_ButtonSizeWidth;
        myStyle.margin=new RectOffset(0,offsetX,0,20);
		var TheRect = GUILayoutUtility.GetRect(new GUIContent("Button"), myStyle);
		//TheRect.width = m_ButtonSizeWidth;
		return TheRect;
    }
}

public static class EditorExtensionMethods
{
    public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty property)
    {
        property = property.Copy();
        var nextElement = property.Copy();
        bool hasNextElement = nextElement.NextVisible(false);
        if (!hasNextElement)
        {
            nextElement = null;
        }
 
        property.NextVisible(true);
        while (true)
        {
            if ((SerializedProperty.EqualContents(property, nextElement)))
            {
                yield break;
            }
 
            yield return property;
 
            bool hasNext = property.NextVisible(false);
            if (!hasNext)
            {
                break;
            }
        }
    }
    public static IEnumerable<SerializedProperty> GetChildren(this SerializedObject serializedObject)
    {
        var property = serializedObject.GetIterator();
        var nextElement = serializedObject.GetIterator();
        bool hasNextElement = nextElement.NextVisible(false);
        if (!hasNextElement)
        {
            nextElement = null;
        }
 
        property.NextVisible(true);
        while (true)
        {
            if ((SerializedProperty.EqualContents(property, nextElement)))
            {
                yield break;
            }
 
            yield return property;
 
            bool hasNext = property.NextVisible(false);
            if (!hasNext)
            {
                break;
            }
        }
    }
}