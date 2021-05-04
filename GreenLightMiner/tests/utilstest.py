import unittest

import utils


class UtilsTest_remove_all_none_from_list(unittest.TestCase):
    def test_empty(self):
        rt = []

        utils.remove_all_none_from_list(rt)
        self.assertEqual(0, len(rt))

    def test_one(self):
        rt = [1]

        utils.remove_all_none_from_list(rt)
        self.assertEquals([1], rt)

    def test_no_none(self):
        rt = [1, 2, "sdf"]

        utils.remove_all_none_from_list(rt)
        self.assertEqual(3, len(rt))
        self.assertEqual(1, rt[0])
        self.assertEqual(2, rt[1])
        self.assertEqual("sdf", rt[2])

    def test_all_none(self):
        rt = [None, None, None]

        utils.remove_all_none_from_list(rt)
        self.assertEqual(0, len(rt))

    def test_with_none(self):
        rt = [
            None,
            None,
            123,
            None,
            None,
            "a",
            None,
            None,
            None
        ]

        utils.remove_all_none_from_list(rt)
        self.assertEqual(2, len(rt))
        self.assertEqual(123, rt[0])
        self.assertEqual("a", rt[1])

    def test_remove_second_from_two(self):
        rt = ["Val", None]

        utils.remove_all_none_from_list(rt)
        self.assertEqual(1, len(rt))
