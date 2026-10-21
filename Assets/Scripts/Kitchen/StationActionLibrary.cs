using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CursedCookbook.Kitchen
{
    [CreateAssetMenu(menuName = "Cookbook/Station Action Library")]
    public class StationActionLibrary : ScriptableObject
    {
        [Serializable]
        public class Row
        {
            [TableColumnWidth(120, Resizable = false), Required, HideLabel]
            [AssetSelector(Paths = "Assets/Data/StationActions")]
            public StationActionDefinition action;

            [ShowInInspector, TableColumnWidth(80, Resizable = false)]
            public Color Marker
            {
                get => action != null ? action.MarkerColor : Color.clear;
#if UNITY_EDITOR
                set { if (action != null) action.EditorSet(value, action.MarkerGlow, action.UsedFor); }
#endif
            }

#if UNITY_EDITOR
            [ShowInInspector, TableColumnWidth(70, Resizable = false), PropertyRange(0f, 3f)]
            public float Glow
            {
                get => action != null ? action.MarkerGlow : 0f;
                set { if (action != null) action.EditorSet(action.MarkerColor, value, action.UsedFor); }
            }
#endif

            [ShowInInspector, MultiLineProperty(2)]
            public string UsedFor
            {
                get => action != null ? action.UsedFor : string.Empty;
#if UNITY_EDITOR
                set { if (action != null) action.EditorSet(action.MarkerColor, action.MarkerGlow, value); }
#endif
            }
        }

        [TableList(AlwaysExpanded = true, ShowIndexLabels = false)]
        [ValidateInput(nameof(HasNoDuplicates), "An action is listed more than once.")]
        [SerializeField] private List<Row> actions = new();

        public IEnumerable<StationActionDefinition> Actions => actions.Where(r => r.action != null).Select(r => r.action);

        private bool HasNoDuplicates(List<Row> rows) =>
            rows.Where(r => r.action != null).GroupBy(r => r.action).All(g => g.Count() == 1);

#if UNITY_EDITOR
        [Button(ButtonSizes.Medium), PropertyOrder(-1)]
        [Tooltip("Add every StationActionDefinition asset in the project that isn't listed yet.")]
        private void FindAllActions()
        {
            UnityEditor.Undo.RecordObject(this, "Find Station Actions");
            foreach (var guid in UnityEditor.AssetDatabase.FindAssets("t:" + nameof(StationActionDefinition)))
            {
                var def = UnityEditor.AssetDatabase.LoadAssetAtPath<StationActionDefinition>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid));
                if (def != null && actions.All(r => r.action != def)) actions.Add(new Row { action = def });
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
