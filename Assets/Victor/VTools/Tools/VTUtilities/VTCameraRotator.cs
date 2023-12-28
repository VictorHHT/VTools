using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Victor.Tools;

namespace Victor.Tools
{
    public class VTCameraRotator : MonoBehaviour
    {
        [VTInspectorButton("ResetView", VTColorLibrary.VTColors.VictorRed)]
        [VTReadOnlyInEditorMode]
        public bool resetViewBool;

        [VTInspectorButton("ResetDistance", VTColorLibrary.VTColors.VictorRed)]
        [VTReadOnlyInEditorMode]
        public bool resetDistanceBool;

        [VTInspectorButton("ResetPan", VTColorLibrary.VTColors.VictorRed)]
        [VTReadOnlyInEditorMode]
        public bool resetPanBool;

        [VTInspectorButton("FrontView", VTColorLibrary.VTColors.VictorBlue)]
        [VTReadOnlyInEditorMode]
        public bool frontViewBool;

        [VTInspectorButton("TopView", VTColorLibrary.VTColors.VictorGreen)]
        [VTReadOnlyInEditorMode]
        public bool topViewBool;

        [VTInspectorButton("LeftView", VTColorLibrary.VTColors.VictorYellow)]
        [VTReadOnlyInEditorMode]
        public bool leftViewBool;

        public enum PanMethods { Mouse, Keyboard };

        // The target of the camera rotate sphere
        public Camera sceneCamera;
        // Target of the camera
        public Transform target;

        public PanMethods panMethod;

        [VTReadOnly]
        public bool canInteract = true;

        [Header("Rotate")]
        [Range(5f, 15f)]
        [Tooltip("How sensitive the mouse drag to camera rotation")]
        public float mouseRotateSpeed = 5f;

        [Header("Distance")]
        [Range(1, 15)]
        public float defaultCameraDistance = 12f;

        [VTMinMaxSlider(1f, 30f)]
        public Vector2 MinMaxDistance = new Vector2(2f, 20f);

        [Range(1f, 5f)]
        public float distanceChangeSpeed = 1f;

        [Header("Pan")]
        [VTEnumCondition("panMethod", (int)PanMethods.Mouse)]
        [Range(0.1f, 0.5f)]
        public float mousePanSpeed = 0.25f;

        [VTEnumCondition("panMethod", (int)PanMethods.Keyboard)]
        [Range(10f, 50f)]
        public float keyboardPanSpeed = 20f;

        [Header("Smooth")]
        [Range(0.1f, 0.5f)]
        public float rotateSmoothValue = 0.3f;

        [Range(0.1f, 0.3f)]
        public float distanceChangeSmoothValue = 0.12f;

        [Range(0.1f, 0.3f)]
        public float panSmoothValue = 0.12f;

        [Header("Pan Key Bindings")]
        [VTEnumCondition("panMethod", (int)PanMethods.Keyboard)]
        public KeyCode leftPanKeyCode = KeyCode.A;
        [VTEnumCondition("panMethod", (int)PanMethods.Keyboard)]
        public KeyCode rightPanKeyCode = KeyCode.D;
        [VTEnumCondition("panMethod", (int)PanMethods.Keyboard)]
        public KeyCode upPanKeyCode = KeyCode.W;
        [VTEnumCondition("panMethod", (int)PanMethods.Keyboard)]
        public KeyCode downPanKeyCode = KeyCode.S;

        // Clamp Value
        private readonly float m_MinXRotAngle = -85; //min angle around x axis
        private readonly float m_MaxXRotAngle = 85; // max angle around x axis

        // Mouse rotation
        private float m_RotAroundX;
        private float m_RotAroundY;

        // View Distance
        private float m_TargetCameraDistance;
        private float m_CurrentCameraDistance;
        private float m_DummyDampFloatVelocity = 0;

        private Vector3 m_MousePosBeforePanning;
        private Vector3 m_RightBeforePannining;
        private Vector3 m_UpBeforePanning;
        private Vector3 m_CurrentPanDelta;
        private Vector3 m_TargetPanDelta;
        private Vector3 m_DummyDampVector3Velocity;

        private Quaternion m_CurrentRot;
        private Quaternion m_TargetRot;

        void Awake()
        {
            if (sceneCamera == null)
            {
                sceneCamera = Camera.main;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            m_TargetCameraDistance = defaultCameraDistance;
            m_CurrentCameraDistance = defaultCameraDistance;

            // Initialize camera position
            sceneCamera.transform.position = target.position + new Vector3(0, 0, defaultCameraDistance);

            // Initial view angle
            m_RotAroundX = 45;
            m_RotAroundY = -45;
        }

        // Update is called once per frame
        void Update()
        {
            if (!canInteract)
            {
                return;
            }

            EditorCameraInput();
            HandlePanning();
        }

        private void LateUpdate()
        {
            RotateCamera();
            SetCameraDistance();
        }

        public void ResetView()
        {
            m_RotAroundX = 40;
            m_RotAroundY = 40;
            ResetPan();
            ResetDistance();
        }

        public void ResetPan()
        {
            m_TargetPanDelta = Vector3.zero;
        }

        public void ResetDistance()
        {
            m_TargetCameraDistance = defaultCameraDistance;
        }

        public void FrontView()
        {
            m_RotAroundX = 0;
            m_RotAroundY = 0;
            ResetPan();
        }

        //May be the problem with Euler angles
        public void TopView()
        {
            m_RotAroundX = 85;
            m_RotAroundY = 0;
            ResetPan();
        }

        public void LeftView()
        {
            m_RotAroundY = 85;
            m_RotAroundX = 0;
            ResetPan();
        }

        private void EditorCameraInput()
        {
            // Camera Rotation
            if (Input.GetMouseButton(0))
            {
                m_RotAroundX += Input.GetAxis("Mouse Y") * mouseRotateSpeed * -1; // around X
                m_RotAroundY += Input.GetAxis("Mouse X") * mouseRotateSpeed;

                // Clamp rotX
                if (m_RotAroundX < m_MinXRotAngle)
                {
                    m_RotAroundX = m_MinXRotAngle;
                }
                else if (m_RotAroundX > m_MaxXRotAngle)
                {
                    m_RotAroundX = m_MaxXRotAngle;
                }
            }

            if (panMethod == PanMethods.Mouse)
            {
                if (Input.GetMouseButtonDown(2))
                {
                    m_RightBeforePannining = transform.right;
                    m_UpBeforePanning = transform.up;
                    m_MousePosBeforePanning = Input.mousePosition;
                }

                if (Input.GetMouseButton(2))
                {
                    Vector2 mouseDelta = Input.mousePosition - m_MousePosBeforePanning;
                    m_TargetPanDelta += m_RightBeforePannining * mouseDelta.x * mousePanSpeed * Time.deltaTime;
                    m_TargetPanDelta += m_UpBeforePanning * mouseDelta.y * mousePanSpeed * Time.deltaTime;
                }

                if (Input.GetMouseButtonUp(2))
                {
                    m_MousePosBeforePanning = Vector2.zero;
                }
            }
            else
            {
                if (Input.GetKeyDown(leftPanKeyCode) || Input.GetKeyDown(rightPanKeyCode))
                {
                    m_RightBeforePannining = transform.right;
                }

                if (Input.GetKeyDown(upPanKeyCode) || Input.GetKeyDown(downPanKeyCode))
                {
                    m_UpBeforePanning = transform.up;
                }

                if (Input.GetKey(leftPanKeyCode) ^ Input.GetKey(rightPanKeyCode))
                {
                    if (Input.GetKey(leftPanKeyCode))
                    {
                        m_TargetPanDelta -= m_RightBeforePannining * keyboardPanSpeed * Time.deltaTime;
                    }

                    if (Input.GetKey(rightPanKeyCode))
                    {
                        m_TargetPanDelta += m_RightBeforePannining * keyboardPanSpeed * Time.deltaTime;
                    }
                }

                if (Input.GetKey(downPanKeyCode) ^ Input.GetKey(upPanKeyCode))
                {
                    if (Input.GetKey(downPanKeyCode))
                    {
                        m_TargetPanDelta -= m_UpBeforePanning * keyboardPanSpeed * Time.deltaTime;
                    }

                    if (Input.GetKey(upPanKeyCode))
                    {
                        m_TargetPanDelta += m_UpBeforePanning * keyboardPanSpeed * Time.deltaTime;
                    }
                }
            }

            if (Input.mouseScrollDelta.magnitude > 0)
            {
                m_TargetCameraDistance += Input.mouseScrollDelta.y * distanceChangeSpeed * -1;
            }
        }

        private void RotateCamera()
        {
            Vector3 targetRot = new Vector3(m_RotAroundX, m_RotAroundY, 0);
            m_TargetRot = Quaternion.Euler(targetRot);

            //Rotate Camera
            m_CurrentRot = Quaternion.Slerp(m_CurrentRot, m_TargetRot, Time.smoothDeltaTime * rotateSmoothValue * 50);

            //Multiplying a quaternion by a Vector3 is essentially to apply the rotation to the Vector3
            sceneCamera.transform.rotation = m_CurrentRot;
            sceneCamera.transform.position = target.position + sceneCamera.transform.forward * (-m_CurrentCameraDistance);
            sceneCamera.transform.position += m_CurrentPanDelta;
        }

        void SetCameraDistance()
        {
            if (m_TargetCameraDistance >= MinMaxDistance.y)
            {
                m_TargetCameraDistance = MinMaxDistance.y;
            }
            else if (m_TargetCameraDistance <= MinMaxDistance.x)
            {
                m_TargetCameraDistance = MinMaxDistance.x;
            }

            m_CurrentCameraDistance = Mathf.SmoothDamp(m_CurrentCameraDistance, m_TargetCameraDistance, ref m_DummyDampFloatVelocity, distanceChangeSmoothValue);
        }

        void HandlePanning()
        {
            m_CurrentPanDelta = Vector3.SmoothDamp(m_CurrentPanDelta, m_TargetPanDelta, ref m_DummyDampVector3Velocity, panSmoothValue);
        }
    }
}