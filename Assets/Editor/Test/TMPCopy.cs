// // Copyright ©  2024 no-pact
// // Author: Can Karakuzulu

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

namespace @Editor.Test

{
    public static class TMPCopy
    {
        public static List<string> ForbiddenProperties = new List<string>()
        {
            "m_ObjectHideFlags",
            "m_CorrespondingSourceObject",
            "m_PrefabInstance",
            "m_PrefabAsset",
            "m_GameObject",
            "m_Enabled",
            "m_EditorHideFlags",
            "m_Script",
            "m_Name",
            "m_EditorClassIdentifier",
            "m_Material",
            "m_Color",
            "m_RaycastTarget",
            "m_RaycastPadding",
            "m_Maskable",
            "m_OnCullStateChanged"
        };

        [MenuItem("no-pact/Component Copy/Load Properties")]
        public static void WriteComponentProperties()
        {
            var selection = Selection.activeGameObject;
            var comp = selection.GetComponent<TextMeshProUGUI>();

            var ser = new SerializedObject(comp);
            var path = EditorUtility.OpenFilePanel("Load", "", "txt");
            var json=File.ReadAllText(path);

            var record = JsonUtility.FromJson<DictWrapper>(json);
            foreach (var entry in record.record)
            {
                var sp=ser.FindProperty(entry.key.path);
                if(sp is null || sp.isArray || entry.key.path is null) continue;
                Debug.Log(entry.key.path);
                sp.boxedValue = entry.value;
            }

            ser.ApplyModifiedProperties();
            EditorUtility.SetDirty(comp);

        }

        [MenuItem("no-pact/Component Copy/Save Properties")]
        public static void ListComponentProperties()
        {
            StringBuilder sb = new StringBuilder();
            var selection = Selection.activeGameObject;
            var comp = selection.GetComponent<TextMeshProUGUI>();

            var ser = new SerializedObject(comp);

            SerializedProperty it = ser.GetIterator();
            var data = new List<DictEntry>();
            var parents = new List<string>();
            it.Next(true);
            do
            {
                if (!it.editable) continue;
                var parent = GetParentKey(it.propertyPath);
                if (ForbiddenProperties.Contains(parent))
                {
                    continue; 
                }

                data.Add(new()
                    {
                        key =
                            new SerializedKey()
                            {
                                path = it.propertyPath,
                                type = it.propertyType
                            },
                        value =
                            GetPropertyValue(it)
                    }
                );
            } while (it.Next(true));

            var wrapper = new DictWrapper()
            {
                record = data.ToArray()
            };

            var json = JsonUtility.ToJson(wrapper);
            Debug.Log(json);
            var path = EditorUtility.SaveFilePanel("Save", "", "propertyData", "txt");
            File.WriteAllText(path, json);
        }

        private static string GetParentKey(string itPropertyPath)
        {
            return string.IsNullOrEmpty(itPropertyPath) ? string.Empty : itPropertyPath.Split('.')[0];
        }

        [Serializable]
        public class DictEntry
        {
            public SerializedKey key;
            public object value;
        }

        [Serializable]
        public class DictWrapper
        {
            public DictEntry[] record;
        }


        [Serializable]
        public struct SerializedKey : IEquatable<SerializedKey>
        {
            public string path;
            public SerializedPropertyType type;

            public bool Equals(SerializedKey other)
            {
                return path == other.path && type == other.type;
            }

            public override bool Equals(object obj)
            {
                return obj is SerializedKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(path, (int)type);
            }

            public static bool operator ==(SerializedKey left, SerializedKey right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(SerializedKey left, SerializedKey right)
            {
                return !left.Equals(right);
            }
        }


        public static object GetPropertyValue(SerializedProperty sp)
        {
            switch (sp.propertyType)
            {
                case SerializedPropertyType.Generic:
                    return null;
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Enum:
                    switch (sp.numericType)
                    {
                        case SerializedPropertyNumericType.Int8:
                            return (object)(sbyte)sp.intValue;
                        case SerializedPropertyNumericType.UInt8:
                            return (object)(byte)sp.uintValue;
                        case SerializedPropertyNumericType.Int16:
                            return (object)(short)sp.intValue;
                        case SerializedPropertyNumericType.UInt16:
                            return (object)(ushort)sp.uintValue;
                        case SerializedPropertyNumericType.UInt32:
                            return (object)sp.uintValue;
                        case SerializedPropertyNumericType.Int64:
                            return (object)sp.longValue;
                        case SerializedPropertyNumericType.UInt64:
                            return (object)sp.ulongValue;
                        default:
                            return (object)sp.intValue;
                    }
                case SerializedPropertyType.Boolean:
                    return (object)sp.boolValue;
                case SerializedPropertyType.Float:
                    return sp.numericType == SerializedPropertyNumericType.Double
                        ? (object)sp.doubleValue
                        : (object)sp.floatValue;
                case SerializedPropertyType.String:
                    return (object)sp.stringValue;
                case SerializedPropertyType.Color:
                    return (object)sp.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return (object)sp.objectReferenceValue;
                case SerializedPropertyType.LayerMask:
                    return (object)(LayerMask)sp.intValue;
                case SerializedPropertyType.Vector2:
                    return (object)sp.vector2Value;
                case SerializedPropertyType.Vector3:
                    return (object)sp.vector3Value;
                case SerializedPropertyType.Vector4:
                    return (object)sp.vector4Value;
                case SerializedPropertyType.Rect:
                    return (object)sp.rectValue;
                case SerializedPropertyType.ArraySize:
                    return (object)sp.intValue;
                case SerializedPropertyType.Character:
                    return (object)(ushort)sp.uintValue;
                case SerializedPropertyType.AnimationCurve:
                    return (object)sp.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return (object)sp.boundsValue;
                case SerializedPropertyType.Gradient:
                    return (object)sp.gradientValue;
                case SerializedPropertyType.Quaternion:
                    return (object)sp.quaternionValue;
                case SerializedPropertyType.ExposedReference:
                    return (object)sp.exposedReferenceValue;
                case SerializedPropertyType.FixedBufferSize:
                    return (object)sp.intValue;
                case SerializedPropertyType.Vector2Int:
                    return (object)sp.vector2IntValue;
                case SerializedPropertyType.Vector3Int:
                    return (object)sp.vector3IntValue;
                case SerializedPropertyType.RectInt:
                    return (object)sp.rectIntValue;
                case SerializedPropertyType.BoundsInt:
                    return (object)sp.boundsIntValue;
                case SerializedPropertyType.ManagedReference:
                    return sp.managedReferenceValue;
                case SerializedPropertyType.Hash128:
                    return (object)sp.hash128Value;
                default:
                    throw new NotSupportedException(string.Format(
                        "The boxedValue property is not supported on \"{0}\" because it has an unsupported propertyType {1}.",
                        (object)sp.propertyPath, (object)sp.propertyType));
            }
        }

        public static void SetPropertyValue(SerializedProperty sp, object value)
        {
            switch (sp.propertyType)
            {
                case SerializedPropertyType.Generic:
                    break;
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.ArraySize:
                    if (sp.numericType == SerializedPropertyNumericType.UInt64)
                    {
                        sp.ulongValue = Convert.ToUInt64(value);
                        break;
                    }

                    sp.longValue = Convert.ToInt64(value);
                    break;
                case SerializedPropertyType.Boolean:
                    sp.boolValue = (bool)value;
                    break;
                case SerializedPropertyType.Float:
                    if (sp.numericType == SerializedPropertyNumericType.Double)
                    {
                        sp.doubleValue = Convert.ToDouble(value);
                        break;
                    }

                    sp.floatValue = Convert.ToSingle(value);
                    break;
                case SerializedPropertyType.String:
                    sp.stringValue = (string)value;
                    break;
                case SerializedPropertyType.Color:
                    sp.colorValue = (Color)value;
                    break;
                case SerializedPropertyType.ObjectReference:
                    sp.objectReferenceValue = (UnityEngine.Object)value;
                    break;
                case SerializedPropertyType.LayerMask:
                    try
                    {
                        sp.intValue = ((LayerMask)value).value;
                        break;
                    }
                    catch (InvalidCastException ex)
                    {
                        sp.intValue = Convert.ToInt32(value);
                        break;
                    }
                case SerializedPropertyType.Vector2:
                    sp.vector2Value = (Vector2)value;
                    break;
                case SerializedPropertyType.Vector3:
                    sp.vector3Value = (Vector3)value;
                    break;
                case SerializedPropertyType.Vector4:
                    sp.vector4Value = (Vector4)value;
                    break;
                case SerializedPropertyType.Rect:
                    sp.rectValue = (Rect)value;
                    break;
                case SerializedPropertyType.Character:
                    sp.uintValue = (uint)Convert.ToUInt16(value);
                    break;
                case SerializedPropertyType.AnimationCurve:
                    sp.animationCurveValue = (AnimationCurve)value;
                    break;
                case SerializedPropertyType.Bounds:
                    sp.boundsValue = (Bounds)value;
                    break;
                case SerializedPropertyType.Gradient:
                    sp.gradientValue = (Gradient)value;
                    break;
                case SerializedPropertyType.Quaternion:
                    sp.quaternionValue = (Quaternion)value;
                    break;
                case SerializedPropertyType.ExposedReference:
                    sp.exposedReferenceValue = (UnityEngine.Object)value;
                    break;
                case SerializedPropertyType.Vector2Int:
                    sp.vector2IntValue = (Vector2Int)value;
                    break;
                case SerializedPropertyType.Vector3Int:
                    sp.vector3IntValue = (Vector3Int)value;
                    break;
                case SerializedPropertyType.RectInt:
                    sp.rectIntValue = (RectInt)value;
                    break;
                case SerializedPropertyType.BoundsInt:
                    sp.boundsIntValue = (BoundsInt)value;
                    break;
                case SerializedPropertyType.ManagedReference:
                    sp.managedReferenceValue = value;
                    break;
                case SerializedPropertyType.Hash128:
                    sp.hash128Value = (Hash128)value;
                    break;
                default:
                    throw new NotSupportedException(string.Format(
                        "Set on boxedValue property is not supported on \"{0}\" because it has an unsupported propertyType {1}.",
                        (object)sp.propertyPath, (object)sp.propertyType));
            }
        }
    }
}