#pragma once

#include "GpsLocation.h"
#include "SqliteHandler.h"

#include <cstdint>

namespace glm
{
	class DBManager
	{
	public:
		DBManager(SqliteHandler & handler);

		uint16_t GetGpsLocations(GpsLocations & buffer) const;
		uint16_t GetGpsLocationsFromTo(uint16_t startFrom, uint16_t cnt, GpsLocations & buffer) const;

	private:
		SqliteHandler & m_hanler;
	};
}
