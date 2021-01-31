﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TranslationCommon
{
    [Serializable]
    public class LanguageData
    {
        [NonSerialized]
        public const string TranslationFileName = "translation.json";
        [NonSerialized]
        public const string TranslationDumpFileName = "translation.dump.txt";
        
        [SerializeField]
        public List<TranslationProto> TranslationTable;
        
        public LanguageData(ProtoSet<StringProto> stringProto)
        {
            TranslationTable = new List<TranslationProto>(stringProto.Length);
            for (var i = 0; i < stringProto.dataArray.Length; i++)
            {
                var proto = stringProto.dataArray[i];
                TranslationProto translationProto = new TranslationProto();
                translationProto.IsValid = true;
                translationProto.Original = proto.ENUS;
                translationProto.Translation = proto.ENUS;
                translationProto.Name = proto.Name;
                translationProto.ID = proto.ID;
                TranslationTable.Add(translationProto);
            }
        }

        public LanguageData()
        {
        }
        
        public LanguageData(LanguageSettings settings, LanguageData template)
        {
            UpdateTranslationItems(settings, template);
        }

        public void UpdateTranslationItems(LanguageSettings settings, LanguageData template)
        {
            var missMatchList = new List<TranslationProto>();
            var tempTranslationTable = TranslationTable.ToList();
            // Find invalid or missing translations
            foreach (var translationProto in tempTranslationTable)
            {
                var match = template.TranslationTable.FirstOrDefault(proto => proto.ID == translationProto.ID);
                if (match != null)
                {
                    if (match.Original != translationProto.Original)
                    {
                        translationProto.IsValid = false;
                        missMatchList.Add(translationProto);
                        ConsoleLogger.LogWarning($"Translation for {translationProto.Original} -- {translationProto.Translation} is no longer valid! This entry original meaning has changed");
                    }
                }
                else
                {
                    translationProto.IsValid = false;
                    missMatchList.Add(translationProto);
                    ConsoleLogger.LogWarning($"Translation for {translationProto.Original} -- {translationProto.Translation} is no longer valid! This entry was probably removed");
                }
            }
            // New translations
            foreach (var translationProto in template.TranslationTable)
            {
                var match = TranslationTable.FirstOrDefault(proto => proto.ID == translationProto.ID);
                if (match == null)
                {
                    missMatchList.Add(translationProto);
                    tempTranslationTable.Add(translationProto);
                    ConsoleLogger.LogWarning($"New translation entry for {translationProto.Original}  (Upgrade from {settings.GameVersion} to {GameConfig.gameVersion.ToFullString()})");
                }
            }

            TranslationTable = tempTranslationTable;
            settings.GameVersion = GameConfig.gameVersion.ToFullString();
        }
    }
}