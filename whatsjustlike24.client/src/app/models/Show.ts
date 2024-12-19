export interface ShowIsLike {
  id?: number,
  idShowA: number,
  idShowB: number,
  showId?: number,
  description: string,
  similarityScore: number
}

export interface Show {
  id?: number;
  title: string;
}
