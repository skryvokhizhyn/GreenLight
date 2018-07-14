#include <boost/test/unit_test.hpp>

#include <PointUtils.h>

using namespace glm;
using namespace boost::test_tools;

namespace glm
{
	bool operator == (const Point3d & l, const Point3d & r)
	{
		return l.x == r.x && l.y == r.y && l.z == r.z;
	}

	std::ostream & operator << (std::ostream & o, const Point3d & p)
	{
		o << "{" << p.x << ", " << p.y << ", " << p.z << "}";
		return o;
	}
}

BOOST_AUTO_TEST_CASE(PointUtilsGetDistance2dTest1)
{
	const auto res = GetDistance
	(
		Point2d{ 0, 0 },
		Point2d{ 10, 0 }
	);

	BOOST_TEST(10.0 == res, tolerance(0.001));
}

BOOST_AUTO_TEST_CASE(PointUtilsGetDistance2dTest2)
{
	const auto res = GetDistance
	(
		Point2d{ 10, 0 },
		Point2d{ 10, 0 }
	);

	BOOST_TEST(0.0 == res, tolerance(0.001));
}

BOOST_AUTO_TEST_CASE(PointUtilsGetDistance2dTest3)
{
	const auto res = GetDistance
	(
		Point2d{ 0, 10 },
		Point2d{ 0, 0 }
	);

	BOOST_TEST(10.0 == res, tolerance(0.001));
}

BOOST_AUTO_TEST_CASE(PointUtilsGetDistance3dTest1)
{
	const auto res = GetDistance
	(
		Point3d{ 0, 0, 0 },
		Point3d{ 10, 0, 0 }
	);

	BOOST_TEST(10.0 == res, tolerance(0.001));
}

BOOST_AUTO_TEST_CASE(PointUtilsGetDistance3dTest2)
{
	const auto res = GetDistance
	(
		Point3d{ 0, 0, 10 },
		Point3d{ 0, 0, 10 }
	);

	BOOST_TEST(0.0 == res, tolerance(0.001));
}

BOOST_AUTO_TEST_CASE(PointUtilsGetDistance3dTest3)
{
	const auto res = GetDistance
	(
		Point3d{ 0, 10, 0},
		Point3d{ 0, 0, 0 }
	);

	BOOST_TEST(10.0 == res, tolerance(0.001));
}

BOOST_AUTO_TEST_CASE(PointUtilsFilterTooClosePointsTest1)
{
	Point3dVector points
	{
		Point3d{0, 0, 0},
		Point3d{1, 0, 0},
		Point3d{4, 0, 0}
	};

	auto filtered = FilterTooClosePoints(points, 2.0);

	BOOST_CHECK_EQUAL(3u, filtered.size());
	BOOST_CHECK_EQUAL(points[0], filtered[0]);
	BOOST_CHECK_EQUAL(points[1], filtered[1]);
	BOOST_CHECK_EQUAL(points[2], filtered[2]);
}

BOOST_AUTO_TEST_CASE(PointUtilsFilterTooClosePointsTest2)
{
	Point3dVector points
	{
		Point3d{ 0, 0, 0 },
		Point3d{ 1, 0, 0 },
		Point3d{ 1.5, 0, 0 },
		Point3d{ 4, 0, 0 }
	};

	auto filtered = FilterTooClosePoints(points, 2.0);

	BOOST_CHECK_EQUAL(3u, filtered.size());
	BOOST_CHECK_EQUAL(points[0], filtered[0]);
	BOOST_CHECK_EQUAL(points[2], filtered[1]);
	BOOST_CHECK_EQUAL(points[3], filtered[2]);
}

BOOST_AUTO_TEST_CASE(PointUtilsFilterTooClosePointsTest3)
{
	Point3dVector points
	{
		Point3d{ 0, 0, 0 },
		Point3d{ 1, 0, 0 }
	};

	auto filtered = FilterTooClosePoints(points, 2.0);

	BOOST_CHECK_EQUAL(2u, filtered.size());
	BOOST_CHECK_EQUAL(points[0], filtered[0]);
	BOOST_CHECK_EQUAL(points[1], filtered[1]);
}

BOOST_AUTO_TEST_CASE(PointUtilsFilterTooClosePointsTest4)
{
	Point3dVector points{ Point3d{ 0, 0, 0 } };

	auto filtered = FilterTooClosePoints(points, 2.0);

	BOOST_CHECK_EQUAL(1u, filtered.size());
	BOOST_CHECK_EQUAL(points[0], filtered[0]);
}

BOOST_AUTO_TEST_CASE(PointUtilsFilterTooClosePointsTest5)
{
	Point3dVector points{};

	auto filtered = FilterTooClosePoints(points, 2.0);

	BOOST_CHECK_EQUAL(0u, filtered.size());
}
