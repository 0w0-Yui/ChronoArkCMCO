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
namespace CampfireMoreCharacterOption
{
    public class CampfireMoreCharacterOption_ModDefinition:ModDefinition
    {
        public override Type ModItemKeysType => typeof(ModItemKeys);
        /* //Example
        [CustomGDE(nameof(GDEItemKeys.Character_Sizz),nameof(GDECharacterData.name))]
        public string SizzName
        {
            get
            {
                return $"Sizz(New Name of {ModId})";
            }
        }
        [CustomGDE(nameof(GDEItemKeys.Skill_S_Sizz_2), "UseAp")]
        [CustomGDE(nameof(GDEItemKeys.Skill_S_Sizz_1), "UseAp")]
        public static int Sizz1AP(int oldap)
        {
            return oldap + 1;
        }
        [CustomGDE("S_Sizz_1", nameof(GDESkillData.Except))]
        public static bool Sizz1Except = true;
        */
    }
    /* //Example
    public class ExampleSkill : CustomSkillGDE<CampfireMoreCharacterOption_ModDefinition>
    {
        public override string Key()
        {
            return "ExampleSkill"; //it will override your "ExampleSkill" gdata in the mod editor
        }
        public override ModGDEInfo.LoadingType GetLoadingType()
        {
            return ModGDEInfo.LoadingType.Add; 
        }
        public override void SetValue()
        {
            PlusSkillView = ModKey<ExampleSkill>();//for ModDefinition gdata
            User = GDEItemKeys.Character_Azar;//for gdata of orginial game
            SkillExtended = new List<string> { typeof(ExampleSkill_SkillExtended).AssemblyQualifiedName };//for script
            //Image_0 = assetInfo.ImageFromAsset("Your AssetBundle Path", "Path in Unity"); 
        }
        public class ExampleSkill_SkillExtended:Skill_Extended
        {

        }
    }*/
}