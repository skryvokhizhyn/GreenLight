#pragma once

#include "Point2.h"
#include "Point3.h"

namespace glm
{
	double GetDistance(const Point2d & l, const Point2d & r);
	double GetDistance(const Point3d & l, const Point3d & r);

	Point3dVector FilterTooClosePoints(const Point3dVector & points, double tolerance);
}
