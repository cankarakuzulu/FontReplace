using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voodoo.Sauce.Font.Common
{
    public class MultiSelection : AbstractSelector
    {
        static readonly bool IsMac = SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX;

        protected override int[] GetSelection(int clickedIndex)
        {
            if (Selection.Length <= 0)
            {
                return new int[1] { clickedIndex };
            }

            if (Event.current.shift)
            {
                return OnShiftPressed(clickedIndex);
            }

            bool controlPressed = IsMac && Event.current.command || IsMac == false && Event.current.control;
            if (controlPressed)
            {
                return OnControlPressed(clickedIndex);
            }

            return new int[1] { clickedIndex };
        }

        int[] OnControlPressed(int clickedIndex)
        {
            List<int> result = new List<int>(Selection);
            int selectionIndex = Array.IndexOf(Selection, clickedIndex);
            if (selectionIndex < 0)
            {
                result.Add(clickedIndex);
            }
            else
            {
                result.RemoveAt(selectionIndex);
            }

            return result.ToArray();
        }

        int[] OnShiftPressed(int clickedIndex)
        {
            int start = Selection[0];
            int end = clickedIndex;

            List<int> result = new List<int>();
            int current = start;
            int step = start < end ? 1 : -1;
            while (current != end)
            {
                result.Add(current);
                current += step;
            }

            result.Add(clickedIndex);

            return result.ToArray();
        }
    }
}
