import { Injectable } from '@angular/core';
import { Subject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SetSearchValueService {
  private searchValue = new Subject<string>();

  updateSearchValue(value: string) {
    this.searchValue.next(value);
  }

  getSearchValue() {
    return this.searchValue.asObservable();
  }
}
