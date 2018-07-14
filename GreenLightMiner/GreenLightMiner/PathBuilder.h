#pragma once

#include "Point3.h"

namespace glm
{
	struct PathRange
	{
		Point3dVector::const_iterator begin;
		Point3dVector::const_iterator end;
	};

	struct PathBuilderResult
	{
		using Ranges = std::vector<PathRange>;
		Ranges ranges;
		uint16_t skipped = 0;
	};

	PathBuilderResult GeneratePath(const Point3dVector & points, double tolerance, uint16_t minSeriesLength);
}
