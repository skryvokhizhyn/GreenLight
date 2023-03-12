from typing import List

from kivy.app import App
from kivy.core.window import Window as AppWindow
from kivy.graphics import Color, Point, Scale, Translate, Rectangle
from kivy.input.motionevent import MotionEvent
from kivy.uix.widget import Widget
from kivy.uix.floatlayout import FloatLayout
from kivy.core.window import Window
from pointxyz import XyzRoute, XyzRoutes, PointXyz
from xyminmax import XYMinMax

_VIEW_SCROLL_ADD = 0.1


class _Window(Widget):
    def __init__(self, xy_bounds: XYMinMax, **kwargs):
        super(_Window, self).__init__(**kwargs)

        Window.bind(on_key_down=self._on_key_pressed)

        self.__min_max = xy_bounds
        self.__zoom_scale = 1.0
        self.__shift_x = 0.0
        self.__shift_y = 0.0

        self.__rts: XyzRoutes = []

        self.__points = []

        #self.size = (_WINDOW_WIDTH, _WINDOW_HEIGHT)

    def add_points(self, rt: XyzRoute) -> None:
        x_diff = self.__min_max.x_max - self.__min_max.x_min
        y_diff = self.__min_max.y_max - self.__min_max.y_min

        scale = self.width / x_diff if (x_diff / self.width > y_diff / self.height) else self.height / y_diff

        pts: XyzRoute = []
        for p in rt:
            pts.append((p.x - self.__min_max.x_min) * scale)
            pts.append((p.y - self.__min_max.y_min) * scale)

        self.__rts.append(pts)

    def _on_key_pressed(self, *args):
        keycode = args[1]

        if keycode == 275:  # right
            self.__shift_x -= 5
            self.draw_routes()
        elif keycode == 276:  # left
            self.__shift_x += 5
            self.draw_routes()
        elif keycode == 273:  # up
            self.__shift_y -= 5
            self.draw_routes()
        elif keycode == 274:  # down
            self.__shift_y += 5
            self.draw_routes()
        elif keycode == 270 or keycode == 61:  # +
            self.__zoom_scale += _VIEW_SCROLL_ADD
            self.draw_routes()
        elif keycode == 269 or keycode == 45:  # -
            self.__zoom_scale -= _VIEW_SCROLL_ADD
            self.draw_routes()

    def draw_routes(self, *args) -> None:
        self.canvas.clear()

        colors = [
            [1, 1, 1],
            [1, 0, 0],
            [0, 1, 0],
            [0, 0, 1],
            [1, 1, 0],
            [1, 0, 1],
            [0, 1, 1]
        ]

        color_id = 0

        with self.canvas:
            Translate((self.width - self.width * self.__zoom_scale) / 2, (self.height - self.height * self.__zoom_scale) / 2, 0)
            Scale(self.__zoom_scale, self.__zoom_scale, 1)
            Translate(self.__shift_x / self.__zoom_scale, self.__shift_y / self.__zoom_scale, 0)

            for pts in self.__rts:
                color = colors[color_id]
                color_id += 1
                if color_id == len(colors):
                    color_id = 0

                Color(color[0], color[1], color[2])

                self.__points.append(Point(pointsize=0.5 / self.__zoom_scale, points=pts))

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
        App.__init__(self)
        __WINDOW_WIDTH = 800
        __WINDOW_HEIGHT = 600
        Window.size = (__WINDOW_WIDTH, __WINDOW_HEIGHT)
        self.__window = _Window(xy_bounds)
        self.__window.size = (__WINDOW_WIDTH, __WINDOW_HEIGHT)

    def add_points(self, rt: XyzRoute) -> None:
        self.__window.add_points(rt)

    def build(self):
        self.__window.draw_routes()
        return self.__window
