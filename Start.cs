using UnityEngine;
using ColossalFramework;
using ICities;

namespace Bordercities
{
    public class Mod : IUserMod
    {
        public string Name
        {
            get { return "Bordercities 2"; }
        }

        public string Description
        {
            get { return "Edge detect & bloom.  Backslash to config."; }
        }
    }

    public class ModLoad : LoadingExtensionBase
    {
        private Camera camera;

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame)
            {
                return;
            }
            var cameraController = GameObject.FindObjectOfType<CameraController>();
            camera = cameraController.gameObject.GetComponent<Camera>();
            EffectController toggler = camera.gameObject.AddComponent<EffectController>();
            AttachEffects();
        }
        private void AttachEffects()
        {

            if (camera != null)
            {
                EdgeDetection edge = camera.gameObject.AddComponent<EdgeDetection>();
                edge.enabled = false;
                BloomOptimized bloom = camera.gameObject.AddComponent<BloomOptimized>();
                bloom.enabled = false;
            }
            else
            {
                LogFX.Warning("Null camera reference!");
            }
        }
    }
    public static class LogFX
    {
        public static void Message(string s)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, s);
        }

        public static void Error(string s)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Error, s);
        }

        public static void Warning(string s)
        {
            DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Warning, s);
        }
    }
}
