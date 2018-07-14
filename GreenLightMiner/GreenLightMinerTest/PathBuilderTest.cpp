#include <boost/test/unit_test.hpp>

#include <PathBuilder.h>

using namespace glm;

BOOST_AUTO_TEST_CASE(PathBuilderTest1)
{
	const Point3dVector points = { { 1, 0, 0 },{ 2, 0, 0 },{ 3, 0, 0 },{ 4, 0, 0 },{ 5, 0, 0 } };

	auto res = GeneratePath(points, 1.0, 3);

	BOOST_CHECK_EQUAL(res.ranges.size(), 1u);
	BOOST_CHECK(res.ranges[0].begin == points.begin());
	BOOST_CHECK(res.ranges[0].end == points.end());
	BOOST_CHECK_EQUAL(res.skipped, 0);
}