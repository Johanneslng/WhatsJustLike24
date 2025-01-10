import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { ContentRelation } from 'src/app/models/ContentRelation';
import { IPostContentService } from 'src/app/models/IPostContentService';


@Injectable({
  providedIn: 'root'
})
export class PostBooksService implements IPostContentService {

  private API_URL = environment.API_URL;

  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };

  constructor(private http: HttpClient) { }

  /** POST: add a new book to the server */
  addContent(bookRelation: ContentRelation): Observable<ContentRelation> {
    return this.http.post<ContentRelation>(
      `${this.API_URL}/books/AddSimilarity`,
      bookRelation,
      this.httpOptions
    ).pipe(
      catchError(this.handleError<ContentRelation>('addBook'))
    );
  }

  /**
   * Handle Http operation that failed.
   * Let the app continue.
   * @param operation - name of the operation that failed
   * @param result - optional value to return as the observable result
   */
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(`${operation} failed: ${error.message}`);
      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
}
