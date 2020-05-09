using UnityEngine;
using UnityEngine.VR;

public class VRDeviceManager : Singleton<VRDeviceManager>
{
    [SerializeField]
    private float m_RenderScale = 1.4f;

    public VREyeRaycaster mVREyeRaycaster;
    public VRInput mVRInput;
    public VRRecenter mVRRecenter;
    public void Init()
    {
        if (mVRInput == null)
            mVRInput = gameObject.AddComponent<VRInput>();
        if (mVREyeRaycaster == null)
            mVREyeRaycaster = gameObject.AddComponent<VREyeRaycaster>();
        if (mVRRecenter == null)
            mVRRecenter = gameObject.AddComponent<VRRecenter>();

        mVREyeRaycaster.InitInput(mVRInput);
        SetupVR();
    }


    private void SetupVR()
    {
        //Gear VR does not currently support renderScale
#if !UNITY_ANDROID
        UnityEngine.XR.XRSettings.eyeTextureResolutionScale = m_RenderScale;
#endif

#if UNITY_STANDALONE
//        UnityEngine.XR.XRSettings.loadedDevice = VRDeviceType.Oculus;
#endif

#if UNITY_PS4 && !UNITY_EDITOR
		VRSettings.loadedDevice = VRDeviceType.Morpheus;
#endif

        UnityEngine.XR.XRSettings.enabled = true;
    }
}
