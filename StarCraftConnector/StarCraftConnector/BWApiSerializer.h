#pragma once

#include <BWAPI.h>

using namespace std;
using namespace BWAPI;

/// Collection of static methods for serializing BWAPI data to be passed across a wire.
class BWApiSerializer
{
public:
	static string GetPlayerInfo();
	static string GetUnitTypes();
	static string GetStartingLocations();
	static string GetMapHeader();
	static string GetMapData();
	static string GetTechTypes();
	static string GetUpgradeTypes();
};
