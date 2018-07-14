#pragma once

#include "Point3.h"
#include "GpsLocation.h"

namespace glm
{
	extern const double EARTH_RADIUS_METERS;

	Point3d GpsLocationToCoordinate(const GpsLocation & loc);
}
