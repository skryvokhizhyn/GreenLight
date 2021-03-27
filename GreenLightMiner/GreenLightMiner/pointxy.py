class PointXy:
    def __init__(self, x: float, y: float):
        self.__x = x
        self.__y = y

    def __eq__(self, other: object) -> bool:
        if not isinstance(other, PointXy):
            return NotImplemented

        return self.__x == other.__x and self.__y == other.__y

    @property
    def x(self) -> float:
        return self.__x

    @property
    def y(self) -> float:
        return self.__y
