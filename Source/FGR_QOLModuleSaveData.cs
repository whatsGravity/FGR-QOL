namespace Celeste.Mod.FGR_QOL;
public class FGR_QOLModuleSaveData : EverestModuleSaveData {
    public bool FGRModeEnabled {get; set;} = false;
	// Maybe overkill, but stating a savefile necessarily overwrites the data in the current slot. Keeping a separate reference to the CurrRun prevents that.
	public SaveData CurrentRun;
	//TODO: having a whole ass save file for run data is wayyy too heavyweight. Do something like IzumiQOL journals instead?
	public SaveData RunsData;
	public SaveData PracticeFile;
	public string ActiveCampaign {get; set;} = "Celeste";
}