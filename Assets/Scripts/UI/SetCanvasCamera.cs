using UnityEngine;

namespace BML.Scripts.UI
{
    public class SetCanvasCamera : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Camera _camera;

        private void Start()
        {
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.worldCamera = _camera;
        }
    }
}