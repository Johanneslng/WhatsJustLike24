export interface MovieIsLike {
  id?: number,
  idMovieA: number,
  idMovieB: number,
  movieId?: number,
  description: string,
  similarityScore: number
}

export interface Movie {
  id?: number;
  title: string;
}
