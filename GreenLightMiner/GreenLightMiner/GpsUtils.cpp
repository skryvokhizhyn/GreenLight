#include "GpsUtils.h"

const double glm::EARTH_RADIUS_METERS = 6'378'100.0;

glm::Point3d glm::GpsLocationToCoordinate(const glm::GpsLocation & loc)
{
	static const double pi = std::acos(-1);

	const double latRad = (loc.latitude * pi) / 180.0;
	const double lonRad = (loc.longitude * pi) / 180.0;

	const double cosLat = std::cos(latRad);

	return 
	{ 
		EARTH_RADIUS_METERS * cosLat * std::cos(lonRad),
		EARTH_RADIUS_METERS * cosLat * std::sin(lonRad),
		EARTH_RADIUS_METERS * std::sin(latRad) + loc.altitude
	};
}
