#pragma once
#include <BWAPI.h>

#include <BWTA.h>
#include <windows.h>

using namespace std;
using namespace BWAPI;

extern bool analyzed;
extern bool analysis_just_finished;
extern BWTA::Region* home;
extern BWTA::Region* enemy_base;
DWORD WINAPI AnalyzeThread();

class StarCraftConnector : public BWAPI::AIModule
{
public: //
	StarCraftConnector();
	virtual ~StarCraftConnector();

public:
  virtual void onStart();
  virtual void onEnd(bool isWinner);
  virtual void onFrame();
  virtual void onSendText(std::string text);
  virtual void onReceiveText(BWAPI::Player* player, std::string text);
  virtual void onPlayerLeft(BWAPI::Player* player);
  virtual void onNukeDetect(BWAPI::Position target);
  virtual void onUnitDiscover(BWAPI::Unit* unit);
  virtual void onUnitEvade(BWAPI::Unit* unit);
  virtual void onUnitShow(BWAPI::Unit* unit);
  virtual void onUnitHide(BWAPI::Unit* unit);
  virtual void onUnitCreate(BWAPI::Unit* unit);
  virtual void onUnitDestroy(BWAPI::Unit* unit);
  virtual void onUnitMorph(BWAPI::Unit* unit);
  virtual void onUnitRenegade(BWAPI::Unit* unit);
  virtual void onSaveGame(std::string gameName);

  void drawStats(); //not part of BWAPI::AIModule
  void drawBullets();
  void drawVisibilityData();
  void drawTerrainData();
  void showPlayers();
  void showForces();
  bool show_bullets;
  bool show_visibility_data;

private:
	int InitSocket();
	void HandleCommand(int command, int unitID, int arg0, int arg1, int arg2);

	// Helpers for accessing internal lookups
	Unit* getUnit(int unitID);
	UnitType getUnitType(int type);
	TechType getTechType(int type);
	UpgradeType getUpgradeType(int type);
	Position getPosition(int x, int y);
	TilePosition getTilePosition(int x, int y);

private:
	// Socket identifier
	int m_proxyBotSocket;

	// Mapping each BWAPI Unit to an integer ID, which the .NET side can refer to
	map<Unit*, int> m_unitMap;
	map<int, Unit*> m_unitIDMap;
	map<int, UnitType> m_typeMap;
	map<int, TechType> m_techMap;
	map<int, UpgradeType> m_upgradeMap;

	// Used to asssign unit IDs
	int m_unitIDCounter;

	// Shall we be garrulous?
	bool m_logCmds;
};