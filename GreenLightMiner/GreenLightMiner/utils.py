from typing import Dict, List, Tuple


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

def connect_ranges(ranges: List[Tuple[int, int]]) -> List[Tuple[int, List[int]]]:
    starts: Dict[int, List[int]] = {}

    for range in ranges:
        starts[range[0]] = [range[1]]

    has_updates: bool = True
    while has_updates:
        has_updates = False
        for (_, chain) in starts.items():
            if chain is None:
                continue

            last_in_chain = chain[-1]
            if last_in_chain in starts:
                chain.extend(starts[last_in_chain])
                starts[last_in_chain] = None
                has_updates = True

    return [(range[0], range[1]) for range in starts.items() if range[1] is not None]