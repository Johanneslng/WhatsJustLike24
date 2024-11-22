export interface ContentIsLike {
  id?: number,
  idContentA: number,
  idContentB: number,
  contentId?: number,
  description: string,
  similarityScore: number
}

export interface Content {
  id?: number;
  title: string;
}
