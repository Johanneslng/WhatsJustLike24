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



// Combine into a union type
export type ContentWithDetails = MovieWithDetails | GameWithDetails;
