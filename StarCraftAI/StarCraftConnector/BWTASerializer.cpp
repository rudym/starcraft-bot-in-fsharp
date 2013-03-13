#include "BWTASerializer.h"

// NOTE: This code makes HEAVY use of building up immutable
// strings. This is crazy inefficent. Please find and replace
// with any STL string builder type.
// mb stl:rope http://stackoverflow.com/questions/6834343/ultra-quick-way-to-concatenate-byte-values

#include <sstream>

string toString(int value);
string toString(bool value);

string BWTASerializer::GetBaseLocations()
{
	string baseLocations(";BaseLocations;");

	set<BaseLocation*> bloc = BWTA::getBaseLocations();
	for(set<BaseLocation*>::iterator i = bloc.begin(); i != bloc.end(); i++)
	{		
		int isStartLocation = (*i)->isStartLocation();
		int gas = (*i)->gas();
		int minerals = (*i)->minerals();

		// Encode the type as a string
		baseLocations += toString(isStartLocation);
		baseLocations += ",";
		baseLocations += toString(gas);
		baseLocations += ",";
		baseLocations += toString(minerals);

		// To seperate each unit
		baseLocations += "|";
	}

	return baseLocations;
}