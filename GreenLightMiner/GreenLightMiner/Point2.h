#pragma once

#include <vector>

namespace glm
{
	template<typename T>
	struct Point2
	{
		T x = T();
		T y = T();
	};

	template<typename T>
	using Point2Vector = std::vector<Point2<T>>;

	using Point2i = Point2<int>;
	using Point2d = Point2<double>;
	
	using Point2iVector = Point2Vector<int>;
	using Point2dVector = Point2Vector<double>;
}
