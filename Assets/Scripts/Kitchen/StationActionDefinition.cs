using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CursedCookbook.Kitchen
{
    [CreateAssetMenu(menuName = "Cookbook/Station Action")]
    public class StationActionDefinition : ScriptableObject
    {
        [HorizontalGroup("Top", 70), PreviewField(70, ObjectFieldAlignment.Left), HideLabel]
        [SerializeField] private Sprite icon;

        [VerticalGroup("Top/Info"), LabelWidth(90)]
        [SerializeField] private string displayName;

        [VerticalGroup("Top/Info"), LabelWidth(90)]
        [SerializeField] private Color markerColor = Color.white;

        [VerticalGroup("Top/Info"), LabelWidth(90), Range(0f, 3f)]
        [SerializeField] private float markerGlow = 0.5f;

        [BoxGroup("Used for"), HideLabel, MultiLineProperty(3)]
        [SerializeField] private string usedFor;

        public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;
        public Sprite Icon => icon;
        public Color MarkerColor => markerColor;
        public Color MarkerEmission => markerColor * markerGlow;
        public string UsedFor => usedFor;

        /// <summary>Raised when any definition is edited, so scene markers can refresh in edit mode.</summary>
        public static event Action<StationActionDefinition> Changed;

        private void OnValidate() => Changed?.Invoke(this);

#if UNITY_EDITOR
        /// <summary>Editor-only setters so overview tables (e.g. the library) can edit this asset with undo.</summary>
        public void EditorSet(Color color, float glow, string uses)
        {
            UnityEditor.Undo.RecordObject(this, "Edit Station Action");
            markerColor = color;
            markerGlow = glow;
            usedFor = uses;
            UnityEditor.EditorUtility.SetDirty(this);
            Changed?.Invoke(this);
        }

        public float MarkerGlow => markerGlow;
#endif
    }
}
