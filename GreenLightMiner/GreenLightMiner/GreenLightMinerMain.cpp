#include "SqliteHandler.h"
#include "DBManager.h"
#include "SDLWrapper.h"

#include "Point3.h"
#include "Point2.h"
#include "GpsUtils.h"
#include "PointUtils.h"

#include <boost/range/algorithm/transform.hpp>

#include <iostream>
#include <execution>
#include <algorithm>
#include <exception>

using namespace glm;

int main(int, char**)
{
	int ret = 0;

	try
	{
		SqliteHandler sh("db_green_light_tracker.db");

		DBManager dbm(sh);

		GpsLocations buff;
		dbm.GetGpsLocations(buff);

		if (buff.empty())
			throw std::runtime_error("No gps locations provided");

		Point3dVector filteredCoordinates;

		{
			Point3dVector coordinates;
			coordinates.reserve(buff.size());

			boost::transform(buff, std::back_inserter(coordinates), &glm::GpsLocationToCoordinate);

			filteredCoordinates = FilterTooClosePoints(coordinates, 100.0);
		}

		const auto xs = std::minmax_element(std::begin(filteredCoordinates), std::end(filteredCoordinates),
			[](const auto & gl, const auto & gr) { return gl.x < gr.x; });
		const auto ys = std::minmax_element(std::begin(filteredCoordinates), std::end(filteredCoordinates),
			[](const auto & gl, const auto & gr) { return gl.y < gr.y; });

		const auto xMin = xs.first->x;
		const auto yMin = ys.first->y;

		const double xDiff = xs.second->x - xs.first->x;
		const double yDiff = ys.second->y - ys.first->y;
		
		const double scale = (xDiff > yDiff) ? SDLWrapper::SCREEN_WIDTH / xDiff : SDLWrapper::SCREEN_HEIGHT;

		Point2iVector points;
		points.reserve(filteredCoordinates.size());

		boost::transform(filteredCoordinates, std::back_inserter(points), [=](auto c)
		{
			Point2i p
			{ 
				static_cast<int>((c.x - xMin) * scale), 
				static_cast<int>((c.y - yMin) * scale)
			};
			
			return p;
		});

		SDLWrapper s;
		s.Draw(points);
	}
	catch (const std::exception & e)
	{
		std::cout << e.what() << std::endl;
		ret = 1;
	}

	//std::cin.get();
	return ret;
}