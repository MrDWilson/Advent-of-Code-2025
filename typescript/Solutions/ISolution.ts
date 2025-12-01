export interface ISolution {
  readonly day: number;
  solve(): Promise<string>;
}

