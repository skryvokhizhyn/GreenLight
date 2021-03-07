from kivy.graphics import Point
from kivy.uix.widget import Widget
from kivy.app import App
from kivy.core.window import Window as AppWindow
from typing import List

import route
from xyminmax import XYMinMax

_WHITE_COLOR = [255, 255, 255, 255]
_VIEW_WIDTH = 1200
_VIEW_HEIGHT = 1200


class _Window(Widget):
    def __init__(self, xy_bounds: XYMinMax, **kwargs):
        super(_Window, self).__init__(**kwargs)
        self.__min_max = xy_bounds

    def add_points(self, rt: route.XyzRoute) -> None:
        x_diff = self.__min_max.x_max - self.__min_max.x_min
        y_diff = self.__min_max.y_max - self.__min_max.y_min

        scale = _VIEW_WIDTH / x_diff if (x_diff > y_diff) else _VIEW_HEIGHT / y_diff

        pts = []
        for p in rt:
            pts.append((p.x - self.__min_max.x_min) * scale)
            pts.append((p.y - self.__min_max.y_min) * scale)

        with self.canvas:
            Point(pointsize=0.5, points=pts)


class RoutesVisualizer(App):
    def __init__(self, xy_bounds: XYMinMax):
        AppWindow.size = (_VIEW_WIDTH, _VIEW_HEIGHT)
        App.__init__(self)
        self.__window = _Window(xy_bounds)

    def add_points(self, rt: route.XyzRoute) -> None:
        self.__window.add_points(rt)

    def build(self):
        return self.__window
