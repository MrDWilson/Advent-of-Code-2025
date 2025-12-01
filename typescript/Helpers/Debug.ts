const flattenStructure = <T>(values: Iterable<T>, separator: string): string => {
  return Array.from(values).join(separator);
};

const isStringArray = (values: unknown[]): values is string[] => values.every((item) => typeof item === "string");

function writeLine(values: Iterable<unknown>): void {
  const entries = Array.from(values);

  if (entries.length === 0) {
    console.log("Empty");
    return;
  }

  if (isStringArray(entries)) {
    console.log(flattenStructure(entries, "\n"));
    return;
  }

  console.log(flattenStructure(entries, ", "));
}

function writeNested<T>(values: Iterable<Iterable<T>>): void {
  const nested = Array.from(values).map((entry) => flattenStructure(entry, ", "));
  writeLine(nested);
}

export const Debug = {
  writeLine,
  writeNested,
};

