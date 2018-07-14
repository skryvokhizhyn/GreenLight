#include "DBManager.h"

#include <sstream>

using namespace glm;

namespace
{
	uint16_t FillGpsLocationsFromStatement(DBQueryResult & res, GpsLocations & buffer)
	{
		uint16_t rows = 0;

		while (res.Next())
		{
			auto lat = res.Get<double>(0);
			auto lon = res.Get<double>(1);
			auto alt = res.Get<double>(2);
			auto spe = res.Get<float>(3);

			buffer.push_back({ lat, lon, alt, spe });

			++rows;
		}

		return rows;
	}
}

DBManager::DBManager(SqliteHandler & handler)
	: m_hanler(handler)
{
}

uint16_t DBManager::GetGpsLocations(GpsLocations & buffer) const
{
	auto res = m_hanler.Execute("select latitude, longitude, altitude, speed from GpsLocation");

	return FillGpsLocationsFromStatement(res, buffer);
}

uint16_t DBManager::GetGpsLocationsFromTo(uint16_t startFrom, uint16_t cnt, GpsLocations & buffer) const
{
	if (cnt == 0)
	{
		return 0;
	}

	buffer.reserve(buffer.size() + cnt);

	std::stringstream s;
	s << "select longitude, latitude, altitude, speed from GpsLocation where rowid between " 
		<< std::to_string(startFrom) << " and " << std::to_string(startFrom + cnt - 1);
	auto res = m_hanler.Execute(s.str());

	return FillGpsLocationsFromStatement(res, buffer);
}
