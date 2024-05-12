using UnityEngine;

namespace Off_Road.Core
{
    public class Bootstrap : MonoBehaviour
    {
        void Awake()
        {
            LogSettings();
        }
        
        void LogSettings()
        {

#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
            Debug.logger.logEnabled = false;
#endif
        }
    }
}