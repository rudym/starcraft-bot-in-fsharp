#include "BWApiSerializer.h"

// NOTE: This code makes HEAVY use of building up immutable
// strings. This is crazy inefficent. Please find and replace
// with any STL string builder type.

#include <sstream>

string toString(int value);
string toString(bool value);

string BWApiSerializer::GetPlayerInfo()
{
	string playerInfo("PlayerInfo;");
	playerInfo += toString(Broodwar->self()->getID());
	playerInfo += ",";
	playerInfo += toString(Broodwar->self()->getRace().getID());
	playerInfo += ",";
	playerInfo += toString(Broodwar->enemy()->getID());	
	playerInfo += ",";
	playerInfo += toString(Broodwar->enemy()->getRace().getID());	
	
	return playerInfo;
}

string BWApiSerializer::GetUnitTypes()
{
	string unitTypes(";UnitTypes;");

	set<UnitType> types = UnitTypes::allUnitTypes();
	for(set<UnitType>::iterator i = types.begin(); i != types.end(); i++)
	{
		int id = i->getID();
		string race = i->getRace().getName();
		string name = i->getName();
		int minerals = i->mineralPrice();
		int gas = i->gasPrice();
		int hitPoints = i->maxHitPoints()/256;
		int shields = i->maxShields();
		int energy = i->maxEnergy();
		int buildTime = i->buildTime();
		bool canAttack = i->canAttack();
		bool canMove = i->canMove();
		int width = i->tileWidth();
		int height = i->tileHeight();
		int supplyRequired = i->supplyRequired();
		int supplyProvided = i->supplyProvided();
		int sightRange = i->sightRange();
		int groundMaxRange = i->groundWeapon().maxRange();
		int groundMinRange = i->groundWeapon().minRange();
		int groundDamage = i->groundWeapon().damageAmount();
		int airRange = i->airWeapon().maxRange();
		int airDamage = i->airWeapon().damageAmount();
		bool isBuilding = i->isBuilding();
		bool isFlyer = i->isFlyer();
		bool isSpellCaster = i->isSpellcaster();
		bool isWorker = i->isWorker();
		bool canBuildAddon = i->canBuildAddon();
		int whatBuilds = i->whatBuilds().first.getID(); //UnitTypes::Terran_Academy.getID()

		// Encode the type as a string
		unitTypes += toString(id);
		unitTypes += ",";
		unitTypes += race;
		unitTypes += ",";
		unitTypes += name;
		unitTypes += ",";
		unitTypes += toString(minerals);
		unitTypes += ",";
		unitTypes += toString(gas);
		unitTypes += ",";
		unitTypes += toString(hitPoints);
		unitTypes += ",";
		unitTypes += toString(shields);
		unitTypes += ",";
		unitTypes += toString(energy);
		unitTypes += ",";
		unitTypes += toString(buildTime);
		unitTypes += ",";
		unitTypes += toString(canAttack);
		unitTypes += ",";
		unitTypes += toString(canMove);
		unitTypes += ",";
		unitTypes += toString(width);
		unitTypes += ",";
		unitTypes += toString(height);
		unitTypes += ",";
		unitTypes += toString(supplyRequired);
		unitTypes += ",";
		unitTypes += toString(supplyProvided);
		unitTypes += ",";
		unitTypes += toString(sightRange);
		unitTypes += ",";
		unitTypes += toString(groundMaxRange);
		unitTypes += ",";
		unitTypes += toString(groundMinRange);
		unitTypes += ",";
		unitTypes += toString(groundDamage);
		unitTypes += ",";
		unitTypes += toString(airRange);
		unitTypes += ",";
		unitTypes += toString(airDamage);
		unitTypes += ",";
		unitTypes += toString(isBuilding);
		unitTypes += ",";
		unitTypes += toString(isFlyer);
		unitTypes += ",";
		unitTypes += toString(isSpellCaster);
		unitTypes += ",";
		unitTypes += toString(isWorker);
		unitTypes += ",";
		unitTypes += toString(canBuildAddon);
		unitTypes += ",";
		unitTypes += toString(whatBuilds);

		// To seperate each unit
		unitTypes += "|";
	}

	return unitTypes;
}

string BWApiSerializer::GetStartingLocations()
{
	string locations(";StartLocations;");

	set<TilePosition> startSpots = Broodwar->getStartLocations();
	for(set<TilePosition>::iterator i = startSpots.begin(); i != startSpots.end(); i++)
	{
		int x = i->x();
		int y = i->y();

		locations += toString(x);
		locations += ",";
		locations += toString(y);

		locations += "|";
	}

	return locations;
}

string BWApiSerializer::GetMapHeader()
{
	string mapHeader (";MapHeader;");
	mapHeader+= Broodwar->mapName();

	int mapWidth  = Broodwar->mapWidth();
	int mapHeight = Broodwar->mapHeight();

	mapHeader += ",";
	mapHeader += toString(mapWidth);
	mapHeader += ",";
	mapHeader += toString(mapHeight);

	return mapHeader;
}

string BWApiSerializer::GetMapData()
{
	string mapData (";MapData;");

	int mapWidth  = Broodwar->mapWidth();
	int mapHeight = Broodwar->mapHeight();

	for (int y = 0; y < mapHeight; y++) {	
		for (int x = 0; x < mapWidth; x++) {

			// char 0: height
			int h = Broodwar->getGroundHeight(4*x, 4*y);
			mapData += toString(h);

			// char 1: buildable
			if (Broodwar->isBuildable(x, y)) 
			{
				mapData += toString(1);
			}
			else { 
				mapData += toString(0);
			}

			// char 2: walkable
			if (Broodwar->isWalkable(4*x, 4*y)) 
			{
				mapData += toString(1);		
			}
			else {
				mapData += toString(0);
			}
		}
	}

	return mapData;
}

string BWApiSerializer::GetTechTypes()
{
	string techTypes(";TechTypes;");

	set<TechType> tektypes = TechTypes::allTechTypes();
	for(set<TechType>::iterator i = tektypes.begin(); i != tektypes.end(); i++)
	{
		int id = i->getID();
		string name = i->getName();
		// unit type id of what researches it
		int whatResearchesID = i->whatResearches().getID(); 
		int mins = i->mineralPrice();
		int gas = i->gasPrice();

		techTypes += toString(id);
		techTypes += ",";
		techTypes += name;
		techTypes += ",";
		techTypes += toString(whatResearchesID);
		techTypes += ",";
		techTypes += toString(mins);
		techTypes += ",";
		techTypes += toString(gas);
		
		techTypes += "|";
	}

	return techTypes;
}

string BWApiSerializer::GetUpgradeTypes()
{
	string upgradeTypes(";UpgradeTypes;");

	set<UpgradeType> upTypes = UpgradeTypes::allUpgradeTypes();
	for(set<UpgradeType>::iterator i = upTypes.begin(); i != upTypes.end(); i++)
	{
		int id = i->getID();
		string name = i->getName();
		int whatUpgradesID = i->whatUpgrades().getID(); // unit type id of what researches it
		int repeats = i->maxRepeats();
		int minBase = i->mineralPrice();
		int minFactor = i->mineralPriceFactor();
		int gasBase = i->gasPriceFactor();
		int gasFactor = i->gasPriceFactor();

		upgradeTypes += toString(id);
		upgradeTypes += ",";
		upgradeTypes += name;
		upgradeTypes += ",";
		upgradeTypes += toString(whatUpgradesID);
		upgradeTypes += ",";
		upgradeTypes += toString(repeats);
		upgradeTypes += ",";
		upgradeTypes += toString(minBase);
		upgradeTypes += ",";
		upgradeTypes += toString(minFactor);
		upgradeTypes += ",";
		upgradeTypes += toString(gasBase);
		upgradeTypes += ",";
		upgradeTypes += toString(gasFactor);

		upgradeTypes += "|";
	}

	return upgradeTypes;
}

/**
 * Utiliity function for int to string conversion.
 */
string toString(int value) 
{
	stringstream ss;
	ss << value;
	return ss.str();
}

/**
 * Utiliity function for bool to string conversion.
 */
string toString(bool value) 
{
	if (value) return std::string("1");
	else return std::string("0");
}