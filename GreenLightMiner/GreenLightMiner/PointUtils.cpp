#include "PointUtils.h"

double glm::GetDistance(const Point2d & l, const Point2d & r)
{
	return std::sqrt(std::pow(l.x - r.x, 2) + std::pow(l.y - r.y, 2));
}

double glm::GetDistance(const Point3d & l, const Point3d & r)
{
	return std::sqrt(std::pow(l.x - r.x, 2) + std::pow(l.y - r.y, 2) + std::pow(l.z - r.z, 2));
}

glm::Point3dVector glm::FilterTooClosePoints(const Point3dVector & points, double tolerance)
{
	if (points.empty())
		return Point3dVector{};

	std::vector<double> diffs;
	diffs.reserve(points.size());

	auto curr = points.begin();
	auto prev = curr++;

	Point3dVector result;
	result.reserve(points.size());
	result.push_back(*prev);
	
	double totalDist = 0.0;
	bool hasSkippedPoint = false;

	while (curr != points.end())
	{
		totalDist += GetDistance(*prev, *curr);

		if (totalDist >= tolerance)
		{
			if (hasSkippedPoint)
			{
				result.push_back(*prev);
			}
			
			totalDist = 0.0;
		}
		else
		{
			hasSkippedPoint = true;
		}

		prev = curr++;
	}

	if (hasSkippedPoint)
	{
		result.push_back(*prev);
	}

	result.shrink_to_fit();

	return result;
}
