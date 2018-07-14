#include <boost/test/unit_test.hpp>

#include <GpsUtils.h>

using namespace glm;
using namespace boost::test_tools;

BOOST_AUTO_TEST_CASE(GpsLocationToCoordinateTest1)
{
	const auto res = GpsLocationToCoordinate({ 90, 0, 10, 0 });

	BOOST_TEST(0.0 == res.x, tolerance(0.001));
	BOOST_CHECK_EQUAL(0.0, res.y);
	BOOST_CHECK_EQUAL(EARTH_RADIUS_METERS + 10, res.z);
}

BOOST_AUTO_TEST_CASE(GpsLocationToCoordinateTest2)
{
	const auto res = GpsLocationToCoordinate({ 0, 0, 10, 0 });

	BOOST_CHECK_EQUAL(EARTH_RADIUS_METERS + 10, res.x);
	BOOST_CHECK_EQUAL(0.0, res.y);
	BOOST_TEST(0.0 == res.z, tolerance(0.001));
}

BOOST_AUTO_TEST_CASE(GpsLocationToCoordinateTest3)
{
	const auto res = GpsLocationToCoordinate({ 0, 90, 10, 0 });

	BOOST_TEST(0.0 == res.x, tolerance(0.001));
	BOOST_CHECK_EQUAL(EARTH_RADIUS_METERS + 10, res.y);
	BOOST_CHECK_EQUAL(0.0, res.z);
}