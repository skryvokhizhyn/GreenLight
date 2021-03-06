from kivy.graphics import Point, Scale, Translate
from kivy.uix.widget import Widget
from kivy.app import App
from kivy.core.window import Window as AppWindow
from kivy.input.motionevent import MotionEvent
from typing import List

import route
from xyminmax import XYMinMax

_WHITE_COLOR = [255, 255, 255, 255]
_VIEW_WIDTH = 1200
_VIEW_HEIGHT = 1200
_VIEW_SCROLL_ADD = 0.1


class _Window(Widget):
    def __init__(self, xy_bounds: XYMinMax, **kwargs):
        super(_Window, self).__init__(**kwargs)
        self.__min_max = xy_bounds
        self.__zoom_scale = 1.0
        self.__shift_x = 0.0
        self.__shift_y = 0.0

        self.__rts: List[route.XyzRoute] = []

    def add_points(self, rt: route.XyzRoute) -> None:
        x_diff = self.__min_max.x_max - self.__min_max.x_min
        y_diff = self.__min_max.y_max - self.__min_max.y_min

        scale = _VIEW_WIDTH / x_diff if (x_diff > y_diff) else _VIEW_HEIGHT / y_diff

        pts: route.XyzRoute = []
        for p in rt:
            pts.append((p.x - self.__min_max.x_min) * scale)
            pts.append((p.y - self.__min_max.y_min) * scale)

        self.__rts.append(pts)

    def draw_routes(self, *args) -> None:
        self.canvas.clear()

        with self.canvas:
            Translate((_VIEW_WIDTH - _VIEW_WIDTH * self.__zoom_scale) / 2, (_VIEW_HEIGHT - _VIEW_HEIGHT * self.__zoom_scale) / 2, 0)
            Scale(self.__zoom_scale, self.__zoom_scale, 1)
            Translate(self.__shift_x / self.__zoom_scale, self.__shift_y / self.__zoom_scale, 0)
            for pts in self.__rts:
                Point(pointsize=0.5 / self.__zoom_scale, points=pts)

    def on_touch_down(self, evt: MotionEvent) -> None:
        if evt.is_mouse_scrolling:
            shift = _VIEW_SCROLL_ADD if evt.button == "scrolldown" else -_VIEW_SCROLL_ADD
            if self.__zoom_scale + shift >= 1:
                self.__zoom_scale += shift
            self.draw_routes()
        else:
            evt.grab(self)

    def on_touch_move(self, evt: MotionEvent) -> None:
        if evt.grab_current is self:
            self.__shift_x += evt.dx
            self.__shift_y += evt.dy
            self.draw_routes()

    def on_touch_up(self, evt: MotionEvent) -> None:
        if evt.grab_current is self:
            evt.ungrab(self)


class RoutesVisualizer(App):
    def __init__(self, xy_bounds: XYMinMax):
        AppWindow.size = (_VIEW_WIDTH, _VIEW_HEIGHT)
        App.__init__(self)
        self.__window = _Window(xy_bounds)

    def add_points(self, rt: route.XyzRoute) -> None:
        self.__window.add_points(rt)

    def build(self):
        self.__window.draw_routes()
        return self.__window
