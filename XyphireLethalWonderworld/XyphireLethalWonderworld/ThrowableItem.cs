﻿using GameNetcodeStuff;
using UnityEngine;

namespace XyphireLethalWonderworld
{
    public class ThrowableItem : NoisemakerProp
    {
        public AnimationCurve itemFallCurve;
        public AnimationCurve itemVerticalFallCurve;
        public AnimationCurve itemVerticalFallCurveNoBounce;
        public RaycastHit itemHit;
        public Ray itemThrowRay;
        public float throwForce = 2;

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
        }

        public void DiscardHeld()
        {
            if (IsOwner)
            {
                GameNetworkManager.Instance.localPlayerController.DiscardHeldObject(true, placePosition: GetItemThrowDestination(playerHeldBy, throwForce));
            }
        }

        public override void FallWithCurve()
        {
            ItemFall(this, itemFallCurve, itemVerticalFallCurve, itemVerticalFallCurveNoBounce);
        }

        public static void ItemFall(GrabbableObject item, AnimationCurve? itemFallCurve = null, AnimationCurve? itemVerticalFallCurve = null, AnimationCurve? itemVerticalFallCurveNoBounce = null)
        {
            AnimationCurve curve1 = itemFallCurve ?? new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 0.75f), new Keyframe(1, 1));
            AnimationCurve curve2 = itemVerticalFallCurve ?? new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(0.6f, 0.9f), new Keyframe(0.75f, 1), new Keyframe(0.85f, 0.95f), new Keyframe(1, 1));
            AnimationCurve curve3 = itemVerticalFallCurveNoBounce ?? new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 1));
            float magnitude = (item.startFallingPosition - item.targetFloorPosition).magnitude;
            item.transform.rotation = Quaternion.Lerp(item.transform.rotation, Quaternion.Euler(item.itemProperties.restingRotation.x, item.transform.eulerAngles.y, item.itemProperties.restingRotation.z), 14f * Time.deltaTime / magnitude);
            item.transform.localPosition = Vector3.Lerp(item.startFallingPosition, item.targetFloorPosition, curve1.Evaluate(item.fallTime));
            if (magnitude > 5f)
            {
                item.transform.localPosition = Vector3.Lerp(new Vector3(item.transform.localPosition.x, item.startFallingPosition.y, item.transform.localPosition.z), new Vector3(item.transform.localPosition.x, item.targetFloorPosition.y, item.transform.localPosition.z), curve3.Evaluate(item.fallTime));
            }
            else
            {
                item.transform.localPosition = Vector3.Lerp(new Vector3(item.transform.localPosition.x, item.startFallingPosition.y, item.transform.localPosition.z), new Vector3(item.transform.localPosition.x, item.targetFloorPosition.y, item.transform.localPosition.z), curve2.Evaluate(item.fallTime));
            }
            item.fallTime += Mathf.Abs(Time.deltaTime * 12f / magnitude);
        }

        public static Vector3 GetItemThrowDestination(PlayerControllerB player, float throwForce = 1)
        {
            Ray itemThrowRay = new Ray(player.gameplayCamera.transform.position, player.gameplayCamera.transform.forward);
            Vector3 position = ((!Physics.Raycast(itemThrowRay, out RaycastHit itemHit, 12f * throwForce, StartOfRound.Instance.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore)) ? itemThrowRay.GetPoint(10f * throwForce) : itemThrowRay.GetPoint(itemHit.distance - 0.05f));
            itemThrowRay = new Ray(position, Vector3.down);
            if (Physics.Raycast(itemThrowRay, out itemHit, 30f, StartOfRound.Instance.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore))
            {
                return itemHit.point + Vector3.up * 0.05f;
            }
            return itemThrowRay.GetPoint(30f);
        }
    }
}
