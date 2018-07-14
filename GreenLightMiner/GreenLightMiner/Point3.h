#pragma once

#include <vector>

namespace glm
{
	template<typename T>
	struct Point3
	{
		T x = T();
		T y = T();
		T z = T();
	};

	using Point3d = Point3<double>;

	using Point3dVector = std::vector<Point3d>;
}
