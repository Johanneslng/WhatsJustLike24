import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { SimilarContent } from 'src/app/models/SimilarContent';
import { Movie } from 'src/app/models/Movie'
import { MovieWithDetails } from 'src/app/models/ContentWithDetails';
import { IContentService } from 'src/app/models/IContentService';

@Injectable({
  providedIn: 'root'
})

export class GetMoviesService implements IContentService {

  private API_URL = environment.API_URL;
  constructor(private http: HttpClient) { }


  //Get all movies by Id
  getMovies(): Observable<string> {
    return this.http.get<string>(
      this.API_URL + `/movies`
    );
  }

  //Get a single Movie by Id
  getMovie(searchId: number): Observable<Movie> {
    return this.http.get<Movie>(
      this.API_URL + `/movies/${searchId}`
    );
  }

  //Get a single movie by title
  getMovieByTitle(title: string): Observable<Movie> {
    return this.http.get<Movie>(
      this.API_URL + `/movies/title?name=${title}`
    );
  }

  fetchContent(title: string): Observable<SimilarContent[]> {
    return this.http.get<SimilarContent[]>(
      this.API_URL + `/movies/SimilarByTitle?title=${title}`
    ).pipe(
      map((response: any[]) => {
        return response.map(item => ({
          name: item.title,
          averageSimilarityScore: item.averageSimilarityScore,
          similarityScoreCount: item.similarityScoreCount,
          image: item.posterPath,
          descriptionList: item.descriptionList,
          similarityScoreList: item.similarityScoreList
        }));
      })
    );
  }

  fetchSimilarTitle(title: string): Observable<string> {
    return this.http.get(
      this.API_URL + `/movies/SimilarTitles?title=${title}`
      , { responseType: 'text' }
    );
  }

  fetchDetails(title: string): Observable<MovieWithDetails> {
    return this.http.get<any>(
      `${this.API_URL}/movies/DetailsByTitle?title=${title}`
    ).pipe(
      map(response => ({
        type: "Movie",
        name: response.title,         // Mapping API's `title` to `MovieWithDetails.title`
        image: response.posterPath, // Mapping API's `posterPath` to `MovieWithDetails.posterPath`
        genre: response.genre,         // Mapping API's `genre` to `MovieWithDetails.genre`
        director: response.director,     // Mapping API's `director` to `MovieWithDetails.director`
        description: response.description // Include additional fields if needed
      }))
    );
  }


}
