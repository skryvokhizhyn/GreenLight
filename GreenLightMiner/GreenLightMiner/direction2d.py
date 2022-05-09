import math


class Direction2d:
    def __init__(self, x: float = 0, y: float = 0) -> None:
        self.__x = x
        self.__y = y

    @property
    def x(self) -> float:
        return self.__x

    @property
    def y(self) -> float:
        return self.__y

    def is_valid(self) -> bool:
        return not math.isclose(self.__x, 0, abs_tol=1e-6, rel_tol=1e-6) or not math.isclose(self.__y, 0, abs_tol=1e-6, rel_tol=1e-6)
