using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Celeste.Mod.Core;
using Monocle;
using MonoMod.Cil;
using System.Text.RegularExpressions;
using System.Linq;

namespace Celeste.Mod.FGR_QOL;

public class FGR_QOLModule : EverestModule {
    public static FGR_QOLModule Instance { get; private set; }

    public override Type SettingsType => typeof(FGR_QOLModuleSettings);
    public static FGR_QOLModuleSettings Settings => (FGR_QOLModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(FGR_QOLModuleSession);
    public static FGR_QOLModuleSession Session => (FGR_QOLModuleSession) Instance._Session;

    public override Type SaveDataType => typeof(FGR_QOLModuleSaveData);
    public static FGR_QOLModuleSaveData SaveData => (FGR_QOLModuleSaveData) Instance._SaveData;

    public bool IsMoving;
    public FGR_QOLModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(FGR_QOLModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(FGR_QOLModule), LogLevel.Verbose);
#endif
        IsMoving = false;
    }

    public override void Load() {
        // TODO: apply any hooks that should always be active
        // IL.Celeste.OuiFileSelectSlot.Setup += onOuiFileSelectSetup;
        Logger.Log(nameof(FGR_QOLModule), "Loading Hooks");
        Logger.Log(nameof(FGR_QOLModule), "Hooks Loaded");

    }

    public override void Unload() {
        // TODO: unapply any hooks applied in Load()
    }

    // #region Hook Definitions
}