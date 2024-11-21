import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { SimilarContent } from 'src/app/models/SimilarContent';
import { Game } from 'src/app/models/Game'
import { GameWithDetails } from 'src/app/models/ContentWithDetails';
import { IContentService } from 'src/app/models/IContentService';

@Injectable({
  providedIn: 'root'
})

export class GetGamesService implements IContentService {

  private API_URL = environment.API_URL;
  constructor(private http: HttpClient) { }


  //Get a single game by title
  getGameByTitle(title: string): Observable<Game> {
    return this.http.get<Game>(
      this.API_URL + `/Games/title?name=${title}`
    );
  }
  //Fetch similar games by title
  fetchContent(title: string): Observable<SimilarContent[]> {
    return this.http.get<SimilarContent[]>(
      this.API_URL + `/Games/SimilarByTitle?title=${title}`
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
  // Get similar Titles
  fetchSimilarTitle(title: string): Observable<string> {
    return this.http.get(
      this.API_URL + `/Games/SimilarTitles?title=${title}`
      , { responseType: 'text' }
    );
  }
  // fetch game details
  fetchDetails(title: string): Observable<GameWithDetails> {
    return this.http.get<any>(
      `${this.API_URL}/Games/DetailsByTitle?title=${title}`
    ).pipe(
      map(response => ({
        type: "Game",
        name: response.title,         // Mapping API's `title` to `MovieWithDetails.title`
        image: response.cover, // Mapping API's `posterPath` to `MovieWithDetails.posterPath`
        genre: response.genre,         // Mapping API's `genre` to `MovieWithDetails.genre`
        developer: response.developer,     // Mapping API's `director` to `MovieWithDetails.director`
        description: response.description // Include additional fields if needed
      }))
    );
  }
}
