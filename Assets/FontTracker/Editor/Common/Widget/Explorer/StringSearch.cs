using System;
using UnityEditor;

namespace Voodoo.Sauce.Font.Common
{
    public class BasicStringSearch : IFilterEditor<string>
    {
        string _filter;

        public bool IsValid(string value) => value.IndexOf(_filter) >= 0;

        public void OnGUI()
        {
            _filter = EditorGUILayout.TextField(_filter);
        }
    }
}
