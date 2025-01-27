using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Celeste.Mod.Core;
using Celeste.Mod.UI;
using Monocle;
using MonoMod.Cil;
using System.Text.RegularExpressions;
using System.Linq;
using MonoMod.Utils;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.FGR_QOL;

public class FGR_QOLModule : EverestModule {
    public static FGR_QOLModule Instance { get; private set; }

    public override Type SettingsType => typeof(FGR_QOLModuleSettings);
    public static FGR_QOLModuleSettings Settings => (FGR_QOLModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(FGR_QOLModuleSession);
    public static FGR_QOLModuleSession Session => (FGR_QOLModuleSession) Instance._Session;

    public override Type SaveDataType => typeof(FGR_QOLModuleSaveData);
    public static FGR_QOLModuleSaveData FGRSaveData => (FGR_QOLModuleSaveData) Instance._SaveData;

    public OuiFileSelectSlot currentSlot;
    private OuiFileSelectSlot.Button toggleButton;
    private OuiFileSelectSlot.Button practiceButton;
    private OuiFileSelectSlot.Button newRunButton;
    private TextMenu.Button restartRunButton;
    public Dictionary<int, bool> FGRFiles = new Dictionary<int, bool>();


    public FGR_QOLModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(FGR_QOLModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(FGR_QOLModule), LogLevel.Verbose);
#endif
    }

    public override void Load() {
        // TODO: apply any hooks that should always be active
        // IL.Celeste.OuiFileSelectSlot.Setup += onOuiFileSelectSetup;
        Logger.Log(nameof(FGR_QOLModule), "Loading Hooks");
        // On.Celeste.OuiFileSelectSlot.Setup += On_OuiFileSelectSlot_Setup;
        // On.Celeste.OuiFileSelectSlot.Render += On_OuiFileSelectSlot_Render;
        Everest.Events.FileSelectSlot.OnCreateButtons += FileSelectSlot_OnCreateButtons;
        Everest.Events.Level.OnCreatePauseMenuButtons += Level_OnCreatePauseMenuButtons;
        On.Celeste.OuiFileSelectSlot.OnNewGameSelected += On_OuiFileSelectSlot_OnNewGameSelected;
        Logger.Log(nameof(FGR_QOLModule), "Hooks Loaded");

    }

    private void Level_OnCreatePauseMenuButtons(Level level, TextMenu menu, bool minimal)
    {
        restartRunButton = new TextMenu.Button("Restart Run");
        restartRunButton.OnEnter = RestartRun;
        menu.Insert(2, restartRunButton);
    }

    private void RestartRun()
    {
        SaveStats();
        ExitToMenu();
        if(Settings.RestartToMenu) {
            GotoSaveFile(currentSlot);
        } else {
            StartNewRun();
        }
        throw new NotImplementedException();
    }

    private void StartNewRun()
    {
        throw new NotImplementedException();
    }

    private void GotoSaveFile(OuiFileSelectSlot currentSlot)
    {
        throw new NotImplementedException();
    }

    private void ExitToMenu()
    {
        throw new NotImplementedException();
    }

    private void SaveStats()
    {
        throw new NotImplementedException();
    }

    public override void Unload() {
        // TODO: unapply any hooks applied in Load()
        // On.Celeste.OuiFileSelectSlot.Render -= On_OuiFileSelectSlot_Render;
        Everest.Events.FileSelectSlot.OnCreateButtons -= FileSelectSlot_OnCreateButtons;
        On.Celeste.OuiFileSelectSlot.OnNewGameSelected -= On_OuiFileSelectSlot_OnNewGameSelected;

    }

    #region Hook Definitions


    public void On_OuiFileSelectSlot_Render(On.Celeste.OuiFileSelectSlot.orig_Render orig, OuiFileSelectSlot self)
    {
        if (IsFGRFile(self.FileSlot))
        {
            // mimic the original code for drawing tabs
            DynData<OuiFileSelectSlot> slotData = new DynData<OuiFileSelectSlot>(self);
            float highlightEase = slotData.Get<float>("highlightEase");
            float newgameFade = slotData.Get<float>("newgameFade");
            float scaleFactor = Ease.CubeInOut(highlightEase);
            Vector2 vector = self.Position - Vector2.UnitX * scaleFactor * 360f;
            float scale = self.Exists ? 1f : newgameFade;
            if (!self.Corrupted && (newgameFade > 0f || self.Exists))
            {
                MTN.FileSelect["hardcoretab"].DrawCentered(vector, Color.White * scale);
            }
        }
        orig(self);
    }

    public void FileSelectSlot_OnCreateButtons(List<OuiFileSelectSlot.Button> buttons, OuiFileSelectSlot slot, EverestModuleSaveData modSaveData, bool fileExists)
    {
        currentSlot = slot;
        if (!fileExists)
        {
            string dialogId = FGRFiles[slot.FileSlot] ? "FILE_FGR_ON" : "FILE_FGR_OFF";
            // string dialogId = "FILE_FGR_ON";
            toggleButton = new OuiFileSelectSlot.Button()
            {
                Label = Dialog.Clean(dialogId),
                Action = OnFGRToggleSelected,
                Scale = 0.7f
            };
            buttons.Add(toggleButton);
        }
        if(IsFGRFile(slot.FileSlot)) {
            // TODO: Make a "Practice", "Continue Run", and "Start New Run" button
            practiceButton = new OuiFileSelectSlot.Button() 
            {
                Label = Dialog.Clean("FILE_FGR_PRACTICE"),
                Action = EnterPracticeFile,
                Scale = 0.7f
            };
            buttons.Add(practiceButton);
            newRunButton = new OuiFileSelectSlot.Button()
            {
                Label = Dialog.Clean("FILE_FGR_PRACTICE"),
                Action = StartNewRun,
                Scale = 0.7f
            };
        }
    }

    private void EnterPracticeFile()
    {
        LoadSaveData(currentSlot.FileSlot);
        SaveData PracticeSave = FGRSaveData.PracticeFile;

        currentSlot.StartingGame = true;
        Audio.Play("event:/ui/main/savefile_begin");
        SaveData.Start(FGRSaveData.PracticeFile, currentSlot.FileSlot);
        SaveData.Instance.AssistMode = currentSlot.AssistModeEnabled;
        SaveData.Instance.VariantMode = currentSlot.VariantModeEnabled;
        SaveData.Instance.AssistModeChecks();
        if (SaveData.Instance.CurrentSession_Safe != null && SaveData.Instance.CurrentSession_Safe.InArea)
        {
            Audio.SetMusic(null);
            Audio.SetAmbience(null);
            currentSlot.fileSelect.Overworld.ShowInputUI = false;
            new FadeWipe(currentSlot.Scene, wipeIn: false, delegate
            {
                LevelEnter.Go(SaveData.Instance.CurrentSession_Safe, fromSaveData: true);
            });
        }
        else if (SaveData.Instance.Areas_Safe[0].Modes[0].Completed || SaveData.Instance.CheatMode)
        {
            if (SaveData.Instance.CurrentSession_Safe != null && SaveData.Instance.CurrentSession_Safe.ShouldAdvance)
            {
                SaveData.Instance.LastArea_Safe.ID = SaveData.Instance.UnlockedAreas_Safe;
            }
            SaveData.Instance.CurrentSession_Safe = null;
            (currentSlot.Scene as Overworld).Goto<OuiChapterSelect>();
        }
        else
        {
            Audio.SetMusic(null);
            Audio.SetAmbience(null);
            currentSlot.EnterFirstArea();
        }
    }

    public void On_OuiFileSelectSlot_OnNewGameSelected(On.Celeste.OuiFileSelectSlot.orig_OnNewGameSelected orig, OuiFileSelectSlot self)
    {
        orig(self);
        LoadSaveData(self.FileSlot);
        FGRSaveData.FGRModeEnabled = FGRFiles[self.FileSlot];
        DynData<OuiFileSelectSlot> fileSlotData = new DynData<OuiFileSelectSlot>(self);

        string newGameLevelSet = fileSlotData.Get<OuiFileSelectSlot.Button>("newGameLevelSetPicker").Label;
        FGRSaveData.ActiveCampaign = newGameLevelSet;
        FGRSaveData.PracticeFile = MakePracticeFile(self.FileSlot, self.Name, self.AssistModeEnabled, self.VariantModeEnabled);
        SaveSaveData(self.FileSlot);
        if (FGRSaveData.FGRModeEnabled)
        {
            SaveData.Instance.AssistMode = false;
            // DisableDebugMode();
        }
    }

    private SaveData MakePracticeFile(int fileSlot, string Name, bool AssistModeEnabled, bool VariantModeEnabled)
    {
        return new SaveData {
            Name=Name,
            AssistMode=AssistModeEnabled,
            VariantMode=VariantModeEnabled
        };
    }

    private void OnFGRToggleSelected()
    {
        int index = currentSlot.FileSlot;
        if (!IsFGRFile(index))
        {
            FGRFiles[index] = true;
            toggleButton.Label = Dialog.Clean("FILE_FGR_ON");
            Audio.Play("event:/ui/main/button_toggle_on");
        }
        else
        {
            FGRFiles[index] = false;
            toggleButton.Label = Dialog.Clean("FILE_FGR_OFF");
            Audio.Play("event:/ui/main/button_toggle_off");
        }
    }

    // Largely borrowed from Hardcore mode 
    private bool IsFGRFile(int slot)
    {
        // attempt to get from dictionary
        if (FGRFiles.TryGetValue(slot, out bool value))
        {
            return value;
        }
        // default to false if the save file doesn't exist at all
        else if (!UserIO.Exists(SaveData.GetFilename(slot)))
        {
            return false;
        }
        else
        {
            // attempt to load from save file
            try
            {
                Logger.Log(LogLevel.Info, "FGR_QOL", $"Getting save data from file {slot}");
                base.LoadSaveData(slot);
                FGRFiles[slot] = FGRSaveData.FGRModeEnabled;
                return FGRFiles[slot];
            }
            // default to false if that fails somehow
            catch
            {
                Logger.Log(LogLevel.Warn, "FGR_QOL", $"Could not get save data from file {slot}");
                FGRFiles[slot] = false;
                return false;
            }
        }
    }

    #endregion
}