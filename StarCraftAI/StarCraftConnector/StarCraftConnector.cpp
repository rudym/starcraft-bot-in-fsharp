/**
 * StarCraftConnector. Broadcasts BWAPI game state to a socket on each tick.
 * Code origonates from http://eis.ucsc.edu/StarProxyBot\
 */
#include "StarCraftConnector.h"
#include "BWApiSerializer.h"

using namespace std;
using namespace BWAPI;

bool analyzed;
bool analysis_just_finished;
BWTA::Region* home;
BWTA::Region* enemy_base;

LPDWORD  dwThreadID;//Thread ID for map analyze

// Port used to connect to .NET side
#define PORTNUM 12620

string toString(int value);
string toString(bool value);

// ----------------------------------------------------------------------------

StarCraftConnector::StarCraftConnector()
{
	m_unitIDCounter = 1;
	m_logCmds = true;
}

StarCraftConnector::~StarCraftConnector()
{
}

/**
 * Called at the start of a match. 
 *
 * Establishes a connection with the proxybot and sends the following messages:
 * - An initial ACK
 * - Player information
 * - Data about the unit types
 * - Data about the starting locations
 * - Map tile information
 * - Tech types
 * - Upgrade types
 */
void StarCraftConnector::onStart()
{
	// TODO: only init if not connected
	m_proxyBotSocket = InitSocket();

	if (m_proxyBotSocket == -1) 
	{
		Broodwar->sendText("Proxy Bot connection FAILURE!");
	}
	else 
	{
		string ack("ProxyBotACK");
		ack += "\n";

		send(m_proxyBotSocket,(char*)ack.c_str(), ack.size(), 0);
		Broodwar->sendText("Connected to Proxy Bot!");
	}	

	// Wait for bot options
	char buf[1024];
	int numBytes = recv(m_proxyBotSocket , buf , 1024 , 0);

	// cheats
	if (buf[0] == '1') Broodwar->enableFlag(Flag::UserInput);
	if (buf[1] == '1') Broodwar->enableFlag(Flag::CompleteMapInformation); 

	//read map information into BWTA so terrain analysis can be done in another thread
	BWTA::readMap();
	analyzed=false;
	analysis_just_finished=false;

	// Player info
	string playerInfo = BWApiSerializer::GetPlayerInfo();
	char *piBuf = (char*)playerInfo.c_str();
	send(m_proxyBotSocket, piBuf, playerInfo.size(), 0);

	// Unit types
	set<UnitType> types = UnitTypes::allUnitTypes();
	for(set<UnitType>::iterator i = types.begin(); i != types.end(); i++)
	{
		int id = i->getID();
		m_typeMap[id] = (*i);
	}

	string unitTypes = BWApiSerializer::GetUnitTypes();
	char *utBuf = (char*)unitTypes.c_str();
	send(m_proxyBotSocket, utBuf, unitTypes.size(), 0);

	// Starting locations
	string locations = BWApiSerializer::GetStartingLocations();
	char *slBuf = (char*)locations.c_str();
	send(m_proxyBotSocket, slBuf, locations.size(), 0);

	// Get the map data
	string mapHeader = BWApiSerializer::GetMapHeader();
	char *mhbuf = (char*)mapHeader.c_str();
	send(m_proxyBotSocket, mhbuf, mapHeader.size(), 0);

	string mapData = BWApiSerializer::GetMapData();
	char *mdbuf = (char*)mapData.c_str();
	send(m_proxyBotSocket, mdbuf, mapData.size(), 0);

	// Tech types
	set<TechType> tektypes = TechTypes::allTechTypes();
	for(set<TechType>::iterator i = tektypes.begin(); i != tektypes.end(); i++)
	{
		int id = i->getID();
		m_techMap[id] = (*i);
	}

	string techTypes = BWApiSerializer::GetTechTypes();
	char *ttbuf = (char*)techTypes.c_str();
	send(m_proxyBotSocket, ttbuf, techTypes.size(), 0);

	// Upgrade types
	set<UpgradeType> upTypes = UpgradeTypes::allUpgradeTypes();
	for(set<UpgradeType>::iterator i = upTypes.begin(); i != upTypes.end(); i++)
	{
		int id = i->getID();
		m_upgradeMap[id] = (*i);
	}

	string upgradeTypes = BWApiSerializer::GetUpgradeTypes();
	char *utbuf = (char*) upgradeTypes.c_str();
	send(m_proxyBotSocket, utbuf, upgradeTypes.size(), 0);

	// At this point we are done dumping meta info and
	// will just spew everything we know on each frame.
}

void StarCraftConnector::onEnd(bool isWinner)
{
}

/**
 * Sends the unit status to the ProxyBot, then waits for a command message.
 */
void StarCraftConnector::onFrame()
{
	if (show_visibility_data) drawVisibilityData();

	if (analyzed) drawTerrainData();

	if (analysis_just_finished)
	{
		Broodwar->printf("Finished analyzing map.");
		analysis_just_finished=false;
	}

	if (analyzed && Broodwar->getFrameCount()%30==0)
  {
    //order one of our workers to guard our chokepoint.
    for(std::set<Unit*>::const_iterator i=Broodwar->self()->getUnits().begin();i!=Broodwar->self()->getUnits().end();i++)
    {
      if ((*i)->getType().isWorker())
      {
        //get the chokepoints linked to our home region
        std::set<BWTA::Chokepoint*> chokepoints= home->getChokepoints();
        double min_length=10000;
        BWTA::Chokepoint* choke=NULL;

        //iterate through all chokepoints and look for the one with the smallest gap (least width)
        for(std::set<BWTA::Chokepoint*>::iterator c=chokepoints.begin();c!=chokepoints.end();c++)
        {
          double length=(*c)->getWidth();
          if (length<min_length || choke==NULL)
          {
            min_length=length;
            choke=*c;
          }
        }

        //order the worker to move to the center of the gap
        (*i)->rightClick(choke->getCenter());
        break;
      }
    }
  }

	// Figure out what units and upgrades the bot can produce
	bool unitProduction[230];
	for (int i=0; i<230; i++) 
		unitProduction[i] = false;

	std::set<Unit*> selfUnits = Broodwar->self()->getUnits();
	std::set<UnitType> types = UnitTypes::allUnitTypes();
	for(std::set<UnitType>::iterator i=types.begin();i!=types.end();i++)
	{
		for(std::set<Unit*>::iterator j=selfUnits.begin();j!=selfUnits.end();j++)
		{
			if (Broodwar->canMake((*j), (*i))) {
				unitProduction[(*i).getID()] = true;
				break;
			}
		}
	}

	bool upgradeProduction[63];
	for (int i=0; i < 63; i++) 
		upgradeProduction[i] = false;

	std::set<UpgradeType> upTypes = UpgradeTypes::allUpgradeTypes();
	for(std::set<UpgradeType>::iterator i=upTypes.begin();i!=upTypes.end();i++)
	{
		for(std::set<Unit*>::iterator j=selfUnits.begin();j!=selfUnits.end();j++)
		{
			if (Broodwar->canUpgrade((*j), (*i))) {
				upgradeProduction[(*i).getID()] = true;
				break;
			}
		}
	}

	bool techProduction[47];
	for (int i=0; i<47; i++)
		techProduction[i] = false;

	std::set<TechType> tektypes = TechTypes::allTechTypes();
	for(std::set<TechType>::iterator i = tektypes.begin();i != tektypes.end(); i++)
	{
		for(std::set<Unit*>::iterator j = selfUnits.begin();j != selfUnits.end(); j++)
		{
			if (Broodwar->canResearch((*j), (*i))) {
				techProduction[(*i).getID()] = true;
				break;
			}
		}
	}

	std::string canProduce("");
	for (int i=0; i<230; i++) {
		if (unitProduction[i]) canProduce += "1";
		else canProduce += "0";
	}

	std::string canUpgrade("");
	for (int i=0; i<63; i++) {
		if (upgradeProduction[i]) canUpgrade += "1";
		else canUpgrade += "0";
	}

	std::string canTech("");
	for (int i=0; i<47; i++) {
		if (techProduction[i]) canTech += "1";
		else canTech += "0";
	}

	// send the unit status's to the Proxy Bot
	std::string status("status");
	std::set<Unit*> myUnits = Broodwar->getAllUnits();

	// also send current resources
	int minerals = Broodwar->self()->minerals();
	int gas = Broodwar->self()->gas();
	int supplyUsed = Broodwar->self()->supplyUsed();
	int supplyTotal = Broodwar->self()->supplyTotal();

	status += ";";
	status += toString(minerals);
	status += ";";
	status += toString(gas);
	status += ";";
	status += toString(supplyUsed);
	status += ";";
	status += toString(supplyTotal);
	status += ";";
	status += canProduce;
	status += ";";
	status += canTech;
	status += ";";
	status += canUpgrade;

	// Print Units
	// Now switch to a PIPE delimited list of units...
	// ... with their stats COMMA delimited
	status += ";";
	for(std::set<Unit*>::iterator i = myUnits.begin(); i != myUnits.end(); i++)
	{
		// get the unit ID
		int unitID = m_unitMap[*i];
		if (unitID == 0) {

			// assign an ID if there is not one currently associated with the unit
			unitID = m_unitIDCounter++; 

			m_unitMap[*i] = unitID;
			m_unitIDMap[unitID] = *i;
		}

		status += toString(unitID);
		status += ",";
		status += toString((*i)->getPlayer()->getID());
		status += ",";
		status += toString((*i)->getType().getID());
		status += ",";
		status += toString((*i)->getPosition().x()/32);
		status += ",";
		status += toString((*i)->getPosition().y()/32);
		status += ",";
		status += toString((*i)->getHitPoints()/256);
		status += ",";
		status += toString((*i)->getShields()/256);
		status += ",";
		status += toString((*i)->getEnergy()/256);
		status += ",";
		status += toString((*i)->getOrderTimer());
		status += ",";
		status += toString((*i)->getOrder().getID());
		status += ",";
		status += toString((*i)->isTraining());
		status += ",";
		status += toString((*i)->getResources());
		status += "|";  // NOTE: This leaves a trailing '|' character
	} 

	// Send status update to proxy bot
	status += ";\n";
	char *sbuf = (char*)status.c_str();
	send(m_proxyBotSocket, sbuf, status.size(), 0);
	
	// Process any pending commands
	char buf[1024];
	int numBytes = recv(m_proxyBotSocket , buf , 1024 , 0);

	char *message = new char[numBytes + 1];
	message[numBytes] = 0;
	for (int i = 0; i < numBytes; i++) 
	{
		message[i] = buf[i];
	}

	// tokenize the commands
	char* next_token;
	char* token = strtok_s(message, ":", &next_token);
	// eat the command part of the message
	token = strtok_s(NULL, ":", &next_token);

	int commandCount = 0;
    char* commands[100];

	while (token != NULL) 
	{
		commands[commandCount] = token;
		commandCount++;
		token = strtok_s(NULL, ":", &next_token);
	}

	// tokenize the arguments
	for (int i = 0; i < commandCount; i++) 
	{
		char* command = strtok_s(commands[i], ";", &next_token);
		char* unitID = strtok_s(NULL, ";", &next_token);
		char* arg0 = strtok_s(NULL, ";", &next_token);
		char* arg1 = strtok_s(NULL, ";", &next_token);
		char* arg2 = strtok_s(NULL, ";", &next_token);

		HandleCommand(atoi(command), atoi(unitID), atoi(arg0), atoi(arg1), atoi(arg2));
	}
}

void StarCraftConnector::onUnitCreate(BWAPI::Unit* unit)
{
}

void StarCraftConnector::onUnitDestroy(BWAPI::Unit* unit)
{
}

void StarCraftConnector::onUnitMorph(BWAPI::Unit* unit)
{
}

void StarCraftConnector::onUnitShow(BWAPI::Unit* unit)
{
}

void StarCraftConnector::onUnitHide(BWAPI::Unit* unit)
{
}

void StarCraftConnector::onUnitRenegade(BWAPI::Unit* unit)
{
}

void StarCraftConnector::onSaveGame(std::string gameName)
{
}

void StarCraftConnector::onReceiveText(BWAPI::Player* player, std::string text)
{
}

void StarCraftConnector::onPlayerLeft(BWAPI::Player* player)
{
}

void StarCraftConnector::onNukeDetect(BWAPI::Position target)
{
}

void StarCraftConnector::onUnitDiscover(BWAPI::Unit* unit)
{
}
void StarCraftConnector::onUnitEvade(BWAPI::Unit* unit)
{
}

void StarCraftConnector::onSendText(string text)
{
	if (text=="/show bullets")
	{
		//show_bullets = !show_bullets;
	} else if (text=="/show players")
	{
		//showPlayers();
	} else if (text=="/show forces")
	{
		//showForces();
	} else if (text=="/show visibility")
	{
		//show_visibility_data=!show_visibility_data;
	} else if (text=="/analyze")
	{
		if (analyzed == false)
		{
			Broodwar->printf("Analyzing map... this may take a minute");
			
			CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)AnalyzeThread, NULL, 0, dwThreadID);
			
			Broodwar->printf("test after  CreateThread() ");
			Broodwar->printf("Analyzing map... this may take a minute");
		}
	} else
	{
		Broodwar->printf("You typed '%s'!",text.c_str());
	}
}

/**
 * Establishes a connection with the ProxyBot.
 *
 * Returns -1 if the connection fails
 */
int StarCraftConnector::InitSocket() 
{
      int sockfd;
      int size;
      struct hostent *h;
      struct sockaddr_in client_addr;
      char myname[256];
      WORD wVersionRequested;
      WSADATA wsaData;

      wVersionRequested = MAKEWORD( 1, 1 );
      WSAStartup( wVersionRequested, &wsaData );
      gethostname(myname, 256);      
      h=gethostbyname(myname);

      size = sizeof(client_addr);
      memset(&client_addr , 0 , sizeof(struct sockaddr_in));
      memcpy((char *)&client_addr.sin_addr , h -> h_addr ,h -> h_length);
     
	  client_addr.sin_family = AF_INET;
      client_addr.sin_port = htons((u_short) PORTNUM);
      client_addr.sin_addr =  *((struct in_addr*) h->h_addr) ;
      if ((sockfd = socket(AF_INET , SOCK_STREAM , 0)) == -1){
		  return -1;
      }

      if ((connect(sockfd , (struct sockaddr *)&client_addr , sizeof(client_addr))) == -1){
		  return -1;
	  }

	  return sockfd;
}

void StarCraftConnector::HandleCommand(int command, int unitID, int arg0, int arg1, int arg2)
{

	// check that the unit ID is valid
	Unit* unit = m_unitIDMap[unitID];
	if (unit == NULL) 
	{
		Broodwar->sendText("Issued command to invalid unit ID: %d", unitID);
		return;
	}

	// execute the command
	switch (command) {
	    // virtual bool attackMove(Position position) = 0;
		case 1:
			if (m_logCmds) Broodwar->sendText("Unit:%d attackMove(%d, %d)",unitID, arg0, arg1);
			unit->attack(getPosition(arg0, arg1));
			break;
		// virtual bool attackUnit(Unit* target) = 0;
		case 2:
			if (getUnit(arg0) == NULL) {
				Broodwar->sendText("Invalid Command, Unit:%d attackUnit(%d)", unitID, arg0);
			}
			else {
				if (m_logCmds) Broodwar->sendText("Unit:%d attackUnit(%d)", unitID, arg0);
				unit->attack(getUnit(arg0));
			}
			break;
		// virtual bool rightClick(Position position) = 0;
		case 3:
			if (m_logCmds) Broodwar->sendText("Unit:%d rightClick(%d, %d)",unitID, arg0, arg1);
			unit->rightClick(getPosition(arg0, arg1));
			break;
		// virtual bool rightClick(Unit* target) = 0;
		case 4:
			if (getUnit(arg0) == NULL) {
				Broodwar->sendText("Invalid Command, Unit:%d rightClick(%d)", unitID, arg0);
			}
			else {
				if (m_logCmds) Broodwar->sendText("Unit:%d rightClick(%d)", unitID, arg0);
				unit->rightClick(getUnit(arg0));
			}
			break;
		// virtual bool train(UnitType type) = 0;
		case 5:
			if (getUnitType(arg0) == NULL) {
				Broodwar->sendText("Invalid Command, Unit:%d train(%d)", unitID, arg0);
			}
			else {
				if (m_logCmds) Broodwar->sendText("Unit:%d train(%d)", unitID, arg0);
				unit->train(getUnitType(arg0));
			}
			break;
		// virtual bool build(TilePosition position, UnitType type) = 0;
		case 6:
			if (getUnitType(arg2) == NULL) {
				Broodwar->sendText("Invalid Command, Unit:%d build(%d, %d, %d)", unitID, arg0, arg1, arg2);
			}
			else {
				if (m_logCmds) Broodwar->sendText("Unit:%d build(%d, %d, %d)", unitID, arg0, arg1, arg2);
				unit->build(getTilePosition(arg0, arg1), getUnitType(arg2));
			}
			break;
		// virtual bool buildAddon(UnitType type) = 0;
		case 7:
			if (getUnitType(arg0) == NULL) {
				Broodwar->sendText("Invalid Command, Unit:%d buildAddon(%d)", unitID, arg0);
			}
			else {
				if (m_logCmds) Broodwar->sendText("Unit:%d buildAddon(%d)", unitID, arg0);
				unit->buildAddon(getUnitType(arg0));
			}
			break;
		// virtual bool research(TechType tech) = 0;
		case 8:
			if (getTechType(arg0) == NULL) {
				Broodwar->sendText("Invalid Command, Unit:%d research(%d)", unitID, arg0);
			}
			else {
				if (m_logCmds) Broodwar->sendText("Unit:%d research(%d)", unitID, arg0);
				unit->research(getTechType(arg0));
			}
			break;
		// virtual bool upgrade(UpgradeType upgrade) = 0;
		case 9:
			if (getUpgradeType(arg0) == NULL) {
				Broodwar->sendText("Invalid Command, Unit:%d upgrade(%d)", unitID, arg0);
			}
			else {
				if (m_logCmds) Broodwar->sendText("Unit:%d upgrade(%d)", unitID, arg0);
				unit->upgrade(getUpgradeType(arg0));
			}
			break;
		// virtual bool stop() = 0;
		case 10:
			if (m_logCmds) Broodwar->sendText("Unit:%d stop()", unitID);
			unit->stop();
			break;
		// virtual bool holdPosition() = 0;
		case 11:
			if (m_logCmds) Broodwar->sendText("Unit:%d holdPosition()", unitID);
			unit->holdPosition();
			break;
		// virtual bool patrol(Position position) = 0;
		case 12:
			if (m_logCmds) Broodwar->sendText("Unit:%d patrol(%d, %d)", unitID, arg0, arg1);
			unit->patrol(getPosition(arg0, arg1));
			break;
		// virtual bool follow(Unit* target) = 0;
		case 13:
			if (getUnit(arg0) == NULL) {
				Broodwar->sendText("Invalid Command, Unit:%d follow(%d)", unitID, arg0);
			}
			else {
				if (m_logCmds) Broodwar->sendText("Unit:%d follow(%d)", unitID, arg0);
				unit->follow(getUnit(arg0));
			}
			break;
		// virtual bool setRallyPosition(Position target) = 0;
		case 14:
			if (m_logCmds) Broodwar->sendText("Unit:%d setRallyPosition(%d, %d)", unitID, arg0, arg1);
			unit->setRallyPoint(getPosition(arg0, arg1));
			break;
		// virtual bool setRallyUnit(Unit* target) = 0;
		case 15:
			if (getUnit(arg0) == NULL) {
				Broodwar->sendText("Invalid Command, Unit:%d setRallyUnit(%d)", unitID, arg0);
			}
			else {
				if (m_logCmds) Broodwar->sendText("Unit:%d setRallyUnit(%d)", unitID, arg0);
				unit->setRallyPoint(getUnit(arg0));
			}
			break;
		// virtual bool repair(Unit* target) = 0;
		case 16:
			if (getUnit(arg0) == NULL) {
				Broodwar->sendText("Invalid Command, Unit:%d repair(%d)", unitID, arg0);
			}
			else {
				if (m_logCmds) Broodwar->sendText("Unit:%d repair(%d)", unitID, arg0);
				unit->repair(getUnit(arg0));
			}
			break;
		// virtual bool morph(UnitType type) = 0;
		case 17:
			if (getUnitType(arg0) == NULL) {
				Broodwar->sendText("Invalid Command, Unit:%d morph(%d)", unitID, arg0);
			}
			else {
				if (m_logCmds) Broodwar->sendText("Unit:%d morph(%d)", unitID, arg0);
				unit->morph(getUnitType(arg0));
			}
			break;
		// virtual bool burrow() = 0;
		case 18:
			if (m_logCmds) Broodwar->sendText("Unit:%d burrow()", unitID);
			unit->burrow();
			break;
		// virtual bool unburrow() = 0;
		case 19:
			if (m_logCmds) Broodwar->sendText("Unit:%d unburrow()", unitID);
			unit->unburrow();
			break;
		// virtual bool siege() = 0;
		case 20:
			if (m_logCmds) Broodwar->sendText("Unit:%d siege()", unitID);
			unit->siege();
			break;
		// virtual bool unsiege() = 0;
		case 21:
			if (m_logCmds) Broodwar->sendText("Unit:%d unsiege()", unitID);
			unit->unsiege();
			break;
		// virtual bool cloak() = 0;
		case 22:
			if (m_logCmds) Broodwar->sendText("Unit:%d cloak()", unitID);
			unit->cloak();
			break;
		// virtual bool decloak() = 0;
		case 23:
			if (m_logCmds) Broodwar->sendText("Unit:%d decloak()", unitID);
			unit->decloak();
			break;
		// virtual bool lift() = 0;
		case 24:
			if (m_logCmds) Broodwar->sendText("Unit:%d lift()", unitID);
			unit->lift();
			break;
		// virtual bool land(TilePosition position) = 0;
		case 25:
			if (m_logCmds) Broodwar->sendText("Unit:%d land(%d, %d)", unitID, arg0, arg1);
			unit->land(getTilePosition(arg0, arg1));
			break;
		// virtual bool load(Unit* target) = 0;
		case 26:
			if (getUnit(arg0) == NULL) {
				Broodwar->sendText("Invalid Command, Unit:%d load(%d)", unitID, arg0);
			}
			else {
				if (m_logCmds) Broodwar->sendText("Unit:%d load(%d)", unitID, arg0);
				unit->load(getUnit(arg0));
			}
			break;
		// virtual bool unload(Unit* target) = 0;
		case 27:
			if (getUnit(arg0) == NULL) {
				Broodwar->sendText("Invalid Command, Unit:%d unload(%d)", unitID, arg0);
			}
			else {
				if (m_logCmds) Broodwar->sendText("Unit:%d unload(%d)", unitID, arg0);
				unit->unload(getUnit(arg0));
			}
			break;
		// virtual bool unloadAll() = 0;
		case 28:
			if (m_logCmds) Broodwar->sendText("Unit:%d unloadAll()", unitID);
			unit->unloadAll();
			break;
		// virtual bool unloadAll(Position position) = 0;
		case 29:
			if (m_logCmds) Broodwar->sendText("Unit:%d unloadAll(%d, %d)", unitID, arg0, arg1);
			unit->unloadAll(getPosition(arg0, arg1));
			break;
		// virtual bool cancelConstruction() = 0;
		case 30:
			if (m_logCmds) Broodwar->sendText("Unit:%d cancelConstruction()", unitID);
			unit->cancelConstruction();
			break;
		// virtual bool haltConstruction() = 0;
		case 31:
			if (m_logCmds) Broodwar->sendText("Unit:%d haltConstruction()", unitID);
			unit->haltConstruction();
			break;
		// virtual bool cancelMorph() = 0;
		case 32:
			if (m_logCmds) Broodwar->sendText("Unit:%d cancelMorph()", unitID);
			unit->cancelMorph();
			break;
		// virtual bool cancelTrain() = 0;
		case 33:
			if (m_logCmds) Broodwar->sendText("Unit:%d cancelTrain()", unitID);
			unit->cancelTrain();
			break;
		// virtual bool cancelTrain(int slot) = 0;
		case 34:
			if (m_logCmds) Broodwar->sendText("Unit:%d cancelTrain(%d)", unitID, arg0);
			unit->cancelTrain(arg0);
			break;
		// virtual bool cancelAddon() = 0;
		case 35:
			if (m_logCmds) Broodwar->sendText("Unit:%d cancelAddon()", unitID);
			unit->cancelAddon();
			break;
		// virtual bool cancelResearch() = 0;
		case 36:
			if (m_logCmds) Broodwar->sendText("Unit:%d cancelResearch()", unitID);
			unit->cancelResearch();
			break;
		// virtual bool cancelUpgrade() = 0;
		case 37:
			if (m_logCmds) Broodwar->sendText("Unit:%d cancelUpgrade()", unitID);
			unit->cancelUpgrade();
			break;
		// virtual bool useTech(TechType tech) = 0;
		case 38:
			if (getTechType(arg0) == NULL) {
				Broodwar->sendText("Invalid Command, Unit:%d useTech(%d)", unitID, arg0);
			}
			else {
				if (m_logCmds) Broodwar->sendText("Unit:%d useTech(%d)", unitID, arg0);
				unit->useTech(getTechType(arg0));
			}
			break;
		// virtual bool useTech(TechType tech, Position position) = 0;
		case 39:
			if (getTechType(arg0) == NULL) {
				Broodwar->sendText("Invalid Command, Unit:%d useTech(%d, %d, %d)", unitID, arg0, arg1, arg2);
			}
			else {
				if (m_logCmds) Broodwar->sendText("Unit:%d useTech(%d, %d, %d)", unitID, arg0, arg1, arg2);
				unit->useTech(getTechType(arg0), getPosition(arg1, arg2));
			}
			break;
		// virtual bool useTech(TechType tech, Unit* target) = 0;
		case 40:
			if (getTechType(arg0) == NULL || getUnit(arg1) == NULL) {
				Broodwar->sendText("Invalid Command, Unit:%d useTech(%d, %d)", unitID, arg0, arg1);
			}
			else {
				if (m_logCmds) Broodwar->sendText("Unit:%d useTech(%d, %d)", unitID, arg0, arg1);
				unit->useTech(getTechType(arg0), getUnit(arg1));
			}
			break;
		default:
			break;
	}
}

/**
 * Returns the unit based on the unit ID
 */
Unit* StarCraftConnector::getUnit(int unitID)
{
	return m_unitIDMap[unitID];
}

/** 
 * Returns the unit type from its identifier
 */
UnitType StarCraftConnector::getUnitType(int type) 
{
	return m_typeMap[type];
}

/** 
 * Returns the tech type from its identifier
 */
TechType StarCraftConnector::getTechType(int type) 
{
	return m_techMap[type];
}

/** 
 * Returns the upgrade type from its identifier
 */
UpgradeType StarCraftConnector::getUpgradeType(int type)
{
	return m_upgradeMap[type];
}

/**
 * Utility function for constructing a Position.
 *
 * Note: positions are in pixel coordinates, while the inputs are given in tile coordinates
 */
Position StarCraftConnector::getPosition(int x, int y)
{
	return BWAPI::Position(32*x, 32*y);
}

/**
 * Utility function for constructing a TilePosition.
 *
 * Note: not sure if this is correct, is there a way to get a tile position
 *       object from the api rather than create a new one?
 */
TilePosition StarCraftConnector::getTilePosition(int x, int y)
{
	return BWAPI::TilePosition(x, y);
}




DWORD WINAPI AnalyzeThread()
{
  Broodwar->printf("test before   BWTA::analyze(); ");
  BWTA::analyze();
  Broodwar->printf("test after analyze(); !!!");

  //self start location only available if the map has base locations
  if (BWTA::getStartLocation(BWAPI::Broodwar->self())!=NULL)
  {
    home       = BWTA::getStartLocation(BWAPI::Broodwar->self())->getRegion();
  }
  //enemy start location only available if Complete Map Information is enabled.
  if (BWTA::getStartLocation(BWAPI::Broodwar->enemy())!=NULL)
  {
    enemy_base = BWTA::getStartLocation(BWAPI::Broodwar->enemy())->getRegion();
  }
  analyzed   = true;
  analysis_just_finished = true;

  Broodwar->printf("test before    return 0; ");
  return 0;
}

void StarCraftConnector::drawStats()
{
  std::set<Unit*> myUnits = Broodwar->self()->getUnits();
  Broodwar->drawTextScreen(5,0,"I have %d units:",myUnits.size());
  std::map<UnitType, int> unitTypeCounts;
  for(std::set<Unit*>::iterator i=myUnits.begin();i!=myUnits.end();i++)
  {
    if (unitTypeCounts.find((*i)->getType())==unitTypeCounts.end())
    {
      unitTypeCounts.insert(std::make_pair((*i)->getType(),0));
    }
    unitTypeCounts.find((*i)->getType())->second++;
  }
  int line=1;
  for(std::map<UnitType,int>::iterator i=unitTypeCounts.begin();i!=unitTypeCounts.end();i++)
  {
    Broodwar->drawTextScreen(5,16*line,"- %d %ss",(*i).second, (*i).first.getName().c_str());
    line++;
  }
}

void StarCraftConnector::drawBullets()
{
  std::set<Bullet*> bullets = Broodwar->getBullets();
  for(std::set<Bullet*>::iterator i=bullets.begin();i!=bullets.end();i++)
  {
    Position p=(*i)->getPosition();
    double velocityX = (*i)->getVelocityX();
    double velocityY = (*i)->getVelocityY();
    if ((*i)->getPlayer()==Broodwar->self())
    {
      Broodwar->drawLineMap(p.x(),p.y(),p.x()+(int)velocityX,p.y()+(int)velocityY,Colors::Green);
      Broodwar->drawTextMap(p.x(),p.y(),"\x07%s",(*i)->getType().getName().c_str());
    }
    else
    {
      Broodwar->drawLineMap(p.x(),p.y(),p.x()+(int)velocityX,p.y()+(int)velocityY,Colors::Red);
      Broodwar->drawTextMap(p.x(),p.y(),"\x06%s",(*i)->getType().getName().c_str());
    }
  }
}

void StarCraftConnector::drawVisibilityData()
{
  for(int x=0;x<Broodwar->mapWidth();x++)
  {
    for(int y=0;y<Broodwar->mapHeight();y++)
    {
      if (Broodwar->isExplored(x,y))
      {
        if (Broodwar->isVisible(x,y))
          Broodwar->drawDotMap(x*32+16,y*32+16,Colors::Green);
        else
          Broodwar->drawDotMap(x*32+16,y*32+16,Colors::Blue);
      }
      else
        Broodwar->drawDotMap(x*32+16,y*32+16,Colors::Red);
    }
  }
}

void StarCraftConnector::drawTerrainData()
{
  //we will iterate through all the base locations, and draw their outlines.
  for(std::set<BWTA::BaseLocation*>::const_iterator i=BWTA::getBaseLocations().begin();i!=BWTA::getBaseLocations().end();i++)
  {
    TilePosition p=(*i)->getTilePosition();
    Position c=(*i)->getPosition();

    //draw outline of center location
    Broodwar->drawBox(CoordinateType::Map,p.x()*32,p.y()*32,p.x()*32+4*32,p.y()*32+3*32,Colors::Blue,false);

    //draw a circle at each mineral patch
    for(std::set<BWAPI::Unit*>::const_iterator j=(*i)->getStaticMinerals().begin();j!=(*i)->getStaticMinerals().end();j++)
    {
      Position q=(*j)->getInitialPosition();
      Broodwar->drawCircle(CoordinateType::Map,q.x(),q.y(),30,Colors::Cyan,false);
    }

    //draw the outlines of vespene geysers
    for(std::set<BWAPI::Unit*>::const_iterator j=(*i)->getGeysers().begin();j!=(*i)->getGeysers().end();j++)
    {
      TilePosition q=(*j)->getInitialTilePosition();
      Broodwar->drawBox(CoordinateType::Map,q.x()*32,q.y()*32,q.x()*32+4*32,q.y()*32+2*32,Colors::Orange,false);
    }

    //if this is an island expansion, draw a yellow circle around the base location
    if ((*i)->isIsland())
      Broodwar->drawCircle(CoordinateType::Map,c.x(),c.y(),80,Colors::Yellow,false);
  }

  //we will iterate through all the regions and draw the polygon outline of it in green.
  for(std::set<BWTA::Region*>::const_iterator r=BWTA::getRegions().begin();r!=BWTA::getRegions().end();r++)
  {
    BWTA::Polygon p=(*r)->getPolygon();
    for(int j=0;j<(int)p.size();j++)
    {
      Position point1=p[j];
      Position point2=p[(j+1) % p.size()];
      Broodwar->drawLine(CoordinateType::Map,point1.x(),point1.y(),point2.x(),point2.y(),Colors::Green);
    }
  }

  //we will visualize the chokepoints with red lines
  for(std::set<BWTA::Region*>::const_iterator r=BWTA::getRegions().begin();r!=BWTA::getRegions().end();r++)
  {
    for(std::set<BWTA::Chokepoint*>::const_iterator c=(*r)->getChokepoints().begin();c!=(*r)->getChokepoints().end();c++)
    {
      Position point1=(*c)->getSides().first;
      Position point2=(*c)->getSides().second;
      Broodwar->drawLine(CoordinateType::Map,point1.x(),point1.y(),point2.x(),point2.y(),Colors::Red);
    }
  }
}

void StarCraftConnector::showPlayers()
{
  std::set<Player*> players=Broodwar->getPlayers();
  for(std::set<Player*>::iterator i=players.begin();i!=players.end();i++)
  {
    Broodwar->printf("Player [%d]: %s is in force: %s",(*i)->getID(),(*i)->getName().c_str(), (*i)->getForce()->getName().c_str());
  }
}

void StarCraftConnector::showForces()
{
  std::set<Force*> forces=Broodwar->getForces();
  for(std::set<Force*>::iterator i=forces.begin();i!=forces.end();i++)
  {
    std::set<Player*> players=(*i)->getPlayers();
    Broodwar->printf("Force %s has the following players:",(*i)->getName().c_str());
    for(std::set<Player*>::iterator j=players.begin();j!=players.end();j++)
    {
      Broodwar->printf("  - Player [%d]: %s",(*j)->getID(),(*j)->getName().c_str());
    }
  }
}
