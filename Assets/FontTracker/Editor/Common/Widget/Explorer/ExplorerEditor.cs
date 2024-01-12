using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Voodoo.Sauce.Font.Common
{
    public class ExplorerEditor<T> : IExplorerEditor<T>, IEditor
    {
        Vector2 _scrollview = Vector2.zero;

        List<int> _displayedItems = new List<int>();

        ICollectionSelector     _selector;
        List<IExplorerItem<T>> _entries;

        public IFilterEditor<T> Filter { get; private set; }
        public List<T> Selection { get; private set; } = new List<T>();

        public event Action<T[]> selectionChanged;

        public ExplorerEditor(IFilterEditor<T> filter, ICollectionSelector selector, List<IExplorerItem<T>> entries)
        {
            Filter = filter;
            _entries = entries;
            _selector = selector;
            _selector.collectionChanged += UpdateSelection;

            UpdateFilter();
        }

        public void Fill(List<IExplorerItem<T>> entries)
        {
            _entries = entries;

            UpdateFilter();
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                OnFilter();
                OnExplorerList();
            }
            EditorGUILayout.EndVertical();
        }

        void OnFilter()
        {
            EditorGUI.BeginChangeCheck();
            Filter?.OnGUI();
            if (EditorGUI.EndChangeCheck())
            {
                UpdateFilter();
            }
        }

        void UpdateFilter()
        {
            _displayedItems.Clear();
            if (_entries == null )
            {
                return;
            }

            int count = _entries.Count;
            
            for (int i = 0; i < count; i++)
            {
                if (Filter.IsValid(_entries[i].Value))
                {
                    _displayedItems.Add(i);
                }
            }

            int[] selection = _displayedItems.Count > 0 ? new int[1] { 0 } : new int[0];
            UpdateSelection(selection);
        }

        void OnExplorerList()
        {
            _scrollview = EditorGUILayout.BeginScrollView(_scrollview);
            {
                int count = _displayedItems.Count;
                for (int i = 0; i < count; i++)
                {
                    var item = DisplayedItem(i);
                    item.OnGUI();
                    
                    OnItemEvent(item, i);
                    item.IsSelected = Array.IndexOf(_selector.Selection, i) >= 0;
                }
            }
            EditorGUILayout.EndScrollView();
        }

        void OnItemEvent(IExplorerItem<T> item, int index)
        {
            var currentEvent = Event.current;
            if (item.Rect.Contains(currentEvent.mousePosition) == false || currentEvent.type != EventType.MouseUp)
            {
                return;
            }

            if (currentEvent.button == 0)
            {
                _selector.UpdateSelection(index);
                EditorWindow.focusedWindow.Repaint();
                return;
            }

            if (currentEvent.button == 1)
            {
                item.ContextClick();
            }
        }

        void UpdateSelection(int[] selection) 
        {
            Selection.Clear();
            for (int i = 0; i < selection.Length; i++)
            {
                var item = DisplayedItem(selection[i]);
                Selection.Add(item.Value);
            }

            selectionChanged?.Invoke(Selection.ToArray());
        }

        IExplorerItem<T> DisplayedItem(int index) => _entries[_displayedItems[index]];
        
        int ItemIndex(IExplorerItem<T> item)
        {
            int entryIndex = _entries.IndexOf(item);
            return _displayedItems.IndexOf(entryIndex);
        }
    }
}
