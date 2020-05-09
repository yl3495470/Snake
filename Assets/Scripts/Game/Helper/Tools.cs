using UnityEngine;
using System.Collections;

public class Tools
{    public static void DynamicAddAnimationEvent(Animator pAnimator, int pIndex, string pFunction)
    {
        AnimationClip clip = pAnimator.runtimeAnimatorController.animationClips[pIndex];
        AnimationEvent overEvent = new AnimationEvent();
        overEvent.time = clip.length;
        overEvent.functionName = pFunction;
        clip.AddEvent(overEvent);
    }

    /// <summary>
    /// 这个方法暂时还有问题，先不要引用
    /// </summary>
    public static void DynamicAddAnimationEvent(Animator pAnimator, string pClipName, string pFunction)
    {
        for (int i = 0; i < pAnimator.runtimeAnimatorController.animationClips.Length; i++)
        {
            if (pAnimator.runtimeAnimatorController.animationClips[i].name == pClipName)
            {
                AnimationClip clip = pAnimator.runtimeAnimatorController.animationClips[i];
                AnimationEvent overEvent = new AnimationEvent();
                overEvent.time = clip.length;
                overEvent.functionName = pFunction;
                clip.AddEvent(overEvent);
            }
        }
    }


    public static AnimationClip GetAnimationClip(Animator pAnimator, string pClipName)
    {
        AnimationClip rClip = null;
        for (int i = 0; i < pAnimator.runtimeAnimatorController.animationClips.Length; i++)
        {
            if (pAnimator.runtimeAnimatorController.animationClips[i].name == pClipName)
                rClip = pAnimator.runtimeAnimatorController.animationClips[i];
        }
        return rClip;
    }

    public static float GetAnimationLength(Animator pAnimator, int pIndex)
    {
        AnimationClip clip = pAnimator.runtimeAnimatorController.animationClips[pIndex];
        float pLength = clip.length;
        return pLength;
    }


    public static float GetAnimationLength(Animator pAnimator, string pClipName)
    {
        for (int i = 0; i < pAnimator.runtimeAnimatorController.animationClips.Length; i++)
        {
            if (pAnimator.runtimeAnimatorController.animationClips[i].name == pClipName)
                return pAnimator.runtimeAnimatorController.animationClips[i].length;
        }
        return 0;
    }

	public static float GetAngle360(Vector3 pFrom, Vector3 pTo)
	{
		Vector3 vec3 = Vector3.Cross (pFrom, pTo);
		if (vec3.z > 0)
			return Vector3.Angle (pFrom, pTo);
		return 360.0f - Vector3.Angle (pFrom, pTo);
	}

	public static float GetAnglePN(Vector3 pFrom, Vector3 pTo)
	{
		Vector3 vec3 = Vector3.Cross (pFrom, pTo);
		if (vec3.z > 0)
			return Vector3.Angle (pFrom, pTo);
		return -Vector3.Angle (pFrom, pTo);
	}
}
