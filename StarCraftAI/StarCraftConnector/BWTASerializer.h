#pragma once

#include <BWTA.h>

using namespace std;
using namespace BWTA;

/// Collection of static methods for serializing BWAPI data to be passed across a wire.
class BWTASerializer
{
public:
	static string GetBaseLocations();
};
