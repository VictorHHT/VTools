using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Victor.Tools
{
    public class VTSetActiveOnPlay : MonoBehaviour
    {
        public enum SetStages { Awake, Start }

        public List<TargetObject> TargetObjects;

        public SetStages SetStage;

        [System.Serializable]
        public class TargetObject
        {
            public GameObject otherObj;
            public bool ActiveState;
            public bool SetChildren = false;
        }

        void Awake()
        {
            if (SetStage == SetStages.Awake)
            {
                SetObject();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (SetStage == SetStages.Start)
            {
                SetObject();
            }
        }

        private void SetObject()
        {
            foreach (var otherObject in TargetObjects)
            {
                if (otherObject == null)
                {
                    continue;
                }

                if (otherObject.SetChildren == false)
                {
                    otherObject.otherObj.SetActive(otherObject.ActiveState);
                }
                else
                {
                    SetChildrenRecursively(otherObject.otherObj, otherObject.ActiveState);
                }
            }
        }

        private void SetChildrenRecursively(GameObject obj, bool activeState)
        {
            obj.SetActive(activeState);
            foreach (Transform child in obj.transform)
            {
                SetChildrenRecursively(child.gameObject, activeState);
            }
        }
    }

}
