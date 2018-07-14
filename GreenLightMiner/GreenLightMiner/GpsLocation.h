#pragma once

#include <vector>

namespace glm
{
	struct GpsLocation
	{
		double latitude = 0.0;
		double longitude = 0.0;
		double altitude = 0.0;
		float speed = 0.0f;
	};

	using GpsLocations = std::vector<GpsLocation>;
}
