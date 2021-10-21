using RedLightGreenLight.Configuration;
using System;
using UnityEngine;
using Zenject;

namespace RedLightGreenLight.GameScene
{
    internal class Judge : MonoBehaviour, IInitializable, IDisposable
    {
        public float remainingTime;
        public event Action TimerStartedEvent;
        public event Action TimerStoppedEvent;

        private AudioPlayer audioPlayer;
        private GameEnergyCounter gameEnergyCounter;

        private readonly float positionRange = PluginConfig.Instance.PositionRange;
        private readonly float rotationRange = PluginConfig.Instance.RotationRange;

        private Transform hmd, leftController, rightController;
        private Vector3 hmdOriginalPos, leftControllerOriginalPos, rightControllerOriginalPos;
        private Vector3 hmdOriginalRot, leftControllerOriginalRot, rightControllerOriginalRot;

        [Inject]
        public void Construct(AudioPlayer audioPlayer, SaberManager saberManager, GameEnergyCounter gameEnergyCounter)
        {
            this.audioPlayer = audioPlayer;
            hmd = saberManager.GetComponentInChildren<MainCamera>().transform;
            leftController = saberManager.leftSaber.GetComponentInParent<VRController>().transform;
            rightController = saberManager.rightSaber.GetComponentInParent<VRController>().transform;
            this.gameEnergyCounter = gameEnergyCounter;
        }

        public void Initialize()
        {
            audioPlayer.ClipFinishedEvent += EnableTimer;
        }

        public void Dispose()
        {
            audioPlayer.ClipFinishedEvent -= EnableTimer;
        }

        public void Start() => enabled = false;

        public void Update()
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0)
            {
                StopTimer();
                return;
            }

            if (!(PositionAndRotationWithinRange(hmd, hmdOriginalPos, hmdOriginalRot) &&
                    PositionAndRotationWithinRange(leftController, leftControllerOriginalPos, leftControllerOriginalRot) &&
                    PositionAndRotationWithinRange(rightController, rightControllerOriginalPos, rightControllerOriginalRot)))
            {
                audioPlayer.PlayGun();
                enabled = false;
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

        public void StartTimer(float time)
        {
            Plugin.Log.Debug("Timer on");
            remainingTime = time;
            audioPlayer.PlayRedLight();
        }

        private void EnableTimer()
        {
            GetPositionFromTransform(hmd, out hmdOriginalPos);
            GetRotationFromTransform(hmd, out hmdOriginalRot);

            GetPositionFromTransform(leftController, out leftControllerOriginalPos);
            GetRotationFromTransform(leftController, out leftControllerOriginalRot);

            GetPositionFromTransform(rightController, out rightControllerOriginalPos);
            GetRotationFromTransform(rightController, out rightControllerOriginalRot);

            TimerStartedEvent?.Invoke();
            enabled = true;
        }

        private void GetRotationFromTransform(Transform obj, out Vector3 rotation) => rotation = obj.eulerAngles;

        private void GetPositionFromTransform(Transform obj, out Vector3 position) => position = obj.position;

        public void StopTimer()
        {
            Plugin.Log.Debug("Stopping timer");
            enabled = false;
            audioPlayer.PlayGreenLight();
            TimerStoppedEvent?.Invoke();
        }
    }
}
