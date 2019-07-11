using BepInEx;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HasAnyoneHitTheNewt
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.HasAnyoneHitTheNewt", "HasAnyoneHitTheNewt", "1.0.1")]
    public class HasAnyoneHitTheNewt : BaseUnityPlugin
    {
        private GameObject NewtActiveIcon { get; set; }
        private HUD MyHUD { get; set; }

        public void Awake()
        {
            On.RoR2.PortalStatueBehavior.GrantPortalEntry += PortalStatueBehavior_GrantPortalEntry;

            On.RoR2.UI.HUD.Start += HUD_Start;
        }

        private void HUD_Start(On.RoR2.UI.HUD.orig_Start orig, HUD self)
        {
            this.MyHUD = self;
            orig(self);
        }

        private void PortalStatueBehavior_GrantPortalEntry(On.RoR2.PortalStatueBehavior.orig_GrantPortalEntry orig, PortalStatueBehavior self)
        {
            InitialiseUI();
            NewtActiveIcon.transform.SetParent(MyHUD.buffDisplay.transform, false);
            orig(self);
        }

        private void InitialiseUI()
        {
            Debug.Log("InitialisingUI");
            NewtActiveIcon = new GameObject
            {
                name = "newtActiveUI"
            };
            NewtActiveIcon.AddComponent<RectTransform>();
            NewtActiveIcon.GetComponent<RectTransform>().position = new Vector3(0f, 0f);
            NewtActiveIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 2);
            NewtActiveIcon.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            NewtActiveIcon.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            NewtActiveIcon.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            NewtActiveIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
            NewtActiveIcon.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
            NewtActiveIcon.AddComponent<Image>().color = new Color(0.2f, 0.2f, 1, 1);
            NewtActiveIcon.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1f);            
            NewtActiveIcon.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1f);
            Debug.Log("UI initialised");
        }
    }
}
