import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { SimilarMovie } from 'src/app/models/SimilarMovie';
import { Movie } from 'src/app/models/Movie'
import { MovieWithDetails } from 'src/app/models/MovieWithDetails';

@Injectable({
  providedIn: 'root'
})

export class GetMoviesService {

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

  getSimilarMoviesByTitle(title: string): Observable<SimilarMovie[]> {
    return this.http.get<SimilarMovie[]>(
      this.API_URL + `/movies/SimilarByTitle?title=${title}`
    );
  }

  getSimilarTitle(title: string): Observable<string> {
    return this.http.get(
      this.API_URL + `/movies/SimilarTitles?title=${title}`
      , { responseType: 'text' }
    );
  }

  getMovieWithDetails(title: string): Observable<MovieWithDetails> {
    return this.http.get<MovieWithDetails>(
      this.API_URL + `/movies/DetailsByTitle?title=${title}`
    );
  }
}
