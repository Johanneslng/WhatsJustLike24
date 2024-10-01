import {
  Component,
  EventEmitter,
  Output,
  Input
} from '@angular/core';
import { SetSearchValueService } from 'src/app/services/set-search-value.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss'
})
export class SearchComponent {
  @Input() searchValue = '';
  selectedType: string = 'Movie';
  types: string[] = ['Movie', 'Series', 'Game', 'Novel'];
  constructor(private setSearchValueService: SetSearchValueService) { }

  updateSearchString(): void {
    this.setSearchValueService.updateSearchValue(this.searchValue);
  }
}
