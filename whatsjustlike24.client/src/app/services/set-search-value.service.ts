import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { ContentType } from '../models/Enums/ContentType';

@Injectable({
  providedIn: 'root',
})
export class SetSearchValueService {
  private searchStateSubject = new BehaviorSubject<{ value: string; type: ContentType }>({
    value: '',
    type: ContentType.Movies,
  });

  updateSearchState(value: string, type: ContentType): void {
    this.searchStateSubject.next({ value, type });
  }

  getSearchState(): Observable<{ value: string; type: ContentType }> {
    return this.searchStateSubject.asObservable();
  }
}
