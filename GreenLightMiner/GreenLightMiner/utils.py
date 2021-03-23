from typing import List


def remove_all_none_from_list(rt: List) -> None:
    ln = len(rt)

    k = 0
    while k < ln and rt[k] is None:
        k += 1

    if k == ln:
        rt.clear()
        return

    first_non_None = k
    k += 1

    while k < ln and not rt[k] is None:
        k += 1

    i = k + 1

    while i < ln:
        if not rt[i] is None:
            rt[k] = rt[i]
            k += 1

        i += 1

    if k > first_non_None:
        del rt[k:]

    if first_non_None > 0:
        del rt[:first_non_None]
