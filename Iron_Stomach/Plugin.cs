using BepInEx;
using System;
using UnityEngine;
using Utilla;
using GorillaLocomotion;
using UnityEngine.XR;

namespace Iron_Stomach
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;
        public bool primaryR;
        public bool primaryL;
        Rigidbody rigidbody;
        public GameObject GorillaPlayer;
        public Quaternion InitialQuaternion;
        private XRNode leftHandNode = XRNode.LeftHand;
        private XRNode rightHandNode = XRNode.RightHand;
        public float coolDown;

        void Start()
        {
            /* A lot of Gorilla Tag systems will not be set up when start is called /*
			/* Put code in OnGameInitialized to avoid null references */

            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnEnable()
        {

            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            GorillaPlayer.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            rigidbody.freezeRotation = true;

            HarmonyPatches.RemoveHarmonyPatches();
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            GorillaPlayer = GameObject.Find("GorillaPlayer");
            InitialQuaternion = GorillaPlayer.transform.rotation;
            rigidbody = GorillaPlayer.GetComponent<Rigidbody>();
            rigidbody.freezeRotation = true;
        }

        void Update()
        {
            if (inRoom)
            {
                InputDevices.GetDeviceAtXRNode(rightHandNode).TryGetFeatureValue(CommonUsages.primaryButton, out primaryR);
                InputDevices.GetDeviceAtXRNode(leftHandNode).TryGetFeatureValue(CommonUsages.primaryButton, out primaryL);
                if (primaryL || primaryR && coolDown == 0)
                {
                    if (rigidbody.freezeRotation == true)
                    {
                        rigidbody.freezeRotation = false;
                        coolDown = 1f;
                    }
                    else if (rigidbody.freezeRotation == false)
                    {
                        GorillaPlayer.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        rigidbody.freezeRotation = true;
                        coolDown = 1f;
                    }
                }
                if (coolDown > 0) 
                {
                    Debug.Log(coolDown);
                    coolDown -= Time.deltaTime;
                }
                if (coolDown < 0)
                {
                    coolDown = 0;
                }
            }
        }

        void reset()
        {

        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = true;
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            GorillaPlayer.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            rigidbody.freezeRotation = true;

            inRoom = false;
        }
    }
}
