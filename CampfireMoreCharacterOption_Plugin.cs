using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GameDataEditor;
using I2.Loc;
using DarkTonic.MasterAudio;
using ChronoArkMod;
using ChronoArkMod.Plugin;
using ChronoArkMod.Template;
using Debug = UnityEngine.Debug;
using ChronoArkMod.ModData;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using System.Reflection.Emit;
using ChronoArkMod.ModEditor;
using ChronoArkMod.ModData.Settings;
using DG.Tweening;

namespace CampfireMoreCharacterOption
{
    [PluginConfig("CampfireMoreCharacterOption", "0w0yui", "1.0.0")]
    public class CampfireMoreCharacterOption_Plugin : ChronoArkPlugin
    {
        public const string GUID = "0w0yui.campfirecharacterplus";
        public const string version = "1.0.0";


        private static int character_count = 64;
        private static int patch_count = 0;
        private Harmony harmony;

        public override void Dispose()
        {
            patch_count = 0;
            this.harmony.UnpatchAll();
        }

        private void update_character_count()
        {
            ModInfo modInfo = ModManager.getModInfo("CampfireMoreCharacterOption");
            character_count = modInfo.GetSetting<InputFieldSetting_Int>("character_count").Value;
            character_count = Mathf.Clamp(character_count, 3, 127);
        }

        public override void OnModSettingUpdate()
        {
            base.OnModSettingUpdate();
            update_character_count();
            log($"count: {character_count}");
        }

        public override void Initialize()
        {
            patch_count = 0;
            this.harmony = new Harmony(base.GetGuid()); 
            update_character_count();
            harmony.PatchAll();
            log("campfire character option initialized");
        }

        private static void log(string s)
        {
            Debug.Log($"CMCO: {s}");
        }

        [HarmonyPatch(typeof(CharSelect_CampUI), nameof(CharSelect_CampUI.Init))]
        public static class Patch_Init
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                bool found = false;
                patch_count++;

                log($"start patching {patch_count}/3");
                for (int i = 0; i < codes.Count; i++)
                {
                    // Æ¥Åä num = 3 (ldc.i4.3 ¡ú stloc.1)
                    if (codes[i].opcode == OpCodes.Ldc_I4_3 &&
                        i + 1 < codes.Count &&
                        codes[i + 1].opcode == OpCodes.Stloc_1)
                    {
                        codes[i] = new CodeInstruction(OpCodes.Ldc_I4, character_count);
                        found = true;
                        log($"num patched, {3}->{character_count}");
                    }
                    // Æ¥Åä num = 4 (ldc.i4.4 ¡ú stloc.1)
                    else if (codes[i].opcode == OpCodes.Ldc_I4_4 &&
                             i + 1 < codes.Count &&
                             codes[i + 1].opcode == OpCodes.Stloc_1)
                    {
                        codes[i] = new CodeInstruction(OpCodes.Ldc_I4, character_count);
                        found = true;
                        log($"num patched, {4}->{character_count}");
                    }
                }

                if (!found)
                {
                    log("cant find num in IL code");
                    patch_count--;
                }
                return codes;
            }

            [HarmonyPrefix]
            public static void Prefix(CharSelect_CampUI __instance)
            {
                patch_count++;
                log($"start patching {patch_count}/3");
                Transform transform = __instance.FaceList[0].transform;
                Transform parent = transform.parent;
                int num = character_count;
                List<string> dataKeys = new List<string>();
                GDEDataManager.GetAllDataKeysBySchema(GDESchemaKeys.Character, out dataKeys);
                if (num > dataKeys.Count)
                {
                    num = dataKeys.Count;
                }
                for (int i = 0; i < num; i++)
                {
                    if (i >= parent.childCount)
                    {
                        Transform transform2 = UnityEngine.Object.Instantiate(transform, parent);
                        __instance.FaceList.Add(transform2.GetComponent<CharSelect_Face>());
                    }
                }
                __instance.StartCoroutine(PosChange(__instance, parent));
                log($"method CharSelect_CampUI.Init patched");
            }

            public static IEnumerator PosChange(CharSelect_CampUI __instance, Transform parent)
            {
                yield return new WaitForSeconds(0.1f);
                yield return new WaitForFixedUpdate();
                yield return new WaitForEndOfFrame();
                float dx = __instance.FaceList[3].transform.position.x - __instance.FaceList.Last((CharSelect_Face x) => x.gameObject.activeSelf).transform.position.x;
                Debug.Log(dx);
                parent.position = new Vector3(parent.position.x + dx, parent.position.y, parent.position.z);
            }
        }

        [HarmonyPatch(typeof(CharSelect_CampUI), "MoveCharacterDoc")]
        public static class Patch_MoveCharacterDoc
        {
            [HarmonyPrefix]
            public static bool Prefix(CharSelect_CampUI __instance, bool Save)
            {
                patch_count++;
                log($"start patching {patch_count}/3");
                float[] array = new float[3] { 0.5f, 0.85f, 0.25f };
                for (int i = 0; i < __instance.CharList.Count; i++)
                {
                    Debug.Log(i + ": " + __instance.CharList[i].data.name);
                    int num;
                    switch (i)
                    {
                        case 0:
                            num = 0;
                            break;
                        case 1:
                            num = 1;
                            break;
                        default:
                            num = (i != __instance.CharList.Count - 1) ? 3 : 2;
                            break;
                    }
                    if (num <= 2)
                    {
                        Vector3 vector = Vector3.Lerp(__instance.LeftTr.position, __instance.RightTr.position, array[num]);
                        Quaternion identity = Quaternion.identity;
                        float num2 = Mathf.Sqrt(Mathf.Pow(0.5f, 2f) - Mathf.Pow(array[num] - 0.5f, 2f));
                        vector.y += num2 * 2f;
                        identity = Quaternion.Slerp(__instance.LeftTr.rotation, __instance.RightTr.rotation, array[num]);
                        __instance.CharList[i].transform.DOMove(vector, 0.1f);
                        __instance.CharList[i].transform.DORotateQuaternion(identity, 0.1f);
                        if (i == 0)
                        {
                            __instance.CharList[i].NonSelectFGObj.SetActive(value: false);
                            __instance.NowSelectedKey = __instance.CharList[i].data.Key;
                        }
                        if (Save)
                        {
                            Traverse.Create(__instance).Field("PosList").GetValue<List<Vector3>>()
                                .Add(vector);
                            Traverse.Create(__instance).Field("RotList").GetValue<List<Quaternion>>()
                                .Add(identity);
                        }
                    }
                }
                log($"method CharSelect_CampUI.MoveCharacterDoc patched");
                return false;
            }
        }


        [HarmonyPatch(typeof(CharacterDocument), "MoveList")]
        public static class Patch_MoveList
        {
            [HarmonyPrefix]
            public static bool Prefix(CharacterDocument __instance, List<Vector3> _posList, List<Quaternion> _rotList, List<int> _orderList, int i)
            {
                __instance.Index = i;
                __instance.NonSelectFGObj.SetActive(value: true);
                int num;
                switch (i)
                {
                    case 0:
                        num = 0;
                        break;
                    case 1:
                        num = 1;
                        break;
                    default:
                        num = (i != __instance.Main.CharList.Count - 1) ? 3 : 2;
                        break;
                }
                if (num <= 2)
                {
                    __instance.transform.DOMove(_posList[num], 0.15f);
                    __instance.transform.DORotateQuaternion(_rotList[num], 0.15f);
                }
                if (i == 0)
                {
                    __instance.NonSelectFGObj.SetActive(value: false);
                    __instance.Main.NowSelectedKey = __instance.data.Key;
                    BattleAlly battleAlly = PlayData.TempBattleChar(__instance.data);
                    __instance.Main.Preview.Info = battleAlly.Info;
                }
                else
                {
                    __instance.NonSelectFGObj.SetActive(value: true);
                }
                return false;
            }
        }

    }
}