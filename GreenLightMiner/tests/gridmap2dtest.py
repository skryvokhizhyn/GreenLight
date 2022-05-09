import unittest
from GreenLightMiner.gridmap2d import GridMap2d, PointXy


class GridMap2dTest(unittest.TestCase):
    def test_empty_get(self):
        grid = GridMap2d(1)
        self.assertEquals([], grid.get(PointXy(0, 0), 2))

    def test_get_all(self):
        grid = GridMap2d(100)
        grid.put(PointXy(10, 10), 1)
        grid.put(PointXy(11, 11), 2)
        self.assertEquals([1, 2], grid.get(PointXy(12, 12), 2))

    def test_get_in_cell_only(self):
        grid = GridMap2d(10)
        grid.put(PointXy(10, 10), 1)
        grid.put(PointXy(11, 11), 2)
        grid.put(PointXy(0, 1), 3)
        grid.put(PointXy(0, 0), 4)
        self.assertEquals([1, 2], grid.get(PointXy(12, 12), 2))

    def test_get_in_surraunding_cells(self):
        grid = GridMap2d(10)
        grid.put(PointXy(10, 10), 1)
        grid.put(PointXy(11, 11), 2)
        grid.put(PointXy(0, 1), 3)
        grid.put(PointXy(0, 0), 4)
        self.assertEquals({1, 2, 3, 4}, {*grid.get(PointXy(12, 12), 3)})

    def test_get_in_big_range(self):
        grid = GridMap2d(2)
        grid.put(PointXy(1, 1), 1)
        grid.put(PointXy(2, 2), 2)
        grid.put(PointXy(3, 3), 3)
        grid.put(PointXy(4, 4), 4)
        grid.put(PointXy(5, 5), 5)
        grid.put(PointXy(6, 6), 6)
        self.assertEquals({1, 2, 3, 4, 5, 6}, {*grid.get(PointXy(5, 5), 10)})

    def test_get_pos(self):
        grid = GridMap2d(2)
        self.assertEqual((1, 1), grid._GridMap2d__get_pos(PointXy(3, 3)))
        self.assertEqual((-2, -2), grid._GridMap2d__get_pos(PointXy(-3, -3)))

    def test_get_pos_within_dist_one_cell(self):
        grid = GridMap2d(2)
        self.assertEqual([(1, 1)], grid._GridMap2d__get_pos_within_dist(PointXy(3, 3), 0.2))

    def test_get_pos_within_dist_multi_cell(self):
        grid = GridMap2d(2)
        self.assertEquals({(0, 1), (1, 1)}, {*grid._GridMap2d__get_pos_within_dist(PointXy(2.2, 3), 0.3)})
        self.assertEquals({(1, 1), (2, 1)}, {*grid._GridMap2d__get_pos_within_dist(PointXy(3.8, 3), 0.3)})
        self.assertEquals({(1, 1), (1, 2)}, {*grid._GridMap2d__get_pos_within_dist(PointXy(3, 3.8), 0.3)})
        self.assertEquals({(1, 0), (1, 1)}, {*grid._GridMap2d__get_pos_within_dist(PointXy(3, 2.2), 0.3)})

        self.assertEquals({(0, 0), (1, 0), (1, 1), (0, 1)}, {*grid._GridMap2d__get_pos_within_dist(PointXy(2.2, 2.2), 0.3)})
        self.assertEquals({(0, 2), (1, 1), (0, 1), (1, 2)}, {*grid._GridMap2d__get_pos_within_dist(PointXy(2.2, 3.8), 0.3)})
        self.assertEquals({(1, 1), (1, 2), (2, 1), (2, 2)}, {*grid._GridMap2d__get_pos_within_dist(PointXy(3.8, 3.8), 0.3)})
        self.assertEquals({(1, 0), (1, 1), (2, 0), (2, 1)}, {*grid._GridMap2d__get_pos_within_dist(PointXy(3.8, 2.2), 0.3)})

        self.assertEquals({(x, y) for x in range(-2, 5) for y in range(-2, 5)}, {*grid._GridMap2d__get_pos_within_dist(PointXy(3, 3), 6)})
