import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { SimilarContent } from 'src/app/models/SimilarContent';
import { Show } from 'src/app/models/Show'
import { ShowWithDetails } from 'src/app/models/ContentWithDetails';
import { IContentService } from 'src/app/models/IContentService';

@Injectable({
  providedIn: 'root'
})

export class GetShowsService implements IContentService {

  private API_URL = environment.API_URL;
  constructor(private http: HttpClient) { }


  //Get all shows
  getShows(): Observable<string> {
    return this.http.get<string>(
      this.API_URL + `/shows`
    );
  }

  //Get a single show by Id
  getShow(searchId: number): Observable<Show> {
    return this.http.get<Show>(
      this.API_URL + `/shows/${searchId}`
    );
  }

  //Get a single show by title
  getShowByTitle(title: string): Observable<Show> {
    return this.http.get<Show>(
      this.API_URL + `/shows/title?name=${title}`
    );
  }

  fetchContent(title: string): Observable<SimilarContent[]> {
    return this.http.get<SimilarContent[]>(
      this.API_URL + `/shows/SimilarByTitle?title=${title}`
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
      this.API_URL + `/shows/SimilarTitles?title=${title}`
      , { responseType: 'text' }
    );
  }

  fetchDetails(title: string): Observable<ShowWithDetails> {
    return this.http.get<any>(
      `${this.API_URL}/shows/DetailsByTitle?title=${title}`
    ).pipe(
      map(response => ({
        type: "Show",
        name: response.title,  
        image: response.posterPath,
        genre: response.genre,
        director: response.director,
        description: response.description,
        firstAirDate: response.firstAirDate
      }))
    );
  }


}
