function groupWhile<T>(sequence: Iterable<T>, condition: (previous: T, current: T) => boolean): T[][] {
  const iterator = sequence[Symbol.iterator]();
  const first = iterator.next();

  if (first.done) {
    return [];
  }

  let previous = first.value;
  let currentGroup: T[] = [previous];
  const groups: T[][] = [];

  for (let next = iterator.next(); !next.done; next = iterator.next()) {
    const value = next.value;
    if (!condition(previous, value)) {
      groups.push(currentGroup);
      currentGroup = [];
    }

    currentGroup.push(value);
    previous = value;
  }

  groups.push(currentGroup);
  return groups;
}

export { groupWhile };

