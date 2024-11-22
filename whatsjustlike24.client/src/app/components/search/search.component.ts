import {
  Component,
  EventEmitter,
  Output,
  Input
} from '@angular/core';
import { ContentType } from 'src/app/models/Enums/ContentType';
import { SetSearchValueService } from 'src/app/services/set-search-value.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss'
})
export class SearchComponent {
  @Input() searchValue = '';
  selectedType: number = ContentType.Movies;
  types = [
    { label: 'Movie', value: ContentType.Movies },
    { label: 'TV-Show', value: ContentType.Shows },
    { label: 'Game', value: ContentType.Games },
    { label: 'Novel', value: ContentType.Books }
  ];
  constructor(private setSearchValueService: SetSearchValueService) { }

  updateSearchState(): void {
    this.setSearchValueService.updateSearchState(this.searchValue, this.selectedType);
  }

}
