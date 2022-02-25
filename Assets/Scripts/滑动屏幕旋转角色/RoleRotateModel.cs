using UnityEngine;
using UnityEngine.EventSystems;

namespace Yoozoo.Modules.CommanderScene
{
    public class RoleRotateModel : MonoBehaviour
    {
        private Vector3 dragPosStart;

        private bool isDragging;

        public Vector3 Sensibility = new Vector3(0, 0.8f, 0);

        private Vector3 startEuler;

        private int layerUI;

        /// <summary>
        /// UI画出的允许拖动范围
        /// </summary>
        private RectTransform allowArea;

        private Camera uiCamera;

        private void Start()
        {
            layerUI = LayerMask.NameToLayer("UI");
        }

        public void SetAllowArea(RectTransform allowArea, Camera uiCamera)
        {
            this.allowArea = allowArea;
            this.uiCamera = uiCamera;
        }

        private void Update()
        {
            if (isDragging)
            {
                if (Input.GetMouseButton(0))
                {
                    InputControllerOnDragUpdate(Input.mousePosition);
                }
                else
                {
                    isDragging = false;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //当前选中的对象是YSlider 则不允许拖动模型
                    /*var pointerPress = EventSystem.current.currentSelectedGameObject;
                    if (pointerPress && pointerPress.layer == layerUI)
                        return;

                    if (allowArea)
                    {
                        if (!RectTransformUtility.RectangleContainsScreenPoint(allowArea, Input.mousePosition, uiCamera))
                            return;
                    }*/
                    

                    isDragging = true;
                    dragPosStart = Input.mousePosition;
                    InputControllerOnDragStart();
                }
            }
        }

        private void InputControllerOnDragStart()
        {
            startEuler = transform.localEulerAngles;
        }

        private void InputControllerOnDragUpdate(Vector3 dragPosCurrent)
        {
            transform.localEulerAngles = startEuler - Sensibility * (dragPosCurrent.x - dragPosStart.x);
        }

        public void ResetRotation()
        {
            isDragging = false;
            transform.localEulerAngles = Vector3.zero;
        }
    }
}
