import { Movie } from './Movie';

export interface MovieRelation {
  titleMovieA: string;
  titleMovieB: string;
  similarityScore?: number;
  description: string;
}
