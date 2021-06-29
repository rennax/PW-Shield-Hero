using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using MelonLoader;
using System.IO;
using TMPro;
using UnityEngine.UI;

namespace PW_Shield_Hero
{
    public class SH_Settings : MonoBehaviour
    {
        public SH_Settings(IntPtr ptr) : base(ptr) { }

        // Optional, only used in case you want to instantiate this class in the mono-side
        // Don't use this on MonoBehaviours / Components!
        public SH_Settings() : base(ClassInjector.DerivedConstructorPointer<SH_Settings>()) => ClassInjector.DerivedConstructorBody(this);

        GameModifierEntry mod;

        Vector3 localPos = new Vector3(0.900f, -1.200f, 0.000f);
        Vector3 rot = new Vector3(0, 285, 0);

        ScriptableObjectArchitecture.BoolReference enableShield;
        bool initialized = false;

        private Shield shield;
        private TrackModSettingNodeToggle nodeToggle;

        const string prefab = "Managers/UI State Controller/PF_CHUI_AnchorPt_Settings/PF_CHUI_AnchorPt_ModSettings/PF_ModSettingsCanvas_UIv2/PW_HorizontalLayoutElementPanel/PF_MaskPanel/PW_VerticalLayoutElementPanel/PW_GridLayoutElementPanel/SettingNode: Vengeance";
        const string uiGroup = "Managers/UI State Controller/PF_CHUI_AnchorPt_Settings/PF_CHUI_AnchorPt_ModSettings/PF_ModSettingsCanvas_UIv2/PW_HorizontalLayoutElementPanel/PF_MaskPanel/PW_VerticalLayoutElementPanel/PW_GridLayoutElementPanel";
        const string trackModPath = "Managers/UI State Controller/PF_CHUI_AnchorPt_Settings/PF_CHUI_AnchorPt_ModSettings/PF_ModSettingsCanvas_UIv2";
        void Start()
        {
            GameObject prefabGO = GameObject.Find(prefab);
            GameObject toParent = GameObject.Find(uiGroup);
            GameObject trackMod = GameObject.Find(trackModPath);

            if (prefabGO == null)
            {
                MelonLogger.Msg("prefabGO is null");
            }

            if (toParent == null)
            {
                MelonLogger.Msg("toParent is null");
            }


            var go = Instantiate(prefabGO);
            go.transform.SetParent(toParent.transform, false);
            go.transform.localPosition = localPos;
            go.transform.rotation = Quaternion.Euler(rot);
            go.name = "SettingNode: Shield Hero";

            nodeToggle = go.GetComponent<TrackModSettingNodeToggle>();
            CHUI_ToggleOptionButton toggleBtn = go.GetComponent<CHUI_ToggleOptionButton>();
            
            TrackModUIController controller = trackMod.GetComponent<TrackModUIController>();

            nodeToggle.controller = controller;
            nodeToggle.chuiToggleOptionButton = toggleBtn;

            InitGameModifier();

            nodeToggle.modifierEntry = mod;

            var mods = controller.modEntries.ToList();
            mods.Add(mod);
            controller.modEntries = mods.ToArray();
            controller.modCount += 1;
            controller.modTickboxButtons.Add(nodeToggle);

            nodeToggle.Init();
            nodeToggle.Register(controller, "Shield Hero");
            nodeToggle.toggleVariable = ScriptableObject.CreateInstance<ScriptableObjectArchitecture.BoolVariable>();

            mod.setting = new ScriptableObjectArchitecture.BoolReference();
            mod.setting.Variable = nodeToggle.toggleVariable;
            enableShield = new ScriptableObjectArchitecture.BoolReference();
            enableShield.Variable = nodeToggle.toggleVariable;

            CHMenuSettingNode settingNode = go.GetComponent<CHMenuSettingNode>();
            settingNode.iSettingNode = nodeToggle.TryCast<ICHSettingNode>();



            var settingsPanelGO = GameObject.Find("Managers/UI State Controller/PF_CHUI_AnchorPt_Settings/PF_CHUI_AnchorPt_ModSettings/PF_ModSettingsCanvas_UIv2");
            var settingsPanel = settingsPanelGO.GetComponent<CHSettingsPanel>();
            //settingsPanel.selectableSettingsList.Add(settingNode);

            var onClickObject = GameObject.Find("Managers/UI State Controller/PF_CHUI_AnchorPt_Settings/PF_CHUI_AnchorPt_ModSettings/PF_ModSettingsCanvas_UIv2/PW_HorizontalLayoutElementPanel/PF_MaskPanel/PW_VerticalLayoutElementPanel/PW_GridLayoutElementPanel/SettingNode: Shield Hero/PW_HorizontalLayoutElementPanel/PW_HorizontalLayoutElementPanel/PF_CHUI_Trigger_UnityEvents/PF_UnityEventTrigger_Click");
            var clickTrigger = onClickObject.GetComponent<UnityEventTrigger>();
            //clickTrigger.Event.RemoveAllListeners();
            //clickTrigger.Event.AddListener(new Action(nodeToggle.onModToggle));
            clickTrigger.Event.AddListener(new Action(this.OnClick));

            GameObject.Find("Managers/UI State Controller/PF_CHUI_AnchorPt_Settings/PF_CHUI_AnchorPt_ModSettings/PF_ModSettingsCanvas_UIv2/PW_HorizontalLayoutElementPanel/PF_MaskPanel/PW_VerticalLayoutElementPanel/PW_GridLayoutElementPanel/SettingNode: Shield Hero/PW_HorizontalLayoutElementPanel/PF_FieldText")
                .GetComponent<TextMeshProUGUI>().text = mod.Name;

            GameObject.Find("Managers/UI State Controller/PF_CHUI_AnchorPt_Settings/PF_CHUI_AnchorPt_ModSettings/PF_ModSettingsCanvas_UIv2/PW_HorizontalLayoutElementPanel/PF_MaskPanel/PW_VerticalLayoutElementPanel/PW_GridLayoutElementPanel/SettingNode: Shield Hero/PW_HorizontalLayoutElementPanel/PW_LayoutElementPanel/PF_Sprite")
                .GetComponent<Image>().sprite = mod.menuIcon;

            //Set mod in gameplay database
            var db = GameplayManager.gameplayDB;

            mods = db.modifiers.ToList();
            mods.Add(mod);
            db.modifiers = mods.ToArray();

            mods = db.Modifiers.ToList();
            mods.Add(mod);
            db.Modifiers = mods.ToArray();

            //Handle mod conflics
            Il2CppSystem.Collections.Generic.List<GameModifier> conflicts = new Il2CppSystem.Collections.Generic.List<GameModifier>();
            conflicts.Add(GameModifier.DualWield);
            db.modConflictLookup.Add(mod.legacy_id, conflicts);

            //if (db.modConflictLookup.TryGetValue(GameModifier.DualWield, out conflicts))
            //{
            //    conflicts.Add(mod.legacy_id);
            //}
            //else
            //    MelonLogger.Msg("Could not update mod conflicts for GameModifier DualWield");

            RescaleModPanel();

            //Register callback for toggle shield
            //GameObject modClickObj = GameObject.Find("Managers/UI State Controller/PF_CHUI_AnchorPt_Settings/PF_CHUI_AnchorPt_ModSettings/PF_ModSettingsCanvas_UIv2/PW_HorizontalLayoutElementPanel/PF_MaskPanel/PW_VerticalLayoutElementPanel/PW_GridLayoutElementPanel/SettingNode: Deadeye/PW_HorizontalLayoutElementPanel/PW_HorizontalLayoutElementPanel/PF_CHUI_Trigger_UnityEvents");
            //CHUI_TriggerEvents triggerEvents = modClickObj.GetComponent<CHUI_TriggerEvents>();
            //triggerEvents.chuiButtonEvent_Click
            //    .TryCast<GameObject>()
            //    .GetComponent<UnityEventTrigger>()
            //    .Event.AddListener(new System.Action(OnClick));


        }

        void OnClick()
        {
            //MelonLogger.Msg("SH_Settings: OnClick called");
            if (!initialized)
            {
                MelonLogger.Msg("Initializing Shield");
                //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);

                EnemyDatabase db = EnemyDatabase.Current;
                EnemySet enemySet = db.enemySets[1];


                Enemy shieldEnemy = enemySet.prefabs[5]; //Shield
                var shieldPrefab = shieldEnemy.transform.FindChild("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/Shield/riot_shield_2089").gameObject;

                GameObject go = Instantiate(shieldPrefab);

                go.name = "shield";
                go.layer = 8; //Layer 8 = player
                Transform leftHand = GameObject.Find("UnityXR_VRCameraRig(Clone)/TrackingSpace/Left Hand").transform;
                if (leftHand == null)
                {
                    MelonLogger.Msg("Did not find UnityXR_VRCameraRig(Clone)/TrackingSpace/Left Hand");
                    return;
                }

                go.transform.localScale = new Vector3(0.8f, 0.4f, 1f);

                //Fix box collider
                BoxCollider collider = go.AddComponent<BoxCollider>();
                Renderer rend = go.GetComponent<MeshRenderer>();
                collider.bounds.Encapsulate(rend.bounds);

                //Material
                rend.material = new Material(Shader.Find("Cloudhead/Universal/HighEnd/PW_Props"));

                //Custom shield component
                shield = go.AddComponent<Shield>();
                shield.parent = leftHand;

                Rigidbody rb = go.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;
                go.SetActive(false);
                initialized = true;
            }

            //Enable or disable shield based on current value
            shield.gameObject.SetActive(enableShield.Value);

            nodeToggle.chuiToggleOptionButton.SetIconSelectedState(enableShield.Value);
        }

        private void RescaleModPanel()
        {
            //Offset panel so it doesn't overlap with settings title
            Transform panelTransform = GameObject.Find("Managers/UI State Controller/PF_CHUI_AnchorPt_Settings/PF_CHUI_AnchorPt_ModSettings").transform;
            Vector3 localPos = panelTransform.localPosition;
            localPos.y = -0.15f;
            panelTransform.localPosition = localPos;

            //Resize mask
            RectTransform maskRect = GameObject.Find("Managers/UI State Controller/PF_CHUI_AnchorPt_Settings/PF_CHUI_AnchorPt_ModSettings/PF_ModSettingsCanvas_UIv2/PW_HorizontalLayoutElementPanel/PF_MaskPanel").GetComponent<RectTransform>();
            maskRect.sizeDelta = new Vector2(3.826667f, 3.3f);

            //Resize background image
            RectTransform imageRect = GameObject.Find("Managers/UI State Controller/PF_CHUI_AnchorPt_Settings/PF_CHUI_AnchorPt_ModSettings/PF_ModSettingsCanvas_UIv2/PW_HorizontalLayoutElementPanel/PF_MaskPanel/PW_VerticalLayoutElementPanel/PW_GridLayoutElementPanel").GetComponent<RectTransform>();
            imageRect.sizeDelta = new Vector2(0, 0.5f);

            //Offset tooltip
            RectTransform tooltipRect = GameObject.Find("Managers/UI State Controller/PF_CHUI_AnchorPt_Settings/PF_CHUI_AnchorPt_ModSettings/PF_ModSettingsCanvas_UIv2/PF_HoverTooltip/PF_ToolTip/Image").GetComponent<RectTransform>();
            tooltipRect.pivot = new Vector2(0.5f, 0);
            tooltipRect.anchoredPosition = new Vector2(0, -100);
        }

        private void InitGameModifier()
        {
            mod = new GameModifierEntry();

            mod.name = "Shield Hero";
            mod.description = "Up your defence and fight back!";
            mod.gameplayElements = new Il2CppSystem.Collections.Generic.List<GameplayElementHandler>();
            mod.id = 1337;
            mod.Index = 1337;
            mod.leaderboardID = (LeaderboardModifier)1337;
            mod.legacy_id = (GameModifier)1337;
            mod.multiplier = 1;
            mod.showInModifierPane = true;
            mod.showInLeaderboard = false;
            mod.scoreModStr = "+0%";
            mod.showInModifierPane = true;
            

            string path = Directory.GetCurrentDirectory();
            Il2CppAssetBundle bundle = Il2CppAssetBundleManager.LoadFromFile("Mods/Shield Hero/pw_shield_hero");
            mod.menuIcon = bundle.Load<Sprite>("icon");
        }
    }
}
