using System.Collections;
using UnityEngine;
using UnityEditor;
using FarmManagerWorld.Modding.ObjectProperties;

namespace FarmManagerWorld.Editors
{ 
    [CustomEditor(typeof(EntranceProperties))]
    public class EntranceEditor : Editor
    {
        public EntranceProperties entrance;

        public Vector3 _forward;

        private Vector3 _closedPosition;
        private Vector3 _openedPosition;

        private Quaternion _closedRotation;
        private Quaternion _openedRotation;

        private void OnEnable()
        {
            entrance = target as EntranceProperties;
            Initialize();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Label("Options only available in play mode");
            if (Application.isPlaying)
            {
                if (GUILayout.Button("Open entrance"))
                {
                    Open();
                }

                if (GUILayout.Button("Close entrance"))
                {
                    Close();
                }
            }
        }

        void Initialize()
        {
            _closedRotation = entrance.transform.localRotation;
            _openedRotation = _closedRotation * Quaternion.AngleAxis(-90f, Vector3.right);

            if (entrance.Type == ModEntranceType.Door || entrance.Type == ModEntranceType.EntranceForStaffAnim)
            {
                _openedRotation = _closedRotation * Quaternion.AngleAxis(-90f, Vector3.up);
            }

            _closedPosition = entrance.transform.localPosition;
            var mr = entrance.GetComponent<MeshRenderer>();
            if (mr != null)
                _openedPosition = _closedPosition + Vector3.up * mr.bounds.extents.y * 1.5f;
        }

        private void Open()
        {
            Initialize();
            entrance.StartCoroutine(OpenCoroutine());
        }

        private IEnumerator OpeningGate()
        {
            int i = 0;
            bool wasBreak = false;
            while (Mathf.Abs(Quaternion.Angle(entrance.transform.localRotation, _openedRotation)) > 1)
            {
                yield return null;
                entrance.transform.localRotation = Quaternion.RotateTowards(entrance.transform.localRotation, _openedRotation, 3 * Time.timeScale);
                if (entrance.MoveForward) entrance.transform.localPosition += (Vector3.forward * 0.05f * Time.timeScale);

                if (i > 240)
                {
                    wasBreak = true;
                    break;
                }
                i++;
            }

            entrance.transform.localRotation = _openedRotation;

            if (wasBreak)
                entrance.transform.localPosition = _openedPosition;
        }

        private IEnumerator ClosingGate()
        {
            while (Mathf.Abs(Quaternion.Angle(entrance.transform.localRotation, _closedRotation)) > 1)
            {
                yield return null;

                entrance.transform.localRotation = Quaternion.RotateTowards(entrance.transform.localRotation, _closedRotation, 3 * Time.timeScale);

                if (entrance.MoveForward)
                    entrance.transform.localPosition -= (Vector3.forward * 0.05f * Time.timeScale);
            }

            entrance.transform.localRotation = _closedRotation;
            entrance.transform.localPosition = _closedPosition;
        }

        // access via editor
        private void Close()
        {
            entrance.StartCoroutine(CloseCoroutine());
        }

        private IEnumerator OpenCoroutine()
        {
            if (entrance.Type == ModEntranceType.Entrance)
            {
                yield break;
            }
            else if (entrance.Type == ModEntranceType.Gate)
            {
                yield return entrance.StartCoroutine(OpeningGate());
            }
            else
            {
                while (Mathf.Abs(Quaternion.Angle(entrance.transform.localRotation, _openedRotation)) > 1)
                {
                    yield return null;

                    entrance.transform.localRotation = Quaternion.RotateTowards(entrance.transform.localRotation, _openedRotation, 4 * Time.timeScale);
                }
            }
        }

        private IEnumerator CloseCoroutine()
        {
            if (entrance.Type == ModEntranceType.Entrance)
            {
                yield break;
            }
            else if (entrance.Type == ModEntranceType.Gate)
            {
                yield return entrance.StartCoroutine(ClosingGate());
            }
            else
            {
                while (Mathf.Abs(Quaternion.Angle(entrance.transform.localRotation, _closedRotation)) > 1)
                {
                    yield return null;
                    entrance.transform.localRotation = Quaternion.RotateTowards(entrance.transform.localRotation, _closedRotation, 4 * Time.timeScale);
                }
            }
        }
    }
}
