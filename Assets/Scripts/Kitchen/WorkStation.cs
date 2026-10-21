using Sirenix.OdinInspector;
using UnityEngine;

namespace CursedCookbook.Kitchen
{
    [ExecuteAlways]
    public class WorkStation : MonoBehaviour
    {
        [Required, InlineEditor, AssetSelector(Paths = "Assets/Data/StationActions")]
        [SerializeField] private StationActionDefinition action;
        [Tooltip("Where the player stands to work. Its forward should face the station.")]
        [SerializeField] private Transform workPoint;
        [Tooltip("Where a held ingredient is placed on the station.")]
        [SerializeField] private Transform placePoint;
        [Tooltip("Floor marker tinted with the action's colour.")]
        [SerializeField] private Renderer marker;
        [SerializeField] private float useRadius = 0.9f;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
        private MaterialPropertyBlock _block;

        public StationActionDefinition Action => action;
        public Transform WorkPoint => workPoint != null ? workPoint : transform;
        public Transform PlacePoint => placePoint != null ? placePoint : transform;

        public bool InReach(Vector3 position)
        {
            Vector3 d = position - WorkPoint.position;
            d.y = 0f;
            return d.sqrMagnitude <= useRadius * useRadius;
        }

        private void OnEnable()
        {
            StationActionDefinition.Changed += OnDefinitionChanged;
            ApplyMarker();
        }

        private void OnDisable() => StationActionDefinition.Changed -= OnDefinitionChanged;

        private void OnValidate() => ApplyMarker();

        private void OnDefinitionChanged(StationActionDefinition def)
        {
            if (def == action) ApplyMarker();
        }

        private void ApplyMarker()
        {
            if (marker == null || action == null) return;
            _block ??= new MaterialPropertyBlock();
            marker.GetPropertyBlock(_block);
            _block.SetColor(BaseColorId, action.MarkerColor);
            _block.SetColor(EmissionColorId, action.MarkerEmission);
            marker.SetPropertyBlock(_block);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = action != null ? action.MarkerColor : Color.magenta;
            Vector3 p = WorkPoint.position;
            Gizmos.DrawWireSphere(p, useRadius);
            Gizmos.DrawLine(p + Vector3.up * 0.05f, PlacePoint.position);
        }
    }
}
