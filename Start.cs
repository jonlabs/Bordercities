﻿using UnityEngine;
using ColossalFramework;
using ICities;

namespace Bordercities
{
    public class Mod : IUserMod
    {
        public string Name
        {
            get { return "Bordered Skylines"; }
        }

        public string Description
        {
            get { return "Edge detection FX suite for both gameplay and screenshot art!"; }
        }
    }

    public class ModLoad : LoadingExtensionBase
    {
        private Camera camera;
        private EffectController toggler;
        private EdgeDetection edge;
        private BloomOptimized bloom;

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame)
            {
                return;
            }
            var cameraController = GameObject.FindObjectOfType<CameraController>();
            camera = cameraController.gameObject.GetComponent<Camera>();
            toggler = camera.gameObject.AddComponent<EffectController>();
            AttachEffects();
        }

        void Deinitialize()
        {
            if (toggler != null)
            {
                toggler.SaveBank();
                GameObject.Destroy(toggler);
            }
            if (edge != null)
                GameObject.Destroy(edge);
            if (bloom != null)
                GameObject.Destroy(bloom);
        }

        public override void OnLevelUnloading()
        {
            Deinitialize();
        }

        //public override void OnReleased()
        //{
       //     Deinitialize();
        //}

        

        private void AttachEffects()
        {

            if (camera != null)
            {
                edge = camera.gameObject.AddComponent<EdgeDetection>();
                edge.enabled = false;
                bloom = camera.gameObject.AddComponent<BloomOptimized>();
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
