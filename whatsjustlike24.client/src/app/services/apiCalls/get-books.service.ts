import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { SimilarContent } from 'src/app/models/SimilarContent';
import { Book } from 'src/app/models/Book'
import { BookWithDetails } from 'src/app/models/ContentWithDetails';
import { IContentService } from 'src/app/models/IContentService';

@Injectable({
  providedIn: 'root'
})

export class GetBooksService implements IContentService {

  private API_URL = environment.API_URL;
  constructor(private http: HttpClient) { }


  //Get all books
  getBooks(): Observable<string> {
    return this.http.get<string>(
      this.API_URL + `/books`
    );
  }

  //Get a single book by Id
  getBook(searchId: number): Observable<Book> {
    return this.http.get<Book>(
      this.API_URL + `/books/${searchId}`
    );
  }

  //Get a single show by title
  getShowByTitle(title: string): Observable<Book> {
    return this.http.get<Book>(
      this.API_URL + `/books/title?name=${title}`
    );
  }

  fetchContent(title: string): Observable<SimilarContent[]> {
    return this.http.get<SimilarContent[]>(
      this.API_URL + `/books/SimilarByTitle?title=${title}`
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
      this.API_URL + `/books/SimilarTitles?title=${title}`
      , { responseType: 'text' }
    );
  }

  fetchDetails(title: string): Observable<BookWithDetails> {
    return this.http.get<any>(
      `${this.API_URL}/books/DetailsByTitle?title=${title}`
    ).pipe(
      map(response => ({
        type: "Book",
        name: response.title,
        image: response.cover,
        genre: response.genre,
        author: response.author,
        description: response.description,
        firstRelease: response.firstRelease,
        languages: response.languages,
        publisher: response.publisher,
        isbn: response.isbn,
        series: response.series,
        pages: response.pages
      }))
    );
  }
}
