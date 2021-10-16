using System;
using UnityEngine;
using Zenject;

namespace ReaxtIsASussyBaka.GameObjects
{
    internal class Judge : MonoBehaviour
    {
        public float RemainingTime { get; private set; }
        public event Action TimerStoppedEvent;

        private GameEnergyCounter gameEnergyCounter;
        private Transform hmd;
        private Transform leftController;
        private Transform rightController;

        private const float positionRange = 1f;
        private const float rotationRange = 15f;

        private Vector3 hmdOriginalPos;
        private Vector3 leftControllerOriginalPos;
        private Vector3 leftControllerOriginalRot;

        private Vector3 hmdOriginalRot;
        private Vector3 rightControllerOriginalPos;
        private Vector3 rightControllerOriginalRot;

        [Inject]
        public void Construct(SaberManager saberManager, GameEnergyCounter gameEnergyCounter)
        {
            hmd = saberManager.GetComponentInChildren<MainCamera>().transform;
            leftController = saberManager.leftSaber.GetComponentInParent<VRController>().transform;
            rightController = saberManager.rightSaber.GetComponentInParent<VRController>().transform;
            this.gameEnergyCounter = gameEnergyCounter;
        }

        public void Start() => enabled = false;

        public void Update()
        {
            RemainingTime -= Time.deltaTime;
            if (RemainingTime <= 0)
            {
                StopTimer();
                TimerStoppedEvent?.Invoke();
            }

            if (!(PositionAndRotationWithinRange(hmd, hmdOriginalPos, hmdOriginalRot) &&
                    PositionAndRotationWithinRange(leftController, leftControllerOriginalPos, leftControllerOriginalRot) &&
                    PositionAndRotationWithinRange(rightController, rightControllerOriginalPos, rightControllerOriginalRot)))
            {
                gameEnergyCounter.ProcessEnergyChange(-gameEnergyCounter.energy);
            }
        }

        private bool PositionAndRotationWithinRange(Transform controller, Vector3 originalPos, Vector3 originalRot)
        {
            bool xPositionWithinRange = originalPos.x + positionRange > controller.position.x && originalPos.x - positionRange < controller.position.x;
            bool yPositionWithinRange = originalPos.y + positionRange > controller.position.y && originalPos.y - positionRange < controller.position.y;
            bool zPositionWithinRange = originalPos.z + positionRange > controller.position.z && originalPos.z - positionRange < controller.position.z;

            bool xRotationWithinRange = originalRot.x + rotationRange > controller.eulerAngles.x && originalRot.x - rotationRange < controller.eulerAngles.x;
            bool yRotationWithinRange = originalRot.y + rotationRange > controller.eulerAngles.y && originalRot.y - rotationRange < controller.eulerAngles.y;
            bool zRotationWithinRange = originalRot.z + rotationRange > controller.eulerAngles.z && originalRot.z - rotationRange < controller.eulerAngles.z;

            bool finalJudgement = xPositionWithinRange && yPositionWithinRange && zPositionWithinRange && xRotationWithinRange && yRotationWithinRange && zRotationWithinRange;

            if (!finalJudgement)
            {
                Plugin.Log.Debug($"{xPositionWithinRange}, {yPositionWithinRange}, {zPositionWithinRange}     {xRotationWithinRange}, {yRotationWithinRange}, {zRotationWithinRange}");
            }

            return finalJudgement;
        }

        internal void StartTimer(float time)
        {
            RemainingTime = time;

            hmdOriginalPos = hmd.position;
            hmdOriginalRot = hmd.eulerAngles;

            leftControllerOriginalPos = leftController.position;
            leftControllerOriginalRot = leftController.eulerAngles;

            rightControllerOriginalPos = rightController.position;
            rightControllerOriginalRot = rightController.eulerAngles;

            enabled = true;
        }

        internal void StopTimer() => enabled = false;
    }
}
