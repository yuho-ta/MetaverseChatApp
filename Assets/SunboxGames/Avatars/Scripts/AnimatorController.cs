using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunbox.Avatars {
    public class AnimatorController : MonoBehaviour
    {
        public GameObject CurrentBase;  
        public GameObject MaleBase;     
        public GameObject FemaleBase;   

        void Start()
        {
        }

        void Update()
        {
        }

        public Animator Animator
        {
            get
            {
                if (CurrentBase != null)
                {
                    return CurrentBase.GetComponent<Animator>();
                }
                return null;
            }
        }

        public RuntimeAnimatorController RuntimeAnimatorController
        {
            get
            {
                if (Animator != null)
                {
                    return Animator.runtimeAnimatorController;
                }
                return null;
            }
            set
            {
                if (MaleBase != null)
                {
                    MaleBase.GetComponent<Animator>().runtimeAnimatorController = value;
                }
                if (FemaleBase != null)
                {
                    FemaleBase.GetComponent<Animator>().runtimeAnimatorController = value;
                }
                if (CurrentBase != null)
                {
                    CurrentBase.GetComponent<Animator>().runtimeAnimatorController = value;
                }
            }
        }
    }
}