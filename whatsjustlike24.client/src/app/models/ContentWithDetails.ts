export type MovieWithDetails = {
  type: 'Movie'; // Discriminant
  name: string;
  genre: string;
  director: string;
  image: string;
  description: string;
};

export type GameWithDetails = {
  type: 'Game'; // Discriminant
  name: string;
  genre: string;
  developer: string;
  image: string;
  description: string;
};

export type ShowWithDetails = {
  type: 'Show'; // Discriminant
  name: string;
  genre: string;
  director: string;
  image: string;
  description: string;
  firstAirDate: string;
};

export type BookWithDetails = {
  type: 'Book'; // Discriminant
  name: string;
  genre: string;
  author: string;
  image: string;
  description: string;
  firstRelease: string;
  series: string;
  isbn: string;
  pages: number;
  publisher: string;
  languages: string;
};


// Combine into a union type
export type ContentWithDetails = MovieWithDetails | GameWithDetails | ShowWithDetails | BookWithDetails;
